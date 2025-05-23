using System.Text.Json;
using Chapter10.IntelligentRequestRouting.Shared;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter10.IntelligentRequestRouting.Steps;

public class RoutePromptStep : KernelProcessStep
{
    [KernelFunction]
    public async Task RoutePromptAsync(Kernel kernel, KernelProcessStepContext context, string prompt)
    {
        var classificationResult = await ClassifyPromptAsync(kernel, prompt);

        if (classificationResult == "gpt-4o")
        {
            await context.EmitEventAsync("HandleComplexPrompt", prompt);
        }
        else if (classificationResult == "gpt-4o-mini")
        {
            await context.EmitEventAsync("HandleBasicPrompt", prompt);
        }
    }

    private async Task<string> ClassifyPromptAsync(Kernel kernel, string prompt)
    {
        var promptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("routing-prompt.yml"), new HandlebarsPromptTemplateFactory());

        var executionSettings = new AzureOpenAIPromptExecutionSettings
        {
            Temperature = 0.1,
            ResponseFormat = typeof(RequestRoutingResponseData),
            // Important: You have to set the AI service to use for this prompt,
            // otherwise this requires a default service registration that we don't
            // have.
            ServiceId = "basicPrompts" 
        };

        var response = await promptTemplate.InvokeAsync(
            kernel, new KernelArguments(executionSettings)
            {
                ["prompt"] = prompt
            });

        var responseData = JsonSerializer.Deserialize<RequestRoutingResponseData>(
            response.GetValue<string>()!);

        return responseData!.Model;
    }
}

public record RequestRoutingResponseData(string Model, string Reason);