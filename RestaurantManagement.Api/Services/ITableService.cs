using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public interface ITableService
{
    Task<List<RestaurantTable>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RestaurantTable?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> NumberExistsAsync(string number, int? excludedId = null, CancellationToken cancellationToken = default);
    Task<RestaurantTable> CreateAsync(RestaurantTable table, CancellationToken cancellationToken = default);
    Task<RestaurantTable?> UpdateAsync(int id, RestaurantTable table, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
