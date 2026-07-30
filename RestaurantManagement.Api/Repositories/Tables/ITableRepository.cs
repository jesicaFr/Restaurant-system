using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Repositories;

public interface ITableRepository
{
    Task<List<RestaurantTable>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RestaurantTable?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RestaurantTable?> GetTrackedByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
    Task<bool> NumberExistsAsync(
        string number,
        int? excludedId = null,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ClaimAsync(int id, CancellationToken cancellationToken = default);
    Task ReleaseAsync(int id, CancellationToken cancellationToken = default);
    void Add(RestaurantTable table);
    void Remove(RestaurantTable table);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
