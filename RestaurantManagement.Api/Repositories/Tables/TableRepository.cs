using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Data;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Repositories;

public class TableRepository(RestaurantDbContext db) : ITableRepository
{
    public Task<List<RestaurantTable>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.RestaurantTables
            .AsNoTracking()
            .OrderBy(table => table.Number)
            .ToListAsync(cancellationToken);

    public Task<RestaurantTable?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        db.RestaurantTables
            .AsNoTracking()
            .FirstOrDefaultAsync(table => table.Id == id, cancellationToken);

    public Task<RestaurantTable?> GetTrackedByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        db.RestaurantTables.FirstOrDefaultAsync(table => table.Id == id, cancellationToken);

    public Task<bool> NumberExistsAsync(
        string number,
        int? excludedId = null,
        CancellationToken cancellationToken = default) =>
        db.RestaurantTables.AnyAsync(
            table =>
                (!excludedId.HasValue || table.Id != excludedId.Value) &&
                table.Number.ToLower() == number.ToLower(),
            cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        db.RestaurantTables.AsNoTracking().AnyAsync(table => table.Id == id, cancellationToken);

    public async Task<bool> ClaimAsync(int id, CancellationToken cancellationToken = default) =>
        await db.RestaurantTables
            .Where(table => table.Id == id && !table.IsOccupied)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(table => table.IsOccupied, true)
                    .SetProperty(table => table.Status, RestaurantValues.Occupied),
                cancellationToken) > 0;

    public async Task ReleaseAsync(int id, CancellationToken cancellationToken = default) =>
        await db.RestaurantTables
            .Where(table => table.Id == id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(table => table.IsOccupied, false)
                    .SetProperty(table => table.Status, RestaurantValues.Available),
                cancellationToken);

    public void Add(RestaurantTable table) => db.RestaurantTables.Add(table);

    public void Remove(RestaurantTable table) => db.RestaurantTables.Remove(table);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqliteException
            {
                SqliteErrorCode: 19,
                SqliteExtendedErrorCode: 2067
            })
        {
            return false;
        }
    }
}
