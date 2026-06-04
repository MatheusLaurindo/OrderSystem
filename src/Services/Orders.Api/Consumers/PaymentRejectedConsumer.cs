using Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;

namespace Orders.Api.Consumers;

public class PaymentRejectedConsumer : IConsumer<PaymentRejected>
{
    private readonly OrdersDbContext _dbContext;

    public PaymentRejectedConsumer(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<PaymentRejected> context)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);

        if (order is null) return;

        order.Status = "PaymentRejected";
        await _dbContext.SaveChangesAsync();
    }
}