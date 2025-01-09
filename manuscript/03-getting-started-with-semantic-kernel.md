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

When the prompt is rendered, the prompt is sent to the LLM provider. In section
[#d](#llmops-rate-limits) we discussed that sometimes the LLM provider may not be
available, or you may have run out of quota. Semantic Kernel makes sure to handle these
failure modes for you as much as possible.

When the response comes back from the LLM, we need to process it. The monitoring data we
discussed in section [#](#llmops-monitoring) is automatically generated for you so you
can export it through tools like Application Insights or OpenTelemetry.

Ultimately, the result of a prompt execution is returned to you. Depending on what you
asked for, you may need structured data or just plain text. Semantic Kernel will make
sure that the data is deserialized as necessary so you can work with it in your
application.

The kernel allows you to inject filters at various points in the process. This is useful
to for example filter out harmful responses (see section [#d](#llmops-user-safety)) or
to remove PII data in the request (see section [#d](#llmops-data-privacy)).

### Enterprise Readiness

- Filtering input and output in plugins and the kernel
- Monitoring capabilities

### Language support

## Setting up your development environment

### System requirements

- .NET 9.0 or later
- Visual Studio Code, Rider 2024.3 or Visual Studio 2022
- Access to the OpenAI API or Azure OpenAI

### Getting access to an LLM

- Before you can start using Semantic Kernel, you'll need access to an LLM. You can use OpenAI's GPT-3 or GPT-4.
- I'm not covering other LLM providers here,  but you can certainly use them with Semantic Kernel. Just make sure to use the right connector package. We'll discuss those in the next section when we set up a project with Semantic Kernel.
- Let's get started by setting up a new Azure OpenAI resource in Azure. Make sure you have access to an Azure subscription before you start.

#### Getting access to Azure OpenAI

#### Getting access to OpenAI

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