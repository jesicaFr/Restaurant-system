using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Api.DTOs;

public class AddOrderItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El producto seleccionado no es válido.")]
    public int MenuItemId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
    public int Quantity { get; set; }
}
