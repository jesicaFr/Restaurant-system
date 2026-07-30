using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult<Order>> CreateAsync(
        CreateOrderDto dto,
        CancellationToken cancellationToken = default);
    Task<OperationResult<Order>> UpdateStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
