namespace RestaurantManagement.Api.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}
