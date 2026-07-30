using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Mappers;
using RestaurantManagement.Api.Services;

namespace RestaurantManagement.Api.Controllers;

[ApiController]
[Route("api/menuitems")]
public class MenuItemsController(IMenuItemService menuItemService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await menuItemService.GetAllAsync(cancellationToken);
        return Ok(items.Select(item => item.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MenuItemDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var item = await menuItemService.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<MenuItemDto>> Create(
        [FromBody] CreateMenuItemDto dto,
        CancellationToken cancellationToken)
    {
        var created = await menuItemService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MenuItemDto>> Update(
        int id,
        [FromBody] UpdateMenuItemDto dto,
        CancellationToken cancellationToken)
    {
        var result = await menuItemService.UpdateAsync(id, dto, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value!.ToDto())
            : this.ToErrorResult(result.Failure, result.Message);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await menuItemService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
