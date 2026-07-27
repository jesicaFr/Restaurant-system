using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.DTOs;
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
        var normalizedNumber = dto.Number.Trim();
        if (await tableService.NumberExistsAsync(normalizedNumber, cancellationToken: cancellationToken))
        {
            return Conflict("Ya existe una mesa con ese número.");
        }

        var created = await tableService.CreateAsync(
            new RestaurantTable
            {
                Number = normalizedNumber,
                Capacity = dto.Capacity,
                Status = dto.IsOccupied ? RestaurantValues.Occupied : RestaurantValues.Available,
                IsOccupied = dto.IsOccupied
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RestaurantTableDto>> Update(
        int id,
        [FromBody] SaveRestaurantTableDto dto,
        CancellationToken cancellationToken)
    {
        var normalizedNumber = dto.Number.Trim();
        if (await tableService.NumberExistsAsync(normalizedNumber, id, cancellationToken))
        {
            return Conflict("Ya existe una mesa con ese número.");
        }

        var updated = await tableService.UpdateAsync(
            id,
            new RestaurantTable
            {
                Number = normalizedNumber,
                Capacity = dto.Capacity,
                Status = dto.IsOccupied ? RestaurantValues.Occupied : RestaurantValues.Available,
                IsOccupied = dto.IsOccupied
            },
            cancellationToken);

        return updated is null ? NotFound() : Ok(updated.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await tableService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
