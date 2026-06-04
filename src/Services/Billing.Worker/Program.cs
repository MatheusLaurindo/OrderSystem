using Billing.Worker.Consumers;
using Billing.Worker.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InventoryReservedConsumer>();

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
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
    db.Database.Migrate();
}

await host.RunAsync();