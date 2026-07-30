using RestaurantManagement.Api.Models;
using RestaurantManagement.Api.Services;

namespace RestaurantManagement.Api.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Order?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Order order);
    void Remove(Order order);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<T>> ExecuteInTransactionAsync<T>(
        Func<Task<OperationResult<T>>> operation,
        CancellationToken cancellationToken = default);
}
