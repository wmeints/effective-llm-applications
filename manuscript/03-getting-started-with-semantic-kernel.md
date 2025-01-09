# Getting Started with Semantic Kernel

In chapter 2, we covered essential LLMOps knowledge. In this chapter, we're finally
going to talk about code. We'll get started understanding one of the frameworks you can
use to build LLM-based applications: Semantic Kernel. We'll cover the following topics:

- Understanding Semantic Kernel
- Setting up your development environment
- Setting up a project with Semantic Kernel
- Running a basic prompt with Semantic Kernel
- Common use cases for Semantic Kernel

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

In chapter [#d](#chapter-1), we covered how LLMs generate a response. The kernel plays
an important role in this process. Let's explore what happens when you run a prompt
through the kernel.

{#kernel-interactions}
![Kernel interactions](semantic-kernel-architecture.png)

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

We will cover functions in great depth through chapters 4 and 5, for now it's important
to understand a trick I like to call the kernel loop in relation to function calling.

Modern LLMs support function calling and can, if you design your application right,
combine several calls to functions to complete a reasonably complex task.
[#s](#function-calling-loop) demonstrates the pattern that Semantic Kernel uses to turn
the LLM into a planner by using the LLM's function calling capabilities.

{#function-calling-loop}
![Function calling loop](function-calling-loop.png)

At the end of the prompt execution flow in [#s](#kernel-interactions), when the prompt
is submitted to the LLM it isn't just submitted. We can't just submit a piece of text,
we need to turn it into a chat history object. While an LLM only works with an input
sequence to produce an output sequence, all modern LLMs are trained as if they're
chatbots. They use conversations as a concept to generate better responses.

The concept of a conversation will sound familiar to you if you've ever worked with
tools like ChatGPT. The idea is great because if you train the LLM just right, you can
use the concept of a conversation to guide the LLM to follow a plan. Also, you can use
the concept of a conversation to let the user refine their input in response to the
LLM's output. At first, I thought it was silly to use the concept in a context other
than a chatbot, but now I see the power of it. You'll see me use the concept of a
conversation in the design patterns in the upcoming chapters a lot.

Let me be clear here, before you start thinking that LLMs are chatbots, they still
aren't. Although they're trained as if they are. The conversation you submit to the LLM
is flattened to a text sequence with special markers to delimit the start and end of
messages in the conversation. The LLM doesn't produce a conversation as output, it will
only spit out tokens that you can combine to a response. Frameworks like Semantic Kernel
will turn the response back into a response message and add it to the chat history
object if you like.

Going back to function calling, once we have a conversation history object, the kernel
will send it to the LLM, and wait for a response.

If the response is a `tool_call` response, the kernel will find the function identified
by the `tool_call` and execute it with the parameter values provided by the LLM. The
result of the function call is added to the conversation as a tool message and new
history is submitted to the LLM again. This process repeats until the LLM returns a
response that doesn't contain a `tool_call`.

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
support matrix][LANGUAGE_SUPPORT].

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

- We'll use both ASP.NET Core and Console Worker applications in the book. Because LLMs are resource-intensive and you might want to run them as an online process as well as in a more batch-oriented way.
- We'll use the `Microsoft.SemanticKernel` NuGet package to get started with Semantic Kernel. This package provides the core functionality for working with LLMs in a way that's optimized for performance and ease of use.
- You'll need a separate package like `Microsoft.SemanticKernel.Connectors.AzureOpenAI` to connect to a specific LLM provider. If you ever want to switch, just configure another provider and you're good to go.

### Setting up an ASP.NET Core project

### Setting up a worker application

## Running a basic prompt with Semantic Kernel

- To get used to working with Semantic Kernel, let me demonstrate how to run a basic prompt through GPT-4o using the `Microsoft.SemanticKernel.Connectors.AzureOpenAI` package.
- We'll use the worker application we created in the previous section to run a prompt that will generate a list of 5 interesting names for a new product.
- Don't worry about testing right now, we'll cover that in chapter 4, when we cover prompt engineering in greater depth.

## Common use cases for Semantic Kernel

- Semantic Kernel is designed to be flexible in what you can do with it.
- You can use Semantic Kernel to build chatbots, AI-driven search pages, or more complex applications.
- It may come as no surprise that all the design patterns in the upcoming chapters work with Semantic Kernel, although some may require a bit more setup than others.
- The goal of this book is to show you how to use Semantic Kernel to build real-world applications. So we'll focus on the most common use cases and how to implement them with Semantic Kernel.
- We'll start with basic prompt engineering and then move on to more advanced topics like integrating tools. 
- In chapter 11 and 12 we'll look at the new features in Semantic Kernel involving agents and process automation. 

## Summary

[SEMANTIC_KERNEL_DOCS]: https://learn.microsoft.com/en-us/semantic-kernel/
[LANGUAGE_SUPPORT]: https://learn.microsoft.com/en-us/semantic-kernel/get-started/supported-languages