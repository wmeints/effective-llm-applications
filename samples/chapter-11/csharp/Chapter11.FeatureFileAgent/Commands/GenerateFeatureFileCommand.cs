using System.ComponentModel;
using Chapter11.FeatureFileAgent.Filters;
using Chapter11.FeatureFileAgent.Plugins;
using Chapter11.FeatureFileAgent.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chapter11.FeatureFileAgent.Commands;

public class GenerateFeatureFileCommand : AsyncCommand<GenerateFeatureFileCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Path to the feature file to generate")]
        [CommandOption("--output|-o")]
        public required string Output { get; init; }

        [Description("Path to the input file containing the task description")]
        [CommandOption("--input|-i")]
        public required string Input { get; init; }

        [Description("The task description to use for generating the feature file")]
        [CommandArgument(1, "[prompt]")]
        public required string[] Prompt { get; init; }

        [CommandOption("--reference|-r")]
        [Description("Path to the documentation folder for reference")]
        public string? ReferencePath { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var configuration = BuildConfiguration();
        var kernel = BuildKernel(configuration, null, settings.Output);
        var agent = new FeatureFileGenerator(kernel);

        // Extract the prompt from the input file if specified, otherwise use the provided prompt.
        // The prompt command argument array is used to enter a full prompt on the command line.
        var prompt = settings.Input is not null && File.Exists(settings.Input)
            ? await File.ReadAllTextAsync(settings.Input)
            : string.Join(" ", settings.Prompt);

        while (prompt != "/exit")
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                await foreach (var chunk in agent.InvokeAsync(prompt))
                {
                    Console.Write(chunk);
                }
            }

            prompt = AnsiConsole.Prompt(new TextPrompt<string>("> "));

            if (prompt == "/clear")
            {
                agent.ClearChatHistory();
                AnsiConsole.MarkupLine("[green]Chat history cleared.[/]");
            }
        }

        return 0;
    }

    private Kernel BuildKernel(IConfiguration configuration, string? referenceDirectory, string outputFilePath)
    {
        var kernelBuilder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                configuration["LanguageModel:DeploymentName"]!,
                configuration["LanguageModel:Endpoint"]!,
                configuration["LanguageModel:ApiKey"]!);

        kernelBuilder.Plugins.AddFromObject(new FeatureFilePlugin(outputFilePath));
        kernelBuilder.Plugins.AddFromObject(new TodoItemsPlugin());

        kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, ToolInvocationFilter>();

        // Only allow the agent to access reference files if a directory is specified.
        if (referenceDirectory != null)
        {
            kernelBuilder.Plugins.AddFromObject(new FileSystemPlugin(referenceDirectory));
        }

        return kernelBuilder.Build();
    }

    private IConfiguration BuildConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Join(".agent", "config.json"), optional: true, reloadOnChange: true)
            .AddJsonFile(Path.Join(".agent", "config.local.json"), optional: true, reloadOnChange: true)
            .Build();

        return configuration;
    }
}