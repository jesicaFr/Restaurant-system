using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Data;
using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public class CashRegisterService(RestaurantDbContext db) : ICashRegisterService
{
    public async Task<DailySalesDto> GetDailySalesAsync(
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        var paidOrders = db.Orders
            .AsNoTracking()
            .Where(order =>
                order.PaidAt.HasValue &&
                order.PaidAt.Value >= start &&
                order.PaidAt.Value < end);

        var paidOrdersCount = await paidOrders.CountAsync(cancellationToken);
        var totalSales = await paidOrders.SumAsync(order => order.TotalAmount, cancellationToken);
        var cashSales = await paidOrders
            .Where(order => order.PaymentMethod == RestaurantValues.Cash)
            .SumAsync(order => order.TotalAmount, cancellationToken);
        var cardSales = await paidOrders
            .Where(order => order.PaymentMethod == RestaurantValues.Card)
            .SumAsync(order => order.TotalAmount, cancellationToken);

        return new DailySalesDto
        {
            Date = start,
            TotalSales = totalSales,
            PaidOrdersCount = paidOrdersCount,
            CashSales = cashSales,
            CardSales = cardSales,
            AverageTicket = paidOrdersCount == 0 ? 0 : totalSales / paidOrdersCount
        };
    }
}
