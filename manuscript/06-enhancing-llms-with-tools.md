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

- Why you need to use tools with LLMs
- What are tools, skills, and plugins
- Building a kernel function
- Sharing functions across applications with OpenAPI
- Applying filters to functions
- Using function transformations

Let's first understand why you need functions if you want to go beyond simple prompts.

## Why you need to use tools with LLMs

If you're just starting out with building an application that uses an LLM, you maybe
wondering why you need to use functions at all. After all, you can get a lot done with
a single prompt. But as you start building more complex applications there's no escaping
the need for functions.

For example, if you want a chatbot that can answer questions based on manuals, you'll
want to implement a RAG pattern that uses functions to find relevant information related
to your question in a manual.

If you want to build a workflow that needs to generate a Github issue from a rough
description, you'll want to post the issue to Github as a draft or possibly find
information using the Github API.

If you want to generate or parse images in your application, you'll need to use a
function too. Because despite what ChatGPT might have you believe, LLMs don't generate
images. They only work with text.

A> Observant readers maybe wondering why I'm saying that LLMs don't do pictures, because
A> you can post an URL to an image to GTP-4o and it will interpret it. And you're right,
A> it does. But it uses a different model to do so. It just doesn't tell you about it.
A> And while GPT-4o can interpret images, it doesn't generate them.

Finally, if you want to interact with the environment, like updating data in a database,
turning on lights, or something similar, you're going to need functions as well.

## What are tools, skills, and plugins

Tools, skills, and functions are used interchangeably in the context of LLM-based
applications. Some providers use the term tools, others use skills, and some use
plugins. In this book we'll stick to functions to keep in line with what you're going to
find in the Semantic Kernel manual.

When we talk about functions in relation to LLMs, we're talking about two kinds of
functions:

- **Information retrieval functions**: These are typically used to get information
  needed to provide a response to a prompt. For example, you'll need to use an
  information retrieval tool if you're going to build a chatbot that can answer
  questions based on manuals or internal information.\
- **Task automation functions**: These are tools that change things or store
  information. For example, a calculator is a task automation tool. Another tool could
  be a function that produces an image (this is how ChatGPT generates images).

It's good to remember that functions require structured input and produce structured
output. And you can only use text input parameters and generated output that can be
converted to text. Because, as we've seen in the previous chapters, LLMs can only work
with text.

So if you're generating an image from one of your functions, you need to return the URL
to the image rather than the data for the image. And the image must be accessible to the
person who's going to read the output.

To use a function with an LLM, you need to provide the LLM with information about the
function. This information is provided as extra metadata in addition the the prompt. For
each function we need to tell the LLM the following details

- The name of the function
- The description of what the function achieves
- The input parameters with their names, datatypes, and descriptions
- The output parameter with a description and type information

When you call the LLM, it will try to figure out if it needs to call a function based on
the prompt you're sending. If it does, it will return a special response identifying
which tool it wants to call and the data for the input parameters. It is your job to
parse this response and call the appropriate function with the provided input and call
the LLM again with the output of the function.

Remember from [#s](#llm-function-calling) that Semantic Kernel comes with standard code
that takes care of calling functions for you. It even attempts to use multiple tools if
necessary. So that's one less thing you need to worry about. Although sometimes you
might want to control the process yourself.

Functions in Semantic Kernel are usually part of a plugin. Plugins group related
functions together to make it easier to package them as a separate library to maximize
their reuse across various applications within your organization. For me, plugins are
part of what makes Semantic Kernel useful in an enterprise environment. 

## Building a kernel function

We've already seen functions at work in previous chapters for generating content based
on a prompt template. It's worth diving deeper into what makes a function and how to
build more complicated functions.

You can create functions in one of three ways:

1. You can create a prompt file and load it as a function.
2. You can write C# code to create a function.
3. You can import an external HTTP based API through OpenAPI.

We'll cover all three possibilities in this chapter. Let's first go back to the prompt
that we've been working on in the last chapter and turn it into a function that
the kernel can use.

### Adding functions to the kernel

In the previous chapters we created a prompt using the following code:

```yaml


```

We could then load that prompt into the application using the following C# code:

```csharp

```

This code produces a `KernelFunction` object that we can call directly if we like. But
you can also provide it to the kernel as an optional function to call when the kernel
generates a response. The following code adds the loaded prompt as a function into the
kernel:

```csharp

```

### Authoring code-based functions

- Explain how to build plugin classes with one or more functions.
- Explain how to use the plugins with semantic kernel

### Advanced configuration patterns

- Explain the importance of controlling what functions are available to the kernel at one time.
- Explain where to configure plugins and functions in a project to control what's available to the kernel.

## Sharing functions across applications with OpenAPI

- Explain why you would share plugins and functions.
- Explain how to use the OpenAPI specification to share plugins across languages and frameworks.

## Applying filters to functions

- Explain what filters are, the types of filters available, and how to apply filters.
- Show an example of a filter that modifies the output of a function.

## Using function transformations

- Explain why it is useful to apply function transformations
- URL: https://devblogs.microsoft.com/semantic-kernel/transforming-semantic-kernel-functions/

## Summary

## Further reading