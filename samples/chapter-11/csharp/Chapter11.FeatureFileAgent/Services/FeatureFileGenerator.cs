using System.ClientModel;
using Chapter11.FeatureFileAgent.Shared;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Polly;
using Spectre.Console;

namespace Chapter11.FeatureFileAgent.Services;

public class FeatureFileGenerator
{
    private ChatCompletionAgent _agent;
    private ChatHistoryAgentThread _agentThread;
    private ChatHistory _chatHistory;
    private ResiliencePipeline _retryPipeline;

#pragma warning disable CS8618 // Fields are non-nullable but initialized in the methods mentiond in the constructor.

    public FeatureFileGenerator(Kernel kernel)
    {
        ClearChatHistory();
        ClearPendingWork();
        InitializeAgent(kernel);
    }

#pragma warning restore CS8618 // Fields are non-nullable but initialized in the methods mentiond in the constructor.

    public async IAsyncEnumerable<string> InvokeAsync(string prompt)
    {
        _chatHistory.AddUserMessage(prompt);

        var responseStream = _retryPipeline.ExecuteEnumerableAsync(context => _agent.InvokeStreamingAsync(_agentThread));

        await foreach (var chunk in responseStream)
        {
            if (chunk.Message is not null && chunk.Message.Content is not null)
            {
                yield return chunk.Message.Content;
            }
        }
    }

    public void ClearChatHistory()
    {
        _chatHistory = new ChatHistory();
        _agentThread = new ChatHistoryAgentThread(_chatHistory);
    }

    public void InitializeAgent(Kernel kernel)
    {
        var instructions = EmbeddedResource.Read("Prompts.AgentInstructions.md");

        var executionOptions = new AzureOpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = 0.7,
            MaxTokens = 8096,
        };

        _agent = new ChatCompletionAgent
        {
            Instructions = instructions,
            Name = "FeatureFileGenerator",
            Kernel = kernel,
            Arguments = new KernelArguments(executionOptions),
        };

        // Configure Polly retry pipeline for HTTP 429 errors
        _retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new Polly.Retry.RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<ClientResultException>(IsRateLimitException),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                MaxDelay = TimeSpan.FromSeconds(60),
                OnRetry = args =>
                {
                    AnsiConsole.WriteLine(
                        $"[red]Retrying due to rate limit.[/] Attempt {args.AttemptNumber + 1}. " +
                        $"Waiting {args.RetryDelay.TotalSeconds} seconds...");

                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    private void ClearPendingWork()
    {
        var temporaryTaskFile = Path.Join(Directory.GetCurrentDirectory(), ".agent", "todo-items.json");

        if (File.Exists(temporaryTaskFile))
        {
            File.Delete(temporaryTaskFile);
        }
    }

    private static bool IsRateLimitException(ClientResultException exception)
    {
        return exception.Status == 429;
    }
}