using RestaurantManagement.Api.DTOs;
using RestaurantManagement.Api.Models;
using RestaurantManagement.Api.Repositories;

namespace RestaurantManagement.Api.Services;

public class TableService(ITableRepository repository) : ITableService
{
    private const string DuplicateNumberMessage = "Ya existe una mesa con ese número.";

    public Task<List<RestaurantTable>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public Task<RestaurantTable?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<OperationResult<RestaurantTable>> CreateAsync(
        SaveRestaurantTableDto dto,
        CancellationToken cancellationToken = default)
    {
        var normalizedNumber = dto.Number.Trim();
        if (await repository.NumberExistsAsync(
                normalizedNumber,
                cancellationToken: cancellationToken))
        {
            return OperationResult<RestaurantTable>.Fail(
                OperationFailure.Conflict,
                DuplicateNumberMessage);
        }

        var table = BuildTable(dto, normalizedNumber);
        repository.Add(table);
        if (!await repository.SaveChangesAsync(cancellationToken))
        {
            return OperationResult<RestaurantTable>.Fail(
                OperationFailure.Conflict,
                DuplicateNumberMessage);
        }

        return OperationResult<RestaurantTable>.Success(table);
    }

    public async Task<OperationResult<RestaurantTable>> UpdateAsync(
        int id,
        SaveRestaurantTableDto dto,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return OperationResult<RestaurantTable>.Fail(
                OperationFailure.NotFound,
                "La mesa no existe.");
        }

        var normalizedNumber = dto.Number.Trim();
        if (await repository.NumberExistsAsync(normalizedNumber, id, cancellationToken))
        {
            return OperationResult<RestaurantTable>.Fail(
                OperationFailure.Conflict,
                DuplicateNumberMessage);
        }

        existing.Number = normalizedNumber;
        existing.Capacity = dto.Capacity;
        existing.IsOccupied = dto.IsOccupied;
        existing.Status = dto.IsOccupied
            ? RestaurantValues.Occupied
            : RestaurantValues.Available;

        if (!await repository.SaveChangesAsync(cancellationToken))
        {
            return OperationResult<RestaurantTable>.Fail(
                OperationFailure.Conflict,
                DuplicateNumberMessage);
        }

        return OperationResult<RestaurantTable>.Success(existing);
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

    private static RestaurantTable BuildTable(
        SaveRestaurantTableDto dto,
        string normalizedNumber) =>
        new()
        {
            Number = normalizedNumber,
            Capacity = dto.Capacity,
            Status = dto.IsOccupied
                ? RestaurantValues.Occupied
                : RestaurantValues.Available,
            IsOccupied = dto.IsOccupied
        };
}
