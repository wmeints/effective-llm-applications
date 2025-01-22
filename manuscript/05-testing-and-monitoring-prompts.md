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

After reading the papers for G-Eval and GPTScore I can tell you that the LLM evaluation
is strange in nature, but does show remarkable agreement with human experts. And that is
all you can ask for at the moment. So if you're wondering, why are we
doing this? It's because it's the best we have at the moment.

## Monitoring prompt interactions in production

From the previous section we learned how to test prompts in a controlled environment.
Testing only gets you so far sadly. The samples we collected in the previous section are
limited to what you can come up with and might not be representative of what users are
going to do. The only way we can get test data that's representative of the real world
is to collect it from production.

And keeping my warning in mind, it's important to gather feedback from users on the
quality of your prompts. LLMs should not have a final say about quality.

Let's look at using monitoring to keep an eye on how the application is being used in
production and using collected telemetry to improve the application.

### Enabling telemetry in your LLM-based application

The first step to get monitoring data is to enable telemetry in your application.
As luck would have it, .NET features a great set of diagnostics tools that are also included
in Semantic Kernel. Let's set up a set of diagnostic tools. We need to collect 3 things:

- Meter data for token usage
- Traces to see how the application and LLM interact
- Logging for any event data related to the traces

The following code demonstrates how to extend a console application with diagnostics
for Semantic Kernel:

```csharp

```

### Writing monitoring data to application insights

- Enabling opentelemetry with semantic kernel
- What data is written to the opentelemetry sink
- Configuring a dashboard for your LLM-based application

### Extracting test data from application insights

- Use application insights to collect telemetry data on prompt interactions.
- Precautions you need to take when extracting test data from production and integrating it into your test set.
- How to handle privacy issues when extracting test data from production.

## Summary

## Further reading

- [GPTScore: Evaluate as you desire](https://arxiv.org/abs/2302.04166)
- [G-Eval: NLG Evaluation using GPT-4 with Better Human Alignment](https://arxiv.org/abs/2303.16634)

[XUNIT_DATA_DRIVEN_TESTS]: https://hamidmosalla.com/2017/02/25/xunit-theory-working-with-inlinedata-memberdata-classdata/
[G_EVAL]: https://arxiv.org/abs/2303.16634
[GPTSCORE]: https://arxiv.org/pdf/2302.04166
[PROMPTFOO]: https://promptfoo.dev
