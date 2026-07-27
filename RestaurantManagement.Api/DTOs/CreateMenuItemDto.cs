using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Api.DTOs;

public class CreateMenuItemDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120, ErrorMessage = "El nombre no puede superar los 120 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "El precio no puede ser negativo.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
    public int Quantity { get; set; }

    public bool IsAvailable { get; set; } = true;
}
