using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Data;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Repositories;

public class MenuItemRepository(RestaurantDbContext db) : IMenuItemRepository
{
    public Task<List<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.MenuItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

    public Task<MenuItem?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        db.MenuItems
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public Task<MenuItem?> GetTrackedByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        db.MenuItems.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<bool> ReserveStockAsync(
        int id,
        int quantity,
        CancellationToken cancellationToken = default) =>
        await db.MenuItems
            .Where(item =>
                item.Id == id &&
                item.IsAvailable &&
                item.Quantity >= quantity)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(item => item.Quantity, item => item.Quantity - quantity)
                    .SetProperty(
                        item => item.IsAvailable,
                        item => item.Quantity - quantity > 0),
                cancellationToken) > 0;

    public void Add(MenuItem menuItem) => db.MenuItems.Add(menuItem);

    public void Remove(MenuItem menuItem) => db.MenuItems.Remove(menuItem);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await db.SaveChangesAsync(cancellationToken);
}
