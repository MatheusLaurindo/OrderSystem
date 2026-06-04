using Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;

namespace Orders.Api.Consumers;

public class InventoryRejectedConsumer : IConsumer<InventoryRejected>
{
    private readonly OrdersDbContext _dbContext;

    public InventoryRejectedConsumer(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<InventoryRejected> context)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);

        if (order is null) return;

        order.Status = "InventoryRejected";
        await _dbContext.SaveChangesAsync();
    }
}