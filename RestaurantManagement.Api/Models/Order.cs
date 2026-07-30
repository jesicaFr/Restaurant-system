namespace RestaurantManagement.Api.Models;

public class Order
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = OrderStatus.Pending.ToDisplayName();
    public string PaymentMethod { get; set; } = RestaurantValues.Cash;
    public DateTime? PaidAt { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderDetail> OrderDetails { get; set; } = new();
}
