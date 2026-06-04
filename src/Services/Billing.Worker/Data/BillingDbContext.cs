using Billing.Worker.Models;
using Microsoft.EntityFrameworkCore;

namespace Billing.Worker.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Amount).HasColumnType("numeric(18,2)");
            entity.Property(x => x.Status).HasMaxLength(50);
        });
    }
}