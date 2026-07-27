using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Data;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public class OrderService(RestaurantDbContext db) : IOrderService
{
    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.Orders
            .Include(order => order.OrderDetails)
            .AsNoTracking()
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync(cancellationToken);

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Orders
            .Include(order => order.OrderDetails)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

    public async Task<Order> CreateAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        db.Orders.Add(order);
        await db.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<Order?> UpdateStatusAsync(
        int id,
        string status,
        DateTime? paidAt,
        CancellationToken cancellationToken = default)
    {
        var existing = await db.Orders
            .Include(order => order.OrderDetails)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Status = status;
        existing.PaidAt = paidAt;
        await db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await db.Orders.FirstOrDefaultAsync(
            order => order.Id == id,
            cancellationToken);
        if (existing is null)
        {
            return false;
        }

        db.Orders.Remove(existing);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
