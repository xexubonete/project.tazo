using Wolverine;
using Wolverine.RabbitMQ;
using Contracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var rabbitUri = builder.Configuration.GetConnectionString("messaging");
builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(new Uri(rabbitUri!))
        .AutoProvision(); // crea lo declarado abajo si no existe

    // Declaramos el exchange + binding de la cola del consumidor
    // Fanout por defecto: bind sin routing key
    opts.ConfigureRabbitMq()
        .DeclareExchange("orders.exchange", ex => ex.BindQueue("products.order-created"));

    // Este servicio escucha la cola
    opts.ListenToRabbitQueue("products.order-created");
});

// builder.Services.AddDbContext<MyDbContext>((sp, options) =>
// {
//     var connStr = sp.GetRequiredService<IConfiguration>()
//         .GetConnectionString("postgres-db");
//     options.UseNpgsql(connStr);
// });

var app = builder.Build();
app.Run();

public class OrderCreatedHandler
{
    private readonly ILogger<OrderCreatedHandler> _log;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> log) => _log = log;

    // Wolverine detecta este "Handle" por convención
    public Task Handle(OrderCreated message, CancellationToken ct)
    {
        _log.LogInformation("OrderCreated recibido: {OrderId} (Product {ProductId}, Qty {Qty})",
            message.OrderId, message.ProductId, message.Quantity);

        // TODO: lógica de negocio en Products (p.ej., reservar stock)
        return Task.CompletedTask;
    }
}
