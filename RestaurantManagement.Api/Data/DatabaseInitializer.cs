using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Data;

public static class DatabaseInitializer
{
    public static async Task SeedAsync(
        RestaurantDbContext db,
        CancellationToken cancellationToken = default)
    {
        if (!await db.RestaurantTables.AnyAsync(cancellationToken))
        {
            db.RestaurantTables.AddRange(
                new RestaurantTable
                {
                    Number = "T1",
                    Capacity = 4,
                    Status = RestaurantValues.Available,
                    IsOccupied = false
                },
                new RestaurantTable
                {
                    Number = "T2",
                    Capacity = 2,
                    Status = RestaurantValues.Available,
                    IsOccupied = false
                });
        }

        if (!await db.MenuItems.AnyAsync(cancellationToken))
        {
            db.MenuItems.AddRange(
                new MenuItem
                {
                    Name = "Burger",
                    Description = "Classic burger",
                    Price = 8.50m,
                    Quantity = 20,
                    IsAvailable = true
                },
                new MenuItem
                {
                    Name = "Salad",
                    Description = "Fresh salad",
                    Price = 6.00m,
                    Quantity = 20,
                    IsAvailable = true
                });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
