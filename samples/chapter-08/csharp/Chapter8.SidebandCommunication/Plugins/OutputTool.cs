using System.ComponentModel;
using System.Threading.Tasks;
using Chapter8.SidebandCommunication.Hubs;
using Chapter8.SidebandCommunication.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;

namespace Chapter8.SidebandCommunication.Plugins;

public class OutputTool(IHubContext<UserStoryGenerationHub, IUserStoryGenerationHubClient> hub, string clientId)
{
    [KernelFunction, Description("Store the created user story")]
    public async Task CreateUserStory(string title, List<string> steps)
    {
        UserStoryContent userStoryContent = new()
        {
            Title = title,
            Steps = steps
        };

        await hub.Clients.Group(clientId).UpdateUserStoryContent(userStoryContent);
    }
}