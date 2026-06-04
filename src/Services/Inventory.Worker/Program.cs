using Inventory.Worker.Consumers;
using Inventory.Worker.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderSubmittedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]!);
            h.Password(builder.Configuration["RabbitMq:Password"]!);
        });

        cfg.ConfigureEndpoints(context);
    });
});

using var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    db.Database.Migrate();

    if (!db.InventoryItems.Any())
    {
        db.InventoryItems.AddRange(
            new Inventory.Worker.Models.InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                ProductName = "Notebook",
                AvailableQuantity = 10
            },
            new Inventory.Worker.Models.InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ProductName = "Mouse",
                AvailableQuantity = 50
            }
        );

        db.SaveChanges();
    }
}

await host.RunAsync();