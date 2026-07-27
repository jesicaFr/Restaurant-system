using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Services;

namespace RestaurantManagement.Api.Controllers;

[ApiController]
[Route("api/cash-register")]
public class CashRegisterController(ICashRegisterService cashRegisterService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DailySalesDto>> GetDailySales(
        [FromQuery] DateTime date,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        if (date.Date < today.AddDays(-7) || date.Date > today)
        {
            return BadRequest("Solo se pueden consultar fechas de los últimos 7 días.");
        }

        var result = await cashRegisterService.GetDailySalesAsync(date, cancellationToken);
        return Ok(result);
    }
}
