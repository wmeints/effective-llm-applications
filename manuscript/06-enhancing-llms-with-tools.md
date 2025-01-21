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

TODO: Chapter topics

Let's take a step back, and look at how we turned a prompt into a function, and use that
starting point to look at plugins and functions to do other things.

## Plugins and functions in Semantic Kernel

In [#s](#reusable-prompts) we used the concept of a kernel function to compile a
prompt to a C# callable piece of code. Functions, together with Plugins, are one of the
key building blocks in Semantic Kernel. You'll use them everywhere.

In Semantic Kernel you can invoke a function directly to get a response to a prompt.
However, in many scenarios you'll want to use the LLM as a reasoning core and provide
it with tools to do the heavy lifting.

### Tools, skills, and functions defined

To fully understand the power of tools, it's important that we go back to what an LLM is.
It's a pattern-matching machine that can predict the likely next token in a sequence of
tokens. It can transform text, and that's it.

But we want to build applications that can do much more and use language to do the
things we want to do. For example, we might want to build an appplication that can
answer questions based on manuals. And we want the LLM to use the most up-to-date
information to answer that question. You could try to train the LLM with your business
information, but that's a very expensive process and extremely slow. Not to mention that
you'd have to retrain the model every time your business information changes.

Instead of training the LLM, we can give it a tool to find information from the manuals.
This tool can be a function in C# that uses a search engine to find relevant pieces of
information that we return to the LLM to use in its response to the user. This way we
don't have to train the LLM to understand our manuals, keeping the costs down.

Another great use case for tools is to let the LLM execute actions in other software.
For example, you can build an application that lets the LLM email a customer
based on your input.

When we talk about tools in LLM-based based application we can make a distinction between
two types of tools:

- **Information retrieval tools**: These are typically used to get information needed to
  provide a response to a prompt. For example, you'll need to use an information
  retrieval tool if you're going to build a chatbot that can answer questions based on
  manuals or internal information.
- **Task automation tools**: These are tools that change things or store information.
  For example, a calculator is a task automation tool. Another tool could be a function
  that produces an image (this is how ChatGPT generates images).

As far as Semantic Kernel is concerned, it only knows about plugins and functions. Some
LLM providers will use the term tools, while others use skills. In this book we'll use
the term functions to keep things simple.

### Plugins in the context of Semantic Kernel

So far we've only used a single function and invoked it directly. Semantic Kernel
however, has the notion of plugins. Plugins are a collection of functions that provide a
set of closely related capabilities. For example, you can create a Writer plugin that
contains a function to generate an outline for a blog post, a function to review the
outline of the blogpost, and a function to write sections of the blog post.

## Building a plugin for Semantic Kernel

### Authoring prompt-based functions

- Explain how to build prompt-based function (repeat from earlier chapter).
- Explain how to group prompts into a plugin structure.

### Authoring code-based functions

- Explain how to build plugin classes with one or more functions.
- Explain how to use the plugins with semantic kernel

## Applying filters to functions

- Explain what filters are, the types of filters available, and how to apply filters.
- Show an example of a filter that modifies the output of a function.

## Providing extra information to your plugins with transformations

- Explain why it is useful to apply function transformations
- URL: https://devblogs.microsoft.com/semantic-kernel/transforming-semantic-kernel-functions/

## Summary

## Further reading