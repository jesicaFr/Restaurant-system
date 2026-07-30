using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Models;
using RestaurantManagement.Api.Repositories;

namespace RestaurantManagement.Api.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ITableRepository tableRepository,
    IMenuItemRepository menuItemRepository) : IOrderService
{
    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default) =>
        orderRepository.GetAllAsync(cancellationToken);

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        orderRepository.GetByIdAsync(id, cancellationToken);

    public async Task<OperationResult<Order>> CreateAsync(
        CreateOrderDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateCreateRequest(dto);
        if (!validation.IsSuccess)
        {
            return OperationResult<Order>.Fail(validation.Failure, validation.Message!);
        }

        var request = validation.Value!;

        return await orderRepository.ExecuteInTransactionAsync(
            async () =>
            {
                var tableValidation = await ValidateAndClaimTableAsync(
                    dto.TableId,
                    cancellationToken);
                if (!tableValidation.IsSuccess)
                {
                    return OperationResult<Order>.Fail(
                        tableValidation.Failure,
                        tableValidation.Message!);
                }

                var orderDetails = new List<OrderDetail>(request.Items.Count);
                foreach (var requestedItem in request.Items)
                {
                    var reservation = await ValidateAndReserveStockAsync(
                        requestedItem,
                        cancellationToken);
                    if (!reservation.IsSuccess)
                    {
                        return OperationResult<Order>.Fail(
                            reservation.Failure,
                            reservation.Message!);
                    }

                    orderDetails.Add(reservation.Value!);
                }

                var order = new Order
                {
                    TableId = dto.TableId,
                    Status = request.Status.ToDisplayName(),
                    PaymentMethod = request.PaymentMethod,
                    PaidAt = request.Status == OrderStatus.Delivered ? DateTime.UtcNow : null,
                    OrderDetails = orderDetails,
                    TotalAmount = orderDetails.Sum(item => item.TotalPrice)
                };

                if (request.Status == OrderStatus.Delivered)
                {
                    await tableRepository.ReleaseAsync(dto.TableId, cancellationToken);
                }

                orderRepository.Add(order);
                return OperationResult<Order>.Success(order);
            },
            cancellationToken);
    }

    public async Task<OperationResult<Order>> UpdateStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!OrderStatusExtensions.TryParseDisplayName(status, out var nextStatus))
        {
            return OperationResult<Order>.Fail(
                OperationFailure.Validation,
                "El estado del pedido no es válido.");
        }

        return await orderRepository.ExecuteInTransactionAsync(
            async () =>
            {
                var existing = await orderRepository.GetTrackedByIdAsync(id, cancellationToken);

                if (existing is null)
                {
                    return OperationResult<Order>.Fail(
                        OperationFailure.NotFound,
                        "El pedido no existe.");
                }

                if (existing.Status == OrderStatus.Delivered.ToDisplayName())
                {
                    return OperationResult<Order>.Fail(
                        OperationFailure.Conflict,
                        "Un pedido entregado está cerrado y no puede volver a abrirse.");
                }

                existing.Status = nextStatus.ToDisplayName();
                if (nextStatus == OrderStatus.Delivered)
                {
                    existing.PaidAt ??= DateTime.UtcNow;
                    await tableRepository.ReleaseAsync(existing.TableId, cancellationToken);
                }

                return OperationResult<Order>.Success(existing);
            },
            cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await orderRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        orderRepository.Remove(existing);
        await orderRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static OperationResult<ValidatedCreateOrder> ValidateCreateRequest(CreateOrderDto dto)
    {
        if (!OrderStatusExtensions.TryParseDisplayName(dto.Status, out var status))
        {
            return OperationResult<ValidatedCreateOrder>.Fail(
                OperationFailure.Validation,
                "El estado del pedido no es válido.");
        }

        var paymentMethod = dto.PaymentMethod.Trim();
        if (!RestaurantValues.PaymentMethods.Contains(paymentMethod, StringComparer.Ordinal))
        {
            return OperationResult<ValidatedCreateOrder>.Fail(
                OperationFailure.Validation,
                "El medio de pago debe ser Efectivo o Tarjeta.");
        }

        var items = dto.Items
            .GroupBy(item => item.MenuItemId)
            .Select(group => new RequestedOrderItem(
                group.Key,
                group.Sum(item => item.Quantity)))
            .ToList();

        return OperationResult<ValidatedCreateOrder>.Success(
            new ValidatedCreateOrder(status, paymentMethod, items));
    }

    private async Task<OperationResult<bool>> ValidateAndClaimTableAsync(
        int tableId,
        CancellationToken cancellationToken)
    {
        if (await tableRepository.ClaimAsync(tableId, cancellationToken))
        {
            return OperationResult<bool>.Success(true);
        }

        var tableExists = await tableRepository.ExistsAsync(tableId, cancellationToken);

        return tableExists
            ? OperationResult<bool>.Fail(
                OperationFailure.Conflict,
                "La mesa seleccionada ya está ocupada.")
            : OperationResult<bool>.Fail(
                OperationFailure.Validation,
                "La mesa seleccionada no existe.");
    }

    private async Task<OperationResult<OrderDetail>> ValidateAndReserveStockAsync(
        RequestedOrderItem requestedItem,
        CancellationToken cancellationToken)
    {
        var menuItem = await menuItemRepository.GetByIdAsync(
            requestedItem.MenuItemId,
            cancellationToken);

        if (menuItem is null)
        {
            return OperationResult<OrderDetail>.Fail(
                OperationFailure.Validation,
                $"El producto con ID {requestedItem.MenuItemId} no existe.");
        }

        if (!await menuItemRepository.ReserveStockAsync(
                requestedItem.MenuItemId,
                requestedItem.Quantity,
                cancellationToken))
        {
            return OperationResult<OrderDetail>.Fail(
                OperationFailure.Conflict,
                $"{menuItem.Name} no está disponible o no tiene stock suficiente.");
        }

        return OperationResult<OrderDetail>.Success(
            new OrderDetail
            {
                MenuItemId = menuItem.Id,
                MenuItemName = menuItem.Name,
                Quantity = requestedItem.Quantity,
                UnitPrice = menuItem.Price
            });
    }

    private sealed record ValidatedCreateOrder(
        OrderStatus Status,
        string PaymentMethod,
        List<RequestedOrderItem> Items);

    private sealed record RequestedOrderItem(int MenuItemId, int Quantity);
}
