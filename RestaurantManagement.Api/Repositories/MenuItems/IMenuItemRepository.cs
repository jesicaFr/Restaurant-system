using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Repositories;

public interface IMenuItemRepository
{
    Task<List<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MenuItem?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ReserveStockAsync(
        int id,
        int quantity,
        CancellationToken cancellationToken = default);
    void Add(MenuItem menuItem);
    void Remove(MenuItem menuItem);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
