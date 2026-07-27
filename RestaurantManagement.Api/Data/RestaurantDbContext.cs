using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Api.Models;

namespace RestaurantManagement.Api.Data;

public class RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : DbContext(options)
{
    public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var table = modelBuilder.Entity<RestaurantTable>();
        table.HasIndex(item => item.Number).IsUnique();
        table.Property(item => item.Number).HasMaxLength(20);
        table.Property(item => item.Status).HasMaxLength(30);

        var menuItem = modelBuilder.Entity<MenuItem>();
        menuItem.Property(item => item.Name).HasMaxLength(120);
        menuItem.Property(item => item.Description).HasMaxLength(500);
        menuItem.Property(item => item.Price).HasPrecision(18, 2);

        var order = modelBuilder.Entity<Order>();
        order.Property(item => item.Status).HasMaxLength(30);
        order.Property(item => item.PaymentMethod).HasMaxLength(20);
        order.Property(item => item.TotalAmount).HasPrecision(18, 2);
        order
            .HasMany(order => order.OrderDetails)
            .WithOne()
            .HasForeignKey(detail => detail.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        var orderDetail = modelBuilder.Entity<OrderDetail>();
        orderDetail.Property(detail => detail.MenuItemName).HasMaxLength(120);
        orderDetail.Property(detail => detail.UnitPrice).HasPrecision(18, 2);
    }
}
