namespace RestaurantManagement.Api.Models;

public class RestaurantTable
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Status { get; set; } = RestaurantValues.Available;
    public bool IsOccupied { get; set; }
}
