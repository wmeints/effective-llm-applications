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

Let's get started by using deterministic testing methods to validate that we're getting
useful responses from the LLM.

## Establishing a good test strategy for prompts

Prompt testing is a difficult subject, because you'll find that the LLM will give you a
different response each time you call it. You can't test for a specific response. And
that makes for brittle tests. I know from experience that not many of you will be very
happy with this situation in their code base. But there are ways to make things better.

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

While there are frameworks available for property-based testing, I'm not using them in
this book or in my own projects. I've found that it is enough to build data-driven tests
with a test framework like xunit or MSTest.

Let's start by looking at how to apply a property-based testing approach to testing a
prompt with a deterministic approach.

## Using deterministic testing methods to validate prompts

To demonstrate how to build prompt testing logic, let's go back to a prompt that we used
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
  settings from the user-secrets store.
- Finally, we define a test method that takes a dish name and a list of ingredients as
  input. We marked the test method as `[Theory]` to enable parameterized testing. The
  test method will be called for each set of input data.

With the skeleton of the test in place, we can write test logic to validate the prompt.
The following code shows how to write the test logic:

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
   we want to run the test for. You can use inline data or you can load data from a test
   file in your code base. You can learn more about the various methods to load test
   samples for your data driven test in [this blogpost][XUNIT_DATA_DRIVEN_TESTS].
2. Next, we call the prompt function with the dish name and the list of ingredients as
   input. The result is stored in the `result` variable.
3. Finally, we use the `Assert.Contains` method to check if the result contains the
   words "ingredients" and "instructions". If the result contains these words, the test
   passes.

You can extend this test with more samples as you see fit. The test will run for each
of the provided samples and report a separate test results for each of the samples.

I must warn you that running more samples will make the test slower, and it isn't fast
to begin with. I highly recommend you mark the test with a separate category and only
run the LLM-based tests periodically, for example when you're about to finish up
a new user story in your application or when you change the prompt, the model, or
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

Our sample covers just one method to validate that a prompt generates a useful response.
This one test isn't going to be enough though, because the LLM could generate a recipe
that's inconsistent. It could also generate a very long response that's hard to read.

To validate more complex properties of the response we're going to use the LLM against
itself. Let's look at how you can use model-based testing to validate more complex
properties of your prompts.

## Using model-based testing methods to validate prompts

- Introduce model-based testing and how it differs from the deterministic methods.
- Explain that we'll only look at G-Eval, but there are other model-based approaches available.
- This field is changing quickly just like LLMs, so it's important to keep an eye out for new developments.

### Understanding the general approach to model-based testing

- Explain the benefits and limitations of model-based testing approaches.
- Explain what you can and can't assert in a model-based test.
- Explain what sort of data you need to build model-based tests.

### Using G-Eval to evaluate prompts and their responses

- Explain how G-Eval helps you evaluate prompts and their responses.
- Show an example unit-test setup with xunit and G-Eval based metric evaluation.
- Repeat the limitations of G-Eval to make sure people know what they're getting into.

## Monitoring prompt interactions in production

From the previous section we learned how to test prompts in a controlled environment.
Testing only gets you so far sadly. The samples we collected in the previous section are
limited to what you can come up with and might not be representative of what users are
going to do. The only way we can get test data that's representative of the real world
is to collect it from production.

Let's look at using monitoring to keep an eye on how the application is being used in
production and using collected telemetry to improve the application.

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