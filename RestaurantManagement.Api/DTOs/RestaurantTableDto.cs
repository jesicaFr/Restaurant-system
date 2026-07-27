namespace RestaurantManagement.Api.DTOs;

public class RestaurantTableDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
}
