using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public interface IMenuItemService
{
    Task<List<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MenuItem> CreateAsync(MenuItem menuItem, CancellationToken cancellationToken = default);
    Task<MenuItem?> UpdateAsync(int id, MenuItem menuItem, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
