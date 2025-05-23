using Chapter10.IntelligentRequestRouting.Shared;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace Chapter10.IntelligentRequestRouting.Steps;

public class HandleBasicPromptStep(ILogger<HandleBasicPromptStep> logger): KernelProcessStep
{
    [KernelFunction]
    public async Task HandlePromptAsync(Kernel kernel, string prompt)
    {
        logger.LogInformation("Handling basic prompt: {Prompt}", prompt);
        
        var completionService = kernel.GetRequiredService<IChatCompletionService>("basicPrompts");
        var chatHistory = new ChatHistory();
        
        chatHistory.AddSystemMessage(EmbeddedResource.Read("instructions.txt"));
        chatHistory.AddUserMessage(prompt);

        var response = await completionService.GetChatMessageContentsAsync(
            chatHistory,new AzureOpenAIPromptExecutionSettings());
        
        // NOTE: The response is a list of choices that you could request in the past.
        // Now, there's only one choice no matter what model you're using!

        kernel.Data["ResponseContent"] = response[0].Content;
    }
}