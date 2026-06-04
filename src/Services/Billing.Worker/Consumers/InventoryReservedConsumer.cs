using Billing.Worker.Data;
using Billing.Worker.Models;
using Contracts.Events;
using MassTransit;

namespace Billing.Worker.Consumers;

public class InventoryReservedConsumer : IConsumer<InventoryReserved>
{
    private readonly BillingDbContext _dbContext;

    public InventoryReservedConsumer(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<InventoryReserved> context)
    {
        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            OrderId = context.Message.OrderId,
            Amount = 0,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.PaymentTransactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        await context.Publish(new PaymentApproved(
            context.Message.OrderId,
            transaction.Id,
            DateTime.UtcNow
        ));
    }
}