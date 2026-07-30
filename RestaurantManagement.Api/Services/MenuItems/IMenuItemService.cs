using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public interface IMenuItemService
{
    Task<List<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MenuItem> CreateAsync(
        CreateMenuItemDto dto,
        CancellationToken cancellationToken = default);
    Task<OperationResult<MenuItem>> UpdateAsync(
        int id,
        UpdateMenuItemDto dto,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
