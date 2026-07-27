using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Data;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Services;

public class TableService(RestaurantDbContext db) : ITableService
{
    public Task<List<RestaurantTable>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.RestaurantTables
            .AsNoTracking()
            .OrderBy(table => table.Number)
            .ToListAsync(cancellationToken);

    public Task<RestaurantTable?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.RestaurantTables.FirstOrDefaultAsync(table => table.Id == id, cancellationToken);

    public Task<bool> NumberExistsAsync(
        string number,
        int? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedNumber = number.Trim().ToLower();
        return db.RestaurantTables.AnyAsync(
            table =>
                (!excludedId.HasValue || table.Id != excludedId.Value) &&
                table.Number.ToLower() == normalizedNumber,
            cancellationToken);
    }

    public async Task<RestaurantTable> CreateAsync(
        RestaurantTable table,
        CancellationToken cancellationToken = default)
    {
        db.RestaurantTables.Add(table);
        await db.SaveChangesAsync(cancellationToken);
        return table;
    }

    public async Task<RestaurantTable?> UpdateAsync(
        int id,
        RestaurantTable table,
        CancellationToken cancellationToken = default)
    {
        var existing = await db.RestaurantTables.FirstOrDefaultAsync(
            current => current.Id == id,
            cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Number = table.Number;
        existing.Capacity = table.Capacity;
        existing.Status = table.Status;
        existing.IsOccupied = table.IsOccupied;

        await db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await db.RestaurantTables.FirstOrDefaultAsync(
            table => table.Id == id,
            cancellationToken);
        if (existing is null)
        {
            return false;
        }

        db.RestaurantTables.Remove(existing);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
