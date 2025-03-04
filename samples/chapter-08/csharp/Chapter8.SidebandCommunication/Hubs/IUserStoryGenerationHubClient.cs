using Chapter8.SidebandCommunication.Models;

namespace Chapter8.SidebandCommunication.Hubs;

public interface IUserStoryGenerationHubClient
{
    public Task UpdateUserStoryContent(UserStoryContent userStoryContent);
}