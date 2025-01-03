using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDataSource("applicationDb");

builder.Services.AddMarten().UseNpgsqlDataSource();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
