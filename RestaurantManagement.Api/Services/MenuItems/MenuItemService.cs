using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Models;
using RestaurantManagement.Api.Repositories;

namespace RestaurantManagement.Api.Services;

public class MenuItemService(IMenuItemRepository repository) : IMenuItemService
{
    public Task<List<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<MenuItem> CreateAsync(
        CreateMenuItemDto dto,
        CancellationToken cancellationToken = default)
    {
        var menuItem = new MenuItem
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            Price = dto.Price,
            Quantity = dto.Quantity,
            IsAvailable = dto.IsAvailable && dto.Quantity > 0
        };

        repository.Add(menuItem);
        await repository.SaveChangesAsync(cancellationToken);
        return menuItem;
    }

    public async Task<OperationResult<MenuItem>> UpdateAsync(
        int id,
        UpdateMenuItemDto dto,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return OperationResult<MenuItem>.Fail(
                OperationFailure.NotFound,
                "El producto no existe.");
        }

        var quantity = dto.Quantity ?? existing.Quantity;
        var isRestocking = existing.Quantity == 0 && quantity > 0;

        existing.Name = dto.Name?.Trim() ?? existing.Name;
        existing.Description = dto.Description?.Trim() ?? existing.Description;
        existing.Price = dto.Price ?? existing.Price;
        existing.Quantity = quantity;
        existing.IsAvailable =
            (isRestocking || (dto.IsAvailable ?? existing.IsAvailable)) &&
            quantity > 0;

        await repository.SaveChangesAsync(cancellationToken);
        return OperationResult<MenuItem>.Success(existing);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        repository.Remove(existing);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
