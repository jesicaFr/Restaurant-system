using RestaurantManagement.Api.DTOs;

namespace RestaurantManagement.Api.Services;

public interface ICashRegisterService
{
    Task<DailySalesDto> GetDailySalesAsync(DateTime date, CancellationToken cancellationToken = default);
}
