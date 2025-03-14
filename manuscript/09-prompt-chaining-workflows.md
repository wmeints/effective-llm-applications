{#prompt-chaining-workflows}
# Prompt chaining workflows

By now you have learned that LLMs are powerful and when combined with tools and RAG (Retrieval Augmented Generation) can produce interesting applications. In this chapter we'll start building workflows with LLMs that use the patterns and practices from the previous chapter.

At the end of the chapter you know why workflows are essential to increase the accuracy of LLMs and how to build a workflow with Semantic Kernel that chains multiple prompts and tools.

We'll cover the following topics:

- Why use a prompt chain workflow
- Understanding and designing prompt chains
- Building a prompt chain with Semantic Kernel
- Testing approaches for prompt chains
- Optimizations of the prompt chain workflow

Let's start by discussing why you would want to use a prompt chain workflow over a plain chat solution.

## Why use a prompt chain workflow

It's hard to imagine you'd have any need for workflows with all the power of modern LLMs. There are two schools of thought running around the internet at the time of writing in relation to this. One school of thought is that prompt engineering is the golden solution to everything. Another school of thought focuses on the idea of agents.

It is true that we can use chain-of-thought prompts, in-context learning, and add detailed instructions to create quite complicated responses. However, the prompts are rarely stable, and quite hard to maintain. The more complex and unfocused the prompt, the harder it becomes to get a reasonable answer.

The other school of thought that focuses on agentic AI thinks that agents are powerful enough to figure a solid workflow on their own. The idea is that agents can use LLMs to build a plan for solving a task and then execute that plan to solve the task. While this sounds like a flexible solution, it rarely works the way we want. We're simply not in the era yet where agents are stable enough to solve complex problems because the reasoning capabilities of LLMs aren't stable enough yet.

Right now, it's much easier and much faster to build a workflow instead of an agent when you know how to solve a problem and it's something that you need to solve again and again. Let me explain why.

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

This chain of thought around testing programming logic applies building LLM-based applications as well. Smaller, focused prompts, are easier to test and replace if they break. Remembmer, that LLM you're using will be replaced in a few months and you'll have to redo all the test work.

### Prompt chains improve security

Monitoring a less complicated prompt is easier. We have to keep in mind that hackers will attempt to abuse your application. Monitoring is an important line of defense to help you capture illegal usage patterns using security tools.

For production cases, if you can turn your complex task into a chain of prompts, I highly recommend doing so.

## Understanding and designing prompt chains

It sounds easy enough, split a big task into smaller tasks, but I've found that it can be quite hard to come up with a good structure to solve some of the more complicated tasks. I've found that it helps to have a few prompt design patterns somewhere in your notebook (or you could grab them from this book).

In my notebook, several patterns emerged as I'm writing more prompts for different use cases. I've categorized these prompts into two categories:

- **Divide and conquer prompts:** I use this pattern to split work into independent sub tasks. This is a useful technique for building a prompt to write a blog post for example. These prompts can be parallelized because you don't have any dependencies between the prompts.

- **Refinement prompts:** I use this pattern of the output from a prompt is a little too unstable and I can't fix it, because the task is a two-step process. For example, if you have to summarize text and then rewrite it to a specific style. Often, an LLM can summarize pretty well sticking to the original style. It's quite hard to change the style at the same time. So I put the rewrite portion into a second follow-up prompt.

Usually a problem can be solved with either a refinement prompt or a divide and conquer prompt. Sometimes you have to combine the two together.

It helps to write down a rough breakdown of the prompt chain you're trying to create. Most of the time I write a rough set of prompts and connect them together and execute one or two test runs. Sometimes I need to take a more elaborate approach to designing a good prompt. In a more complex scenario I prefer to draw a doodle with the steps needed to solve the problem using a few prompts and function calls.

One method that has really helped me through designing complex prompt chains is to use UML sequence diagrams. One tool I use for this is [app.diagrams.net][DRAW_IO], it has reasonable UML support and has free-form drawing capabilities that can be helpful too.

Let's look how we can break down the problem of generating blog content into a prompt chain.

### Creating blog content

One application of a prompt chain that is an interesting case is to write a blog post about a topic. It's interesting, because it shows off how much better a prompt chain works when compared to one big prompt. Let's first look at how you could approach this problem as a single chain-of-thought prompt.

//TODO: Insert prompt

The prompt contains a step-by-step plan to help the LLM generate the right response. We're relying on Semantic Kernel being able to call multiple tools thanks to the function calling loop we discussed in [#s](#llm-function-calling).

Instead of going through the code here, I want to focus on the challenges of this prompt. If you're interested in learning how to build this, I recommend looking at [the example code for this chapter][SAMPLE_CODE].

Running a complex prompt like the one we just discussed is annoying to debug and far from stable. There's a chance the LLM isn't going to follow my plan, because it found content on the internet that influences the reasoning capabilities. It can also fail to detect one or more of the tools for any number of reasons as we discussed in [#s](#what-are-tools-skills-and-plugins).

I like to call these chain-of-thought prompts chain-of-problems prompts, because of the high probability it doesn't do what I want.

You can solve the same complex task but with much more control when you use a prompt chain. [#s](#content-generation-workflow) shows the structure of the prompt chain for creating blog content. I've taken the plan from the original prompt, refined it, and turned it into a nice workflow.

{#content-generation-workflow}
![Content generation workflow](content-generation-workflow.png)

The workflow takes a similar approach as the chain-of-thought prompt we used before, but is more stable, because there's no chance the LLM isn't going to call my tool. That's because I'm using logic to enforce the plan.

Let's go over the worklow to understand how it works:

1. First, we'll research the topic by searching for 5 articles online that cover the topic of the article.
2. Next, we ask the LLM to generate an outline with top-level headings to create the structure of the article.
3. Then, we loop over the sections, and generate a key talking point for the section.
4. After generating the key talking point, we'll research the section in greater depth.
5. Finally, we'll generate for each section, and concatenate the content together.

You pay a price for this stability: You're going to have to write more code to make the workflow run. However, you gain a lot of quality and simplicity back for that extra code. If you didn't do this, you had to write a ton of tests and deal with the fact that the chain-of-thought prompt is never going to achieve the same level of accuracy.

To help you understand how much code we're talking about, let's build the workflow from start to finish with Semantic Kernel components and compare it to the chain-of-thought prompt implementation.

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
[SAMPLE_CODE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-09/csharp