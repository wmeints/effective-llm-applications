{#working-with-structured-output}
# Working with structured output

Building with LLMs means we have to embrace the instability of AI. You never get the same response, and you frequently have to build extra glue code to parse the LLM's output. Working with unstructured output from an LLM is sure to be a challenge, but there are ways to limit the chaos by forcing the LLM to generate output that other program code can parse.

In this chapter, you'll learn two methods to obtain structured output from an LLM. First, we'll talk about working with JSON output, and after that, we'll look at another technique that I like to call sideband communication. This method uses tools to get structured output and regular chat-based output.

We'll cover the following topics in this chapter:

- Why working with structured output is helpful
- Applications that require structured output
- How does structured output work under the hood
- Getting structured output from the LLM
- Working with a sideband channel
- Limitations of structured output

Let's get started by discussing why you want structured output in the first place.

## Why working with structured output is helpful

Working with LLMs can be pure chaos when your applications get bigger. I've run into many problems, particularly with parsing the output generated by an LLM.

In a typical chat application, you can pass the output from the LLM onto the front end without doing much. If you're building a chatbot, you likely don't encounter many challenges of parsing LLM output into a structured format.

However, if you're building a workflow to automate a content-related task, processing the output into a structured format is one of the core operations in your application.

There are numerous ways to process LLM output into a structured format. It usually starts with instructions like: "Output the result in a table", after which you parse the table from the output using a regular expression. Another well-known trick is to ask the LLM to output the content in a fenced markdown code block. You can then parse the fenced markdown code block.

But what happens if the LLM decides not to generate any of those constructs you can parse with a regular expression? Well, it breaks, and it's frustrating. This is not the kind of thing that you want to deal with as a developer.

There is a solution, though. Most LLMs these days allow you to specify a response format, and all of them support doing so by defining a [JSON schema][JSON_SCHEMA_STANDARD] that describes the structure you expect in the output.

And there's more good news: depending on how the LLM provider implemented structured output generation, the LLM follows the output structure flawlessly. OpenAI achieves 100% accuracy in this regard, so you can almost blindly rely on this feature.

Eliminating the content parsing aspect of using an LLM reduces the amount of code in your application, allowing you to focus on building that workflow you need to create for processing blog posts into LinkedIn posts or, even more interesting, translating a prompt into a structured user interface.

## Applications that require structured output

Speaking of applications that work better with structured output, quite a few scenarios benefit from it. For example, structured output benefits anything involving multi-step workflows or intelligent routing patterns.

One application that stands out to me is converting code from one language to another. I've worked on several projects that turned old code into modern equivalents. For one client, I had to translate some weird XML-based low-code solution into typescript, and for another client, a Ruby codebase into C#. Now, this sounds like this: You insert code into the LLM with some instructions, and then it generates the desired code, except that the LLM never does this. Instead, it generates a reply: "Sure, I can help you convert this code. Type in A, then B, and finally C. Good luck!". It's not exactly what I was looking for. But by forcing the LLM to generate the code as a property of a response object in JSON, I could reliably convert weird old code into nice-looking modern code.

Another use case I worked on involved generating [feature files][FEATURE_FILES] from a prompt. I wanted an application where I could enter a prompt that the LLM translated into a feature file. I then wanted to iterate on the results using a chat-based interface with the feature file to the side. I solved the first portion of the application by creating a prompt that returns a structured response, just like I used in the code conversion pipeline. The second part of the application was more complicated. I used a non-existing function to implement a sideband. I declared a function with the LLM that allowed it to edit the feature file. Whenever the LLM returned a tool call result that should invoke the function, I grabbed the tool call. I translated it into an event sent to the frontend via WebSockets, updating the content in the feature film editor. The chat content would also be sent to the frontend but to the chat portion of the screen.

I'm sure there are many more applications that benefit from structured output. But these two applications are prime examples of why having the option to request structured output is essential.

Now, you may be wondering, how does the LLM do this?

## How does structured output work under the hood

In [#s](#llm-output-sampling), we talked about output sampling and how LLMs use this to sound more natural. This goes directly against generating structured output because we can't just sample anything we like if we want valid JSON output. Depending on the kind of LLM you're using, models solve this problem in different ways.

### Instruction-based JSON generation

Some LLM providers, like Claude, only support JSON-based structured output using instructions. If you tell the LLM to output JSON, it will do so in most cases. It's not ideal, though.

Claude, for example, produces valid JSON output in 86% of the cases according to [a test by Datachain.ai][JSON_OUTPUT_TEST]. This percentage is not high enough to be reliable.

There is a way to fix this, though. You can use tool calling as we discussed in [#s](#enhancing-llms-with-tools) to increase the odds of getting structured output. You can declare a tool with the LLM specifying a JSON schema of the production and then [force the LLM to call that tool][FORCE_TOOL_CALL]. The LLM will return a tool call result with the data structured according to the JSON schema you specified. It's nasty work, but it can be done. I'll show this in greater detail later in the chapter.

I like how OpenAI solves this problem much more, though.

### Constrained decoding based JSON generation

Some LLMs, including the [OpenAI-based LLMs][OPENAI_JSON_SUPPORT] and [Google Gemini][GEMINI_JSON_SUPPORT], use constrained output decoding to solve the problem. Here's how that works.

Under normal circumstances, when we're generating a response and need to generate a token, we get to choose from the whole vocabulary subject to the Top-P setting and penalties. However, when applying constrained output decoding, we must overlay a grammar that marks only specific tokens from the vocabulary as valid. Only the valid tokens are passed through the sampling mechanism and will be in the response.

There's a reason why OpenAI supports JSON but not much else for generating a structured response: it has to do with the limitations of the LLM and how some more complex grammars work.

Let me take a step into compiler building for the moment. When building a parser for a language like JSON or C#, you need to determine what a character means at any point in the source code and if it's valid. We need to define a grammar with rules that we can apply to determine what's valid.

For example, if we were parsing a JSON object, we could say that the content should start with an opening bracket `{` followed by a space or double quotes `"`. After that, we expect some string to identify a property ending with another double quote. The following fragment is a simplified representation of a grammar used to parse JSON:

```text
JSON       → Object | Array
Object     → { Members } | { }
Members    → Pair | Pair , Members
Pair       → "string" : Value
Array      → [ Elements ] | [ ]
Elements   → Value | Value , Elements
Value      → "string" 
           | number 
           | true 
           | false 
           | null 
           | Object 
           | Array
```

Here's how to interpret the rules. JSON is either an object or an array. An object has members (properties) or is empty.

The members rule is harder to read here. It defines either a single pair of key and value or a key-value pair followed by more members. So, you can have one property in a JSON object or multiple.

A pair has a string to identify the pair and a value. The value could be a string, a number, true, false, null, or another object or array.

Notice how basic these rules are because this language is context-free. We don't need to know the content of the values to determine what rule to apply for parsing basic JSON.

Languages like C# are much more complicated and often require context to work, so they're unusable for an LLM because it can't look ahead in response to find out the context needed for each grammar rule.

Constrained output decoding ensures that LLMs are 100% accurate in following the structure of a JSON schema. But it doesn't mean it won't output garbage in a perfectly valid structure. We're still using AI.

## Getting structured output from the LLM

To get structured output, we need to specify the output format using a JSON schema. Let's start by looking at a JSON schema and then move on to the Semantic Kernel to understand how to configure it to render structured output.

A JSON schema follows a specific structure specified in the [JSON schema standard][JSON_SCHEMA_STANDARD]. The following fragment shows a basic schema:

```json
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "$id": "https://my-schemas.org/question-generation.schema.json",
  "title": "FeatureFileGenerationResult",
  "description": "A feature file",
  "type": "object",
  "properties": {
    "content": {
        "type": "string"
    }
  },
  "required": ["content"]
}
```

This schema defines an object with a content property. It's not that complicated, but JSON schemas support many different constructions, ranging from simple arrays to type unions. It's not the most straightforward format to work with.

As luck would have it, you don't have to use it when working with Semantic Kernel. In Semantic Kernel, you can specify a type as the output format for a prompt like so:

```csharp
var settings = new AzureOpenAIPromptExecutionSettings
{
    ResponseFormat = typeof(ScenarioResult)
};

var response = await kernel.InvokePromptAsync(
    "Generate a scenario with given, when, then statements for the " +
    "following user story: As I user I want to be able to chat to " +
    "customer support",
    new KernelArguments(settings)
);
```

In the prompt execution settings, we'll use a `typeof` statement to point Semantic Kernel to the C# output we expect to get from the LLM. Under the covers, the object type is parsed to a JSON schema and sent to the LLM using the expected output format.

Note that for Google Gemini models, you need to specify a different prompt execution settings object that looks like this:

```csharp
var settings = new GeminiPromptExecutionSettings
{
    ResponseMimeType = "application/json",
    ResponseSchema = typeof(ScenarioResult)
};

var response = await kernel.InvokePromptAsync(
    "Generate a scenario with given, when, then statements for the " +
    "following user story: As I user I want to be able to chat to " +
    "customer support",
    new KernelArguments(settings)
);
```

It has the same structure as the format specifier for the OpenAI style models in the previous sample.

A> You can also load a JSON schema from disk and let Semantic Kernel use that, but I found that there's no good way to deserialize the data to a C# class if you do that. Unless, of course, you generate a C# class from the schema or use a dictionary. But it feels like a duplication of work. That's why I left it out. If you are interested in this functionality, you can find more about it [in this blog post][JSON_SCHEMA_POST].

Keep in mind that you can only specify objects in the response format. None of the available LLMs don't support arrays as output format and will raise an error if you try to select an array or list. If you need a list of items as output, you will have to nest it as a property of the output object.

It's essential to tell the LLM that you expect JSON output; otherwise, the API will raise an error or, worse, return some random response. It's helpful to include one or two samples in the prompt as well to help the LLM determine what data should go where in the JSON structure. This is especially helpful when creating more complicated output structures. For simple outputs, I skip this step.

Working with structured output has its limitations. Asking for JSON schema-based output incurs extra latency the first time you submit a request with a new schema. This is because the LLM provider needs to compile the schema down to a grammar to overlay.

The JSON schema is cached on the server; while it usually doesn't contain sensitive business data, you should be aware of it. This information remains even if you have a zero-storage agreement with your LLM provider. 

While the model always follows the structure of your JSON schema, the content could be wrong. If you run into issues with invalid values for properties of your output object, I recommend adding samples to the prompt to help the LLM put the correct information into the right spot in the response.

I wish every LLM provider supported the constrained output decoding technique, but they don't. So, you may have to resort to other techniques, like the one we're discussing in the next section.

## Working with a sideband channel

In the previous section, we covered how to get structured output with OpenAI-based models by setting the output format. The output format technique offered by Semantic Kernel doesn't work for models like Gemini and Claude, because they don't allow you to force the LLM into JSON mode like you can with OpenAI.

However, another trick involves calling a tool that can help. Using tool calling to get structured output helps in two scenarios:

1. The LLM supports tool calling but doesn't support constrained output decoding.
2. You want to combine chat with structured output.

Let's explore both scenarios, as they require a slightly different approach to using tools with the LLM.

### Using tool calling to get structured output

We can get structured output from an LLM by forcing it to use a tool. It takes two steps to implement this:

1. First, we must create a tool that stores the input given to it so we can grab it later as the output.
2. Next, we need to configure Semantic Kernel so it always calls the tool.

To demonstrate how to build this setup, we'll use the same sample as before; we're generating a user story using AI. Let's start by setting up a tool defining the expected output structure.

```csharp
public class OutputTool
{
    public string Title { get; set; } = string.Empty;
    public List<string> Steps { get; set; } = new();

    [KernelFunction, Description("Store the created user story")]
    public void CreateUserStory(string title, List<string> steps)
    {
        Title = title;
        Steps = steps;
    }
}
```

In this code, we define a tool called `CreateUserStory` that accepts a title and a list of steps. We're not creating a story here; we're just storing the values generated by the LLM.

To use the tool, we'll need to set up the prompt invocation in a specific way:

```csharp
var outputTool = new OutputTool();
var outputToolPlugin = kernel.Plugins.AddFromObject(outputTool);

var settings = new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Required(
        [outputToolPlugin.First(x => x.Name == "CreateUserStory")])
};

var response = await kernel.InvokePromptAsync(
    "Generate a scenario with given, when, then statements for the " +
    "following user story: As I user I want to be able to chat to " +
    "customer support",
    new KernelArguments(settings)
);

Console.WriteLine(outputTool.Title);

foreach (var step in outputTool.Steps)
{
    Console.WriteLine(step);
}
```

In the code, we perform the following steps to use the tool to get structured output:

1. First, we create a new instance of the output tool and turn it into a plugin.
2. Next, we configure the prompt execution settings and force the LLM to use our tool.
3. Finally, we invoke a prompt to generate a user story.

The LLM must call a tool and invoke the output tool with the contents of the user story. We'll need to inject the tool instance to capture the structured output and make it easily accessible.

Note that forcing the LLM to always call a tool in Semantic Kernel disables the kernel function calling loop we discussed in [#s](#llm-function-calling). This isn't a big problem if you're building a workflow, though. Just make sure you don't need to invoke multiple tools in the same call.

The idea of using tools to get structured output can be extended toward a more complex sideband communication scenario.

### Integrating sideband communication in chat scenarios

If you've used tools like [v0.dev][V0_DEV] and [bolt.new][BOLT_NEW], you're already familiar with how AI tools combine chat with generating content like source code. Often, you'll see the application respond to your request in a chat box while generating code on the side of the screen. There's no official name for this communication style, but I like sideband communication as a nice way to describe this way of working.

Applications like v0 use this style of communication to make the use of tools feel more interactive. It looks nice, but it's also a powerful way to ensure the LLM outputs usable structured data when generating content.

You can use this style of communication, too, in a similar fashion to how we used tools to force the LLM to output structured content. Except this time, we can set the application's function choice behavior to automatic.

```csharp
var outputTool = new OutputTool();
var outputToolPlugin = kernel.Plugins.AddFromObject(outputTool);

var settings = new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var response = await kernel.InvokePromptAsync(
    "Generate a scenario with given, when, then statements for the " +
    "following user story: As I user I want to be able to chat to " +
    "customer support",
    new KernelArguments(settings)
);
```

To get the content from the tool to the front end, you'll need to communicate changes to the front end that the user is using. For web applications, I recommend looking at [SignalR][SIGNALR_DOCS]. It allows you to build a hub to push messages to a Javascript frontend. I won't go into much detail on this topic, but here's an example demonstrating how to build a hub for generating user story content in the frontend:

```csharp
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

public interface IUserStoryGenerationHubClient
{
    public Task UpdateUserStoryContent(UserStoryContent userStoryContent);
}
```

In this code, we create a hub that has the following code:

1. First, we declare a new hub class with an interface called `IUserStoryGenerationHubClient`. This interface defines the methods the server can call in a connected frontend.
2. Next, we'll allow clients to join and leave editing sessions so we know where to send the update for the user story.
3. Finally, we've created a client interface that allows the server to update user story content on the client.

We need to modify the tool to use the hub instead of storing the content in properties in the tool class. The following code demonstrates what the tool looks like when you connect it to a SignalR hub:

```csharp
public class OutputTool(IHubContext<
    UserStoryGenerationHub, IUserStoryGenerationHubClient> hub, 
    string clientId)
{
    [KernelFunction, Description("Store the created user story")]
    public async Task CreateUserStory(string title, List<string> steps)
    {
        UserStoryContent userStoryContent = new()
        {
            Title = title,
            Steps = steps
        };

        await hub.Clients
            .Group(clientId)
            .UpdateUserStoryContent(userStoryContent);
    }
}
```

In the tool, we made the following changes:

1. First, we inject the hub context for our created hub.
2. Next, we create a new content object that we stream to the clients by calling the `UpdateUserStoryContent` method. We're using a group to ensure the content is only sent to relevant clients.

We're using the groups functionality in SignalR to limit communication to only clients interested in seeing content updates. In production, I recommend adding authorization and only allowing clients to join a group when they own a particular conversation (or user story in this scenario). You can find more information about this [in the manual][SIGNALR_AUTH].

To use the hub we created, we must register it in the application startup using the following code:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<UserStoryGenerationHub>("/hubs/userstories");

app.Run();
```

First, we need to set up the required components for SignalR, after which we can register the hub in the application. I've omitted the other configuration code to set up the kernel and the output tool. Let's consider how you can connect the hub to the Semantic Kernel code.

The following code shows a basic chatbot class that can respond to user prompts.

```csharp
public class UserStoryGenerationAgent(IHubContext<
    UserStoryGenerationHub, IUserStoryGenerationHubClient> hub, 
    Kernel kernel)
{
    public async Task<string> GenerateResponseAsync(
        string sessionId, string prompt)
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
```

In this code, we perform the following steps:

1. First, we create a class that hooks up the kernel and the hub.
2. Next, we create a method to generate a response; this method requires a session identifier for the editing session we're in and a prompt from the user. Both come from the frontend application.
3. Then, we connect the output tool to the hub with the session identifier.
4. After, we configure the LLM to allow the use of the configured output tool.
5. Next, we generate a response and return it to the caller.

I've gone for a non-streaming response, but you can use the code we wrote in [#s](#working-with-chat-completion) to implement streaming.

To use the agent in the application, we need to use the following code:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["LanguageModel:DeploymentName"]!,
        endpoint: builder.Configuration["LanguageModel:Endpoint"]!,
        apiKey: builder.Configuration["LanguageModel:ApiKey"]!
    );

builder.Services.AddTransient<UserStoryGenerationAgent>();

var app = builder.Build();

app.MapHub<UserStoryGenerationHub>("/hubs/userstories");

app.MapPost("/sessions/{sessionId}/", async (
    string sessionId,
    [FromBody] GenerateUserStoryResponseForm form,
    [FromServices] UserStoryGenerationAgent agent
) => await agent.GenerateResponseAsync(sessionId, form.Prompt));

app.Run();
```

This code looks similar to how we configured the application before. I've added a couple of extra statements:

1. First, I added the kernel initialization code to connect the application with Azure OpenAI.
2. Then, I configured the agent class we created as a transient dependency. I want a new one every time we have to handle a request.
3. Finally, I created a POST operation allowing users to submit prompts.

Sideband communication makes for some interesting interaction patterns in applications. I love this pattern for chat applications, but I wouldn't use it for workflow-based scenarios. You need a lot more infrastructure for sideband communication to work in production, especially when your user base grows beyond a few hundred users.

I've included the sample code in the [GitHub repository][GH_SAMPLE_CODE] so you can try it and modify it to your needs. It doesn't contain any client code, but if you're interested in learning how to use SignalR from a Javascript client, I recommend checking out the [SignalR client documentation][SIGNALR_CLIENT_DOCS].

## Summary

In this chapter, we discussed two main patterns for generating structured output from an LLM. First, we looked at using constrained output decoding to force the LLM to generate valid JSON content. Then, we looked at how we can use tool calling as a substitute in case we don't have access to an LLM that supports constrained output decoding.

After examining the basic patterns for generating structured output, we examined using sideband communication to generate structured output in chat scenarios.

In the next chapter, we'll examine prompt chaining as a way to combine multiple prompts to generate more refined output. We'll use all the techniques we learned in this chapter and previous chapters to build a more complex scenario that involves processing documents into a single summary.

[FEATURE_FILES]: https://cucumber.io/docs/gherkin/reference/
[JSON_SCHEMA_STANDARD]: https://json-schema.org/
[JSON_OUTPUT_TEST]: https://datachain.ai/blog/enforcing-json-outputs-in-commercial-llms
[FORCE_TOOL_CALL]: https://docs.anthropic.com/en/docs/build-with-claude/tool-use/overview#forcing-tool-use
[JSON_SCHEMA_POST]: https://devblogs.microsoft.com/semantic-kernel/using-json-schema-for-structured-output-in-net-for-openai-models/
[GEMINI_JSON_SUPPORT]: https://ai.google.dev/gemini-api/docs/structured-output?lang=python
[OPENAI_JSON_SUPPORT]: https://openai.com/index/introducing-structured-outputs-in-the-api/
[V0_DEV]: https://v0.dev/
[BOLT_NEW]: https://bolt.new
[SIGNALR_DOCS]: https://dotnet.microsoft.com/en-us/apps/aspnet/signalr
[GH_SAMPLE_CODE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-08/csharp/Chapter8.SidebandCommunication/
[SIGNALR_AUTH]: https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-9.0
[SIGNALR_CLIENT_DOCS]: https://learn.microsoft.com/en-us/aspnet/core/signalr/javascript-client?view=aspnetcore-9.0&tabs=visual-studio
