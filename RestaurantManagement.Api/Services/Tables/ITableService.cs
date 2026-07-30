using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public interface ITableService
{
    Task<List<RestaurantTable>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RestaurantTable?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult<RestaurantTable>> CreateAsync(
        SaveRestaurantTableDto dto,
        CancellationToken cancellationToken = default);
    Task<OperationResult<RestaurantTable>> UpdateAsync(
        int id,
        SaveRestaurantTableDto dto,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
