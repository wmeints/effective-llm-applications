{#prompt-testing-and-monitoring}
# Testing and Monitoring Prompts

In the previous chapter we covered how to use prompt engineering to get useful results
out of an LLM. Despite all the prompt engineering techniques we covered, you'll find
that building an LLM-based application is hard. The response will be different every
time you use your prompt. There is only one way to make sure your application works
reasonably well, you're going to have to test and continuously improve it.

In this chapter, we'll talk about testing and monitoring prompts. We'll cover the
following topics:

- Establishing a good test strategy for prompts
- Using deterministic testing methods to validate prompts
- Using model-based testing methods to validate prompts
- Monitoring prompt interactions in production

Let's get started by getting a good test strategy in place for testing prompts.

## Establishing a good test strategy for prompts

Prompt testing is a difficult subject, because you'll find that the LLM will give you a
different response each time you call it. You can't test for a specific response. And
that makes for brittle tests. I know from experience that not many of you will be very
happy with brittle tests in their codebase. But there are ways around the problem.

Let's take a step back and look at what you can expect from an LLM-based application
first. You can't expect the same response every time from an LLM. This is because the
LLM uses sampling to generate response tokens. Also, the runtime environment is
inherently undeterministic thanks to the use of modern GPU/TPU hardware. However, you
can expect the response to follow a general pattern. This is what we're going to test
for.

I dabbled in functional programming in the past, and they have the concept of
property-based testing that's quite useful when building LLM-based applications.
Property-based testing is a way to test your code by checking if certain properties hold
true for your code. The test framework used in these types of tests will generate random
input data and check if the properties hold true for that data. While this method has
its own limitations, it's worth considering for testing prompts.

Property-based testing leans on random inputs to test your code. Generating random input
for an LLM is quite hard. You need the input to follow a useful pattern and be
representative for what your users are going to send in. For now, let's put this problem
aside and start with a few handwritten samples as input for the prompt.

While there are frameworks available for property-based testing, I'm not using any of
them in this book or in my own projects. I've found that it is enough to build
data-driven tests with a test framework like xunit or MSTest.

{#prompt-testing-basics}
## Using deterministic testing methods to validate prompts

To demonstrate how to build a prompt test, let's go back to a prompt that we used
earlier in the book to generate a recipe for a dish based on ingredients you have in the
fridge. Here's the prompt we used before:

```text
Help me cook something nice, give me a recipe for {{ dish }}.
Use the ingredients I have in the fridge: 

{{#each ingredients}}
    {{ . }}
{{/each}}
```

This prompt takes a dish name and a list of ingredients as input. The LLM will generate
a recipe for the dish and instructions on how to cook it. At least that's what we
assume, because we didn't explicitly tell the LLM that the recipe should list the
ingredients and instructions on how to cook the dish.

To test this prompt, we need to build a basic data-driven test that accepts a dish name
and a list of ingredients. The following code shows the structure of a prompt test in
xunit:

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
                configuration["LanguageModel:DeploymentName"]!,
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
  instance used in the test, and `_prompt` to store the prompt function that we're
  testing.
- Next, we create a new constructor for the test class that initializes the kernel and
  the prompt function. We're using `Microsoft.Extensions.Configuration` to obtain
  settings from the user-secrets store. This allows us to keep the API key and other
  sensitive information out of the codebase.
- Finally, we define a test method that takes a dish name and a list of ingredients as
  input. We marked the test method as `[Theory]` to enable parameterized testing. The
  test method will be called for each set of input data.

With the skeleton for the test in place, we can write the test logic to validate the
prompt. The following code shows how to write the test logic:

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

1. First, we extended the test method with `[InlineData(...)]` to specify a sample that
   we want to run the test for. You can use inline data, or you can load data from a
   test file in your code base. You can learn more about the various methods to load
   test samples for your data driven test in [this blogpost][XUNIT_DATA_DRIVEN_TESTS].
2. Next, we call the prompt function with the dish name and the list of ingredients as
   input. The result is stored in the `result` variable.
3. Finally, we use the `Assert.Contains` method to check if the result contains the
   words "ingredients" and "instructions". If the result contains these words, the test
   passes.

You can extend this test with more samples as you see fit. The test will run for each
of the provided samples and report a separate test results for each of them.

It may come as no surprise that running more samples will make the test slower, and it
isn't fast to begin with. I highly recommend you mark the test with a separate category
and only run the LLM-based tests periodically, for example when you're about to finish
up a new user story in your application or when you change the prompt, the model, or
model configuration.

You can mark the test with a category by adding the following attribute to the test
method:

```csharp
[Trait("Category", "LLM")]
```

With this attribute in place, you can filter the tests in the test runner by category.
For example, if you use `dotnet test` to run your tests, you can filter the tests by
using the following command:

```bash
dotnet test --filter Category!=LLM
```

This command will run all tests except the ones marked with the "LLM" category. You can
switch the filter around to run just the LLM-based tests.

Our sample covers just one test to validate that a prompt generates a useful response.
This one test isn't going to be enough though, because the LLM could generate a recipe
that's inconsistent. It could also generate a very long response that's hard to read.

To validate more complex properties of the response we're going to use the LLM against
itself. Let's look at how you can use model-based testing to validate more complex
properties of your prompts.

## Using model-based testing methods to validate prompts

So far we've only tested for simple patterns in the output. We've checked if the output
contains specific keywords, and we could extend this to check for things like number of
items in a list or the number of words. But we can't check if instructions are
consistent or if ingredients use the same kind of units of measurement.

This is typical for LLM-based applications. You're automating something that solves a
more complex problem so you need more complex tests to check for correctness. And to be
honest, the test isn't going to cover for all the problems that you'll run into.

However, there's an interesting method that you can use to validate more complex
properties of the response by using the LLM itself as an analysis tool. I prefer to call
this method of testing model-based prompt testing.

There's a lot of research into prompt testing by employing LLMs to validate the
response. For example, there's [G-Eval][G_EVAL] that proposes a specific test prompt
that scores the response on a single metric. Using G-Eval you can for example check if
the response is coherent or if it's consistent with the input. [GPTScore][GPTSCORE]
follows a similar pattern where you employ a prompt with instructions to score a
response on a specific aspect.

GPTScore and G-Eval have a common strategy to them:

1. First, you use the LLM to generate a response to the prompt with test data.
2. Then, put the response back into a test prompt with specific instructions to
   evaluate a single metric for the prompt. In the test instructions you include a scale
   to measure the metric.
3. Finally, you record the score generated by the LLM as the test result.

When you run enough samples with this technique, you end up getting a picture of
how well the LLM handles your prompt. You can assert on the score for a single sample,
but you can also assert on the average score for a set of samples. That's up to you.

I recommend running multiple samples through the model and looking at the average score.
Since LLMs are inherently undeterministic, you can't rely on a single sample to give you
a useful answer.

Let's explore what a test prompt for the setup we just discussed looks like:

```text
## Instructions 

You will be given a recipe for a dish based on ingredients
that the user has in the fridge. Your task is to rate the
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
3. Check if instructions use the ingredients provided.
3. Assign a score for the evaluation criteria in the evaluation form.

## Evaluation form (scores ONLY)
```

You can run this prompt inside a unit-test setup with, for example, xunit. The following
code demonstrates this:

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

1. First, we generate a response for the prompt we're testing. We use the pizza input
   from the previous code samples to keep things simple.
2. Next, we take the output of the prompt and put it into the test prompt. We're using
   a specific set of execution settings to ensure we're generating structured output.
   We haven't covered this yet, but the `ResponseFormat` setting is used to tell the
   LLM we want a structured response that has a `Score` property.
3. Finally, we deserialize the response to JSON and check if the score is
   between 3 and 5. This is a simple check to see if the response is consistent.

You can expand this test case with more samples. But unlike the sample in
[#s](#prompt-testing-basics) you can't use the data-driven approach as xunit tests per
sample rather than the whole collection. Instead, you'll want to load a set of samples
from CSV or other file format and run them all through the model collecting the results
in a list. Then you can calculate the average score and check if it's within a certain
range.

Before you go wild with this approach, there's a warning that I need to leave you with.

## The dangers of the model-based testing approach

You should be aware that model-based testing comes with a huge disclaimer. I can't
stress this enough, you're dealing with a token predicting pattern matching machine. So
the scores you're going to see are not actual scores, but a token in a sequence. It's
quite misleading to present this as a proper evaluation. But still, I'm writing about
it, because testing with this approach does bring some value.

When you ask a human to evaluate a response and score the response on a scale from 1 to
5 you get reasonable sounding results. Until you ask the human to explain their scoring.
There is no mathematical explanation to the score. And this is the same for the LLM
because the score is the result of a pattern matching exercise with output sampling. The
thing you should be looking for is this: Are the LLM and human expert in agreement?

After reading and reproducing the papers for G-Eval and GPTScore I can tell you that the
LLM evaluation is strange in nature, but does show remarkable agreement with human
experts. And that is all you can ask for at the moment. So if you're wondering, why are
we doing this? It's because it's the best we have at the moment.

While testing is a great first step, you'll need to add monitoring as an extra
safety net to keep your LLM-based application running as intended.

## Monitoring prompt interactions in production

From the previous section we learned how to test prompts in a controlled environment.
Testing only gets you so far sadly. The samples we collected in the previous section are
limited to what you can come up with and might not be representative of what users are
going to do. The only way we can get test data that's representative of the real world
is to collect it from production.

A> Gathering telemetry data in Semantic Kernel is experimental at the moment. There are
A> some useful bits of information that you can collect today but it is somewhat
A> incomplete. I'll make sure to point out the stuff that's missing so you can work
A> around them.

### Safety precautions when collecting telemetry

Before you start collecting telemetry data from your LLM-based application, you need to
ensure that you're allowed to. Users will enter all sorts of information into your
application, and you need to take the proper precautions before collecting any of that
data.

For my projects, I notify users when I'm collecting data and what I'm collecting. I also
make sure that I'm not collecting any personal information. For most projects I add
a switch in the configuration, so I can toggle collection of prompts and response.
I enable the switch on a separate environment and only collect data for a limited time.

It's limiting what I can achieve with the data, but it's a good compromise between
collecting data that I don't want to see and improving the application. Most of the data
that you'll get from production doesn't add value for testing, so it's better to target
your collection efforts as much as possible.

With that in mind, let's look at enabling telemetry collection in Semantic Kernel.

### Enabling tracing in your LLM-based application

Semantic Kernel generates telemetry data in the
[OpenTelemetry][OPEN_TELEMETRY] format through the .NET diagnostics stack. In case
you're not familiar with OpenTelemetry and the .NET diagnostics stack, let me give you a
brief overview.

OpenTelemetry is a set of APIs, libraries, and tools to instrument applications to
generate telemetry and process that telemetry into useful insights. The standard is
implemented in a lot of places for a lot of languages.

Because the .NET stack is quite old, Microsoft hasn't adopted quite the same terminology
as OpenTelemetry. The OpenTelemetry standard came after .NET was invented. That's why
you'll find that the .NET stack uses different terminology for the same things. I made
the following table to help you understand the differences:

| Concept | .NET Equivalent | Description                                                               |
| ------- | --------------- | ------------------------------------------------------------------------- |
| Span    | Activity        | A span is a unit of work in a trace. It has a start time and an end time. |
| Tracer  | ActivitySource  | A span/activity is written to a tracer that processes it further          |
| Event   | Log message     | An event with a description and attributes                                |
| Metric  | Meter           | A metric is a value that changes over time.                               |

Multiple spans will form a distributed trace in OpenTelemetry. So you can have calls to
the LLM produce spans as well as other methods in your application. The spans are linked
through a Trace Id. When you write the traces to a monitoring application like
Application Insights, you can see related spans in a single trace.

Let's explore tracing in .NET a bit more with an example. You can generate a trace
in your code using an ``ActivitySource` using the following code:

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

This code generates a trace with the name `DoSomething` when you call the `DoSomething`
method. The trace is generated by the activity source `My.ActivitySource` with version
`1.0.0`.

Activities should always be created with a `using` statement to ensure that it's
collected by the garbage collector as soon as we're done. This is important because when
you clean up an activity, it's recorded by the tracer, and it's somewhat time-sensitive
to accurately measure the time it took to complete the activity.

The span produced by the activity is written to a tracer, which is called an
`ActivitySource` in .NET. The activity source is responsible for writing the span to an
exporter. We haven't configured one in this code, because it's up to the consumer of our
code to choose how and where to collect telemetry. The following code builds a basic
trace exporter that writes traces to the console:

```csharp
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService("Chapter5.Telemetry");

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("My.ActivitySource")
    .AddConsoleExporter()
    .Build();
```

In this code we perform the following steps:

1. First, we create a resource builder that adds metadata to the traces like the
   application name and application version.
2. Next, we create a tracer provider builder that we can configure with a source, and an
   exporter. We add the activity source we want to process and export the traces to the
   terminal.

You can configure multiple sources to the tracer provider. Only traces for sources that
you've added to the tracer provider will be exported. Right now, we only export our
custom source. Sources can be prefixes, so I could say: `My*` to export anything that
starts with `My`.

The exporter is responsible for writing traces to a monitoring system. In this case,
we're writing traces to the terminal, but in production you want something more durable.
For example, you can use an Application Insights exporter to write the traces to
Application Insights. But you can also use other exporters. There are a lot of packages
available on Nuget.

To export traces from Semantic Kernel, we'll need to modify the code a bit. The
following code shows how to add telemetry to the Semantic Kernel:

```csharp
using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Microsoft.SemanticKernel*")
    .AddConsoleExporter()
    .Build();
```

This code adds the `Microsoft.SemanticKernel*` prefix as a source to the tracer
provider. And that's all you need to do to enable tracing for Semantic Kernel.

But what does Semantic Kernel export to the tracer provider? Spans created by Semantic
Kernel contains data as defined in [the semantic conventions for generative AI
systems][GENAI_STANDARD]. This experimental standard provides guidance on what
information is needed to properly monitor LLM-based applications. It provides the
following attributes among others for spans generated when calling an LLM:

- `gen_ai.system`: The LLM provider you called in the code.
- `gen_ai.request.model`: The model you called in the code.
- `gen_ai.response.completion_tokens`: The number of tokens in the response.
- `gen_ai.response.prompt_tokens`: The number of tokens in the prompt.
- `gen_ai.prompt`: The prompt you submitted.
- `gen_ai.completion`: The response generated by the LLM.

In addition to these properties you'll find the name of the kernel function that was
invoked, the time it took to complete the operation and of course the timestamp it was
started.

By default, no sensitive information is collected by Semantic Kernel. To collect the
prompt and response to your prompt you need to add the following code to the startup of
your application:

```csharp
AppContext.SetSwitch(
    "Microsoft.SemanticKernel.Experimental." +
    "GenAI.EnableOTelDiagnosticsSensitive", true);
```

Tracing will be your most important tool to track what data is processed with the LLM in
production so you can later use it for debugging purposes. Next to tracing you can also
use metrics to track the performance of your LLM-based application.

### Enabling metrics in your LLM-based application

Building meters into .NET code is different from adding traces. You need to define a
`Meter` in your code that's responsible for collecting metrics. After you've created a
`Meter` instance you can use it to create histograms, counters, and other metrics to
measure signals in your application. The following code demonstrates how to create a
`Meter` and use it to create a counter:

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

1. First, we create a meter to store all metrics related to a specific category of
   information. The meter is created once and kept in a static variable.
2. Next, we create a counter to measure the number of calls to the `DoSomething` method.
3. Finally, we increment the counter by one each time the `DoSomething` method is
   called.

To export metrics through OpenTelemetry, we need to write the following code in the
startup of the application:

```csharp
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("My.MeterCategory*")
    .AddConsoleExporter()
    .Build();
```

This code performs the following steps:

1. First, we create a `MeterProviderBuilder` using the `Sdk` component offered by
   OpenTelemetry.
2. Next, we provide a resource builder to add metadata like the application name to the
   metrics.
3. Then, we add the meter category we want to export.
4. Finally, we add a console exporter to write the metrics to the terminal.

Just like with traces, you can use different exporters to write the metrics to other
destinations. Only the metrics you configure with `AddSource` are exported to the
configured exporters.

Now that we've covered adding metrics to an application, let's look at how we can export
metrics from Semantic Kernel.

To export metrics for Semantic Kernel, you need to modify the code a bit, just like we
did with the traces:

```csharp
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("Microsoft.SemanticKernel*")
    .AddConsoleExporter()
    .Build();
```

This code adds the `Microsoft.SemanticKernel*` prefix as a meter to the meter provider.
And that's all you need to do to enable metrics for Semantic Kernel.

The metrics generated by Semantic Kernel are defined in [the same
standard][GENAI_STANDARD] that defines the tracing properties. I highly recommend taking
a look to decide what metrics make sense for you.

We haven't covered logging yet, let's look at that before we dive into exporting traces
to a monitoring tool like Application Insights.

### Logging in Semantic Kernel

Aynone who's worked with .NET 6 and later knows about the `Logger<T>` class and logging
in general. You can configure logging as a separate tool in Semantic Kernel. When you're
building a console application you'll need to add the following code to the startup:

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

1. First,  we create a new logger factory using the `LoggerFactory` class.
2. Next, we configure opentelemetry as a logging provider. We configure the same
   OpenTelemetry settings as we did for traces and metrics. We use the console exporter
   to write the logs to the terminal.
3. Finally, we set the minimum log level to `Information`.

You can write log messages straight to the terminal without using OpenTelemetry, but
this will break monitoring when you want to use a tool like Application Insights. Using
OpenTelemetry with the logging factory connects the log messages as events to the traces
generated by the application. This way you can view traces and log messages as a single
unit in the monitoring tool.

The OpenTelemetry console exporter is nice to check that everything is configured as
intended, but I don't recommend using it in production. It will slow down you
application, and it's not durable. Let's look at how to configure Application Insights
as an exporter and how to configure a dashboard for your LLM-based application in this
tool.

### Writing monitoring data to application insights

Configuring Application Insights as an exporter for traces, metrics, and logs requires
an additional package. You can add the `Azure.Monitor.OpenTelemetry.Exporter` package to
your project. The following code demonstrates how to configure the exporter:

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

1. First, we configure the resource builder for the service metadata to include in the
   metrics and traces.
2. Then, we configure the trace provider as before, but this time we're adding the Azure
   Monitor exporter. We use the connection string from the configuration to connect to
   Application Insights.
3. Next, we configure the meter provider as before, but this time we're adding the Azure
   Monitor exporter using the same connection string we used for the tracer provider.

With this code in place, you can start monitoring your LLM-based application in
Application Insights. You need to set up an Application Insights resource in Azure to
obtain the connection string for the exporters. There's a great manual for this
available in [the Application Insights documentation][AI_DOCS].

One way to monitor your application is by setting up a dedicated dashboard in
Application Insights. Let's dive into setting up an LLMOps dashboard in this tool.

### Building a LLMOps dashboard with Application Insights

Application Insights doesn't focus on monitoring LLM-based applications. It was there
before we even had LLMs. So it takes a bit of effort to make a useful dashboard for
applications that use an LLM. But it is doable.

There are a couple of things that you'll want to see in an LLMOps dashboard:

- What are the costs of running your application?
- How fast is the application and is that within an acceptable range?
- Are any of your requests failing?

Let's start by creating a dashboard in your Azure environment. Navigate to the homepage
of [the Azure portal](https://portal.azure.com). Then click on the hamburger menu as
shown in [#s](#azure-portal-hamburger-menu).

{#azure-portal-hamburger-menu}
![The hamburger menu in the Azure Portal](azure-portal-hamburger-menu.png)

From the hamburger menu, select the *Dashboard* option and click on the *Create toolbar
button*. This shows the screen from [#s](#create-dashboard-screen).

{#create-dashboard-screen}
![The create dashboard screen](create-azure-dashboard-screen.png)

The screen shows a number of predefined patterns for building a dashboard. I personally
prefer to start with an empty dashboard. However, if you're new to building a dashboard
it can be useful to choose the Application Insights template to start with. It contains
a set of useful charts that show the basic health signals for your application.

After selecting a starting point, you're presented with the dashboard editor. The
dashboard editor will look similar to the one shown in [#s](#dashboard-editor-screen) if
you've choosen the custom dashboard template.

{#dashboard-editor-screen} 
![The dashboard editor screen](edit-azure-dashboard-screen.png)

The dashboard editor shows a grid to display metrics and other information as tiles. The
tile gallery shows various types of tiles you can add to the dashboard.

For the LLMOps dashboard we'll start by adding two tiles, one for showing the number of
input tokens, and one to display the output tokens as a timeseries chart. Select the
*Metrics Chart* tile type from the list to the right of the screen and click the *Add*
button twice. When you've done that, edit the name of the dashboard and save it. You
should end up with a screen looking like the one in [#s](#edited-dashboard-screen).

{#edited-dashboard-screen}
![The dashboard with two charts added](edited-azure-dashboard.png)

You can click on either one of the charts to configure the metric that you want to
display. After clicking one of the charts, you're presented with the metric
configurator. This tool allows you to select a scope, and a metric from that scope to
display. You'll need to select the following options for the scope, metric namespace,
metric, and aggregation:

- Scope: The name of your application insights resource
- Metric namespace: Log based metrics
- Metric: semantic_kernel.invocation.function.token_usage.prompt
- Aggregation: Avg

After setting the configuration, save the metric by clicking on the *Save to dashboard*
button in the top right of the screen. [#s](#configured-metric-screen) shows the fully
configured metric for average input tokens.

{#configured-metric-screen}
![Fully configured metric for input tokens](configured-input-token-metric.png)

You can repeat the same steps for the output tokens metric. The metric you need to
select is `semantic_kernel.invocation.function.token_usage.completion`.

After configuring the metrics, you can add another tile to the dashboard to show the
duration of each prompt in your application. The metric you need for this is
`semantic_kernel.invocation.function.duration`.

I've found it useful to display separate charts for each prompt template in the
application. This way it's easier to spot problems with specific prompts. You can do
this by adding a filter to the metric configuration using the *Add filter* button. You
can filter on the `semantic_kernel.function.name` property. The name will match the name
of the prompt template. If you've used a YAML file to store your prompt template, you
can filter on the value of the name property in the YAML file.

Note that as your application grows it becomes harder to manage the dashboard. You
probably will have a few prompt templates that you're using. It's a good idea to put
only those metrics in the dashboard that require attention because you've recently
changed them or added them to the application. Dashboards are a living piece of your
monitoring setup and should be updated regularly. If you don't, you're likely not going
to look at it or do anything with the information on it.

Let's add the final piece of the puzzle. The tokens and duration metrics are useful to
find things that are out of the ordinary. But it's the costs that really matter for most
people. Here's how to add them to the dashboard.

Navigate to the OpenAI Resource in the Azure Portal, and select the *Resource
Management* > *Cost analysis* option from the left sidebar. This will show you a chart
displaying the costs for the resource. To add the cost analysis to your dashboard, click
the pushpin next to the title *Cost analysis* and choose the dashboard you want to add
it to.

Now that we have monitoring in place, let's go back to collecting feedback information.
If you've enabled the collection of prompts and responses in your application, you can
export that data and use it to improve your tests.

### Collecting data to improve tests

Telemetry data in Application Insights is stored in a structured format in a Log
Analytics Workspace. You can query the data using the [Kusto Query Language
(KQL)][KUSTO_INTRODUCTION]. But you can also export it to a storage account for later
use. We're going to use the data export feature to retrieve the prompts and responses
for specific prompt templates.

After we've downloaded the data, we'll build an application to extract the prompts and
responses from the raw log data and use it to improve our tests.

Before you start exporting data, make sure you have a storage account with hierarchical
namespaces enabled. You can create one using [this guide][CREATE_STORAGE_ACCOUNT].

Let's start by creating an export rule to move data from the Log Analytics Workspace to
the storage account. You can get access to the Log Analytics Workspace for your
Application Insights resource by going to the Application Insights in the Azure portal
and clicking on the link next to the Workspace property in the overview page of the
resource. This will take you to the Log Analytics Workspace.

In the Log Analytics Workspace, you can find the data export feature in the sidebar
of the workspace resource under *Settings* > *Data export*. [#s](#export-rules-overview)
shows the data export overview page.

{#export-rules-overview}
![Export rules overview page](azure-law-export-rules-overview.png)

Create a new rule by clicking on the *New export rule* button. This will take you to the
rule creation page. On this page you can set a name for the new rule, select the tables
you want to export, and the destination for the data. [#s](#export-rule-step-1) shows
what this page looks like.

{#export-rule-step-1}
![Export rule creation page](azure-law-export-rules-step-1.png)

Provide a descriptive name for the rule, and click *Next* to go to the next step as
shown in [#s](#export-rule-step-2). On the next screen you can select the tables you
want to export. The `AppTraces` table contains the data for the prompts and responses.
Select this table and click *Next* to go to the destination configuration screen.

{#export-rule-step-2}
![Table selection screen](azure-law-export-rules-step-2.png)

You have two choices for the destination of the data. We'll use a storage account to
store the exported data. [#s](#export-rule-step-3) shows the screen to configure the
destination.

{#export-rule-step-3}
![Destination configuration screen](azure-law-export-rules-step-3.png)

Select the storage account you want to store the exported data in and complete the
configuration.

After you've completed the configuration, any new data coming into the Log Analytics
Workspace is automatically exported to your storage account. A new container is created
for each table you export. In this case we'll have a container called `am-apptraces`.

You can download the data from the storage account using the [Azure Storage
Explorer][STORAGE_EXPLORER] or use the [Azure Blob Storage Package][NUGET_BLOB_STORAGE]
to download the data programmatically.

Let's look at how you can use the Blob Storage package to download the data from the
storage account. The following code demonstrates how to set up a connection to the
storage account and download all the data:

```csharp

```

The code performs the following steps:



## Summary

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
