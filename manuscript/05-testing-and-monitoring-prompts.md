{#prompt-testing-and-monitoring} 
# Testing and monitoring prompts

In the [#s](#the-art-and-nonsense-of-prompt-engineering), we covered how to use prompt engineering to get results from an LLM. Despite all the prompt engineering techniques available, you'll find that building an LLM-based application is complex. The response will be different every time you use your prompt. There is only one way to ensure your application works reasonably well: to test and continuously improve it.

In this chapter, we'll talk about testing and monitoring prompts. By the end of this chapter you'll understand how to test prompts and how to use production data to improve your prompt tests.

We'll cover the following topics in this chapter:

- Establishing a good test strategy for prompts
- Using deterministic testing methods to validate prompts
- Using model-based testing methods to validate prompts
- Monitoring prompt interactions in production

Let's get started with a good test strategy for testing prompts.

## Establishing a good test strategy for prompts

Prompt testing is a difficult subject because the LLM will give you a different response each time you call it. You can't test for a specific response. And that makes for brittle tests. I know from experience that not many of you will be very happy with brittle tests in their codebase. But there are ways around the problem.

Let's take a step back and consider what you can expect from an LLM-based application first. You can't expect the same response every time from an LLM because the LLM uses sampling to generate response tokens. Also, the runtime environment is inherently indeterministic thanks to modern GPU/TPU hardware. However, you can expect the response to follow a general pattern. This is what we're going to test for.

I dabbled in functional programming in the past, and they have the concept of property-based testing that's useful when building LLM-based applications. Property-based testing is a way to test your code by checking if specific properties hold for your code. The test framework used in these types of tests will generate a set of random input samples and check if the properties hold true for that data. While this method has limitations, it's worth considering for testing prompts.

Generating random input for an LLM is quite challenging. You need the input to follow a valuable and representative pattern for what your users will send in. Ultimately, production will be the place to get the best input for your tests. But that's not possible until you've put something in production. Until then, let's start with a few handwritten samples as input for the prompt.

While frameworks are available for property-based testing, I'm not using any of them in this book or my projects. Building data-driven tests with a test framework like xunit or MSTest is enough.

{#prompt-testing-basics} 
## Using deterministic testing methods to validate prompts

To demonstrate how to build a prompt test, let's evaluate a prompt that we used earlier in the book to generate a recipe for a dish based on the ingredients you have in the fridge. Here's the prompt we used before:

```text
Help me cook something nice, give me a recipe for {{ dish }}.
Use the ingredients I have in the fridge: 

{{#each ingredients}}
    {{ . }}
{{/each}}
```

This prompt takes input from a dish name and a list of ingredients. The LLM will generate a recipe for the dish and instructions on how to cook it. At least, that's what we assume because we didn't explicitly tell the LLM that the recipe should list the ingredients and instructions on how to cook the dish.

To test this prompt, we must build a basic data-driven test that accepts a dish name and a list of ingredients. The following code shows the structure of a prompt test in xunit:

```csharp
public class RecipeGenerationTests
{
    private readonly Kernel _kernel;
    private readonly KernelFunction _prompt;

    public RecipeGenerationTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<RecipeGenerationTests>()
            .Build();

        _kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: configuration["LanguageModel:DeploymentName"]!,
                endpoint: configuration["LanguageModel:Endpoint"]!,
                apiKey: configuration["LanguageModel:ApiKey"]!
            ).Build();

        _prompt = _kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("prompt.yaml"), 
            new HandlebarsPromptTemplateFactory()
        );
    }

    [Theory]
    public async Task TestGenerateRecipe_ContainsInstructions(
        string dish, string[] ingredients)
    {
        // The content of your test.
    }
}
```

This code performs the following steps:

- First, we define a test class with two variables, `_kernel` to store the kernel
instance used in the test, and `_prompt` to store the prompt function that we're testing.
- Next, we create a new constructor for the test class that initializes the kernel and the prompt function. We're using `Microsoft.Extensions.Configuration` to obtain settings from the user-secrets store. This allows us to keep the API key and other sensitive information from the codebase.
- Finally, we define a test method that inputs a dish name and a list of ingredients. We marked the test method as `[Theory]` to enable parameterized testing. The test method will be called for each set of input data.

With the skeleton for the test in place, we can write the test logic to validate the prompt. The following code shows how to write the test logic:

```csharp
[Theory]
[InlineData("pizza", new string[] { "spinach", "mozzarella" })]
public async Task TestGenerateRecipe_ContainsInstructions(
    string dish, string[] ingredients)
{
    var result = await _prompt.InvokeAsync(_kernel, new KernelArguments
    {
        ["dish"] = dish,
        ["ingredients"] = ingredients
    });

    Assert.Contains("ingredients", result.ToString().ToLower());
    Assert.Contains("instructions", result.ToString().ToLower());
}
```

The test performs the following steps:

1. First, we marked the test method with `[InlineData(...)]` to specify a sample for which we want to run the test. You can use inline data or load data from a test file in your code base. You can learn more about the various methods to load test samples for your data-driven test in [this blog post][XUNIT_DATA_DRIVEN_TESTS].
2. Next, we call the prompt function with the dish name and the list of ingredients as
input. The result is stored in the `result` variable.
3. Finally, we use the `Assert.Contains` method to check if the result contains the
words *ingredients* and *instructions*. If the result includes these words, the test passes.

You can extend this test with more samples as you see fit. The test will run for each provided sample, and separate test results will be reported. You can find the complete source code for the test on [Github][PROMPT_TEST_SAMPLE].

It may come as no surprise that running more samples will slow the test, which isn't fast, to begin with. I highly recommend you mark the test with a separate category and only run the LLM-based tests when you're about to finish up a new user story in your application or when you change the prompt, the LLM version, or the model configuration.

You can mark the test with a category by adding the following attribute to the test method:

```csharp
[Trait("Category", "LLM")]
```

With this attribute in place, you can filter the tests in the test runner by category. For example, if you use `dotnet test` to run your tests, you can filter the tests by using the following command:

```bash
dotnet test --filter Category!=LLM
```

This command will run all tests except those marked with the "LLM" category. You can switch the filter to run only the LLM-based tests.

Our sample covers just one test to validate that a prompt generates a useful response. This one test isn't going to be enough, though, because the LLM could generate an inconsistent recipe or a very long response that's hard to read.

To validate the response's more complex properties, we'll use the LLM against itself. Let's look at how you can use model-based testing to validate the prompts' more complex properties.

{#model-based-testing}
## Using model-based testing methods to validate prompts

So far, we've only tested for simple patterns in the output. We've checked if the output contains specific keywords, and we could extend this to check for things like the number of items in a list or the number of words. But we can't check if instructions are consistent or if ingredients use the same kind of units of measurement.

It's typical for LLM-based applications to need more complex tests. You're automating something that solves a more complex problem, so you need more complex tests to check for correctness. And to be honest, despite your best efforts, the tests aren't going to cover all the problems that you'll run into.

However, there's an interesting method for validating more complex properties of the response by using the LLM itself as an analysis tool. I prefer this method of testing model-based prompt testing.

There's a lot of research into model-based testing approaches for LLM-based applications. For example, there's [G-Eval][G_EVAL] that proposes a specific test prompt that scores the response on a single metric. Using G-Eval, you can, for example, check if the response is coherent or if it's consistent with the input. [GPTScore][GPTSCORE] follows a similar pattern where you employ a prompt with instructions to score a response on a specific aspect.

GPTScore and G-Eval have a common strategy to them:

1. First, you use the LLM to generate a response to the prompt with test data.
2. Then, put the response into a test prompt with specific instructions to evaluate a single metric for the prompt. In the test instructions, you include a scale to measure the metric.
3. Finally, you record the score generated by the LLM as the test result.

When you run enough samples with this technique, you see how well the LLM handles your prompt. You can assert the score for a single sample and the average score for a set of samples. That's up to you.

I recommend running multiple samples through the model and looking at the average score. Since LLMs are inherently indeterministic, so you can't rely on a single sample to give a helpful answer.

Let's explore what a test prompt for the setup we just discussed looks like:

```text
## Instructions 

You will be given a recipe for a dish based on the user's ingredients in the fridge. Your task is to rate the
recipe on a single metric. 

## Recipe

{{ recipe }}

## Evaluation criteria

Consistency (1-5): How consistent is the recipe with the
ingredients provided? Are the ingredients used in the recipe?
Are the instructions logical?

## Evaluation steps

1. Read the recipe carefully and identify the ingredients.
2. Read the instructions. Check if they are logical.
3. Check if the instructions use the ingredients provided.
3. Assign a score for the evaluation criteria in the evaluation form.

## Evaluation form (scores ONLY)
```

The test prompt is quite complicated. Let's break it down a bit so you understand the general pattern behind it:

1. In the first section of the prompt, we provide instructions for the task we are working on.
2. In the second section, we're using the one-shot learning pattern to include the recipe initially generated.
3. In the third section, we provide a chain-of-thought pattern to help produce a more exact response.
4. Finally, we steer the LLM to output just the score by giving it the start of its output.

If you leave the final portion out, the model will yap a lot, and it will be super happy to help you score the output. But that's not what you need at all. Putting in hints like "Score:" will help the model steer clear of the yapping and get to the point.

You can run this prompt inside a unit test setup with, for example, xunit. The following code demonstrates this:

```csharp
[Fact]
public async Task TestGenerateRecipe_GeneratesConsistentResults()
{
    var result = await _prompt.InvokeAsync(_kernel, 
        new KernelArguments
        {
            ["dish"] = "pizza",
            ["ingredients"] = new string[] 
            { 
                "spinach", "mozzarella" 
            }
        });

    var testExecutionSettings = new AzureOpenAIPromptExecutionSettings
    {
        ResponseFormat = typeof(EvaluationResult)
    };

    var testResult = await _testPrompt.InvokeAsync(_kernel,
        new KernelArguments(testExecutionSettings)
        {
            ["recipe"] = result.ToString()
        });

    var evaluationResult = JsonSerializer.Deserialize<EvaluationResult>(
        testResult.ToString())!;

    Assert.InRange(evaluationResult.Score, 3, 5);
}
```

The test performs the following steps:

1. First, we generate a response for the prompt we're testing. To keep things simple, we use the pizza input from the previous code samples.
2. Next, we take the prompt's output and put it into the test prompt. We're using a specific set of execution settings to ensure we're generating structured output. We haven't covered this yet, but the `ResponseFormat` setting tells the LLM we want a structured JSON response with a `Score` property.
3. Finally, we deserialize the response to JSON and check if the score is between 3 and 5 as a simple check to see if the response is consistent.

You can expand this test case with more samples. But unlike the sample in [#s](#prompt-testing-basics), you can't use the data-driven approach as xunit tests per sample rather than the whole collection. Instead, you'll want to load a set of samples from CSV or another file format and run them all through the model, collecting the results in a list. Then, you can calculate the average score and check if it's within a specific range.

Despite my efforts to reduce the yapping, the model sometimes produces somewhat irrelevant output. You can't avoid this unless you set the `ResponseFormat` in the execution settings. We'll cover structured output in greater detail in chapter 7.

You can try the code yourself. The full source code for this sample is on [Github][MODEL_BASED_TEST_SAMPLE].

Testing LLM output with LLMs looks very powerful, and it is. But before you go wild with this approach, there's a warning that I need to leave you with.

## The dangers of the model-based testing approach

You should be aware that model-based testing comes with a huge disclaimer. I can't stress this enough: you're dealing with a token-predicting pattern-matching machine. So, the scores you'll see are not actual scores but tokens in a sequence. It's misleading to present this as a proper evaluation. But still, I'm writing about it because testing with this approach does bring some value.

When you ask a human to evaluate a response and score it on a scale from 1 to 5, you get reasonable-sounding results until you ask the human to explain their scoring. There is no mathematical explanation for the score given by human experts. This is the same for the LLM because the score is the result of a pattern-matching exercise with output sampling. You should be looking for this: Are the LLM and human experts in agreement?

After reading and reproducing the papers for G-Eval and GPTScore, I can tell you that the LLM evaluation is strange but shows remarkable agreement with human experts. And that is all you can ask for at the moment. So why are we doing this? It's because it's the best we have at the moment.

With this warning in mind, we need to conclude that testing is a great first step in sustainable LLM-based application development. But we need additional safety nets. Let's look at how you can monitor prompt interactions in production.

## Monitoring prompt interactions in production

In the previous section, we learned how to test prompts in a controlled environment. Unfortunately, testing only gets you so far. The samples we collected in the last section are limited to what you can come up with and might not represent what users will do. The only way to get test data representative of the real world is to collect it from production.

A> Gathering telemetry data in Semantic Kernel is experimental at the moment. There are some valuable bits of information that you can collect today, but they are somewhat incomplete. I'm sure they'll improve this in the coming months.

### Safety precautions when collecting telemetry

Before collecting telemetry data from your LLM-based application, you must ensure you can. Users will enter all sorts of information into your application, and you must take the proper precautions before collecting any of that data.

For my projects, I notify users when I'm collecting data and what I'm collecting. I also make sure that I'm not collecting any personal information. For most projects, I add a switch in the configuration so I can toggle the collection of prompts and responses. I enable the switch in a separate environment and only collect data for a limited time.

This approach limits what I can achieve, but it's a good compromise between respecting the users' privacy and collecting enough data to give them the best product possible.

There's another reason to limit your data collection efforts. Most of the data you'll get from production doesn't add value for testing, so it's better to target your collection efforts as much as possible.

With that in mind, let's look at enabling telemetry collection in Semantic Kernel.

### Enabling tracing in your LLM-based application

The Semantic Kernel generates telemetry data in the [OpenTelemetry][OPEN_TELEMETRY] format through the .NET diagnostics stack. Let me give you a brief overview of these concepts.

OpenTelemetry is a set of APIs, libraries, and tools for instrumenting applications to generate telemetry and process that telemetry into useful insights. The standard is implemented in many places for many languages. OpenTelemetry doesn't provide monitoring tools; it just uses libraries to get data into a monitoring tool.

Microsoft implements OpenTelemetry in .NET. Because the .NET stack is quite old, Microsoft hasn't adopted the same terminology as OpenTelemetry. The OpenTelemetry standard came after .NET was invented. That's why you'll find that the .NET stack uses different terminology for the same things. I made the following table to help you understand the differences:

| Concept | .NET Equivalent | Description                                                               |
| ------- | --------------- | ------------------------------------------------------------------------- |
| Span    | Activity        | A span is a unit of work in a trace. It has a start time and an end time. |
| Tracer  | ActivitySource  | A span/activity is written to a tracer that processes it further          |
| Event   | Log message     | An event with a description and attributes                                |
| Metric  | Meter           | A metric is a value that changes over time.                               |

Multiple spans will form a distributed trace in OpenTelemetry. So you can have calls to the LLM produce spans and other methods in your application. The spans are linked through a Trace ID. You can see related spans in a single trace when you write the traces to a monitoring application like Application Insights.

Let's explore tracing in .NET more with an example. You can generate a trace in your code using an ``ActivitySource` using the following code:

```csharp

public class MyApplicationService
{
    private static readonly activitySource = 
        new ActivitySource("My.ActivitySource", "1.0.0");

    public void DoSomething()
    {
        using var activity = activitySource.StartActivity("DoSomething");
        // Do something
    }
}
```

This code generates a trace with the name `DoSomething` when you call the `DoSomething` method. The trace is generated by the activity source `My.ActivitySource` with version `1.0.0`.

Activities should always be created with a `using` statement to ensure that they are collected by the garbage collector as soon as we're done. This is important because when you clean up an activity, the tracer records it, including how long it ran. So, cleaning up is a time-sensitive action.

The span produced by the activity is written to a tracer called an `ActivitySource` in .NET. The activity source is responsible for writing the span to an exporter. We haven't configured one in this code because it's up to the consumer of our code to choose how and where to collect telemetry. You can add the following code to the startup of a console application to build an essential trace exporter that writes traces to the console:

```csharp
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService("Chapter5.Telemetry");

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("My.ActivitySource")
    .AddConsoleExporter()
    .Build();
```

In this code, we perform the following steps:

1. First, we create a resource builder that adds metadata to the traces, such as the application name and version.
2. Next, we create a tracer provider builder to configure with a source and an exporter, including the activity source we want to process and export the traces to the terminal.

You can configure multiple sources to the tracer provider. Only traces of sources you've added to the tracer provider will be exported. In the sample, we only export our custom source. You can add sources using a prefix notation to say: `My*` to export anything that starts with `My`.

The exporter is responsible for writing traces to a monitoring system. In this case, we're writing traces to the terminal, but you want something more durable in production. For example, an Azure Monitor exporter can write the traces to Application Insights. But you can also use other exporters. There are a lot of packages available on Nuget.

We'll need to modify the code to export traces from Semantic Kernel. The following code shows how to add telemetry to the Semantic Kernel:

```csharp
using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Microsoft.SemanticKernel*")
    .AddConsoleExporter()
    .Build();
```

This code adds the `Microsoft.SemanticKernel*` prefix as a source to the tracer provider. And that's all you need to do to enable tracing for Semantic Kernel.

But what does Semantic Kernel export to the tracer provider? Spans created by Semantic Kernel contain data as defined in [the semantic conventions for generative AI systems][GENAI_STANDARD]. This experimental standard guides what information is needed to monitor LLM-based applications properly. It provides the following attributes, among others, for spans generated when calling an LLM:

- `gen_ai.system`: The LLM provider you called in the code.
- `gen_ai.request.model`: The model you called in the code.
- `gen_ai.response.completion_tokens`: The number of tokens in the response.
- `gen_ai.response.prompt_tokens`: The number of tokens in the prompt.
- `gen_ai.prompt`: The prompt you submitted.
- `gen_ai.completion`: The response generated by the LLM.

By default, Semantic Kernel collects no sensitive information. To collect the prompt and the LLM's response to your prompt, you need to add the following code to the startup of your application:

```csharp
AppContext.SetSwitch(
    "Microsoft.SemanticKernel.Experimental." +
    "GenAI.EnableOTelDiagnosticsSensitive", true);
```

Tracing will be your most important tool to track what data is processed with the LLM in production so you can later use it for debugging purposes.

Next to tracing, you can also use metrics to track the performance of your LLM-based application.

### Enabling metrics in your LLM-based application

Building meters into .NET code differs from adding traces because you can have various kinds of meters in your application. You can have counters, histograms, and gauges. The `Meter` class is the entry point for creating various types of meters in a specific category. The following code demonstrates how this works:

```csharp
public class MyMeteredService
{
    private static readonly Meter meter = 
        new Meter("My.MeterCategory");

    private static readonly Counter callCounter = 
        meter.CreateCounter<int>(
            "my.metercategory.meteredservice.calls");

    public void DoSomething()
    {
        callCounter.Add(1);
        // Do something
    }
}
```

This code performs the following steps:

1. First, we create a meter to store all metrics related to a specific category of information. The meter is created once and kept in a static variable.
2. Next, we create a counter to measure the number of calls to the `DoSomething` method.
3. Finally, we increment the counter by one each time the `DoSomething` method is called.

If you're interested in learning more about the specific metrics types, I recommend looking [at the OpenTelemetry documentation][OTEL_METRICS].

To export metrics through OpenTelemetry, we need to write the following code in the startup of the application:

```csharp
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("My.MeterCategory*")
    .AddConsoleExporter()
    .Build();
```

This code performs the following steps:

1. First, we create a `MeterProviderBuilder` using the `Sdk` component offered by OpenTelemetry.
2. Next, we provide a resource builder to add metadata like the application name to the metrics.
3. Then, we add the meter category we want to export.
4. Finally, we add a console exporter to write the metrics to the terminal.

For example, with traces, you can use different exporters to write the metrics for other destinations. Only the metrics you configure with `AddSource` are exported to the configured exporters.

Now that we've covered adding metrics to an application, let's examine how to export metrics from the Semantic Kernel category.

To export metrics for the Semantic Kernel category in a console application, we need to modify the code a bit, just like we did with the traces:

```csharp
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("Microsoft.SemanticKernel*")
    .AddConsoleExporter()
    .Build();
```

This code adds the `Microsoft.SemanticKernel*` prefix as a meter to the meter provider. And that's all you need to do to enable metrics for Semantic Kernel.

The metrics generated by Semantic Kernel are defined in [the same standard][GENAI_STANDARD] that establishes the tracing properties. I highly recommend reviewing it to decide what metrics make sense for you.

We haven't covered logging yet; let's look at that before we dive into exporting traces to a monitoring tool like Application Insights.

### Logging in LLM-based applications

You can configure logging as a separate piece of monitoring in Semantic Kernel. When you're building a console application, you'll need to add the following code to the startup:

```csharp
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddConsoleExporter();
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });

    builder.SetMinimumLevel(LogLevel.Information);
});
```

The code performs the following steps:

1. First,  we create a new logger factory using the `LoggerFactory` class.
2. Next, we configure Open Telemetry as a logging provider, using the same OpenTelemetry settings as we did for traces and metrics. We then use the console exporter to write the logs to the terminal.
3. Finally, we set the minimum log level to `Information`.

You can write log messages straight to the terminal without using OpenTelemetry, but this will break monitoring when you want to use a tool like Application Insights. Using OpenTelemetry with the logging factory connects the log messages as events to the traces generated by the application. This way, you can view traces and log messages as a single unit in the monitoring tool.

The OpenTelemetry console exporter provides a nice method to check that everything is configured as intended, but I don't recommend using it in production. It will slow down your application, and it's not durable. Let's look at how to configure Application Insights as an exporter and how to configure a dashboard for your LLM-based application using this tool.

### Writing monitoring data to application insights

Configuring Application Insights as an exporter for traces, metrics, and logs requires an additional package. Add the `Azure.Monitor.OpenTelemetry.Exporter` package to your project. Use the following terminal command to add the package to your project:

```bash
dotnet add package Azure.Monitor.OpenTelemetry.Exporter
```

The following code demonstrates how to configure the exporter in a console application:

```csharp
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService("Chapter5.Telemetry");

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Microsoft.SemanticKernel*")
    .AddAzureMonitorTraceExporter(options =>
    {
        options.ConnectionString =
        configuration["Monitoring:ConnectionString"];
    })
    .Build();

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("Microsoft.SemanticKernel*")
    .AddAzureMonitorMetricExporter(options =>
    {
        options.ConnectionString = 
        configuration["Monitoring:ConnectionString"];
    })
    .Build();
```

This code performs the following steps:

1. First, we configure the resource builder for the service metadata to include in the metrics and traces.
2. Then, we configure the trace provider as before, but we're adding the Azure Monitor exporter this time. We use the connection string from the configuration to connect to Application Insights.
3. Next, we configure the meter provider as before, but this time, we add the Azure Monitor exporter using the same connection string we used for the tracer provider.

You can find the complete source code for this sample on [Github][MONITORING_SAMPLE].

With this code in place, you can start monitoring your LLM-based application in Application Insights. You need to set up an Application Insights resource in Azure to obtain the connection string for the exporters. A great manual for this is available in [the Application Insights documentation][AI_DOCS].

A> The samples in this chapter only talk about console applications. But you can certainly use OpenTelemetry in ASP.NET Core. You can set it up using [this guide][OTEL_ASPNET].

One way to monitor your application is by setting up a dedicated dashboard in Application Insights. Let's dive into setting up an LLMOps dashboard in this tool.

### Building an LLMOps dashboard with Application Insights

I had to choose which tool to show you for monitoring LLM-based applications. Many startups are working on this, but they're not quite there yet. It's not standardized, and many of those tools are expensive while providing limited value.

While Application Insights doesn't focus on monitoring LLM-based applications, it is a good-quality general-purpose monitoring tool in Azure. LLM-based applications have special requirements for monitoring the prompts and responses, but the scope of that is pretty limited, to be honest. It does take a bit of effort to make a useful dashboard for applications that use an LLM, but it is doable.

There are a couple of things that you'll want to see in an LLMOps dashboard:

- What are the costs of running your application?
- How fast is the application, and is it within an acceptable range?
- Are any of your requests failing?

Let's start by creating a dashboard in your Azure environment. Navigate to the homepage of [the Azure portal](https://portal.azure.com). Then click on the hamburger menu in [#s](#azure-portal-hamburger-menu).

{#azure-portal-hamburger-menu} 
![The hamburger menu in the Azure Portal](azure-portal-hamburger-menu.png)

Select the *Dashboard* option from the hamburger menu and click the Create toolbar button. This will show the screen from [#s](#create-dashboard-screen).

{#create-dashboard-screen}
![The create dashboard screen](create-azure-dashboard-screen.png)

The screen shows several predefined patterns for building a dashboard. I personally prefer to start with an empty dashboard. However, if you're new to building a dashboard, it can be useful to choose the Application Insights template. It contains a set of useful charts that show your application's basic health signals.

After selecting a starting point, you're presented with the dashboard editor. If you've chosen the custom dashboard template, it will look similar to the one shown in [#s](#dashboard-editor-screen).

{#dashboard-editor-screen}
![The dashboard editor screen](edit-azure-dashboard-screen.png)

The dashboard editor shows a grid to display metrics and other information as tiles. The tile gallery shows various types of tiles you can add to the dashboard.

For the LLMOps dashboard, we'll add two tiles: one to display the number of input tokens as a time series chart and the output tokens as a time series chart. Select the *Metrics Chart* tile type from the list to the right of the screen and click the *Add* button twice. When you've done that, edit the dashboard's name and save it. You should end up with a screen like the one in [#s](#edited-dashboard-screen).

{#edited-dashboard-screen}
![The dashboard with two charts added](edited-azure-dashboard.png)

You can click on either of the charts to configure the metric you want to display. After clicking one of the charts, you're presented with the metric configurator. This tool allows you to select a scope and display a metric from that scope. You'll need to choose the following options for the scope, metric namespace, metric, and aggregation:

- *Scope:* The name of your application insights resource
- *Metric namespace:* Log-based metrics
- *Metric:* semantic_kernel.invocation.function.token_usage.prompt
- *Aggregation:* Avg

After setting the configuration, save the metric by clicking the *Save to dashboard* button in the top right of the screen. [#s](#configured-metric-screen) shows the fully configured metric for average input tokens.

{#configured-metric-screen}
![Fully configured metric for input tokens](configured-input-token-metric.png)

You can repeat the same steps for the output tokens metric. The metric you must select is `semantic_kernel.invocation.function.token_usage.completion`.

After configuring the metrics, you can add another tile to the dashboard to show the duration of each prompt in your application. The metric you need for this is `semantic_kernel.invocation.function.duration`.

I've found that displaying separate charts for each prompt template in the application is useful for learning how each individual prompt behaves. You can add a filter to the metric configuration using the *Add filter* button. You can filter on the `semantic_kernel.function.name` property. The name will match the name of the prompt template. If you've used a YAML file to store your prompt template, you can filter on the value of the name property in the YAML file.

Note that as your application grows, managing the dashboard becomes harder. You probably have a few prompt templates that you're using. It's a good idea to put only those metrics that require attention in the dashboard because you've recently changed them or added them to the application. Dashboards are a living piece of your monitoring setup and should be updated regularly. If you don't, you'll likely not look at it or do anything with the information on it.

Let's add the final piece of the puzzle. The tokens and duration metrics are useful for finding unusual things. But it's the costs that matter for most people. Here's how to add them to the dashboard.

Navigate to the OpenAI Resource in the Azure Portal, and select the *Resource Management* > *Cost analysis* option from the left sidebar. This will show you a chart displaying the costs for the resource. To add the cost analysis to your dashboard, click the pushpin next to the title *Cost analysis* and choose the dashboard to which you want to add it.

Now that we have monitoring let's return to collecting feedback information. If you've enabled the collection of prompts and responses in your application, you can export that data and use it to improve your tests.

### Collecting data to improve tests

Telemetry data in Application Insights is stored in a structured format in a Log Analytics Workspace. You can query the data using the [Kusto Query Language (KQL)][KUSTO_INTRODUCTION]. But you can also export it to a storage account for later use.

We'll use the data export feature to retrieve the prompts and responses for specific prompt templates. After we've downloaded the data, we'll build an application to extract the prompts and responses from the raw log data and use them to improve our tests.

Before exporting data, ensure you have a storage account with hierarchical namespaces enabled. You can create one using [this guide][CREATE_STORAGE_ACCOUNT].

Let's create an export rule to copy data from the Log Analytics Workspace to the storage account. You can get access to the Log Analytics Workspace for your Application Insights resource by going to the Application Insights in the Azure portal and clicking on the link next to the Workspace property in the overview page of the resource. This will take you to the Log Analytics Workspace.

In the Log Analytics Workspace, you can find the data export feature in the sidebar of the workspace resource under *Settings* > *Data export*. [#s](#export-rules-overview) shows the data export overview page.

{#export-rules-overview} 
![Export rules overview page](azure-law-export-rules-overview.png)

Create a new rule by clicking the *New export rule* button. This will take you to the rule creation page. On this page, you can set a name for the new rule, select the tables you want to export, and select the destination for the data. [#s](#export-rule-step-1) shows what this page looks like.

{#export-rule-step-1} 
![Export rule creation page](azure-law-export-rules-step-1.png)

Provide a descriptive name for the rule, and click *Next* to go to the next step as shown in [#s](#export-rule-step-2). You can select the tables you want to export on the next screen. The `AppTraces` table contains the data for the prompts and responses. Select this table and click *Next* to the destination configuration screen.

{#export-rule-step-2} 
![Table selection screen](azure-law-export-rules-step-2.png)

You have two choices for the data's destination. We'll store the exported data in a storage account. [#s](#export-rule-step-3) shows the screen to configure the destination.

{#export-rule-step-3} 
![Destination configuration screen](azure-law-export-rules-step-3.png)

Select the storage account where you want to store the exported data and complete the configuration.

After you've completed the configuration, any new data coming into the Log Analytics Workspace is automatically exported to your storage account. The export job in Azure will create a new container for each table you've selected. Since we selected the application insights traces, we'll get a container called `am-apptraces`.

You can download the data from the storage account using the [Azure Storage Explorer][STORAGE_EXPLORER] or programmatically using the [Azure Blob Storage Package][NUGET_BLOB_STORAGE].

Let's look at how you can use the Blob Storage package to download the data from the storage account. The following code demonstrates how to set up a connection to the storage account and download all the data:

```csharp
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var blobServiceClient = new BlobServiceClient(
    configuration["ConnectionStrings:BlobStorage"]);

var traceContainerClient = blobServiceClient.GetBlobContainerClient(
    "am-apptraces");
```

The code performs the following steps:

1. First, we build a configuration object to securely load the connection string for the blob storage account.
2. Next, we create a new `BlobServiceClient` object, providing a connection string for the storage account containing the trace data.
3. Finally, we get a reference to the container that contains the trace data.

After we've obtained a connection, we can start processing the JSON files in the storage container one by one:

```csharp
var processor = new TraceEventDataProcessor();

var serializerSettings = new JsonSerializerOptions()
{
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
};

await foreach (var blobItem in traceContainerClient.GetBlobsAsync())
{
    if (blobItem.Name.EndsWith(".json"))
    {
        var blobClient = traceContainerClient.GetBlobClient(blobItem.Name);
        using var reader = new StreamReader(blobClient.OpenRead());

        while (!reader.EndOfStream)
        {
            var rawEventData = reader.ReadLine();

            var eventData = JsonSerializer.Deserialize<TraceEventData>(
            rawEventData!, serializerSettings);

            if (eventData!.Message == "gen_ai.content.completion" || 
                eventData.Message == "gen_ai.content.prompt")
            {
                processor.ProcessEvent(eventData);
            }
        }
    }
}
```

This code performs the following steps:

1. First, we create a new `TraceEventDataProcessor` object to process the trace data into functional test samples.
2. Next, we iterate over the files in the trace storage container and read the contents of each file. The content in the file is stored in JSON lines format. Each line is a JSON object.
3. Then, we deserialize the JSON object data into a `TraceEventData` object, which is a class that will represent the trace data.
4. Finally, we process the trace data using the `TraceEventDataProcessor` object.

Let's look at the shape of the `TraceEventData` class first:

```csharp
public class TraceEventData
{
    public string Message { get; set; }
    public TraceProperties Properties { get; set; }
    public DateTime TimeGenerated { get; set; }
}

public class TraceProperties
{
    [JsonPropertyName("gen_ai.completion")]
    public string? Completion { get; set; }

    [JsonPropertyName("gen_ai.prompt")] 
    public string? Prompt { get; set; }
}
```

The `TraceEventData` class contains the data logged as part of a single trace span. It includes a `Message` property that describes the type of span we're looking at. It also contains a `Properties` property containing the extra metadata we need to process. The `Properties` property includes the content we need for prompts and responses.

Without processing it further, you can't do much with the data in the `TraceEventData` class. You need two copies of it to get the prompt and the completion because they're logged as two separate spans. We have the `TraceEventDataProcessor` class to process the data into helpful test samples.

```csharp
public class TraceEventDataProcessor
{
    private string? _currentPrompt;
    private bool _isParsingPair;

    public List<PromptCompletionPair> ParsedPromptCompletionPairs { get; } = new();

    public void ProcessEvent(TraceEventData eventData)
    {
        if (eventData.Message == "gen_ai.content.prompt")
        {
            _currentPrompt = eventData.Properties.Prompt;
            _isParsingPair = true;
        }

        if (_isParsingPair)
        {
            if (eventData.Message == "gen_ai.content.completion")
            {
                if (!string.IsNullOrEmpty(_currentPrompt))
                {
                    var pair = new PromptCompletionPair()
                    {
                        Prompt = _currentPrompt,
                        Completion = eventData.Properties.Completion!
                    };

                    ParsedPromptCompletionPairs.Add(pair);
                }
            }
        }
    }
}
```

The `TraceEventDataProcessor` class works as follows:

1. First, we look at the message in the incoming `TraceEventData` record. If this message is a prompt, we store the prompt temporarily in the `_currentPrompt` variable and set the `_isParsingPair` flag to true.
2. When parsing a pair and receiving a `gen_ai.content.completion` record, we create a new `PromptCompletionPair` object and add it to the list of parsed pairs.

The `PromptCompletionPair` class is a simple class that contains the prompt and the completion:

```csharp
public class PromptCompletionPair
{
    public string Prompt { get; set; }
    public string Completion { get; set; }
}
```

After you've processed all the trace data, you can store the data from the `ParsedPromptCompletionPairs` property in a CSV file. You can then analyze this CSV file to improve your tests.

Feel free to download and adapt the sample code for your project. The complete source code for the sample is on [Github][EXTRACTION_SAMPLE].

Now, why not use the test data directly in the tests? There are two problems with using the raw test data.

First, we don't have the input variables for the prompt template we executed. We can only see the rendered prompt. Also, we don't know which prompt template was executed because Semantic Kernel doesn't log that information yet.

Second, the most important reason is that the data could be poisoned by someone trying to break your application. People can enter all sorts of instructions in the prompt, including less savory content. You don't want to run that through your tests.

Given these limitations, I understand that expanding your test set will take effort. But it's worth it. Although raw, the data is useful for understanding how users are using your application.

## Summary

In this chapter, we've looked at how you can test and monitor your LLM-based application. We started by looking at how to test prompts in a controlled environment. We then covered how to add monitoring, build a dashboard, and collect data from the monitoring environment to improve the tests.

In the next chapter, we'll expand the basic LLM functionality with custom tools. Learning about using tools finalizes the fundamentals of building an LLM-based application before looking at design patterns.

## Further reading

- [GPTScore: Evaluate as you desire](https://arxiv.org/abs/2302.04166)
- [G-Eval: NLG Evaluation using GPT-4 with Better Human Alignment](https://arxiv.org/abs/2303.16634)

[XUNIT_DATA_DRIVEN_TESTS]: https://hamidmosalla.com/2017/02/25/xunit-theory-working-with-inlinedata-memberdata-classdata/
[G_EVAL]: https://arxiv.org/abs/2303.16634
[GPTSCORE]: https://arxiv.org/pdf/2302.04166
[OPEN_TELEMETRY]: https://opentelemetry.io/
[GENAI_STANDARD]: https://opentelemetry.io/docs/specs/semconv/gen-ai/
[AI_DOCS]: https://learn.microsoft.com/en-us/azure/azure-monitor/app/create-workspace-resource?tabs=bicep#create-a-workspace-based-resource
[KUSTO_INTRODUCTION]: https://learn.microsoft.com/en-us/azure/azure-monitor/logs/log-analytics-overview
[CREATE_STORAGE_ACCOUNT]: https://learn.microsoft.com/en-us/azure/storage/blobs/create-data-lake-storage-account
[STORAGE_EXPLORER]: https://learn.microsoft.com/en-us/azure/storage/storage-explorer/vs-azure-tools-storage-manage-with-storage-explorer?tabs=windows
[NUGET_BLOB_STORAGE]: https://www.nuget.org/packages/Azure.Storage.Blobs
[MONITORING_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-05/csharp/Chapter5.ApplicationInsightsExporter
[EXTRACTION_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-05/csharp/Chapter5.ExtractPromptTestData
[PROMPT_TEST_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-05/csharp/Chapter5.PromptTestingBasics
[MODEL_BASED_TEST_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-05/csharp/Chapter5.ModelBasedTesting
[OTEL_METRICS]: https://opentelemetry.io/docs/concepts/signals/metrics/
[OTEL_ASPNET]: https://medium.com/@jepozdemir/configuring-opentelemetry-tracing-for-asp-net-core-114c2c9cf557
