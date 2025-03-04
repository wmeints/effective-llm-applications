using Chapter8.SidebandCommunication.Hubs;
using Chapter8.SidebandCommunication.Plugins;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace Chapter8.SidebandCommunication.Agents;

public class UserStoryGenerationAgent(IHubContext<UserStoryGenerationHub, IUserStoryGenerationHubClient> hub, Kernel kernel)
{
    public async Task<string> GenerateResponseAsync(string sessionId, string prompt)
    {
        var outputTool = new OutputTool(hub, sessionId);
        var outputToolPlugin = kernel.Plugins.AddFromObject(outputTool);

        var settings = new AzureOpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var response = await kernel.InvokePromptAsync(
            prompt,
            new KernelArguments(settings)
        );

        // You could return a streaming response, the user 
        // story content is already pushed out to the client 
        // via the hub at this point.

        return response.GetValue<string>()!;
    }
}

