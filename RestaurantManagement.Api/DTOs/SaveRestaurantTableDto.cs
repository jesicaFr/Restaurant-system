using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Api.DTOs;

public class SaveRestaurantTableDto
{
    [Required(ErrorMessage = "El número de mesa es obligatorio.")]
    [StringLength(20, ErrorMessage = "El número de mesa no puede superar los 20 caracteres.")]
    public string Number { get; set; } = string.Empty;

    [Range(2, 100, ErrorMessage = "La capacidad debe estar entre 2 y 100 personas.")]
    public int Capacity { get; set; }

    public bool IsOccupied { get; set; }
}
