using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Mappers;
using RestaurantManagement.Api.Models;
using RestaurantManagement.Api.Services;

namespace RestaurantManagement.Api.Controllers;

[ApiController]
[Route("api/tables")]
public class TablesController(ITableService tableService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RestaurantTableDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var tables = await tableService.GetAllAsync(cancellationToken);
        return Ok(tables.Select(table => table.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RestaurantTableDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var table = await tableService.GetByIdAsync(id, cancellationToken);
        return table is null ? NotFound() : Ok(table.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<RestaurantTableDto>> Create(
        [FromBody] SaveRestaurantTableDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tableService.CreateAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return this.ToErrorResult(result.Failure, result.Message);
        }

        var created = result.Value!;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RestaurantTableDto>> Update(
        int id,
        [FromBody] SaveRestaurantTableDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tableService.UpdateAsync(id, dto, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value!.ToDto())
            : this.ToErrorResult(result.Failure, result.Message);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await tableService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
