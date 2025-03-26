{#prompt-chaining-workflows}
# Prompt chaining workflows

In the previous chapters we've covered how LLMs can help us build powerful and interesting applications especially when combined with tools and RAG (Retrieval Augmented Generation). In this chapter, we'll start building workflows with LLMs that use the patterns and practices from the previous chapters.

At the end of the chapter, you know why workflows are essential to increase the accuracy of LLMs and how to build a workflow with Semantic Kernel that chains multiple prompts and tools.

We'll cover the following topics:

- Why use a prompt chain workflow
- Understanding and designing prompt chains
- Building a prompt chain with Semantic Kernel
- Testing approaches for prompt chains
- Optimizations of the prompt chain workflow

Let's start by discussing why you would choose a prompt chain workflow.

## Why use a prompt chain workflow

To understand the importance of a prompt chain, we need to first talk about what a prompt chain is. A prompt chain is a linear workflow that uses two or more steps in the form of prompts after each other. Usually, we feed output from the first prompt into a second prompt. Prompt chains don't have to be prompts only; you can mix and match prompts with tools to create more complicated ones.

Not everyone likes workflows, though. Many believe that prompt engineering is the golden solution to everything. We can create complicated responses using chain-of-thought prompts, in-context learning, and detailed instructions. With recent developments in LLMs, you can get pretty far with prompts. However, the more complicated prompts are rarely stable and hard to maintain. The more complex and unfocused the prompt, the harder it becomes to get a reasonable answer.

Other people dislike building workflows because they think AI agents are powerful enough to figure out a solid workflow independently. The idea is that agents can use LLMs to construct a plan for solving a task and then execute that plan to solve the task. While this sounds like a flexible solution, it rarely works how we want. We're not yet in an era where agents are stable enough to solve complex problems because the reasoning capabilities of LLMs aren't stable enough.

Right now, it's much easier and much faster to build a workflow than an agent when you know how to solve a problem that you need to solve repeatedly. Let me explain why.

### Prompt chains improve quality

LLMs are potent machines in that you can tell them what you want and provide some examples, and they will solve simple tasks with great accuracy. However, as tasks become more complex, it's harder for the LLM to produce coherent and accurate responses.

Remember from [#s](#the-art-and-nonsense-of-prompt-engineering) that you get better results when you write:

- A prompt that solves one goal
- A prompt that includes relevant information to achieve the goal
- A prompt that provides focused instructions to achieve the goal

Complex tasks often involve multiple operations that need to be executed. This chain of operation is usually interpreted as a set of goals that need to be achieved one after another. It's only logical that the more complex prompts don't produce the output you want.

Breaking down a big prompt into a chain of more focused prompts allows the LLM to respond more accurately.

### Prompt chains improve testability

There's another reason why building a prompt chain is better. It's easier to test individual steps than it is to test a complex prompt. Think of a prompt like a function in a computer program but with AI. As software developers, we know that it's hard to test a complex function with many flows. You need more unit tests, and it's easy to forget specific edge cases. When you break down the function into smaller functions, it becomes much easier to reason about the logic and test it.

This chain of thought around testing programming logic applies to building LLM-based applications as well. Smaller, focused prompts are easier to test and replace if they break. Remember that the LLM you're using will be replaced in a few months, and you'll have to redo all the test work.

## Understanding and designing prompt chains

Splitting a big task into smaller tasks sounds easy enough, but I've found that it can be quite hard to come up with a good structure to solve some of the more complicated problems with an LLM. It helps to have a few prompt design patterns somewhere in your notebook (or you could grab them from this book).

In my notebook, several patterns emerged as I wrote more prompts for different use cases. I've categorized these prompts into two categories:

- **Divide and conquer prompts:** I use this pattern to split work into independent sub-tasks. This is a helpful technique for building a prompt to write a blog post, for example. These prompts can be parallelized because you don't have any dependencies between the prompts.

- **Refinement prompts:** I use this pattern to improve the output quality from a prompt if it is too unstable and I can't fix it because I approached the problem with a one-step process in mind, while solving it in two steps is better. For example, if you have to summarize text and then rewrite it to a specific style, an LLM can often summarize pretty well, sticking to the original style. Changing the style and structure simultaneously is challenging, so I put the rewrite portion into a second follow-up prompt.

Usually, you can solve the problem with either a refinement or a divide-and-conquer prompt. Sometimes, you have to combine the two.

It helps to write down a rough breakdown of the prompt chain you're trying to create. I usually write a rough set of prompts, connect them, and execute one or two test runs. Sometimes, I must take a more elaborate approach to designing a good prompt. In a more complex scenario, I prefer to draw a doodle with the steps to solve the problem using a few prompts and function calls.

One method that has really helped me design complex prompt chains is using UML sequence diagrams. One tool I use for this is [app.diagrams.net][DRAW_IO]. It has reasonable UML support and free-form drawing capabilities that can be helpful, too.

Let's look at how we can break down the problem of generating blog content into a prompt chain.

{#creating-blog-content}
### Creating blog content

One application of a prompt chain that is an interesting case is to write a blog post about a topic. It's interesting because it shows how better a prompt chain works than one big prompt. Let's first look at how you could approach this problem as a single chain-of-thought prompt.

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

The prompt contains a step-by-step plan to help the LLM generate the right response. Thanks to the function calling loop we discussed in [#s](#llm-function-calling), we're relying on the Semantic Kernel to call multiple tools.

Instead of going through the code here, I want to focus on the challenges of this prompt. To learn how to build this, look at [the example code for this chapter][SAMPLE_CODE].

Running a complex prompt like the one we just discussed is annoying to debug and far from stable. There's a chance the LLM won't follow my plan because it found content on the internet that influences the reasoning capabilities. It can also fail to detect one or more of the tools for several reasons, as discussed in [#s](#what-are-tools-skills-and-plugins).

I like to call these chain-of-thought prompts chain-of-problems prompts because they are highly likely to fail as intended.

A> With recent developments in reasoning models like OpenAI's o3 and o1 models, you'll find that these chain-of-thought prompts work much better. But I've found that they're still hard to debug. Also, while they tend to fail much less often when a prompt fails with a reasoning model, it's more expensive and fails more spectacularly.

Using a prompt chain allows you to solve the same complex task with much more control. [#s](#content-generation-workflow) shows the structure of the prompt chain for creating blog content. I've taken the plan from the original prompt, refined it, and turned it into a nice workflow.

{#content-generation-workflow}
![Content generation workflow](content-generation-workflow.png)

The workflow takes a similar approach to the chain-of-thought prompt we used before but is more stable because we control when we call a tool instead of using a probability-based approach.

Let's go over the workflow to understand how it works:

1. First, we'll research the topic by searching for five articles online that cover the article's topic.
2. Next, we ask the LLM to generate an outline with top-level headings to create the article's structure.
3. Then, we loop over the sections and generate a key talking point for the section.
4. After generating the key talking point, we'll research the section more deeply.
5. Finally, we'll generate for each section and concatenate the content together.

You pay a price for this stability: You must write more code to make the workflow run. However, you gain a lot of quality and testability for that extra code. If you didn't do this, you had to write many tests and deal with the fact that the chain-of-thought prompt would never achieve the same level of accuracy.

## Building a prompt chain with Semantic Kernel

To help you understand how much code we're talking about, let's build the workflow from start to finish with Semantic Kernel components and compare it to the chain-of-thought prompt implementation.

### Overview of the workflow

The workflow we'll build in Semantic Kernel follows the sequence diagram in [#s](#creating-blog-content). When designing the workflow for the book, I figured it would be a good idea to package each step into a separate component so you can copy and paste those to your own code base if you wish to.

We'll create the following steps in the workflow:

1. Finding research online - This step uses the web search plugin for Semantic Kernel to find content related to the blog article topic.
2. Outlining the article - This step uses a prompt to create an outline with top-level section titles and a title for the blog article.
3. Researching individual sections - This step uses a prompt to create a key question to be answered in each section, and then another search is performed to find an answer to the key question.
4. Generating the article content - This step generates the final content.

The main `Program.cs` file will contain the main orchestration logic for the workflow and some setup logic for the application. Let's start by implementing the first step of the workflow.

### Finding research online with the search tool

In the first step of the workflow, we'll use the `Microsoft.SemanticKernel.Plugins.Web` package that you can use to implement web search as a tool in your LLM-based application. You add the package to your project by executing the command `dotnet add package Microsoft.SemanticKernel.Plugins.Web`.

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

This code contains the following logic:

1. First, we'll create a new class called `ResearchContentStep` to package up the dependencies needed to execute the step. In the class, we can use a method called `InvokeAsync` to execute the step.
2. In the class constructor, we create a new instance of the `WebSearchPlugin` class and give it access to a custom Google Search engine configuration. 
3. Then, in the `InvokeAsync` method, we invoke the web search plugin and return its results to the caller.

The Google web search connector requires an API key and a unique identifier for your application. You can set up a new API key [through the Google JSON search manual][GA_API_KEY]. The manual also includes a link to [the custom search engine control panel][GA_CONTROL_PANEL] that you'll need to obtain the unique search engine identifier for your application.

It may sound like overengineering to package each step into a separate class, but I've found that it makes it much easier to rewire the workflow later. I had to rewire my workflow steps two or three times while making the sample code for the chapter because the order of operations wasn't quite right.

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
}
```

This unit test looks underwhelming, but I'm not looking to unit-test the Semantic Kernel code itself. I want to ensure I'm getting something back in my process step.

I prefer a test object factory to configure some of the more frequently used components in the tests. The `TestObjectFactory` class contains logic to create a configuration object and a kernel for testing purposes. It looks like this:

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

In the `TestObjectFactory` class, you can find two methods:

1. First, there's a method that creates a configuration object for the tests so that I can obtain secrets from [the user-secrets configuration store][USER_SECRETS_STORE].
2. Next, there's a method to create a new kernel instance for every test we run.

Moving logic like kernel initialization into a dedicated factory class compacts the test logic, allowing you to focus more on testing the prompts.

In the next step, after researching the blog topic, we'll create an outline based on the topic of the blog article and the research we've found.

### Outlining the article content with a prompt

In the create outline step, we'll need a prompt that we can use to create an outline that's based on the topic we're working on and the located research. We can use a prompt for this purpose that looks like this:

```yaml
We’re writing a blog post about "{{topic}}". Write an outline of the
article. Follow these instructions carefully when creating the outline
for the article. Refer to the search results below to help you 
create the outline.

<|research|>
{{searchResults}}
<|research_end|>

- Only generate top-level headings
- Follow the hour-glass structure for the article
- Include no formatting in the outline
```

I use specific markup in the prompt to help the LLM distinguish between my prompt and the research content. I've found that [ChatML][CHATML], a markup invented by OpenAI for LLMs, can be a great tool for structuring prompts. Officially, ChatML doesn't support the tag I'm using, but because ChatML has a well-defined structure, the LLM responds really well to my trick.

To use the prompt, we need to store it in a prompt YAML file that's embedded in the program so we can use the following code to generate an outline:

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
2. Next, we create an `InvokeAsync` method that invokes the prompt template with the information about the topic and the research content we located on Google. We'll ask for structured output to help us produce content that we can quickly parse.
3. Finally, we deserialize the structured JSON into the `CreateOutlineResult` class.

We're using structured output with the prompt template to make chaining easier. Without the structured output format, we'd have to resort to regular expressions and other complex parsing logic to obtain the results from the output.

Now that we have the outline, we can research each generated section's content.

### Researching individual sections

In the research section step, we answer a single question about the section's topic. We'll use the Google search we used before to get the information we need.

To research the content of a single section, we need to ask a more detailed question to Google to get relevant information. It's pretty hard to get this right, but thanks to the power of the LLM, we can make a decent attempt.

The code for this step looks like this:

```csharp
public class ResearchSectionStep
{
    private readonly KernelFunction _generateQuestionPromptTemplate;
    private readonly Kernel _kernel;
    private readonly WebSearchEnginePlugin _webSearchPlugin;

    public ResearchSectionStep(Kernel kernel, IConfiguration configuration)
    {
        _kernel = kernel;

        _generateQuestionPromptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.research-section.yml"),
            new HandlebarsPromptTemplateFactory());

        _webSearchPlugin = new WebSearchEnginePlugin(new GoogleConnector(
            apiKey: configuration["Google:ApiKey"]!,
            searchEngineId: configuration["Google:SearchEngineId"]!));
    }

    public async Task<ResearchSectionResult> InvokeAsync(
        string topic, string sectionTitle)
    {
        var searchQuery = await GenerateSearchQueryAsync(topic, sectionTitle);
        var searchResults = await _webSearchPlugin.SearchAsync(searchQuery);

        return new ResearchSectionResult(searchQuery, searchResults);
    }
    
    // Implementation of the GenerateSearchQueryAsync omitted for now.
}
```

In this step, an `InvokeAsync` method takes in the topic and section title. This information is used to build a search query we can then use to find relevant information.

The `GenerateSearchQueryAsync` method uses a prompt to generate a well-written search query. The code for this method looks like this:

```csharp
private async Task<string> GenerateSearchQueryAsync(
    string topic, string sectionTitle)
{
    var promptExecutionSettings = new OpenAIPromptExecutionSettings
    {
        ResponseFormat = typeof(GenerateSearchQueryResult)
    };

    var response = await _generateQuestionPromptTemplate.InvokeAsync(
        _kernel, new KernelArguments(promptExecutionSettings)
        {
            ["topic"] = topic,
            ["sectionTitle"] = sectionTitle
        });

    string responseData = response.GetValue<string>()!;

    var searchQueryResult = 
        JsonSerializer.Deserialize<GenerateSearchQueryResult>(
            responseData);

    if (searchQueryResult is null)
    {
        throw new InvalidOperationException("Failed to generate search query");
    }

    return searchQueryResult.SearchQuery;
}
```

This code performs the following steps:

1. First, we configure a set of prompt execution settings so we can ask for a structured response.
2. Next, we invoke a query generation prompt template, providing it with the correct inputs and settings to generate a new search query for us.
3. Then, we use the `JsonSerializer` to deserialize the content to a `GenerateSearchQueryResult` object containing the query.
4. Finally, we return the generated search query.

The method uses a prompt to generate a question we can ask Google. The prompt looks like this:

```text
I'm writing a blog post about "{{topic}}". I'm looking for a search query
I can run through Google to find out more about "{{sectionTitle}}". I'm
looking for a search query that will give me a good amount of detail. The 
search query should be in the form of a question.
```

The topic and section title are quite short, but they're still enough to generate a high-quality question for a search engine.

For testing this step, I recommend that you ignore Google's search results and focus on the question generation part. Remember from [#s](#model-based-testing) that you can use model-based tests to validate the prompt quality.

Next, let's look at the final step in the process: content generation.

### Generating the article content

The final step in the workflow generates the content per section. We'll use a prompt-based approach to generate content based on the topic, the section title, and the content we've found using Google in the previous step.

This step is repeated for every section we've researched in the previous step.
The step code looks like this:

```csharp
public class WriteSectionStep
{
    private readonly KernelFunction _writeSectionPromptTemplate;
    private readonly Kernel _kernel;

    public WriteSectionStep(Kernel kernel)
    {
        _kernel = kernel;
        _writeSectionPromptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.write-section.yml"),
            new HandlebarsPromptTemplateFactory());
    }

    public async Task<GenerateSectionContentResult> InvokeAsync(
        string topic, string sectionTitle, string query, string searchResults)
    {
        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(GenerateSectionContentResult)
        };

        var response = await _writeSectionPromptTemplate.InvokeAsync(
            _kernel, new KernelArguments(promptExecutionSettings)
            {
                ["topic"] = topic,
                ["sectionTitle"] = sectionTitle,
                ["query"] = query,
                ["searchResults"] = searchResults
            });

        var responseData = response.GetValue<string>()!;

        var result = 
            JsonSerializer.Deserialize<GenerateSectionContentResult>(
                responseData);

        if (result is null)
        {
            throw new InvalidOperationException(
                "Failed to generate section content");
        }

        return result;
    }
}
```

This code performs the following steps:

1. First, we create a new step class that accepts a kernel instance in the constructor. We'll use the kernel to load the prompt for the step.
2. Next, an `InvokeAsync` method invokes a prompt to generate content. Again, we'll use structured output to help the LLM generate helpful content without having to parse the output manually.
3. Then, we deserialize the generated response using the `JsonSerializer` into a `GenerateSectionContentResult` that contains the section title and the content that should answer the question we generated earlier.

Let's now discuss the final piece of the workflow, the orchestration logic for the prompt chain.

### Finishing the workflow

The orchestration logic combines all the steps into a working program. The code for this you can find here:

```csharp
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        configuration["LanguageModel:DeploymentName"]!,
        configuration["LanguageModel:Endpoint"]!,
        configuration["LanguageModel:ApiKey"]!
    ).Build();

var researchContentStep = new ResearchContentStep(configuration);
var createOutlineStep = new CreateOutlineStep(kernel);
var researchSectionStep = new ResearchSectionStep(kernel, configuration);
var writeSectionStep = new WriteSectionStep(kernel);

var topic = "The importance of securing your AI agents in production";

var researchedContent = await researchContentStep.InvokeAsync(topic);

var outline = await createOutlineStep.InvokeAsync(
    topic, researchedContent.SearchResults);

var outputBuilder = new StringBuilder();

outputBuilder.AppendLine($"# {outline.Title}");

foreach (var section in outline.Sections)
{
    var researchedSectionContent =
        await researchSectionStep.InvokeAsync(topic, section);

    var sectionContent = await writeSectionStep.InvokeAsync(
        topic, section, researchedSectionContent.Query,
        researchedSectionContent.SearchResults);

    outputBuilder.AppendLine();
    outputBuilder.AppendLine($"## {sectionContent.Title}");
    outputBuilder.AppendLine();
    outputBuilder.AppendLine(sectionContent.Content);
}

Console.WriteLine(outputBuilder.ToString());
```

The main program performs the following steps:

1. First, we load the configuration from the user secrets so we can store secrets securely.
2. Next, we build the kernel for the application using the configuration values stored in the user secrets.
3. Then, we initialize the steps in the workflow with their dependencies
4. After defining the topic we want to write about, we run it through the first step of researching the content for the article.
5. Using the result from the first step, we'll invoke the next step to come up with an outline based on the research we found.
6. Then, we go over each section in the article, research the section, and generate content for the section.
7. Finally, we combine the content for the sections into the full article content.

I kept things simple to help you understand the chain. In production, I recommend looking at the different aspects of LLMOps to make the chain more robust. For example, add retries to the steps, so we automatically try again when a step fails. I recommend saving the intermediate outputs and skipping steps if you have a valid output. We'll cover more about these topics in chapter 11.

## Testing approach for prompt chains

In the previous sections, we briefly discussed testing. Assuming you have read [#s](#prompt-testing-and-monitoring), you can run two kinds of tests for each step depending on what you need to validate.

There are a few things that we need to discuss about prompt chains and testing. For example, when exactly will you use model-based tests, and how can we make the testing process as smooth as possible? A prompt chain can often grow in complexity quite fast.

### Using property-based tests over model-based tests

When you start building a prompt chain, you typically have a first step we can quickly validate with test input. For example, our workflow has a topic that we can use to generate an outline. At this point, it's helpful to have a model-based test to validate the prompt because you can control the input.

I skipped over the part where we used Google results to generate the outline. That's on purpose because adding content from an external tool will make it harder to write valuable tests. You have to make a choice here:

1. You can run the research step in a unit test, save the Google search results in a test dataset for your tests, and use that dataset to run model-based tests.
2. You can run a property-based test and don't worry about the actual content of the response. In the property-based test, you can validate that you get a result that's a list of items and accept that as enough.

The first option has the advantage that you can switch to a small language model and ensure that it's still producing useful results. The downside is that the search results get stale, so you may run into surprise responses in production.

The second option is more robust and easier to implement. If you're starting, I recommend building a property-based test first and improving your safety net later.

### Following the prompt chain with your tests

As the prompt chain grows, you'll need more results from previous steps to test the next step. I recommend following the prompt chain when writing tests. Start by building the first step, test the first step, and then use the output from the first step to create a test dataset to validate the next step.

Testing prompt chains can be a lot of work, but the more solid the validation process, the more fun you'll have in production and the more thankful the developers who maintain the code will be.

### User testing

User testing will be harder when using a prompt chain like the one we just built because you need to pinpoint exactly where the chain is breaking. I recommend using Application Insights to track interactions with the prompt chain. You can use the content in [#s](#tracing-llm-applications) to help configure Application Insights in your application.

## Optimizations of the prompt chain workflow

The prompt chain in this chapter is helpful if you accept that it sometimes fails. You can take the quality of the prompt chain to a higher level by performing these extra steps:

- First, you can use auto-corrective steps to improve the generated content
- Next, you can speed up the workflow by parallelizing steps
- Finally, you can add intelligence to the step execution to skip steps you executed before for the same input.

Let's review each of these steps to understand how to implement them in the prompt chain we built in this chapter.

### Adding auto-corrective steps

It's important to remember that despite all the effort you put into testing, the prompts in the prompt chain will sometimes fail to generate the desired content.

Using auto-corrective steps can add an additional layer of confidence. To understand this pattern, let's look at the following diagram.

{#auto-corrective-steps-pattern}
![The auto-corrective step pattern](auto-corrective-steps-pattern.png)

In this pattern, we generate content as usual, and then we use a second prompt that takes in the output of the first step along with review instructions and instructions to produce improvements. We then use a third step to execute the improvements on the output generated in step 1. After improving the output generated in step 1, we can go back to the review step to enhance the content further as needed.

It's incredible to see how the LLM can improve itself. However, the auto-corrective step pattern should not be used without any limitations. The LLM will often generate improvement actions and degrade output. I recommend limiting the improvement cycles to one or two attempts. Any more, and you'll risk degrading the output to gibberish.

In chapter 12, we'll explore auto-corrective steps in greater depth when we discuss the artist critic workflow design pattern.

### Adding fan-out operations to parallelize the workflow

As the prompt chain we've built is sequential, it takes a while to generate the full article. After testing the prompt chain from the chapter, I found that it isn't too bad, but you can imagine that it gets worse when you have twenty sections to generate or when one of the steps is slower.

You can use basic fan-out operations to speed up the prompt chain. You could parallelize the for-loop using the built-in parallelization options in C#. We could, for example, convert the for-loop for the research section step and generate the section content step using the following code:

```csharp
var outputBuilder = new StringBuilder();
var output = new Dictionary<int, GenerateSectionContentResult>();

await Parallel.ForAsync(
    0, outline.Sections.Count,
    async (index, cancellationToken) =>
    {
        var section = outline.Sections[index];

        var researchedSectionContent =
            await researchSectionStep.InvokeAsync(topic, section);

        var sectionContent = await writeSectionStep.InvokeAsync(
            topic, section, researchedSectionContent.Query,
            researchedSectionContent.SearchResults);

        output[index] = sectionContent;
    }
);

outputBuilder.AppendLine($"# {outline.Title}");

foreach (var index in output.Keys.OrderBy(x => x))
{
    var sectionContent = output[index];

    outputBuilder.AppendLine($"## {sectionContent.Title}");
    outputBuilder.AppendLine(sectionContent.Content);
}

Console.WriteLine(outputBuilder.ToString());
```

This code is more complex than the original for-loop we had. In this code, we perform the following steps:

1. First, we create a dictionary to collect the results. This is important because while we're going to work in parallel, the sections have an ordering. I need to preserve the order somehow. In this dictionary, I let the parallel algorithm store the output in slots based on the section's index.
2. Next, we iterate over the sections in parallel based on their index and produce the content for each section as usual.
3. Finally, we iterate over the dictionary keys from lowest to highest to construct the final content.

It's faster to parallelize the content generation process. Still, you also increase the likelihood of running into the rate limiter of the LLM, causing delays because you have to wait for it to reset. So, while parallel calls to the LLM sound like a significant improvement, they should be used with care.

### Using intelligent routing to speed up the workflow even more

One of the best tricks I've found in prompt chains like the one we discussed in this chapter is not to parallelize but instead use response caching to speed up the workflow if you need to re-run it.

I'm not going into great depth here, but you can do this to make the prompt chain more solid.

You can store the output generated for a set of inputs in a step. Later, when the step is called again, you can load the result generated earlier based on the input provided to the step. If the output was saved, you can skip the execution of the step and return the result directly. When there's no output available, you can execute the content of the step.

This technique doesn't speed up the initial run, but it does help when the prompt chain fails somewhere down the line, and you have to retry it. I recommend looking into this if you want to build a safer experience in production.

In Chapter 12, we'll look at this technique in greater detail and explore how to implement it for your application.

## Summary

In this chapter, we covered how to use prompt chains to increase the quality of the responses the LLM gives and why you need this approach because the reasoning capabilities remain limited even if you can use LLMs like the o-series models from OpenAI.

We then covered how to implement a prompt chain in Semantic Kernel, explaining step-by-step how to approach and test the steps in the prompt chain. 

Finally, we discussed different steps to improve the chain using caching, parallelization, and auto-corrective steps. In Chapter 12, we'll revisit these optimizations to help you implement them.

In the next chapter, Intelligent Request Routing Workflows, we'll extend the prompt chain pattern by using the LLM to decide the next step in the workflow.

[DRAW_IO]: https://app.diagrams.net/
[SAMPLE_CODE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-09/csharp
[GA_API_KEY]: https://developers.google.com/custom-search/v1/overview
[GA_CONTROL_PANEL]: https://programmablesearchengine.google.com/controlpanel/all
[USER_SECRETS_STORE]: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=windows
[CHATML]: https://github.com/openai/openai-python/blob/release-v0.28.0/chatml.md