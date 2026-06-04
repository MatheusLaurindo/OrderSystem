using Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Models;

namespace Orders.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly OrdersDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrdersController(OrdersDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            Items = request.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new OrderSubmitted(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList(),
            order.CreatedAt
        ), cancellationToken);

        return Accepted(new
        {
            order.Id,
            order.Status
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }
}

public record CreateOrderRequest(Guid CustomerId, List<CreateOrderItemRequest> Items);
public record CreateOrderItemRequest(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);