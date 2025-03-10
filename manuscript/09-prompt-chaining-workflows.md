{#prompt-chaining-workflows}
# Prompt chaining workflows

By now you'll have learned that LLMs are powerful and when combined with tools and RAG can produce interesting applications. In this chapter we'll start building workflows with LLMs.

At the end of the chapter you've learned why workflows are essential to increase accuracy of LLMs and how to build a workflow with Semantic Kernel that chains multiple prompts and tools.

We'll cover the following topics:

- Why use a prompt chain workflow
- Understanding and designing prompt chains
- Building a prompt chain with Semantic Kernel
- Testing approach for prompt chains
- Optimizations of the prompt chain workflow

Let's start by discussing why you would want to use a prompt chain workflow over a plain chat solution.

## Why use a prompt chain workflow

### Prompt chains improve quality

### Prompt chains improve testability

### Prompt chains traceability

### Prompt chains improve security

## Understanding and designing prompt chains

### Example prompt chains

### Breaking down a complex task

### Prototyping prompt chain workflows

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
