using Chapter10.SimpleProcess.Processes;
using Chapter10.SimpleProcess.Steps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddSingleton<IConfiguration>(configuration);

services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        configuration["LanguageModel:ChatCompletionModel"]!,
        configuration["LanguageModel:Endpoint"]!,
        configuration["LanguageModel:ApiKey"]!);

services.AddTransient<CreateArticleProcess>();

var serviceProvider = services.BuildServiceProvider();
var createArticleProcess = serviceProvider.GetRequiredService<CreateArticleProcess>();

await createArticleProcess.StartAsync("The future of AI in education");


