using System.Threading.Tasks;
using Chapter8.SidebandCommunication.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chapter8.SidebandCommunication.Hubs;

public class UserStoryGenerationHub : Hub<IUserStoryGenerationHubClient>
{
    public async Task JoinEditingSessionAsync(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
    }

    public async Task LeaveEditingSessionAsync(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
    }
}
