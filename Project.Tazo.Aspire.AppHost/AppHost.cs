using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var rabbit = builder.AddRabbitMQ("messaging")
    .WithDataVolume()
    .WithManagementPlugin();

// Orders API
var orders = builder.AddProject<Projects.Project_Tazo_Order_Api>("orders-api")
    .WithReference(rabbit);

// Products API
var products = builder.AddProject<Projects.Project_Tazo_Product_Api>("products-api")
    .WithReference(rabbit);

builder.Build().Run();