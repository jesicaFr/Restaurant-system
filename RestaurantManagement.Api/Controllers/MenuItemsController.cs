using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Models;
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
        var created = await menuItemService.CreateAsync(
            new MenuItem
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                Price = dto.Price,
                Quantity = dto.Quantity,
                IsAvailable = dto.IsAvailable && dto.Quantity > 0
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MenuItemDto>> Update(
        int id,
        [FromBody] UpdateMenuItemDto dto,
        CancellationToken cancellationToken)
    {
        var current = await menuItemService.GetByIdAsync(id, cancellationToken);
        if (current is null)
        {
            return NotFound();
        }

        var quantity = dto.Quantity ?? current.Quantity;
        var isRestocking = current.Quantity == 0 && quantity > 0;
        var updated = await menuItemService.UpdateAsync(
            id,
            new MenuItem
            {
                Name = dto.Name?.Trim() ?? current.Name,
                Description = dto.Description?.Trim() ?? current.Description,
                Price = dto.Price ?? current.Price,
                Quantity = quantity,
                IsAvailable =
                    (isRestocking || (dto.IsAvailable ?? current.IsAvailable)) &&
                    quantity > 0
            },
            cancellationToken);

        return updated is null ? NotFound() : Ok(updated.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await menuItemService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
