{#prompt-chaining-workflows}
# Prompt chaining workflows

By now you'll have learned that LLMs are powerful and when combined with tools and RAG (Retrieval Augmented Generation) can produce interesting applications. In this chapter we'll start building workflows with LLMs that use the patterns and practices from the previous chapter.

At the end of the chapter you know why workflows are essential to increase accuracy of LLMs and how to build a workflow with Semantic Kernel that chains multiple prompts and tools.

We'll cover the following topics:

- Why use a prompt chain workflow
- Understanding and designing prompt chains
- Building a prompt chain with Semantic Kernel
- Testing approach for prompt chains
- Optimizations of the prompt chain workflow

Let's start by discussing why you would want to use a prompt chain workflow over a plain chat solution.

## Why use a prompt chain workflow

It's hard to think that with all the power of modern LLMs you'd have any need for workflows. Just by looking at LinkedIn, you can tell that at least one portion of the people think that agents is the only viable solution if you need to solve a complex task with an LLM.

I can tell you from experience that there are multiple reasons that building an agent shouldn't be your first step. It's much easier and much faster to build a workflow like a prompt chain.

By breaking down a complex task into a chain of operations you can improve many aspects of your LLM-based solution. This chain of operations is often called a prompt chain, because most people chain together multiple prompts where the next prompt takes the output of the previous prompt to improve it with more details. While it's true that most prompt chains work like that, it's not a requirement to only use prompts.

Before we dive into building a prompt chain workflow, let's first discuss three reasons why you want to decompose complex tasks with a prompt chain workflow.

### Prompt chains improve quality

LLMs are powerful machines in the sense that you can tell it what you want, provide some examples, and it will solve simple tasks with great accuracy. However, as tasks become more complex, it's harder for the LLM to produce coherent and accurate responses.

Remember from [#s](#the-art-and-nonsense-of-prompt-engineering) that you get better results when you write:

- A prompt that solves one goal
- A prompt that includes relevant information to achieve the goal
- A prompt that provides focused instructions to achieve the goal

Complex tasks often involve multiple operations that need to be executed. This chain of operation is often interpreted as a set of goals that need to be achieved one after another. It's only logical that the more complex prompts don't produce the output you want.

Breaking down a big prompt into a chain of more focused prompts allows for a more accurate response from the LLM.

### Prompt chains improve testability

There's another reason why building a prompt chain is better. It's easier to test individual steps than it is to test a complex prompt. Think of a prompt like a  function in a computer program but with AI. We know from numerous projects that you and I have worked on, that it's hard to test a complex function with many scenarios. You need more unit-tests and it's easy to forget specific edge cases. When you break down the function into smaller functions it becomes much easier to reason about the logic and test it.

This chain of thought around testing programming logic applies building LLM-based applications as well. Smaller, focused prompts, are easier to test and replace if they break. Rembmer, that LLM you're using will be replaced in a few months and you'll have to redo all the test work.

### Prompt chains improve security

Monitoring a basic prompt is also easier. We have to keep in mind that hackers will attempt to abuse your application. Monitoring is an important line of defense to help you capture illegal usage patterns using security tools.

Building a complex prompt makes it harder for monitoring tools to capture abuse. It's also a lot easier for a hacker to write a jailbreak or prompt injection attack when the prompt is more complex. The LLM is more easily distracted.

For production cases, if you can turn your complex task into a chain of prompts, I highly recommend doing so.

## Understanding and designing prompt chains

It sounds easy enough, split a big task into smaller tasks, but I've found that it can be quite hard to come up with a good structure to solve some of the more complicated tasks. I've found that it helps to have a few prompt design patterns somewhere in your notebook (or from this book for that matter).

In my notebook, several patterns emerged as I'm writing more prompts for different use cases. I've categorized these prompt patterns into two categories:

- **Divide and conquer prompts:** I use this pattern to split work into independent sub tasks. This is a useful technique for building a prompt to write a blog post for example.

- **Refinement prompts:** I use this pattern of the output from a prompt is a little too unstable and I can't fix it, because the task is a two-step process. For example, if you have to summarize text and then rewrite it to a specific style. Often, an LLM can summarize pretty well sticking to the original style. It's quite hard to change the style at the same time. So I put the rewrite portion into a second follow-up prompt.

Usually a problem can be solved with either a refinement prompt or a divide and conquer prompt. Sometimes you have to combine the two together.

It helps to write down a rough schematic breakdown of the prompt chain you're trying to create. Most of the time I write a rough set of prompts and connect them together and execute one or two test runs. Sometimes I need to take a more elaborate approach to designing a good prompt. In a more complex scenario I prefer to draw a doodle with the steps needed to solve the problem using a few prompts and function calls.

One method that has really helped me through designing complex prompt chains is to use UML sequence diagrams. One tool I use for this is [app.diagrams.net][DRAW_IO], it has reasonable UML support and has free-form drawing capabilities that can be helpful too.

## Building a prompt chain with Semantic Kernel

### Overview of the workflow

### Finding research online with the search tool

### Outlining the article content with a prompt

### Researching individual sections

### Generating the article content

## Testing approach for prompt chains

### Using property-based tests over model-based tests

### Following the prompt chain with your tests

### User testing

## Optimizations of the prompt chain workflow

### Adding auto-corrective steps

- The LLM can generate the wrong content, you can use the pattern from chapter 12 to improve the quality by usign the artist/critic workflow.

### Adding fan-out operations to parallelize the workflow

- Running prompts is slow, use a fanout operation to improve how the workflow works. See chapter 11 for more information.

### Using intelligent routing to speed up the workflow even more

- Some parts of the outline may be perfect already, intelligent routing can help make a more informed decision whether a step should be executed or not. Read chapter 10 to learn more about this technique.

## Summary

[DRAW_IO]: https://app.diagrams.net/