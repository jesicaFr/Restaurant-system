namespace RestaurantManagement.Api.DTOs;

public class DailySalesDto
{
    public DateTime Date { get; set; }
    public decimal TotalSales { get; set; }
    public int PaidOrdersCount { get; set; }
    public decimal CashSales { get; set; }
    public decimal CardSales { get; set; }
    public decimal AverageTicket { get; set; }
}
