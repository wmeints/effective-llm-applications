var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpLogging();

var app = builder.Build();

app.MapOpenApi();

app.UseHttpLogging();

app.MapGet(
        "/api/time", 
        () => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
    )
    .WithDescription("Gets the current date/time.")
    .WithName("get_time")
    .WithTags("time");

app.Run();