using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Consumers;
using Orders.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentApprovedConsumer>();
    x.AddConsumer<PaymentRejectedConsumer>();
    x.AddConsumer<InventoryRejectedConsumer>();

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

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.Migrate();
}

app.Run();