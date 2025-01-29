{#enhancing-llms-with-tools}
# Enhancing LLMs with tools

In [#s](#the-art-and-nonsense-of-prompt-engineering) and
[#s](prompt-testing-and-monitoring) we focused on using prompts to build LLM-based
applications. While powerful, prompts aren't going to solve every problem. LLMs don't
have access to the internet, can't generate images (surprise!), and can't interact with
your business information. They can however call tools that can do all of these things.
In this chapter you'll learn how to enhance the capabilities of an LLM with tools
through the use of Semantic Kernel functions.

We'll cover the following topics in this chapter:

- What are tools, skills, and plugins
- When and where to use tools
- Building a tool using a kernel function
- Sharing functions across applications with OpenAPI
- Applying filters to functions
- Using function transformations

Let's first understand what tools are in the context of an LLM, and where they're
useful.

## What are tools, skills, and plugins

Tools, skills, functions, they're all used interchangeably in the context of LLMs. Most
of the providers call them tools and they provide access to capabilities not available
in the LLM itself. For example, you can find a tool on ChatGPT.com that allows the LLM
to search the web. Even image generation on ChatGPT is really just a tool provided to
GPT-4o for it to generate images.

Here's how it works. When you call the LLM you normally provide a prompt and you get a
response. However, you can also provide other data alongside the prompt. You can give
the LLM a list of tools with a specification of what each tool does. The LLM can detect
from the prompt which tool to use and call it for you with data extracted from the
prompt.

There's no guarantuee that the LLM will call your tool though. It is a neural network
trained to detect tool use. However, if your prompt is too unclear or the tool
description doesn't match well enough with the prompt, the LLM will do something
different. So you need to test whether it is calling your tool at the right moments with
the right data.

Tools come in two shapes:

1. **Information retrieval tools**: This category of tools provide information to the
   LLM to generate more grounded responses. For example, you can connect a search engine
   or a database to the LLM to provide additional context information.
2. **Task automation tools**: This category of tools allow the LLM to interact with its
   environment. For example, you can connect a tool that invokes an API to complete an
   order or to send a notification.

As far as the LLM is concerned, it only knows about tools. It doesn't know whether a
tool is an information retrieval tool or a task automation tool.

Now there's one more layer that we need to discuss: plugins. In an enterprise
environment you may have tools that you use across multiple LLM-based applications. You
don't want to copy and paste the same tools across all your applications. To help you
solve this problem, Semantic Kernel introduced the notion of plugins. Plugins contain a
collection of tools related to the same system or category of operations. You can have
plugins in your main LLM-based application, in a separate library or even provide them
as HTTP-based APIs to your LLM-based application. It's up to you to decide the structure
of a plugin, because ultimately, we only use plugins to organize the code in the
application. The LLM doesn't care.

The big question is, when and where should you use tools?

## When and where to use tools

I've found that tools are a great way to extend the capabilities of an LLM. But only so
in a chat-based application. Here's why. You don't control in what order and when tools
are called. This is perfect for chat applications because we don't know how the user
will use the chatbot. They may ask for information in a different order than we expect.

However, if you're building a workflow that has a fixed set of operations that need to
happen in a specific order, you're better off not using tools at all. It's much more
effective to call the database yourself, parse the data, and then call the LLM with the
right information.

If you need to flow information from a response into the next prompt, you maybe
wondering at this point how you're going to make that work with just a workflow. It's
quite hard to parse information from a response in a structured way. As luck would have
it, you can ask for structured responses. We'll discuss this in chapter 7 when we talk
about using LLMs to create structured outputs.

Before we dive into code, it's important to note that at the time of writing not all
LLM providers available in Semantic Kernel support tool use. OpenAI and Azure OpenAI,
for example, are supported, while Microsoft is still working on support for Claude,
OLlama, and many other providers.

Let's take a look at building a tool using a kernel function.

## Building tools using a kernel function

Before we start building a tool in Semantic Kernel, it's good to know that tools are
called functions in Semantic Kernel. The framework has an abstraction for tools in the
form of `KernelFunction`. Every function you ever create will be based on this class.
For the sake of consistency with Semantic Kernel, I'll use the term functions throughout
the rest of this chapter.

There are many ways to build functions in Semantic Kernel:

1. You can turn prompts into functions
2. Functions in C# code can be used too
3. Then there's also the possibility of using API endpoints as functions by importing
   their OpenAPI specification
4. Finally, you can even connect Azure Logic Apps as functions to Semantic Kernel
   applications

There's a lot to choose from here, so let's start by turning a prompt into a function.

### Using prompt-based functions

Now you maybe wondering, why start with prompts? You may have noticed that when building
our first prompts, especially with the YAML format in [#s](#yaml-based-prompts), we
already used functions for a bit. When you load a prompt from a YAML file, you're
creating a prompt-based kernel function. This is a function that the LLM can call,
however we haven't used the function as a tool yet.

To use a prompt-based function as a tool, all we need to do is add it to the kernel as
the following code demonstrates:

```csharp
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
```

This code performs the following steps:

1. First, we create a new kernel based on configuration in the application.
2. Next, we create a new plugin `ghost_writer` with a single function loaded from YAML.

The YAML file used in the sample looks like this:

```yaml
name: generate_outline
description: Generates an outline for a blogpost based on a topic that you provide.
template: |
  Generate an outline for a blogpost about {{ topic }}.
  Output the outline as a list of bullet points.
  
  Outline:
template_format: handlebars
input_variables:
  - name: topic
    description: The topic you want to discuss in the blogpost.
    is_required: true
execution_settings:
  default:
    top_p: 0.98
    temperature: 0.7
    presence_penalty: 0.0
    frequency_penalty: 0.0
    max_tokens: 1200
```

When you call the LLM with the prompt `generate an outline for a blogpost about AI`, the
LLM will call the function `generate_outline` with the input variable `topic` set to
`AI`. The function will then generate an outline for a blogpost about AI. This output is
then returned to the LLM, which will use it to generate a final response to the user.
This all happens thanks to the function calling loop as shown in
[#s](#function-calling-loop-reminder).

{#function-calling-loop-reminder} ![Function calling loop](function-calling-loop.png)

This function calling loop is not part of the LLM, but something that Semantic Kernel
provides. It works like this:

1. First, We call the LLM with one or more messages and a set of tools that the LLM can
   use.
2. Next, the LLM will detect that it needs to call a tool to generate a proper response
   resulting in a tool_call response.
3. Then, Semantic Kernel parses the response and calls the tool with the data provided
   by the LLM.
4. After, the output of the tool is added to the chat history, and the whole thing is
   sent back into the LLM.
5. Finally, if the LLM is satisfied and doesn't need to call more tools, it will
   generate a final response.

Note that the loop can be repeated multiple times. The LLM can call multiple tools
within the scope of processing a prompt. This works the same for functions that call
prompts as well as for other functions such as code-based functions.

### Creating a kernel function in C#

Not all functions should be built with prompts, because you may need to do other things
that you can't do with prompts. It's good to know you can also create functions in C#
code. Here's an example of a very basic function that returns the current date/time.

```csharp
public class TimePlugin
{
    [KernelFunction("get_current_time")]
    [Description("Get the current date and time.")]
    public string GetCurrentTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
```

This code does the following:

1. First, we create a plugin class `TimePlugin` that contains a single function
   `GetCurrentTime`.
2. Next, we add the `KernelFunction` attribute to the function to tell Semantic Kernel
   that this is a function that it can use.
3. Finally, we add the `Description` attribute to provide a description of what the
   function does.

Notice the name of the function `get_current_time`. You would expect me to use
`GetCurrentTime` as the name of the function, because that is what the function is
called in C#. However, I learned that functions are detected much better if you use
snake casing. This comes from the fact that all LLMs are trained mostly on Python code
and are much better at detecting snake case because of this.

To add the function to the kernel, you can use the following code:

```csharp
var kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    );

kernelBuilder.Plugins.AddFromType<TimePlugin>();

var kernel = kernelBuilder.Build();

var chatCompletionService =
    kernel.Services.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
history.AddSystemMessage("You're a digital assistant");
history.AddUserMessage("What time is it?");

var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var response = await chatCompletionService.GetChatMessageContentAsync(
    history, executionSettings, kernel);

Console.WriteLine(response.ToString());
```

In this code we perform the following steps:

1. First, we create a new kernel builder object to construct a kernel with GPT-4o as the
   model based on configuration.
2. Next, we configure a plugin based on a C# type, and use our class `TimePlugin`.
3. Then, we create a new kernel and ask for the current time by sending a message to the
   LLM.

We have to tell Semantic Kernel that it is allowed to call functions. If you don't do
this, the tools available in the kernel aren't provided to the LLM. This is a great
safety measure to prevent the LLM from calling all sorts of functions that you never
intended it to call.

The first argument for the `FunctionChoiceBehavior.Auto()` is a list of functions that
the LLM can call. By providing this a list of functions you control what's available at
what point to the LLM. If you don't provide this argument, all registered functions in
the kernel are available.

Try to change [the sample code][CODE_BASED_FUNCTIONS_SAMPLE] and remove the function
choice behavior to see what happens. The LLM will still provide a response, but will not
be able to reproduce the current date and time.

There's another use for the function choice behavior setting. You can also force the LLM
to call functions instead of generating a response by setting the
`FunctionChoiceBehavior` to `FunctionChoiceBehavior.Required()`. This setting allows you
to set a list of functions too, but this time the list should contain functions that the
LLM must call. The required setting is mostly useful when working with structured output
that we'll discuss in chapter 7. When you use this setting, only the first call to the
LLM will force it to use a tool, otherwise we'd end up with an endless loop.

### Providing functions to the kernel

You may have noticed that the samples in the previous sections use different spots to
introduce functions to the kernel. When creating the prompt-based functions, we added
the plugin with the functions to a kernel instance. In the second sample with the C#
based time function, we added the function to the kernel builder.

You'll want to create kernel instances often and throw them away after you're done.
Kernel instances contain state information in relation to the operation you're
executing, so it's best to create a new one for every request that enters your
application.

By creating a new kernel instance for every request, you also have more control over
what functions are available to the LLM. And that's important because if you provide too
many functions to the LLM, it will have a hard time detecting the right tool and using
it correctly.

Semantic Kernel is flexible in how you provide plugins. When you provide a plugin in the
kernel builder you're saying: I want this plugin to be available to every single kernel
instance in the application. The upside to this approach is that you have just one spot
to define the plugin. The downside is that the functions from the plugin are always
there.

Adding a function to a kernel instance is like saying: At this point in the
conversation, you're allowed to use this tool. It limits the scope of what the LLM can
do, and consequently what users can do. Remember, there are two kinds of users here that
I'm referring to: your friendly colleague, and the maybe less friendly hacker that's
going to use your application too if they get the chance.

### Dependency injection in kernel functions

A time plugin doesn't need any external dependencies, but a plugin that needs to lookup
pieces of a manual will need access to a database or a search engine. This is where it
becomes useful to inject dependencies into your plugin.

There are two levels at which you can define dependencies. First, there's the plugin
level. You can define dependencies in the constructor of your plugin class like so:

```csharp
public class MyPlugin 
{
   public MyPlugin(ApplicationDbContext dbContext)
   {
      // ...
   }
}
```

To use a plugin that has dependencies defined in the constructor, you'll need to
register it using the following C# code:

```csharp
var kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    );

kernelBuilder.Services.AddScoped<ApplicationDbContext>();
kernelBuilder.Plugins.AddFromType<MyPlugin>();
```

This way the kernel will automatically locate the dependencies from the dependency
injection container in your application. In the case of console applications you need to
provide the `ApplicationDbContext` and other services by adding them to the
`kernelBuilder.Services` collection as shown in the sample. For ASP.NET Core it's enough
to add them to the services collection of the application builder instance in the
startup of your web project.

Next to providing dependencies on plugin level, you can also ask for certain
services in the function itself. There following limited set of services you can ask
for by adding a parameter of the corresponding type to your function:

- `Kernel` - Useful when you need to access parts of the kernel in your function.
- `KernelArguments` - The raw arguments provided when executing the function.
- `ILoggerFactory` and `ILogger` - Use this to log information from the function.
- `IAIServiceSelector` - Useful when you need to locate the correct connector to
  invoke a function.
- `CultureInfo` and `IFormatProvider` - Useful when you support multiple languages.
- `CancellationToken` - Used to cancel a long running asynchronous operation.

Some of these services are immediately useful, while others are only useful if
you're building advanced functions. It's important to remember that you'll need
to define dependencies on the plugin class if you want anything beyond basic
helpers to execute your function logic.

If you don't need to inject dependencies, or you want more control over how
dependencies are injected into your kernel functions, I recommend using the
`kernelBuilder.Plugins.AddFromObject` method. You have to create an instance of
your plugin, which takes more effort, but you also have much more control over
how the function is created.

Both operations to include plugins into the kernel are available on the
`KernelBuilder` and the `Kernel` type. That way you have maximum flexibility in
how your application behaves.

### Architecting with plugins

As long as you're building a small to medium-sized LLM-based applications, I recommend
building one or at least very few plugin classes in C#. It will make it easier to manage
the structure of your application this way.

There's a growth path though from smaller to bigger LLM-based application when it comes
to using functions and plugins. As your application grows, you'll need to build more
plugins. It can be useful to move the plugins into separate libraries to make it easier
to navigate the code base.

As soon as you find that you need to work on an application with multiple teams, it can
be beneficial to split plugins into separate microservice style applications and use
OpenAPI to link the plugins to your main application. That's what we'll discuss in the
next section.

## Sharing functions across applications with OpenAPI

## Applying filters to functions

## Using function transformations

- Explain why it is useful to apply function transformations
- URL: https://devblogs.microsoft.com/semantic-kernel/transforming-semantic-kernel-functions/

## Summary

## Further reading

[CODE_BASED_FUNCTIONS_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-06/csharp/Chapter6.CodeBasedFunctions
