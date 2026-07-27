using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Api.DTOs;

public class UpdateMenuItemDto
{
    [StringLength(120, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 120 caracteres.")]
    public string? Name { get; set; }

    [StringLength(500, MinimumLength = 1, ErrorMessage = "La descripción debe tener entre 1 y 500 caracteres.")]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "El precio no puede ser negativo.")]
    public decimal? Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
    public int? Quantity { get; set; }

    public bool? IsAvailable { get; set; }
}
