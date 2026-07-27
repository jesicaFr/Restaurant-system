using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> UpdateStatusAsync(
        int id,
        string status,
        DateTime? paidAt,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
