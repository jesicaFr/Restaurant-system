using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Data;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public class MenuItemService(RestaurantDbContext db) : IMenuItemService
{
    public Task<List<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.MenuItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

    public Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.MenuItems.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<MenuItem> CreateAsync(
        MenuItem menuItem,
        CancellationToken cancellationToken = default)
    {
        db.MenuItems.Add(menuItem);
        await db.SaveChangesAsync(cancellationToken);
        return menuItem;
    }

    public async Task<MenuItem?> UpdateAsync(
        int id,
        MenuItem menuItem,
        CancellationToken cancellationToken = default)
    {
        var existing = await db.MenuItems.FirstOrDefaultAsync(
            item => item.Id == id,
            cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = menuItem.Name;
        existing.Description = menuItem.Description;
        existing.Price = menuItem.Price;
        existing.Quantity = menuItem.Quantity;
        existing.IsAvailable = menuItem.IsAvailable;

        await db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await db.MenuItems.FirstOrDefaultAsync(
            item => item.Id == id,
            cancellationToken);
        if (existing is null)
        {
            return false;
        }

        db.MenuItems.Remove(existing);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
