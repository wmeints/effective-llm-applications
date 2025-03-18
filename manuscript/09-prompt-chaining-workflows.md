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

There's another reason why building a prompt chain is better. It's easier to test individual steps than it is to test a complex prompt. Think of a prompt like a function in a computer program but with AI. We know from numerous projects that you and I have worked on, that it's hard to test a complex function with many scenarios. You need more unit-tests and it's easy to forget specific edge cases. When you break down the function into smaller functions it becomes much easier to reason about the logic and test it.

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

{#creating-blog-content}
### Creating blog content

One application of a prompt chain that is an interesting case is to write a blog post about a topic. It's interesting, because it shows off how much better a prompt chain works when compared to one big prompt. Let's first look at how you could approach this problem as a single chain-of-thought prompt.

```text
We’re writing a blog post about "{{topic}}". The main goal of the blog post
is to explain the importance of securing your agent properly. Please use
the following step-by-step instructions to write the article:

1. First, research the topic by looking at 5 articles on 
   the internet using the `search` tool that I’ve provided.
2. Next, create an outline based on the research you found. 
   We should only cover the top-level headings in the outline,
   we’ll expand these headings later.
3. After creating the outline, go over each section and figure
   out a key talking point for that section.
4. Finally, expand each section covering the key talking point.

Write the article. Make sure to include the title of the article.
```

The prompt contains a step-by-step plan to help the LLM generate the right response. We're relying on Semantic Kernel being able to call multiple tools thanks to the function calling loop we discussed in [#s](#llm-function-calling).

Instead of going through the code here, I want to focus on the challenges of this prompt. If you're interested in learning how to build this, I recommend looking at [the example code for this chapter][SAMPLE_CODE].

Running a complex prompt like the one we just discussed is annoying to debug and far from stable. There's a chance the LLM isn't going to follow my plan, because it found content on the internet that influences the reasoning capabilities. It can also fail to detect one or more of the tools for any number of reasons as we discussed in [#s](#what-are-tools-skills-and-plugins).

I like to call these chain-of-thought prompts chain-of-problems prompts, because of the high probability these prompts don't work as intended.

A> With recent development in reasoning models like OpenAI's o3 and o1 models, you'll find that these chain-of-thought prompts work much better. But I've found that they're still hard to debug. Also, while they tend to fail much less often, when a prompt fails with a reasoning model, it's more expensive and it fails more spectaculairly.

You can solve the same complex task but with much more control when you use a prompt chain. [#s](#content-generation-workflow) shows the structure of the prompt chain for creating blog content. I've taken the plan from the original prompt, refined it, and turned it into a nice workflow.

{#content-generation-workflow}
![Content generation workflow](content-generation-workflow.png)

The workflow takes a similar approach as the chain-of-thought prompt we used before, but is more stable, because we control when we're calling a tool instead of a probability based approach.

Let's go over the worklow to understand how it works:

1. First, we'll research the topic by searching for 5 articles online that cover the topic of the article.
2. Next, we ask the LLM to generate an outline with top-level headings to create the structure of the article.
3. Then, we loop over the sections, and generate a key talking point for the section.
4. After generating the key talking point, we'll research the section in greater depth.
5. Finally, we'll generate for each section, and concatenate the content together.

You pay a price for this stability: You're going to have to write more code to make the workflow run. However, you gain a lot of quality and testability back for that extra code. If you didn't do this, you had to write a ton of tests and deal with the fact that the chain-of-thought prompt is never going to achieve the same level of accuracy.

## Building a prompt chain with Semantic Kernel

To help you understand how much code we're talking about, let's build the workflow from start to finish with Semantic Kernel components and compare it to the chain-of-thought prompt implementation.

### Overview of the workflow

The workflow we'll build in Semantic Kernel follows the sequence diagram that you can find in [#s](#creating-blog-content). When designing the workflow for the book I figured it would be a good idea to package up each step in the workflow into a separate component so you can copy and paste those to your own code base should you wish to.

We'll create the following steps in the workflow:

1. Finding research online - This step uses the web search plugin for Semantic Kernel to find content related to the blog article topic.
2. Outlining the article - This step uses a prompt to create an outline with top-level section titles and a title for the blog article.
3. Researching individual sections - This step uses a prompt to create a key question to be answered in each section and then performs another search to find an answer to the key question.
4. Generating the article content - This step generates the final content for the article.

The main `Program.cs` file will contain the main orchestration logic for the workflow and some setup logic for the application. Let's start by implementing the first step of the workflow.

### Finding research online with the search tool

In the first step of the workflow, we'll use the `Microsoft.SemanticKernel.Plugins.Web` package that you can use to implement web search as a tool in your LLM-based application. For the purpose of the workflow, though, we'll use the plugin directly.

The research step looks like this:

```csharp
public class ResearchContentStep
{
    private readonly WebSearchEnginePlugin _webSearchPlugin;

    public ResearchContentStep(IConfiguration configuration)
    {
        _webSearchPlugin = new WebSearchEnginePlugin(new GoogleConnector(
            apiKey: configuration["Google:ApiKey"]!,
            searchEngineId: configuration["Google:SearchEngineId"]!));
    }

    public async Task<ResearchContentResult> InvokeAsync(string topic)
    {
        var searchResults = await _webSearchPlugin.SearchAsync(topic);
        return new ResearchContentResult(searchResults);
    }
}
```

This has the following logic:

1. First, we'll create a new class called `ResearchContentStep` that will package up the dependencies needed to execute the step. In the class there's a method called `InvokeAsync` that we can use to execute the step.
2. In the constructor of the class we create a new instance of the `WebSearchPlugin` class and give it access to a custom Google Search engine configuration. 
3. Then, in the `InvokeAsync` method we invoke the web search plugin and return its results to the caller.

The google web search connector requires an API key and a unique identifier for your application. You can set up a new API key [through the Google JSON search manual][GA_API_KEY]. The manual also includes a link to [the custom search engine control panel][GA_CONTROL_PANEL] that you'll need to obtain the unique search engine identifier for your application.

It may sound like over engineering to package each step into a separate class, but I've found that it makes it much easier to rewire the workflow at a later time. I had to rewire my workflow steps two or three times while making the sample code for the chapter because the order of operations just wasn't quite right.

Another benefit of packaging steps into classes I've found is that you can more easily unit-test the steps. I used the following xUnit-based test to validate the step:

```csharp
public class ResearchContentStepTests
{
    private readonly IConfiguration _configuration;
    private readonly ResearchContentStep _researchContentStep;

    public ResearchContentStepTests()
    {
        _configuration = TestObjectFactory.GetTestConfiguration();
        _researchContentStep = new ResearchContentStep(_configuration);
    }

    [Fact]
    public async Task InvokeAsync_ValidTopic_ReturnsResearchContentResult()
    {
        // Arrange
        var topic = "Artificial Intelligence";

        // Act
        var result = await _researchContentStep.InvokeAsync(topic);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SearchResults);
        Assert.NotEmpty(result.SearchResults);
    }
```

This unit-test looks a little underwhelming, but I'm not looking to unit-test the Semantic Kernel code itself. I just want to make sure that I'm getting something back in my process step.

I prefer to use a test object factory to configure some of the more frequently used components in the tests. The `TestObjectFactory` class contains logic to create a configuration object and a kernel for testing purposes. It looks like this:

```csharp
public class TestObjectFactory
{
    public static IConfiguration GetTestConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TestObjectFactory>()
            .Build();

        return configuration;
    }

    public static Kernel GetKernel(IConfiguration configuration)
    {
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                configuration["LanguageModel:DeploymentName"]!,
                configuration["LanguageModel:Endpoint"]!,
                configuration["LanguageModel:ApiKey"]!
            ).Build();

        return kernel;
    }
}
```

In the `TestObjectFactory` class you can find two methods:

1. First, there's a method that creates a configuration object for the tests so that I can obtain secrets from [the user-secrets configuration store][USER_SECRETS_STORE].
2. Next, there's a method to create a new kernel instance for every test we're running.

Moving logic like kernel initialization into a dedicated factory class makes the test logic a lot more compact so you can focus more on testing the prompts.

In the next step, after researching the blog topic, we'll create an outline based on the topic of the blog article, and the research we've found.

### Outlining the article content with a prompt

In the create outline step, we'll need a promp that we can use to create an outline that's based off the topic we're working on and the located research. I created a prompt for this purpose:

```yaml
name: create_outline
description: Generates a full article based on a plan
template: |
   We’re writing a blog post about "{{topic}}". Write an outline of the article. Follow these instructions carefully when
   creating the outline for the article. Refer to the search results below to help you create the outline.

   <|research|>
   {{searchResults}}
   <|research_end|>

   - Only generate top-level headings
   - Follow the hour-glass structure for the article
   - Include no formatting in the outline
template_format: handlebars
input_variables:
  - name: topic
    description: The topic you want to discuss in the blogpost.
    is_required: true
execution_settings:
  default:
    top_p: 0.98
    temperature: 0.7
    presence_penalty: 0.0
    frequency_penalty: 0.0
    max_tokens: 12000
```

I use specific markup in the prompt to help the LLM distinguish between my prompt and the research content. I've found that [ChatML][CHATML], a markup invented by OpenAI for LLMs can be a great tool to help structure prompts. Officially, ChatML doesn't support the tag I'm using, but because ChatML has a well defined structure, the LLM responds really well to my trick.

To use the prompt, we need to store it  in a prompt YAML file that's embedded in the program so we can use the following code to generate an outline:

```csharp
public class CreateOutlineStep
{
    private readonly KernelFunction _promptTemplate;
    private readonly Kernel _kernel;

    public CreateOutlineStep(Kernel kernel)
    {
        _kernel = kernel;
        _promptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.create-outline.yml"),
            new HandlebarsPromptTemplateFactory());
    }

    public async Task<CreateOutlineResult> InvokeAsync(
        string topic, string searchResults)
    {
        var executionSettings = new OpenAIPromptExecutionSettings()
        {
            ResponseFormat = typeof(CreateOutlineResult)
        };

        var response = await _promptTemplate.InvokeAsync(
            _kernel, 
            new KernelArguments(executionSettings)
            {
                ["topic"] = topic,
                ["searchResults"] = searchResults
            }
         );

        var responseData = JsonSerializer.Deserialize<CreateOutlineResult>(
            response.GetValue<string>()!);

        return responseData;
    }
}
```

This step uses the following logic:

1. First, we create a new step class and initialize the prompt template by reading the prompt YAML file that we described earlier.
2. Next, we create an `InvokeAsync` method that invokes the prompt template with the information about the topic and the reseach content that we located earlier on Google. We'll ask for structured output to help the produce content that we can easily parse.
3. Finally, we deserialize the structured JSON into the `CreateOutlineResult` class.

We're using structured output with the prompt template to make chaining easier. Without the structured output format, we'd have to resort to regular expressions and other difficult parsing logic to obtain the results we need.

Now that we have the outline, we can research the content of each of the generated sections in the outline.

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
[GA_API_KEY]: https://developers.google.com/custom-search/v1/overview
[GA_CONTROL_PANEL]: https://programmablesearchengine.google.com/controlpanel/all
[USER_SECRETS_STORE]: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=windows
[CHATML]: https://github.com/openai/openai-python/blob/release-v0.28.0/chatml.md