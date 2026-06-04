using Contracts.Events;
using Inventory.Worker.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Worker.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmitted>
{
    private readonly InventoryDbContext _dbContext;

    public OrderSubmittedConsumer(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        var productIds = context.Message.Items.Select(i => i.ProductId).ToList();
        var inventory = await _dbContext.InventoryItems
            .Where(x => productIds.Contains(x.ProductId))
            .ToListAsync();

        foreach (var item in context.Message.Items)
        {
            var stock = inventory.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (stock is null || stock.AvailableQuantity < item.Quantity)
            {
                await context.Publish(new InventoryRejected(
                    context.Message.OrderId,
                    $"Estoque insuficiente para o produto {item.ProductName}",
                    DateTime.UtcNow
                ));
                return;
            }
        }

        foreach (var item in context.Message.Items)
        {
            var stock = inventory.First(x => x.ProductId == item.ProductId);
            stock.AvailableQuantity -= item.Quantity;
        }

        await _dbContext.SaveChangesAsync();

        await context.Publish(new InventoryReserved(
            context.Message.OrderId,
            DateTime.UtcNow
        ));
    }
}