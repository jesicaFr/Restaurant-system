using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Data;
using RestaurantManagement.Api.Models;
using RestaurantManagement.Api.Services;

namespace RestaurantManagement.Api.Repositories;

public class OrderRepository(RestaurantDbContext db) : IOrderRepository
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
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

    public Task<Order?> GetTrackedByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        db.Orders
            .Include(order => order.OrderDetails)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

    public void Add(Order order) => db.Orders.Add(order);

    public void Remove(Order order) => db.Orders.Remove(order);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await db.SaveChangesAsync(cancellationToken);

    public async Task<OperationResult<T>> ExecuteInTransactionAsync<T>(
        Func<Task<OperationResult<T>>> operation,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var result = await operation();

        if (!result.IsSuccess)
        {
            return result;
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result;
    }
}
