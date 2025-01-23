using Chapter5.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var configuration  = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// Be careful! This switch should not be on in production unless you're absolutely sure you need this information.
// It exposes PII information to the telemetry collection system. That's bad news!
AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService("Chapter5.Telemetry");

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Microsoft.SemanticKernel*")
    .AddConsoleExporter()
    .Build();

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("Microsoft.SemanticKernel*")
    .AddConsoleExporter()
    .Build();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddConsoleExporter();
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });

    builder.SetMinimumLevel(LogLevel.Information);
});

var kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    );

kernelBuilder.Services.AddSingleton(loggerFactory);

var kernel = kernelBuilder.Build();
    
var prompt = kernel.CreateFunctionFromPromptYaml(
    EmbeddedResource.Read("prompt.yaml"), 
    new HandlebarsPromptTemplateFactory());

var result = await prompt.InvokeAsync(kernel, new KernelArguments
{
    ["dish"] = "pizza",
    ["ingredients"] = new List<string>
    {
        "spinach",
        "mozarella"
    }
});