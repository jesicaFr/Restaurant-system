using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Api.DTOs;

public class UpdateOrderStatusDto
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    public string Status { get; set; } = string.Empty;
}
