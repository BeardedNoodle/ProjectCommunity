var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("portfoliodb");

var server = builder.AddProject<Projects.PortfolioProject_Server>("server")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var webfrontend = builder.AddViteApp("webfrontend", "../../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
