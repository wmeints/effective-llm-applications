using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(Path.Join("appsettings.json"), optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

var persistentAgentsClient = AzureAIAgent.CreateAgentsClient(
    configuration["Foundry:Endpoint"]!,
    new DefaultAzureCredential());

var persistentAgent = persistentAgentsClient.Administration.GetAgent(
    configuration["Foundry:AgentId"]);

var agent = new AzureAIAgent(persistentAgent, persistentAgentsClient);

var agentThread = new AzureAIAgentThread(persistentAgentsClient);
var responseStream = agent.InvokeAsync(new ChatMessageContent(AuthorRole.User, "..."), agentThread);



