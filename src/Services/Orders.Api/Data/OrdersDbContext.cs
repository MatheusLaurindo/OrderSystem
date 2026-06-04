using Microsoft.EntityFrameworkCore;
using Orders.Api.Models;

namespace Orders.Api.Data;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TotalAmount).HasColumnType("numeric(18,2)");
            entity.Property(x => x.Status).HasMaxLength(50);
            entity.HasMany(x => x.Items)
                  .WithOne()
                  .HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ProductName).HasMaxLength(200);
            entity.Property(x => x.UnitPrice).HasColumnType("numeric(18,2)");
        });
    }
}