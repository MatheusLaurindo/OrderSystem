using Inventory.Worker.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Worker.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.ProductId).IsUnique();
            entity.Property(x => x.ProductName).HasMaxLength(200);
        });
    }
}