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
- Using functions with Semantic Kernel
- Using filters to enhance functions
- Using transformations for safety

Let's take a step back, and look at how we turned a prompt into a function, and use that
starting point to look at plugins and functions to do other things.

## Why you need to use tools with LLMs

In [#s](#reusable-prompts) we used the concept of a kernel function to compile a
prompt to a C# callable piece of code. We used code similar to this code to compile
a prompt into a function:

```csharp
var promptTemplate = File.ReadAllText(
    Path.Join(Directory.GetCurrentDirectory(), "prompt.yaml")
);

var prompt = kernel.CreateFunctionFromPromptYaml(
    promptTemplate, 
    new HandlebarsPromptTemplateFactory());
```

The output of that code produces a `KernelFunction` object that you can call providing
it with arguments and the kernel to get a response to the prompt.

A single prompt function sometimes gets the job done, but often you'll need more than
that. Let's look at a few cases where functions play an important role.

For example, if you want a chatbot that can answer questions based on manuals, you'll
want to implement a RAG pattern that uses functions to find relevant information related
to your question. We'll cover this pattern in chapter 6, but it's good to know that it
uses functions under the covers.

If you want to generate or parse images in your application, you'll need to use a
function to either parse the image or generate a new image. The LLM can't do this. I'm
aware that it looks like that when you use OpenAI's ChatGPT, but they use the very same
tricks we're going to discuss in this chapter.

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
  questions based on manuals or internal information.
- **Task automation functions**: These are tools that change things or store
  information. For example, a calculator is a task automation tool. Another tool could
  be a function that produces an image (this is how ChatGPT generates images).

It's good to remember that functions require structured input and produce structured
output. And you can only use text input parameters and generated output that can be
converted to text. Because, as we've seen in the previous chapters, LLMs can only work
with text.

So if you're generating an image, you need to return the URL to the image rather than
the data for the image. And the image must be accessible to the person who's going to
read the output.

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

## Using functions with Semantic Kernel

We've already seen functions at work in previous chapters for generating content based
on a prompt template. It's worth diving deeper into what makes a function and how to
build more complicated functions.

Let's get started by converting the prompt functions we built earlier into a plugin.

### Authoring prompt-based functions

Remember from earlier chapters
- Explain how to group prompts into a plugin structure.

### Authoring code-based functions

- Explain how to build plugin classes with one or more functions.
- Explain how to use the plugins with semantic kernel

### Advanced configuration patterns

- Explain the importance of controlling what functions are available to the kernel at one time.
- Explain where to configure plugins and functions in a project to control what's available to the kernel.

## Applying filters to functions

- Explain what filters are, the types of filters available, and how to apply filters.
- Show an example of a filter that modifies the output of a function.

## Providing extra information to your plugins with transformations

- Explain why it is useful to apply function transformations
- URL: https://devblogs.microsoft.com/semantic-kernel/transforming-semantic-kernel-functions/

## Summary

## Further reading