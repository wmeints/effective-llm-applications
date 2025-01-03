using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithInitBindMount("../database")
    .WithPgWeb();

var applicationDb = postgres.AddDatabase("applicationDb", databaseName: "bookbinder");

var apiProject = builder.AddProject<Bookbinder_Api>("api")
    .WithReference(applicationDb)
    .WaitFor(applicationDb)
    .WithHttpEndpoint(env: "PORT");

var frontendProject = builder.AddNpmApp("frontend", "../apps/frontend", "dev")
    .WithReference(apiProject)
    .WaitFor(apiProject)
    .WithHttpEndpoint(env: "PORT");

builder.Build().Run();
