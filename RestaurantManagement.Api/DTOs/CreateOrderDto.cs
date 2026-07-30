using System.ComponentModel.DataAnnotations;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.DTOs;

public class CreateOrderDto
{
    [Range(1, int.MaxValue, ErrorMessage = "La mesa seleccionada no es válida.")]
    public int TableId { get; set; }

    [Required]
    public string Status { get; set; } = OrderStatus.Pending.ToDisplayName();

    [Required]
    public string PaymentMethod { get; set; } = RestaurantValues.Cash;

    [Required]
    [MinLength(1, ErrorMessage = "El pedido debe contener al menos un producto.")]
    public List<AddOrderItemDto> Items { get; set; } = [];
}
