using Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;

namespace Orders.Api.Consumers;

public class PaymentApprovedConsumer : IConsumer<PaymentApproved>
{
    private readonly OrdersDbContext _dbContext;

    public PaymentApprovedConsumer(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<PaymentApproved> context)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);

        if (order is null) return;

        order.Status = "Completed";
        await _dbContext.SaveChangesAsync();
    }
}