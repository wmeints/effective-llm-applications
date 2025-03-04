using Chapter8.SidebandCommunication.Agents;
using Chapter8.SidebandCommunication.Forms;
using Chapter8.SidebandCommunication.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["LanguageModel:DeploymentName"]!,
        endpoint: builder.Configuration["LanguageModel:Endpoint"]!,
        apiKey: builder.Configuration["LanguageModel:ApiKey"]!
    );

builder.Services.AddTransient<UserStoryGenerationAgent>();

var app = builder.Build();

app.MapHub<UserStoryGenerationHub>("/hubs/userstories");

app.MapPost("/sessions/{sessionId}/", async (
    string sessionId,
    [FromBody] GenerateUserStoryResponseForm form,
    [FromServices] UserStoryGenerationAgent agent
) => await agent.GenerateResponseAsync(sessionId, form.Prompt));

app.Run();
