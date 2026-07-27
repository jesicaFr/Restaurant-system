using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Data;
using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Models;
using RestaurantManagement.Api.Services;

namespace RestaurantManagement.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(
    IOrderService orderService,
    IMenuItemService menuItemService,
    ITableService tableService,
    RestaurantDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var orders = await orderService.GetAllAsync(cancellationToken);
        return Ok(orders.Select(order => order.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await orderService.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(
        [FromBody] CreateOrderDto dto,
        CancellationToken cancellationToken)
    {
        var paymentMethod = dto.PaymentMethod.Trim();
        if (!RestaurantValues.PaymentMethods.Contains(paymentMethod, StringComparer.Ordinal))
        {
            return BadRequest("El medio de pago debe ser Efectivo o Tarjeta.");
        }

        var status = dto.Status.Trim();
        if (!RestaurantValues.OrderStatuses.Contains(status, StringComparer.Ordinal))
        {
            return BadRequest("El estado del pedido no es válido.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var table = await tableService.GetByIdAsync(dto.TableId, cancellationToken);
        if (table is null)
        {
            return BadRequest("La mesa seleccionada no existe.");
        }

        if (table.IsOccupied)
        {
            return Conflict("La mesa seleccionada ya está ocupada.");
        }

        var requestedItems = dto.Items
            .GroupBy(item => item.MenuItemId)
            .Select(group => new
            {
                MenuItemId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .ToList();
        var orderDetails = new List<OrderDetail>(requestedItems.Count);

        foreach (var requestedItem in requestedItems)
        {
            var menuItem = await menuItemService.GetByIdAsync(
                requestedItem.MenuItemId,
                cancellationToken);
            if (menuItem is null || !menuItem.IsAvailable)
            {
                return BadRequest(
                    $"El producto con ID {requestedItem.MenuItemId} no está disponible.");
            }

            if (menuItem.Quantity < requestedItem.Quantity)
            {
                return BadRequest($"No hay stock suficiente para {menuItem.Name}.");
            }

            menuItem.Quantity -= requestedItem.Quantity;
            menuItem.IsAvailable = menuItem.Quantity > 0;

            orderDetails.Add(new OrderDetail
            {
                MenuItemId = menuItem.Id,
                MenuItemName = menuItem.Name,
                Quantity = requestedItem.Quantity,
                UnitPrice = menuItem.Price
            });
        }

        var order = new Order
        {
            TableId = dto.TableId,
            Status = status,
            PaymentMethod = paymentMethod,
            PaidAt = status == RestaurantValues.Delivered ? DateTime.UtcNow : null,
            OrderDetails = orderDetails,
            TotalAmount = orderDetails.Sum(item => item.TotalPrice)
        };

        var created = await orderService.CreateAsync(order, cancellationToken);
        table.IsOccupied = created.Status != RestaurantValues.Delivered;
        table.Status = table.IsOccupied ? RestaurantValues.Occupied : RestaurantValues.Available;
        await tableService.UpdateAsync(table.Id, table, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(
        int id,
        [FromBody] UpdateOrderStatusDto dto,
        CancellationToken cancellationToken)
    {
        var nextStatus = dto.Status.Trim();
        if (!RestaurantValues.OrderStatuses.Contains(nextStatus, StringComparer.Ordinal))
        {
            return BadRequest("El estado del pedido no es válido.");
        }

        var existing = await orderService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (existing.Status == RestaurantValues.Delivered)
        {
            return Conflict("Un pedido entregado está cerrado y no puede volver a abrirse.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var paidAt = nextStatus == RestaurantValues.Delivered
            ? existing.PaidAt ?? DateTime.UtcNow
            : existing.PaidAt;
        var updated = await orderService.UpdateStatusAsync(
            id,
            nextStatus,
            paidAt,
            cancellationToken);

        if (updated is null)
        {
            return NotFound();
        }

        if (updated.Status == RestaurantValues.Delivered)
        {
            var table = await tableService.GetByIdAsync(updated.TableId, cancellationToken);
            if (table is not null)
            {
                table.IsOccupied = false;
                table.Status = RestaurantValues.Available;
                await tableService.UpdateAsync(table.Id, table, cancellationToken);
            }
        }

        await transaction.CommitAsync(cancellationToken);
        return Ok(updated.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await orderService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
