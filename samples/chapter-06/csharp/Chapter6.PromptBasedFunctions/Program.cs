using System.Security.Cryptography;
using Chapter6.PromptBasedFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    );

var kernel = kernelBuilder.Build();

kernel.Plugins.AddFromFunctions("ghost_writer", [
    kernel.CreateFunctionFromPromptYaml(
        EmbeddedResource.Read("generate-outline.yml"), 
        new HandlebarsPromptTemplateFactory()
    )
]);

kernel.FunctionInvocationFilters.Add(new FunctionCallLoggingFilter());

var chat = kernel.Services.GetRequiredService<IChatCompletionService>();

var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Required()
};

var history = new ChatHistory();

history.AddSystemMessage("We're working on a blog post for a blog that focuses on technology topics.");
history.AddUserMessage(
   """
   Write a blog post about "the power of static website generators".
   Write the content in markdown and output them in a fenced code block as shown in the sample.
   
   ### Available tools
   
   Make sure to use the following tools to complete the task:
   
   - generate_outline: Generate an outline for the blog post.
   - refine_section: Refine a section with key talking points.
   
   ## Example
   
   ```markdown
   The content for the blogpost
   ```
   
   ## Steps
   
   Follow these steps exactly.
   
   1. First, create an outline for the blog post. 
   2. Next, refine each section using the `refine_section` tool.
   3. Finally, write the content for the post based on the expanded outline.
   """);

var response = chat.GetStreamingChatMessageContentsAsync(history, executionSettings, kernel);

await foreach (var token in response)
{
    Console.Write(token.Content);
}