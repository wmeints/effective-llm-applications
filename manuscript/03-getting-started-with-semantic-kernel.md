{#getting-started-with-semantic-kernel} 
# Getting Started with Semantic Kernel

In [#s](#essential-llmops-knowledge), we covered essential LLMOps knowledge. In this
chapter, we're finally going to talk about code. We'll start working with Semantic
Kernel. We'll cover the following topics:

- Understanding Semantic Kernel
- Setting up your development environment
- Setting up a project with Semantic Kernel
- Running a basic prompt with Semantic Kernel

By the end of the chapter, you'll have a solid foundation to start exploring how to
write effective prompts and design applications using the design patterns in the
upcoming chapters. Let's get started!

## Understanding Semantic Kernel

You can build LLM-based applications by calling the LLM providers' API directly. Calling
the LLM provider directly through their API or package is the best way to access the
latest and greatest features. However, applications built this way can be challenging to
manage over the long term. You have to deal with a lot of boilerplate code, and if you
need to switch providers, you must rewrite a lot of code.

A good framework will solve the boilerplate code for you while providing enough
flexibility to implement your desired design patterns. Semantic Kernel is a flexible
framework that provides enough value to make it worthwhile.

### What is Semantic Kernel

Semantic Kernel is an open-source framework from Microsoft that helps you build
LLM-based applications. Its primary purpose is to abstract away the integration code
needed to talk to various LLM providers so you can easily switch. Microsoft focuses on
providing an enterprise-grade solution to build LLM-based applications. You can be sure
that the concepts from [#s](#essential-llmops-knowledge) are implemented in the
framework.

Like the other LLM-based application frameworks, the framework is relatively new and
constantly evolving. So, you should regularly update your code as the framework evolves.
Microsoft makes the updates relatively easy to apply as long as you don't use any
experimental features. Since Semantic Kernel reached version 1+, Microsoft promises that
the non-experimental features are stable and won't receive breaking changes until 2.0
arrives.

I'll point out which features are still experimental so you can make an informed choice
regardless of whether you want to use them. You can also check the [product
documentation][SEMANTIC_KERNEL_DOCS] to determine if a feature you want to use is
experimental.

Let's look at the core concepts and the architecture of Semantic Kernel so you'll know
what to expect when you start using it.

{#core-concepts-and-architecture} 
### Core concepts and architecture

The architecture for Semantic Kernel is based on the concept of a Kernel that connects
to functions, an LLM provider, and filters. [#s](#semantic-kernel-architecture) shows
the architecture of the framework.

{#semantic-kernel-architecture} 
![Semantic Kernel Architecture](semantic-kernel-architecture.png)

After trying multiple frameworks, I've found that the Semantic Kernel fits quite well in
the mental model of an application built around an intelligent core that uses AI and
logic to fill in parts of the smart behavior. The kernel fills the core role, and the
functions, filters, and other application logic provide the intelligent components
around the core.

The mental model provided by Semantic Kernel allows you to isolate the AI portion of
your solution to a subsystem that can be replaced or upgraded without affecting
everything else in the application. This is important because the architecture of
LLM-based applications will change quickly over the next few years as we learn more
about LLMs' capabilities and limitations.

In [#s](#understanding-llms), we covered how LLMs generate a response. The kernel plays
an essential role in this process. Let's explore what happens when you run a prompt
through the kernel.

{#kernel-interactions} 
![Kernel interactions](kernel-interactions.png)

When you ask Semantic Kernel to execute a prompt, it will determine what AI service to
use. An AI service in Semantic Kernel can be an LLM or another AI service that takes a
prompt to generate a response. For example, with Semantic Kernel, you can:

- Generate text and then translate it to audio (text-to-audio)
- Generate an image from a prompt (text-to-image)
- Process an image to extract meaning from it (image-to-text)
- Process audio to text (audio-to-text)
- Generate embeddings for a document (embedding-generation)

You should be aware that the other modalities (sound and images) are experimental at the
time of writing. I don't use them, and I will stick to text-only tasks for this book.
But it's good to know that you can do more than just process text.

The prompt is rendered after selecting the best AI service to execute a prompt. You can
execute plain prompts with Semantic Kernel. But you'll want to use templating whenever
you can. Templating allows you to store prompts in source control to version them and
reuse them in multiple use cases or applications.

When the prompt is rendered, it is sent to the LLM provider. In
[#s](#llmops-rate-limits), we discussed that sometimes the LLM provider may not be
available, or you may have run out of quota. Semantic Kernel handles these failure modes
for you as much as possible.

When the response comes back from the LLM, we need to process it. The monitoring data we
discussed in [#s](#llmops-monitoring) is automatically generated so that you can export
it through tools like Application Insights or OpenTelemetry. The generated telemetry
contains token usage information, the input prompt, and the generated response.

Your code may be a simple chatbot or a complex workflow, and depending on the use case,
you'll need a different response. Depending on what you asked for, you may need
structured data or just plain text. Semantic Kernel will ensure that the data is
deserialized as necessary so you can work with it in your application.

The kernel allows you to inject filters at various points in the process. This is useful
to, for example, filter out harmful responses (see [#s](#llmops-user-safety)) or remove
PII data in the request (see [#s](#llmops-data-privacy)).

While the kernel is the core of Semantic Kernel, the functions are the most essential
part of the framework. Functions will provide the power needed to turn the LLM from a
simple text generator into the promised semantic intelligence of your application.

{#llm-function-calling} 
### Calling Functions

In Semantic Kernel, you can connect functions to the kernel to provide additional
functionality. Functions play an essential role in LLM-based applications because you
can use them to:

- Perform actions based on the user's prompt.
- Find relevant content to enhance the response to your prompt.

We will cover functions in great depth in [#s](#prompt-testing-and-monitoring) and
[#s](#enhancing-llms-with-tools), but for now, it's important to understand a trick I
like to call the kernel loop.

Modern LLMs support function calling and can combine several calls to functions to
complete a reasonably complex task if you design your application right.
[#s](#function-calling-loop) demonstrates the pattern Semantic Kernel uses to turn the
LLM into a planner using the LLM's function-calling capabilities. The function calling
flow takes place when the rendered prompt is submitted at the end of the prompt
execution flow in [#s](#kernel-interactions).

{#function-calling-loop} 
![Function calling loop](function-calling-loop.png)

This may come as a surprise: LLMs are trained as if they're chatbots, and that's good
even if you don't use them as chatbots. Let me explain.

The concept of a conversation will sound familiar to you if you've ever worked with
tools like ChatGPT. The idea is great because if you train the LLM just right, you can
use the concept of a conversation to guide the LLM to follow a plan. Also, you can use
the idea of a conversation to let the user refine their input in response to the LLM's
output.

At first, I thought it was silly to use the concept in a context other than a chatbot,
but now I see its power. In the upcoming chapters, you'll see me use the idea of a
conversation in the design patterns.

Let me be clear before you start thinking that LLMs are chatbots; they still aren't.
However, they're trained as if they are. The conversation you submit to the LLM is
flattened to a text sequence with unique markers to delimit the start and end of
messages in the conversation. The LLM doesn't produce a conversation as output; it will
only spit out tokens you can combine into a response. Frameworks like Semantic Kernel
will turn the response back into a response message and add it to the chat history
object if you like.

Going back to function calling, your prompt is turned into a chat history object before
it is submitted to the LLM. Then, it is submitted to the LLM, and we wait for a
response.

If the response from the LLM is a `tool_call` response, the kernel will find the
function identified by the `tool_call` and execute it with the parameter values provided
by the LLM. The result of the function call is added to the history object as a tool
message, and the new history is submitted to the LLM again. This process repeats until
the LLM returns a response that isn't a `tool_call.`

You can do some compelling planning with this. If you design a prompt that tells the LLM
to follow a plan and mention tools in your plan, it will likely follow the plan. We'll
use the fact that Semantic Kernel has this loop in the design patterns in upcoming
chapters.

### Language support

Using a semantic core with an LLM, functions, and filters in your application is
universal. It works in all programming languages. Semantic Kernel is available in C#,
Python, and Java. I chose C# for this book because I like C# as a language, but you
shouldn't feel limited by my preference.

With support for both Python and Java, Semantic Kernel currently covers all popular
languages in the enterprise space. However, you should be aware that support for Python
and C# is the furthest along. Microsoft is still working on properly supporting all Java
features.

The [language support matrix][LANGUAGE_SUPPORT] contains a complete list of supported
features for each language. The matrix tells you which features are supported in each
language and what connectors are available for each.

While I only work with C# in this book, I've ensured samples are available in Python and
Java where possible. When we get to those parts of the book, I'll point you to the right
resources.

Now that you understand the core components let's put them into practice by setting up a
development environment and building a project with Semantic Kernel.

## Setting up your development environment

Before using Semantic Kernel, you need to set up your development environment. I
recommend getting these tools set up:

- [.NET 9.0 SDK or later](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio Code](https://code.visualstudio.com), [Rider 2024.3](https://www.jetbrains.com/rider/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)
- Access to the [OpenAI API](https://openai.com/api/) or [Azure OpenAI](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource?pivots=web-portal)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) for working with Azure OpenAI

### Other supported LLM providers

Although I refer only to GPT-4 through the OpenAI API or Azure OpenAI, these are not the
only supported LLMs for Semantic Kernel. Numerous LLM providers are supported in
Semantic Kernel. They're adding more and more with each release. At the time of writing,
there's support for the following:

- OpenAI
- AzureOpenAI
- Amazon Bedrock
- Anthropic
- OLLama
- Google
- Hugging Face
- ONNX

And even if your favorite provider is not listed, it might still work with Semantic
Kernel as long as it has an OpenAI-like API interface. [The Semantic Kernel
Manual][OTHER_LLM_PROVIDER] explains how to use other LLM providers that support the
OpenAI API interface.

Let's start a new project with Semantic Kernel.

{#setting-up-semantic-kernel}
## Setting up a project with Semantic Kernel

Most people will build web API around an LLM use case. However, Semantic Kernel can be
used in more places than just web applications. To help you get started, I want to
provide you with two scenarios:

- Using Semantic Kernel in a standalone console application
- Using Semantic Kernel in an ASP.NET Core application

Let's start with a basic console application to get you started.

### Using Semantic Kernel in a standalone console application

To set up a console application with Semantic Kernel, you must create a new one in
Visual Studio Code, Rider, or Visual Studio. I prefer a terminal with Visual Studio
Code, but you can use the IDE you like. Execute the following command in a terminal to
start a new console application in C#:

```bash
dotnet new console -n Chapter3.ConsoleApp
```

This command will create a new console application in a folder called
`Chapter3.ConsoleApp`. You can open the folder in Visual Studio Code by executing the
following command:

```bash
code Chapter3.ConsoleApp
```

In Visual Studio Code, you'll see the project file for the console application,
Chapter3.Console.csproj, and a `Program.cs` file containing the main program code.

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
install the individual packages instead and replace the OpenAI connector with the
connector of your choice.

I've included the `Azure.Identity` package because it's a good practice to use managed
identities when working with Azure services. This package provides the
`DefaultAzureCredential` class, which you can use to authenticate with Azure services
without the need to store secrets with your application. As an alternative, you can also
specify an API Key for the client.

If you're working from your local machine, you can use [the Azure CLI][AZURE_CLI] to
authenticate with the Azure environment by executing the command `az login` from a
terminal window. When you run the sample application, it will use the credentials from
the Azure CLI to gain access to your Azure OpenAI resource.

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
//     Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

// kernelBuilder.AddOpenAIChatCompletion("gpt-4o", apiClient);

Kernel kernel = kernelBuilder.Build();
```

Let's go over the code step by step:

1. First, we import the necessary namespaces for the kernel and the connectors.
2. Then, we create a new `IKernelBuilder` that we can use to configure the kernel's functions, filters, and LLM provider.
3. we then call the `Build` method on the kernel builder to create the kernel object.

Before attempting to run the application, make sure to configure the necessary
environment variables with the application's configuration.

At this point, you can use the kernel object to execute prompts. We'll cover how to do
that in [#s](#executing-your-first-prompt). But before we do that, let's set up Semantic
Kernel in ASP.NET Core, too.

### Using Semantic Kernel in an ASP.NET Core application

You must create a new web API project to set up Semantic Kernel in an ASP.NET Core
project. You can do this by executing the following command in a terminal:

```bash
dotnet new web -n Chapter3.WebApi
```

This command will create a new web API project in a folder called `Chapter3.WebApi`. You
can open the folder in Visual Studio Code by executing the following command:

```bash
code Chapter3.WebApi
```

In Visual Studio Code, you'll see the project file for the web API called
`Chapter3.WebApi.csproj` and a `Program.cs` file containing the configuration for the
web API. You'll also find a `settings.json` file and a `settings.Development.json` file
in the project directory for web applications.

Next, we need to add the same package as we used in the console application to the web
API project. Execute the following commands in the project directory inside a terminal
window:

```bash
dotnet add package Microsoft.SemanticKernel
dotnet add package Azure.Identity
```

These commands will install the necessary packages, including the Azure OpenAI and
OpenAI connector.

ASP.NET Core uses dependency injection to manage application components. Setting up a
Semantic Kernel in an ASP.NET Core application differs from using it in a console
application. You need to use the dependency injection container to register the kernel
object in the application. The following code demonstrates this.

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
//     builder.Configuration["OpenAI:ApiKey"]));

// var kernelBuilder = builder.Services.AddKernel()
//     .AddOpenAIChatCompletion("gpt-4o");

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

Let's go over the code step by step again to understand the differences between the
ASP.NET Core code and a regular console application:

1. The `using` statements are the same as with a console application. We'll need to use
   the Semantic Kernel namespace and the connector namespace.
2. Next, we register a new `AzureOpenAIClient` in the service collection. We use the
   `AzureOpenAI:Endpoint` configuration value to set the endpoint of the Azure OpenAI
   service. We use the `DefaultAzureCredential` class to authenticate with Azure
   services.
3. After that, instead of creating an `IKernelBuilder` ourselves, we call the
   `AddKernel` method on the application's service collection. This method returns an
   `IKernelBuilder` that we can use to configure the kernel. It also registers the
   kernel builder in the service collection, creating a new kernel instance when we
   later ask for a `Kernel` object in a controller or endpoint.
4. Then, we use the `AddAzureOpenAIChatCompletion` method on the kernel builder to add
   the Azure OpenAI connector. This method takes the deployment name of the model as a
   parameter.
5. Finally, the `Run` method runs the application as usual.

Note that ASP.NET Core provides a nice configuration system that allows you to store
configuration values in a `settings.json` file. You can also store configuration in
environment variables or a key vault. This is one of the advantages of using a web
application over a console application. However, with more effort, you can also
integrate the configuration system into the console application.

Ensure you configure the required information in the settings.json file or environment
variables. You can also use [user-secrets][USER_SECRETS] to store sensitive information,
such as the OpenAI API Key.

Instead of providing an OpenAI client with the `AddAzureOpenAIChatCompletion` method,
we've preregistered the OpenAI client in the service collection. If you're using a tool
like [.NET Aspire][DOTNET_ASPIRE], you'll likely inject the OpenAI client into one of
the Aspire integration libraries. Those libraries only support injecting a client
through the service collection, so it's a bad practice to provide the OpenAI client with
the `AddAzureOpenAIChatCompletion` method yourself. Consider this a nice step up to
using .NET Aspire to manage your LLM-based application environment.

Semantic Kernel works well with ASP.NET Core, but it can be used in any environment,
even without dependency injection and configuration systems.

You can recreate the `Kernel` object we registered as often as you like. It's cheap and
swift to set up. In ASP.NET Core, you get a new instance of the `Kernel` whenever you
ask for one. They're not shared between requests or kept around for the duration of a
request.

By having the kernel instance as a transient dependency, you can have a base kernel
configuration in your application's startup and extend it with additional filters and
functions for specific operations without affecting other future requests.

The pattern ASP.NET Core uses to handle the kernel also works for the console
application. Don't worry about creating many kernel instances in your console
application. Again, it's cheap and makes your code a lot more flexible.

{#executing-your-first-prompt} 
## Executing your first prompt with Semantic Kernel

It's nice to have a bare kernel ready to go, but how do you use it to execute a prompt?
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
a new line of shoes. Prompts can return text-structured data or things like images. We
need to use the `GetValue<T>` method to obtain the text from the response generated by
the LLM.

Now that we have the `ProductNameGenerator` class, we can use it in the console
application or the web API to generate the product names. For the web application, we
can modify the `Program.cs` file to include the following code:

```csharp
builder.Services.AddTransient<ProductNameGenerator>();

var app = builder.Build();

app.MapGet("/", (ProductNameGenerator productNameGenerator) => 
 productNameGenerator.GenerateProductNames());
```

When you start the application and navigate to the root URL with your internet browser,
you should see a list of 5 product names for a new shoe line after a few seconds.

For the console application, you can modify the `Program.cs` file to include the
following code:

```csharp
var kernel = kernelBuilder.Build();
var productNameGenerator = new ProductNameGenerator(kernel);
var productNames = await productNameGenerator.GenerateProductNames();

Console.WriteLine(productNames);
```

When you run the console application, you should see a list of 5 product names for a new
line of shoes printed to the console. We'll see many more prompts and kernel
interactions in the next chapter when we cover prompt engineering. So consider this your
very first step into the world of LLM-based applications.

## Summary

In this chapter, we covered the core concepts and architecture of Semantic Kernel. We
saw how Microsoft solves critical challenges of using an LLM by abstracting them away
with the kernel pattern. We also set up a development environment and built a project
with Semantic Kernel. We created a basic console application and a web API that uses the
kernel to execute a prompt. We also executed a basic prompt to generate a list of
product names for a new line of shoes.

In the next chapter, we'll dive deeper into prompt engineering and learn how to design
and test effective prompts. We'll also explore how to keep prompts with your
application.

[SEMANTIC_KERNEL_DOCS]: https://learn.microsoft.com/en-us/semantic-kernel/ 
[LANGUAGE_SUPPORT]: https://learn.microsoft.com/en-us/semantic-kernel/get-started/supported-languages 
[DOTNET_ASPIRE]: https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview 
[USER_SECRETS]: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=windows
[OTHER_LLM_PROVIDER]: https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/?tabs=csharp-other%2Cpython-AzureOpenAI%2Cjava-AzureOpenAI&pivots=programming-language-csharp#adding-directly-to-the-kernel 
[AZURE_CLI]: https://learn.microsoft.com/en-us/cli/azure/install-azure-cli
[SAMPLE_CODE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-03/csharp