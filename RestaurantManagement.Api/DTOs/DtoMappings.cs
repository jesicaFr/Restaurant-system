using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.DTOs;

public static class DtoMappings
{
    public static RestaurantTableDto ToDto(this RestaurantTable table) => new()
    {
        Id = table.Id,
        Number = table.Number,
        Capacity = table.Capacity,
        Status = table.Status,
        IsOccupied = table.IsOccupied
    };

    public static MenuItemDto ToDto(this MenuItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        Price = item.Price,
        Quantity = item.Quantity,
        IsAvailable = item.IsAvailable
    };

    public static OrderDto ToDto(this Order order) => new()
    {
        Id = order.Id,
        TableId = order.TableId,
        OrderDate = order.OrderDate,
        Status = order.Status,
        PaymentMethod = order.PaymentMethod,
        TotalAmount = order.TotalAmount,
        OrderDetails = order.OrderDetails.Select(detail => new OrderDetailDto
        {
            Id = detail.Id,
            MenuItemId = detail.MenuItemId,
            MenuItemName = detail.MenuItemName,
            Quantity = detail.Quantity,
            UnitPrice = detail.UnitPrice,
            TotalPrice = detail.TotalPrice
        }).ToList()
    };
}
