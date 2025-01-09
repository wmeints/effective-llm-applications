{#getting-started-with-semantic-kernel}
# Getting Started with Semantic Kernel

In chapter 2, we covered essential LLMOps knowledge. In this chapter, we're finally
going to talk about code. We'll get started understanding one of the frameworks you can
use to build LLM-based applications: Semantic Kernel. We'll cover the following topics:

- Understanding Semantic Kernel
- Setting up your development environment
- Setting up a project with Semantic Kernel
- Running a basic prompt with Semantic Kernel

By the end of the chapter you'll have a solid foundation to start exploring how to write
effective prompts and design applications using the design patterns in the upcoming
chapters. Let's get started!

## Understanding Semantic Kernel

You can build LLM-based applications by calling the API of the LLM providers directly.
Calling the LLM provider directly through their API or package is the best way to get
access to the latest and greatest features. However, I found that applications built
this way can be challenging to manage over the long term. You have to deal with a lot of
boilerplate code, and if you need to switch provider, you have to rewrite a lot of code.

A good framework will solve the boilerplate code for you while providing enough
flexibility to implement the design patterns you need. Semantic Kernel is such a
framework that's flexible but provides enough value to make it worthwile to use.

### What is Semantic Kernel

Semantic Kernel is an open-source framework from Microsoft that helps you build
LLM-based applications. Its main purpose is to abstract away the integation code needed
to talk to various LLM providers so you can easily switch. Microsoft focuses on providing
an enterprise-grade solution to build LLM-based applications. You can be sure that
the concepts from chapter 2 are implemented in the framework.

I must warn you that the framework is pretty new and constantly evolving just like the
other LLM-based application frameworks out there. So you should be prepared to regularly
update your code as the framework evolves. Microsoft does make the updates relatively
easy to apply as long as you don't use any of the experimental features. Since Semantic
Kernel reached version 1+ Microsoft promises that the non-experimental
features are stable and don't receive breaking changes until 2.0 arrives.

I'll make sure to point out which of the features are still experimental so you can make
an informed choice about regardless of whether you want to use them. You can also check
the [product documentation][SEMANTIC_KERNEL_DOCS] to find out if a feature you want to
use is experimental or not.

Let's look at the core concepts and the architecture of Semantic Kernel so you'll know
what to expect when you start using it.

{#core-concepts-and-architecture}
### Core concepts and architecture

The architecture for Semantic Kernel is based around the concept of a Kernel that
connects to functions, an LLM provider, and filters. [#s](#semantic-kernel-architecture)
shows the architecture of the framework.

{#semantic-kernel-architecture}
![Semantic Kernel Architecture](semantic-kernel-architecture.png)

After trying multiple frameworks I've found that Semantic Kernel fits quite well in the
mental model of an application that is built around an intelligent core that uses AI and
logic to fill in parts of the intelligent behavior. The kernel fills the role of the
core and the functions, filters, and LLM provide the intelligent components around the
core.

The mental model provided by Semantic Kernel makes it so that you can isolate the AI
portion of your solution to a subsystem that can be replaced or upgraded without
affecting everything else in the application. This is important, because the
architecture of LLM-based applications will change quickly over the next few years as we
learn more about the capabilities and limitations of LLMs.

In [#s](#understanding-llms), we covered how LLMs generate a response. The kernel plays
an important role in this process. Let's explore what happens when you run a prompt
through the kernel.

{#kernel-interactions}
![Kernel interactions](kernel-interactions.png)

When you ask Semantic Kernel to execute a prompt for you, it will first determine what
AI service to use. An AI service in Semantic Kernel can be an LLM or another AI service
that takes a prompt to generate a response.

You can use Semantic Kernel for more than just text, for example you can:

- Generate text and then translate it to audio (text-to-audio)
- Generate an image from a prompt (text-to-image)
- Process an image to extract meaning from it (image-to-text)
- Process audio to text (audio-to-text)
- Generate embeddings for a document (embedding-generation)

It's still early days for Semantic Kernel, so most of these uses are experimental.
That's why we're focusing on working on text-based tasks only in this book. But it's
good to know that you can do more than just process text.

After selecting the ideal AI service to execute a prompt, the prompt is rendered if
you're using a prompt template. The prompt templating engine takes the input you
provided and combines it with other text to produce a full prompt. This is helpful,
because you can reuse prompts across multiple use cases in your application or across
applications.

When the prompt is rendered, it is sent to the LLM provider. In
[#s](#llmops-rate-limits) we discussed that sometimes the LLM provider may not be
available, or you may have run out of quota. Semantic Kernel makes sure to handle these
failure modes for you as much as possible.

When the response comes back from the LLM, we need to process it. The monitoring data we
discussed in [#s](#llmops-monitoring) is automatically generated for you so you
can export it through tools like Application Insights or OpenTelemetry.

Ultimately, the result of a prompt execution is returned to you. Depending on what you
asked for, you may need structured data or just plain text. Semantic Kernel will make
sure that the data is deserialized as necessary so you can work with it in your
application.

The kernel allows you to inject filters at various points in the process. This is useful
to for example filter out harmful responses (see [#s](#llmops-user-safety)) or
to remove PII data in the request (see [#s](#llmops-data-privacy)).

While the kernel is the core of Semantic Kernel, the functions are the most important
part of the framework. Functions will provide the power needed to turn the LLM from a
simple text generator into the promised semantic intelligence of your application.

### Calling Functions

In Semantic Kernel, you can connect functions to the kernel to provide it with
additional functionality. Functions play an important role in LLM-based applications,
because you can use them to:

- Perform actions based on the prompt of the user.
- Find relevant content to enhance the response to the prompt.

We will cover functions in great depth through chapters 5 and 6, for now it's important
to understand a trick I like to call the kernel loop in relation to function calling.

Modern LLMs support function calling and can, if you design your application right,
combine several calls to functions to complete a reasonably complex task.
[#s](#function-calling-loop) demonstrates the pattern that Semantic Kernel uses to turn
the LLM into a planner by using the LLM's function calling capabilities. The function
calling flow takes place when the rendered prompt is submitted at the end of the prompt
execution flow in [#s](#kernel-interactions).

{#function-calling-loop}
![Function calling loop](function-calling-loop.png)

This may come as a bit of a surprise. We can't just submit the rendered prompt to the
LLM, we need to turn it into a chat history object and submit that to the prompt. While
an LLM only works with an input sequence to produce an output sequence, all modern LLMs
are trained as if they're chatbots. They use conversations as a concept to generate
better responses.

The concept of a conversation will sound familiar to you if you've ever worked with
tools like ChatGPT. The idea is great because if you train the LLM just right, you can
use the concept of a conversation to guide the LLM to follow a plan. Also, you can use
the concept of a conversation to let the user refine their input in response to the
LLM's output.

At first, I thought it was silly to use the concept in a context other
than a chatbot, but now I see the power of it. You'll see me use the concept of a
conversation in the design patterns in the upcoming chapters a lot.

Let me be clear here, before you start thinking that LLMs are chatbots, they still
aren't. Although they're trained as if they are. The conversation you submit to the LLM
is flattened to a text sequence with special markers to delimit the start and end of
messages in the conversation. The LLM doesn't produce a conversation as output, it will
only spit out tokens that you can combine to a response. Frameworks like Semantic Kernel
will turn the response back into a response message and add it to the chat history
object if you like.

Going back to function calling: once we have a conversation history object, the kernel
will send it to the LLM, and wait for a response.

If the response from the LLM is a `tool_call` response, the kernel will find the
function identified by the `tool_call` and execute it with the parameter values provided
by the LLM. The result of the function call is added to the history object as a tool
message and the new history is submitted to the LLM again. This process repeats until
the LLM returns a response that doesn't contain a `tool_call`.

You can do some very powerful planning with this. If you design a prompt that tells the
LLM to follow a plan and mention tools in your plan, it is very likely it will follow
the plan. We'll use the fact that Semantic Kernel has this loop in the design patterns
in upcoming chapters.

### Language support

The idea of using a semantic core with an LLM, functions and filters in your application
is universal. It works in all programming languages. Semantic Kernel is available in C#,
Python, and Java. I chose C# for this book because I like C# as a language, but you
shouldn't feel limited by my preference.

With support for both Python and Java, Semantic Kernel covers all popular languages in
the enterprise space right now. But you should be aware that support for Python and C#
is the furthest along. Microsoft is still working on properly supporting all features in
Java.

You can find a full list of supported features for each language in the [language
support matrix][LANGUAGE_SUPPORT]. The matrix will tell you which features are supported
in each of the languages and what connectors are available for each of the languages.

While I only work with C# in this book, I've made sure there are samples available in 
Python and Java as well where possible. I'll point you to the right resources when we
get to those parts of the book.

Now that you have a good understanding of the core components, let's put them into
practice by setting up a development environment and building a project with Semantic
Kernel.

## Setting up your development environment

Before you can start using Semantic Kernel, you need to set up your development
environment. I recommend getting these tools set up:

- [.NET 9.0 SDK or later](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio Code](https://code.visualstudio.com), [Rider 2024.3](https://www.jetbrains.com/rider/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)
- Access to the [OpenAI API](https://openai.com/api/) or [Azure OpenAI](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource?pivots=web-portal)

### Other supported LLM providers

Although I refer only to GPT-4 through the OpenAI API or Azure OpenAI, these are not the
only supported LLMs for Semantic Kernel. There are numerous LLM providers that are
supported in Semantic Kernel. They're adding more and more with each release. At the
time of writing there's support for:

- OpenAI
- AzureOpenAI
- Amazon Bedrock
- Anthropic
- OLLama
- Google
- Hugging Face
- ONNX

I'm not covering other LLM providers here, but you can certainly use them with Semantic
Kernel. Just make sure to use the right connector package. We'll discuss those in the
next section when we set up a project with Semantic Kernel.

## Setting up a project with Semantic Kernel

Most people will build some sort of web API around a LLM use case. But Semantic Kernel
can be used in more places than just web applications. Although to be honest, you can't
use it on your mobile device right now through MAUI. To help you get started, I want to
provide you with two scenarios:

- Using Semantic Kernel in a standalone console application
- Using Semantic Kernel in an ASP.NET Core application

Let's start with a basic console application to get you started.

### Setting up Semantic Kernel in a console application

To set up a console application with Semantic Kernel, you need to create a new console
application in Visual Studio Code, Rider, or Visual Studio. I prefer to use a terminal
with Visual Studio Code, but you can use the IDE you like. Execute the following command
in a terminal to start a new console application in C#:

```bash
dotnet new console -n Chapter1.ConsoleApp
```

This command will create a new console application in a folder called
`Chapter1.ConsoleApp`. You can open the folder in Visual Studio Code by executing the
following command:

```bash
code Chapter1.ConsoleApp
```

In Visual Studio Code, you'll see the project file for the console application called
`Chapter1.Console.csproj` and a `Program.cs` file that will contain the main program
code.

Let's add the Semantic Kernel package to the project by executing the following commands
in the project directory inside a terminal window:

```bash
dotnet add package Microsoft.SemanticKernel
dotnet add package Azure.Identity
```

This command will install the Semantic Kernel package that has references to the
following packages:

- `Microsoft.SemanticKernel.Core`\
  Contains the core components we discussed in [#s](#core-concepts-and-architecture)
- `Microsoft.SemanticKernel.Abstractions`\
  Contains the base layer for various types of connectors.
- `Microsoft.SemanticKernel.Connectors.AzureOpenAI`\
  Contains the connector to Azure OpenAI.
- `Microsoft.SemanticKernel.Connectors.OpenAI`\
  Contains the connector to OpenAI

The Semantic Kernel package provides convenience if you don't want to worry about
finding the individual packages. If you care about the size of your application, you can
install the individual packages instead.

I've included the `Azure.Identity` package because it's a good practice to use managed
identities when you're working with Azure services. This package provides the
`DefaultAzureCredential` class that you can use to authenticate with Azure services
without the need to store secrets with your application. As an alternative you can also
specify an API Key for the client.

With the packages installed, you can configure the kernel object we discussed in
[#s](#core-concepts-and-architecture) in the `Program.cs` file.

```csharp
using Microsoft.SemanticKernel;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI;

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

var apiClient = new AzureOpenAIClient(
    new Uri("https://<your-endpoint>"),
    new DefaultAzureCredential());

kernelBuilder.AddAzureOpenAIChatCompletion(
    deploymentName: "gpt-4o",
    azureOpenAIClient: apiClient);

// ALTERNATIVELY: Use the OpenAI API.

// var apiClient = new OpenAIClient(
//     Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

// kernelBuilder.AddOpenAIChatCompletion("gpt-4o", apiClient);

Kernel kernel = kernelBuilder.Build();
```

Let's go over the code step by step:

1. First, we import the necessary namespaces for the kernel and the connectors.
2. Then, we create a new `IKernelBuilder` that we can use to configure the functions,
   filters, and a LLM provider for the kernel.
3. After that, we call the `Build` method on the kernel builder to create the kernel
   object.

Make sure to configure the necessary environment variables with the configuration for
the application before attempting to run the application.

At this point, you can use the kernel object to execute prompts. We'll cover how to do
that in [#s](#executing-your-first-prompt). But before we do that, let's set up Semantic
Kernel in ASP.NET Core too.

### Setting up Semantic Kernel in an ASP.NET Core project

To set up Semantic Kernel in an ASP.NET Core project, you need to create a new web API
project. You can do this by executing the following command in a terminal:

```bash
dotnet new web -n Chapter1.WebApi
```

This command will create a new web API project in a folder called `Chapter1.WebApi`. You
can open the folder in Visual Studio Code by executing the following command:

```bash
code Chapter1.WebApi
```

In Visual Studio Code, you'll see the project file for the web API called
`Chapter1.WebApi.csproj` and a `Program.cs` file that will contain the configuration for
the web API. For web applications you'll also find a `settings.json` file and a
`settings.Development.json` file in the project directory.

Next, we need to add the same package as we used in the console application to the web
API project. Execute the following commands in the project directory inside a terminal
window:

```bash
dotnet add package Microsoft.SemanticKernel
dotnet add package Azure.Identity
```

These commands will install the necessary packages including the connector for Azure
OpenAI and OpenAI.

ASP.NET Core uses dependency injection to manage components in the application. Setting
up Semantic Kernel in an ASP.NET Core application is different from using it in a
console application. You need to use the dependency injection container to register the
kernel object in the application as shown in [#s](#configure-kernel-in-aspnet-core).

{#configure-kernel-in-aspnet-core}
```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient(sp => new AzureOpenAIClient(
    new Uri(builder.Configuration["AzureOpenAI:Endpoint"]!),
    new DefaultAzureCredential()));

var kernelBuilder = builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(deploymentName: "gpt-4o");

// ALTERNATIVELY: Use the OpenAI API.

// builder.Services.AddTransient(sp => new OpenAIClient(
//     builder.Configuration["OpenAI:ApiKey"]));

// var kernelBuilder = builder.Services.AddKernel()
//     .AddOpenAIChatCompletion("gpt-4o");

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

Let's go over the code step by step to understand the differences between the ASP.NET
Core code and a regular console application:

1. The using statements are the same as with a console application. We'll need to use
   the Semantic Kernel namespace and the connector namespace.
2. Next, we register a new `AzureOpenAIClient` in the service collection. We use the
   `AzureOpenAI:Endpoint` configuration value to set the endpoint of the Azure OpenAI
   service. We use the `DefaultAzureCredential` class to authenticate with Azure
   services.
2. After that, instead of creating a `IKernelBuilder` ourselves, we call the `AddKernel`
   method on the service collection of the application. This method returns a
   `IKernelBuilder` that we can use to configure the kernel. It also registers the
   kernel builder in the service collection so that a new kernel instance is created
   when we later ask for a `Kernel` object in a controller or endpoint.
3. Then, we use the `AddAzureOpenAIChatCompletion` method on the kernel builder to add
   the Azure OpenAI connector to the kernel. This method takes the deployment name of
   the model as a parameter.
4. Finally, we run the application as normal with the `Run` method.

Note that ASP.NET Core provides a nice configuration system that allows you to store
configuration values in a `settings.json` file. You can also store configuration in
environment variables or a key vault. This is one of the advantages of using a web
application over a console application. Although, with a little more effort you can
integrate the configuration system in the console application.

So if you're building a web application, make sure you configure the required
information in the settings.json file or in environment variables. You can also use
[user-secrets][USER_SECRETS] to store sensitive information like the OpenAI API Key.

Instead of providing an OpenAI client with the `AddAzureOpenAIChatCompletion` method,
we've preregistered the OpenAI client in the service collection. If you're using a tool
like [.NET Aspire][DOTNET_ASPIRE] you'll likely inject the OpenAI client through one of
the Aspire integration libraries. Those libraries only support injecting a client
through the service collection so it's a bad practice to provide the OpenAI client to
the `AddAzureOpenAIChatCompletion` method yourself. Consider this a nice step-up to
using .NET Aspire to manage your LLM-based application environment.

Semantic Kernel works well with ASP.NET Core, but you can use it in any kind of
environment even without dependency injection and configuration systems.

The `Kernel` object we registered can be recreated as often as you like. It's cheap and
very quick to set up. In ASP.NET Core you get a new instance of the `Kernel` every time
you ask for one. They're not shared between requests or kept around for the duration of
a request.

By having the kernel instance as a transient dependency you can have a base kernel
configuration in the startup of your application and extend it with additional filters
and functions for specific operations without affecting other future requests.

The pattern that ASP.NET Core uses to handle the kernel works for the console
application as well. Don't worry about creating many instances of the kernel in your
console application. Again, it's cheap and makes your code a lot more flexible.

{#executing-your-first-prompt}
## Executing your first prompt with Semantic Kernel

It's nice to have a basic kernel ready to go, but how do you use it to execute a prompt?
Let's explore this by running a basic prompt that generates a list of 5 interesting
names for a new line of shoes. We'll create a class called the `ProductNameGenerator`
that will use the kernel with a prompt to generate the list of product names.

```csharp
public class ProductNameGenerator(Kernel kernel)
{
    public async Task<string> GenerateProductNames()
    {
        var productNames = await kernel.InvokePromptAsync(
            "Generate 5 product names for a new line of shoes.");

        return productNames.GetValue<string>()!;
    }
}
```

The `ProductNameGenerator` class has a method called `GenerateProductNames` that uses
the kernel to execute a prompt. The prompt asks the LLM to generate 5 product names for
a new line of shoes. Prompts can return text structured data or things like images. We
need to use the `GetValue<T>` method to obtain the text from the response generated by
the LLM.

Now that we have the `ProductNameGenerator` class, we can use it in the console
application or the web API to generate the product names. For the web application we can
modify the `Program.cs` file to include the following code:

```csharp
builder.Services.AddTransient<ProductNameGenerator>();

var app = builder.Build();

app.MapGet("/", (ProductNameGenerator productNameGenerator) => 
    productNameGenerator.GenerateProductNames());
```

When you start the application and navigate to the root URL, you should see a list of 5
product names for a new line of shoes after a few seconds.

For the console application, you can modify the `Program.cs` file to include the
following code:

```csharp
var kernel = kernelBuilder.Build();
var productNameGenerator = new ProductNameGenerator(kernel);
var productNames = await productNameGenerator.GenerateProductNames();

Console.WriteLine(productNames);
```

When you run the console application, you should see a list of 5 product names for a new
line of shoes printed to the console.

To be honest, it doesn't make a lot of sense to generate names for shoes, but it's a
good start to understand how to use the kernel in your application. We'll get to see a
lot more prompts and kernel interactions in the next chapter when we cover
prompt engineering.

## Summary

In this chapter, we covered the core concepts and architecture of Semantic Kernel. We
saw how Microsoft solves some of the important challenges of using an LLM by abstracting
them away with the kernel pattern. We also set up a development environment and built a
project with Semantic Kernel. We created a basic console application and a web API that
uses the kernel to execute a prompt. We also executed a basic prompt to generate a list
of product names for a new line of shoes.

In the next chapter, we'll dive deeper into prompt engineering and learn how to design
and test effective prompts. We'll also explore how to keep prompts with your
application.

[SEMANTIC_KERNEL_DOCS]: https://learn.microsoft.com/en-us/semantic-kernel/
[LANGUAGE_SUPPORT]: https://learn.microsoft.com/en-us/semantic-kernel/get-started/supported-languages
[DOTNET_ASPIRE]: https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview
[USER_SECRETS]: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=windows