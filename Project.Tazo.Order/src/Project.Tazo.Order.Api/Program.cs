using Wolverine;
using Wolverine.RabbitMQ;
using Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Aspire te pasa "messaging" como connection string, pero es realmente un URI.
// Convierte a Uri para Wolverine:
var rabbitUri = builder.Configuration.GetConnectionString("messaging");
builder.Host.UseWolverine(opts =>
{
    // Conexión a RabbitMQ desde Aspire
    opts.UseRabbitMq(new Uri(rabbitUri!))
        .AutoProvision()          // crea exchanges/queues/bindings si no existen (dev)
        .AutoPurgeOnStartup();    // opcional (dev): limpia colas al arrancar

    // Publicaremos OrderCreated al exchange "orders.exchange"
    opts.PublishMessage<OrderCreated>().ToRabbitExchange("orders.exchange");
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Endpoint de ejemplo que crea un pedido y publica el evento
app.MapPost("/orders", async (CreateOrder dto, IMessageBus bus) =>
{
    // TODO: guarda el pedido (Dapper/UoW)
    var evt = new OrderCreated(Guid.NewGuid(), dto.ProductId, dto.Quantity);

    await bus.PublishAsync(evt); // publicación asíncrona
    return Results.Accepted($"/orders/{evt.OrderId}", new { evt.OrderId });
});

app.Run();

public record CreateOrder(Guid ProductId, int Quantity);