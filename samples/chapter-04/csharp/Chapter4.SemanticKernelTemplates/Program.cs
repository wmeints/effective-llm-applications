﻿using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

// The following code adds configuration support to the application.
// You can securely set the configuration values in the user secrets store.
// Use the command dotnet user-secrets set "<key>" "<value>" to configure a secret value.

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    ).Build();

var promptTemplate = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "prompt.txt"));

var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    MaxTokens = 500,
    Temperature = 0.5,
    TopP = 1.0,
    FrequencyPenalty = 0.0,
    PresencePenalty = 0.0
};

var result = await kernel.InvokePromptAsync(promptTemplate,
    arguments: new KernelArguments(executionSettings)
    {
        ["dish"] = "pizza",
    },
    templateFormat: "semantic-kernel");

 Console.WriteLine(result);