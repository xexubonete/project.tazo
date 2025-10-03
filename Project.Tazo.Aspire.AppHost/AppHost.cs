using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// RabbitMQ
var rabbit = builder.AddRabbitMQ("messaging")
    .WithDataVolume()
    .WithManagementPlugin();

// PostgreSQL
var postgres = builder.AddPostgres("postgres-db")
    .WithImage("postgres:16-alpine")
    .WithDataVolume()
    .WithHostPort(5432);
// Credenciales en el panel de aspire
// Crear base de datos específica
var tazoDB = postgres.AddDatabase("tazo-db");

// Orders API
var orders = builder.AddProject<Projects.Project_Tazo_Order_Api>("orders-api")
    .WithReference(rabbit)
    .WithReference(tazoDB);

// Products API
var products = builder.AddProject<Projects.Project_Tazo_Product_Api>("products-api")
    .WithReference(rabbit)
    .WithReference(tazoDB);

builder.Build().Run();