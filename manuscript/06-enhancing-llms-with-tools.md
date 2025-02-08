{#enhancing-llms-with-tools}
# Enhancing LLMs with tools

In [#s](#the-art-and-nonsense-of-prompt-engineering) and
[#s](#prompt-testing-and-monitoring), we focused on using prompts to build LLM-based
applications. While powerful, prompts aren't going to solve every problem. LLMs don't
have access to the internet, can't generate images (surprise!), and can't interact with
your business information. They can, however, call tools that can do all of these
things. In this chapter, you'll learn how to enhance an LLM's capabilities with tools
through Semantic Kernel functions.

We'll cover the following topics in this chapter:

- What are tools, skills, and plugins
- When and where to use tools
- Building a tool using a kernel function
- Sharing functions across applications with OpenAPI
- Applying filters to functions

Let's first understand what tools are in the context of an LLM and where they're
helpful.

## What are tools, skills, and plugins

Tools, skills, and functions are all used interchangeably in the context of LLMs. Most
providers call them tools, and they provide access to capabilities not available in the
LLM itself. For example, you can find a tool on ChatGPT.com that allows the LLM to
search the web. Even image generation on ChatGPT is just a tool provided to GPT-4o to
generate images.

Here's how it works. When you call the LLM, you usually provide a prompt with settings
and get a response. You typically give the temperature and other settings we discussed
in [#s](#llm-output-sampling). But you can do more. You can provide the LLM with a list
of tools with a specification of what each tool does. The LLM can detect which tool to
use from the prompt and call it for you with extracted data.

There's no guarantee that the LLM will call your tool, though. It is a neural network
trained to detect tool use. However, if your prompt is too unclear or the tool
description doesn't match well enough with the prompt, the LLM will do something else.
So, you must test whether it calls your tool at the right moments with the correct data.

Tools come in two shapes:

1. **Information retrieval tools**: This tool category provides information to the LLM
   to generate more grounded responses. For example, you can connect a search engine or
   a database to the LLM to provide additional context information.
2. **Task automation tools**: This tool category allows the LLM to interact with its
   environment. For example, you can connect a tool that invokes an API to complete an
   order or to send a notification.

The LLM only knows about tools. It doesn't know whether a tool is an information
retrieval or task automation tool.

Now, there's one more layer that we need to discuss: plugins. In an enterprise
environment, you may have tools that you use across multiple LLM-based applications. You
don't want to copy and paste the same tools across all your applications. To help you
solve this problem, Semantic Kernel introduced the notion of plugins. Plugins contain a
collection of tools related to the same system or category of operations. You can have
plugins in your main LLM-based application or a separate library or even provide them as
HTTP-based APIs for your LLM-based application. It's up to you to decide the structure
of a plugin because, ultimately, we only use plugins to organize the code in the
application. The LLM doesn't care.

The big question is, when and where should you use tools?

## When and where to use tools

I've found that tools are a great way to extend the capabilities of an LLM, but only so
in a chat-based application. Here's why: You don't control what order and when tools are
called. This is perfect for chat applications because we don't know how the user will
use the chatbot. They may ask for information in a different order than we expect.

However, if you're building a workflow with a fixed set of operations that need to
happen in a specific order, you're better off not using tools. It's much more effective
to call the database yourself, parse the data, and then call the LLM with the correct
information.

If you need to flow information from a response into a follow-up prompt, you may wonder
how you will make that work with just a workflow. It's challenging to parse data from a
response in a structured way. As luck would have it, you can ask for structured
responses. We'll discuss this in Chapter 8 when we discuss using LLMs to create
structured outputs.

Before we dive into code, it's important to note that Semantic Kernel has limited
support for calling tools at the time of writing. OpenAI and Azure OpenAI, for example,
are supported, while Microsoft is still working on tool call support for Claude, OLlama,
and many other providers.

Let's take a look at building a tool using a kernel function.

## Building tools using a kernel function

Before we start building a tool in Semantic Kernel, it's good to know that tools are
called functions in Semantic Kernel. Functions you provide to the LLM are derived from
`KernelFunction`. You'll write some functions yourself, while others are generated when
you invoke prompts, for example. For the sake of consistency with Semantic Kernel, I'll
use the term functions throughout the rest of this chapter.

There are many ways to build functions in Semantic Kernel:

1. You can turn prompts into functions
2. Next, you can write methods in C# to use as functions.
3. Then, you can use API endpoints as functions by importing their OpenAPI
   specification.
4. Finally, you connect Azure Logic Apps as functions to Semantic Kernel applications.
 Using Azure Logic App based plugins is a new technique that I won't discuss here. But
 if you're interested, you can learn more about it [here][LOGIC_APPS]

There's much to choose from here, so let's start by turning a prompt into a function.

### Using prompt-based functions

Now, you may be wondering why you should start with prompts. You may have noticed that
when building our first prompts, especially with the YAML format in
[#s](#yaml-based-prompts), we already used functions for a bit. When you load a prompt
from a YAML file, you're creating a prompt-based kernel function. This is a function
that the LLM can call, however we haven't used the function as a tool yet.

To use a prompt-based function as a tool, all we need to do is add it to the kernel, as
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
2. Next, we create a new plugin, `ghost_writer`, with a single function loaded from YAML.

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

When you call the LLM with the prompt `generate an outline for a blog post about AI`,
the LLM will call the function `generate_outline` with the input variable `topic` set to
`AI.` The function will then generate an outline for a blogpost about AI. This output is
then returned to the LLM, which will use it to generate a final response to the user.
This all happens thanks to the function calling loop as shown in
[#s](#function-calling-loop-reminder).

{#function-calling-loop-reminder}
![Function calling loop](function-calling-loop.png)

This function calling loop is not part of the LLM, but something that Semantic Kernel
provides. It works like this:

1. First, We call the LLM with one or more messages and tools that the LLM can use.
2. Next, the LLM will detect that it needs to call a tool to generate a proper response,
   resulting in a tool_call response.
3. Then, Semantic Kernel parses the response and calls the tool with the data provided
   by the LLM.
4. After that, the tool's output is added to the chat history, and the whole thing is
   returned to the LLM.
5. Finally, if the LLM is satisfied and doesn't need to call more tools, it will
 generate a final response.

Note that Semantic Kernel can repeat the loop multiple times if the LLM detects multiple
tool calls in sequence. The LLM can call various tools within the scope of processing a
prompt.

{#calling-functions}
### Creating a kernel function in C#

It's helpful to have the option to run code for things you can't do with prompts. You
can build functions using C#. The following code demonstrates a basic function that
returns the current date/time.

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

1. First, we create a plugin class, `TimePlugin`, that contains a single function
   `GetCurrentTime`.
2. Next, we add the `KernelFunction` attribute to the function to tell Semantic Kernel
   that this is a function that it can use.
3. Finally, we add the `Description` attribute to describe what the function does.

Notice the name of the function `get_current_time`. You would expect me to use
`GetCurrentTime` as the function's name because that is what the function is called in
C#. However, I learned that functions are detected much better using snake casing. LLMs
are trained mostly on Python code and are much better at detecting snake cased
functions.

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

We have to tell the Semantic Kernel that it is allowed to call functions as an extra
safety measure so you, as the developer, know your LLM has access to functions.

The first argument for the `FunctionChoiceBehavior.Auto()` is a list of functions the
LLM can call. By providing this list of functions, you can control what's available at
what point to the LLM. If you don't offer this argument, all registered functions in the
kernel are available.

To see what happens, try changing [the sample code][CODE_BASED_FUNCTIONS_SAMPLE] and
removing the function choice behavior. The LLM will still generate a response but will
not be able to reproduce the current date and time.

There's another use for the function choice behavior setting. You can also force the LLM
to call functions instead of generating a response by setting the
`FunctionChoiceBehavior` to `FunctionChoiceBehavior.Required()`. This setting allows you
to give a list of functions, too, but this time, the list should contain functions that
the LLM must call. The required setting is mostly useful when working with structured
output, which we'll discuss in chapter 7. When you use this setting, only the first call
to the LLM will force it to use a tool; otherwise, we'd end up with an endless loop.

### Providing functions to the kernel

You may have noticed that the samples in the previous sections use different spots to
introduce functions to the kernel. When creating the prompt-based functions, we added
the plugin with the functions to a kernel instance. In the second sample, we added the
function to the kernel builder with the C#-based time function.

You'll likely create kernel instances often and throw them away after you're done.
Kernel instances contain state information about the operation you're executing, so
creating a new one for every request that enters your application is best.

Creating a new kernel instance for every request gives you more control over what
functions are available to the LLM. That's important because if you provide too many
functions to the LLM, it will have difficulty detecting and using the right tool
correctly.

Semantic Kernel is flexible in how you provide plugins. When you provide a plugin in the
kernel builder, you're saying, "I want this plugin to be available to every single
kernel instance in the application." This approach has the upside that you have just one
spot to define the plugin. The downside is that the functions from the plugin are always
there even if you don't want them to be available.

Adding a function to a kernel instance is like saying, "I know we're having this
conversation, but I'd like you to use a tool for just this question. Thanks." It limits
the scope of what the LLM can do and, consequently, what users can do. Remember, you may
have some unwanted visitors (hackers) in your application who could very well use the
function to abuse your system. 

### Dependency injection in kernel functions

A time plugin doesn't need any external dependencies, but a plugin that needs to look up
pieces of a manual will need access to a database or a search engine. This is where
injecting dependencies into your plugin becomes useful.

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

Adding a function through the dependency injection container lets the kernel
automatically locate the dependencies for your function. When building a console
application, you can provide dependencies to the Services collection in the
KernelBuilder instance. For ASP.NET Core it's enough to add them to the services
collection of the application builder instance in the startup of your web project as you
can see in the following code:

```csharp
var builder = WebApplication.CreateBuilder(args);

var kernelBuilder = builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    );

kernelBuilder.Plugins.AddFromType<MyPlugin>();

builder.Services.AddDbContext<ApplicationDbContext>();
```

Next to providing dependencies on the plugin level, you can also ask for specific
services in the function itself. There is the following limited set of services you can
ask for by adding a parameter of the corresponding type to your function:

- `Kernel` - Useful when you need to access parts of the kernel in your function.
- `KernelArguments` - The raw arguments provided when executing the function.
- `ILoggerFactory` and `ILogger` - Use this to log information from the function.
- `IAIServiceSelector` - Useful when locating the correct connector to invoke a
 function.
- `CultureInfo` and `IFormatProvider` - Useful when you support multiple languages.
- `CancellationToken` - Used to cancel a long-running asynchronous operation.

Some services are immediately useful, while others are only useful if you build advanced
functions. It's important to remember that you'll need to define dependencies on the
plugin class if you want anything beyond basic helpers to execute your function logic.

If you don't need to inject dependencies, or you want more control over how dependencies
are injected into your kernel functions, I recommend using the
`kernelBuilder.Plugins.AddFromObject` method. You have to create an instance of your
plugin, which takes more effort, but you also have much more control over how the
function is created.

Both operations to include plugins into the kernel are available on the `KernelBuilder`
and the `Kernel` type. That way, you have maximum flexibility in how your application
behaves.

### Architecting with plugins

If you're building small to medium-sized LLM-based applications, I recommend building
one or at least very few plugin classes. It will make it easier to manage the structure
of your application this way.

When using functions and plugins, though, there's a growth path from smaller to bigger
LLM-based applications. As your application grows, you'll want to refactor the functions
and move them into separate plugins. It can be useful to move the plugins into separate
libraries to make it easier to navigate the code base.

As soon as you need to work on an application with multiple teams, it can be beneficial
to split plugins into separate microservice-style applications and use OpenAPI to link
them to your main application. We'll discuss this in the next section.

## Sharing functions across applications with OpenAPI

You will usually be OK building plugins for Semantic Kernel in C#. However, I like to
explore working with external plugins through OpenAPI because it's a great way to make
it easier for your organization to explore generative AI use cases.

Integrating plugins through OpenAPI requires a two-step process:

1. First, you need to write a web API that exposes LLM functions.
2. After that, you can integrate the external API into the main LLM-based project.

Before we dive into the code, let's discuss why you'd want to go through this process.

### Why use external API projects as plugins

Building functions into separate external API projects is a great way to share functions
across teams. However, it's also more complicated because you must account for
additional build and deployment steps. I haven't had a case where my application became
so big that I had to move plugins into separate libraries.

There is another reason why you might consider using this pattern. Many organizations
are considering building internal chatbots similar to ChatGPT to help their employees
with various productivity challenges. They're building custom solutions because they
fear that OpenAI or other LLM providers may use their internal data for training
purposes. So far, I haven't seen anything in the terms of service or privacy statement
suggesting that OpenAI is doing this. Having more control over your data and the user
interface is still a good idea, but it will come at a cost.

Building one chatbot for all use cases in your organization is challenging. Rebuilding
the user interface and general control flow is a lot of work, too. Injecting API-based
applications into Semantic Kernel allows a shared core chatbot solution with
configurable plugins for different use cases.

Let's explore how to build an OpenAPI-based plugin for Semantic Kernel.

### Setting up an API as a plugin

You can use any technology to build an API you want to connect to Semantic Kernel if it
has an OpenAPI specification. OpenAPI is one of the most well-known methods to describe
the structure of a web-based API. It's available in almost any language that supports
hosting a website. For this book, I will stick to building one in ASP.NET Core.

To create a web API project in ASP.NET Core, you can use the following terminal command:

```bash
dotnet new web -n TimeApi
```

This command creates a new web project in the directory `TimeApi.` Next, you'll need to
add a package to configure the OpenAPI specification for the project:

```bash
dotnet add package Microsoft.AspNetCore.OpenAPI
```

After you've added the OpenAPI package, you can use it to configure an OpenAPI endpoint
in the application. The following code demonstrates how to setup OpenAPI in ASP.NET
Core:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapGet(
        "/api/time", 
        () => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
    )
    .WithDescription("Gets the current date/time.")
    .WithName("get_time")
    .WithTags("time");

app.Run();
```

This code performs the following steps:

1. First, we create a new builder for the web application.
2. Next, we add the OpenApi services to the service collection.
3. Then, we build the application and map the OpenAPI specification to
   `/openapi/v1.json`.
4. After that, we add an operation we want to include in the application.

When you're mapping an operation to an endpoint in ASP.NET Core, it's automatically
included in the OpenAPI specification, so you don't need to do anything special.
However, by default, the operation follows the C# naming convention, which doesn't work
well with LLMs, as discussed in [#s](#calling-functions). To help the LLM make sense of
the operation, we must add `WithName` to the operation mapping. Also, include
`WithDescription` to help the LLM better understand the operation.

Building an external plugin is the same as creating a regular REST API in ASP.NET Core.
Now, all we need to do is integrate the API into the main LLM-based application.

### Integrating the API into your main project

To integrate an external API into Semantic Kernel, we'll need to add the package,
`Microsoft.SemanticKernel.Plugins.OpenAPI` to the main LLM-based project. You can do
this through your favorite package manager or via the terminal using the following
command:

```bash
dotnet add package Microsoft.SemanticKernel.Plugins.OpenApi --prerelease
```

Note the `--prerelease` flag in the command. At the time of writing, Using OpenAPI-based
plugins is still in preview, so expect changes to how the code to register a plugin
works.

You can link an external plugin using the following code:

```csharp
var kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
         deploymentName: configuration["LanguageModel:DeploymentName"]!,
         endpoint: configuration["LanguageModel:Endpoint"]!,
         apiKey: configuration["LanguageModel:ApiKey"]!
    );

var kernel = kernelBuilder.Build();

await kernel.ImportPluginFromOpenApiAsync(
   "time", new Uri("http://localhost:5019/openapi/v1.json"));

var arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
{
   FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
});

var response = await kernel.InvokePromptAsync(
   "Can you tell me what time it is?", arguments);

Console.WriteLine(response.ToString());
```

This code does the following:

1. First, we create a new kernel instance with the `KernelBuilder` we've seen in
   previous samples.
2. Next, we import a plugin from an OpenAPI specification. We give the plugin the name
   `time` and point it to a local instance of the time plugin we just made.
3. Then, we create a new set of arguments for the prompt, telling Semantic Kernel that
   it can run functions.
4. Finally, we run a prompt asking for the current time with the arguments we created.

The combination of the external API with the main LLM-based application is more complex.
Before running this sample, you'll need to ensure you have a copy of `Chapter6.TimeApi`
running on your local machine. Once you've started the API, you can run
`Chapter6.ExternalApiPlugins` to see the outcome of the code sample.

When you run [the sample code from the Github repository][OPENAPI_SAMPLE], you'll see
that the response contains the current date/time. You can verify that the external API
is used by checking the terminal output in the API project. I've added HTTP logging to
the sample code so it's easier to see when the API is called.

In previous samples, we used the `KernelBuilder` to ensure that plugins are available to
all kernel instances in your application. This is not possible for external plugins. You
need to import the plugin every time you create a new instance of the kernel because the
OpenAPI specification is only available at runtime and you don't want your application
to crash if an external API is not available to you during startup.

Using external APIs means you have to deal with the fact that you can't change the
behavior of the external API from the LLM-based application. If you own the API, you can
modify it to suit your needs but that's not always the case, especially if you share one
API across multiple applications.

Filters allow you to modify the input and output of functions regardless of how you
implemented the function. Let's take a look at how these work and when it's helpful to
implement a filter.

## Applying filters to functions

Function filters in Semantic Kernel are pieces of logic that work as a middleware for
functions. When you add a filter to the kernel, it gets wrapped around each function
call made by the kernel. Filters are executed in a chain, so multiple filters modify the
interaction with a function. [#s](#function-filter-architecture) shows the architecture
of filters within Semantic Kernel.

{#function-filter-architecture}
![Function filter architecture](function-filter-architecture.png)

You can use functions for several things. I like to use filters to check the user's
permissions on behalf of whom I'm calling a function. This way, I can make sure that
we're not accidentally giving responses to questions that the user shouldn't get an
answer to.

Another excellent filter application is capturing information from a function call
before it goes back into the LLM. Capturing data retrieved by a function is especially
useful when you want to implement the RAG pattern (see Chapter 7) and need to store
references to the original documents found.

Let's look at how you can build a function filter so you understand how to use them in
patterns like the RAG design pattern. The following code demonstrates a basic filter
that logs the input and output of a function:

```csharp
using Microsoft.SemanticKernel;

public class LoggingFunctionFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(
        FunctionInvocationContext context, 
        Func<FunctionInvocationContext, Task> next)
    {
        Console.WriteLine($"Invoking {context.Function.Name}");

        await next(context);
        
        Console.WriteLine($"Done invoking {context.Function.Name}");
    }
}
```

The code performs the following steps:

1. First, we create a new class that implements `IFunctionInvocationFilter`.
2. Next, we add a method `OnFunctionInvocationAsync` that accepts the invocation context
   and the next filter in the chain.
3. Then, in the method `OnFunctionInvocationAsync`, we log the name of the function
   that's being called before and after calling the `next` filter.

There's something special happening when you look at the output in
[#s](#filter-sample-output), since we're using `InvokePromptAsync`, we're calling a
function wrapped around the prompt we provide. The output in shows two calls happening,
one for the prompt function and one for the time function.

{#filter-sample-output}
![Function filter sample output](filter-sample-output.png)

The example is quite simple, and I don't recommend adding a logging filter, because
logging is already built into Semantic Kernel. However, this sample does demonstrate how
you can modify the input and output of a function before and after it's called.

For example, you can access the `Arguments` property on the `FunctionInvocationContext`
to modify the function's input. You can also modify the `Result` property to change the
function's output.

Finally, you can decide not to let the LLM call the function. If you don't call the
`next` function, it's required to provide a value for `Result`. Otherwise, the
application will throw an exception.

Filters work best in the following cases:

1. When you can't change the logic in the function itself.
2. When you need to apply a transformation on all functions in the application.

Functions and filters are what make Semantic Kernel powerful. With these two constructs
on top of the kernel, you can build all the patterns from the upcoming chapters.

## Summary

In this chapter we covered how to extend the capabilities of an LLM with functions. We
learned how to use functions to access external tools like relational databases and
search engines. We also covered how to use functions to manipulate state outside the LLM
by, for example, sending notifications.

We then covered how to work with functions in larger applications by moving them into
separate services and connect them by using the Semantic Kernel OpenAPI plugins
extension.

Finally, we covered how to work with filters to validate input for functions and change
function output. We learned that filters can be useful to implement security checks into
your application.

In the next chapter, we'll use the skills obtained in this chapter to learn how to apply
Retrieval Augmented Generation (RAG) to build intelligent chat applications that can use
your organization's internal information to generate useful responses to questions of
users.

## Running the samples in this chapter

Check out the samples for this chapter in the [Github repository][GH_SAMPLE_DIR]. Each sample has a README file containing system requirements and instructions on how to run the sample.

[CODE_BASED_FUNCTIONS_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-06/csharp/Chapter6.CodeBasedFunctions
[OPENAPI_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-06/csharp/Chapter6.ExternalApiPlugins
[LOGIC_APPS]: https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/adding-logic-apps-as-plugins
[GH_SAMPLE_DIR]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-06/