{#intelligent-request-routing}
# Intelligent Request Routing Workflows

In the previous chapter we covered prompt chaining and how this helps get more accurate responses from an LLM. In this chapter we're exploring how to use intelligent request routing to build more complex LLM interactions.

We're going to use the LLM as an intent-based router for chat applications and workflows. By the end of this chapter you know wow to write a prompt that can classify input and integrate that prompt in a chat application and a workflow.

We'll cover the following topics:

- Why use intelligent routing in a workflow
- Building an intelligent request routing workflow in Semantic Kernel
- Using intelligent request routing in a chat scenario in Semantic Kernel

Let's start discussing why you would use an intelligent router in your workflow.

## Why use intelligent routing in a workflow

While most LLM-based workflows will focus on using prompts to process content you can use LLMs in other ways too. You can use an LLM to route requests based on their content.

Imagine you're building a chatbot solution for the website for a health insurance company. You need people to be able to ask questions about policies offered by the insurance company, but you also want people to submit insurance claims through the website. Both use cases require a different approach and a different set of tools for the chatbot. You don't want the chatbot to be able to submit an insurance claim when someone asks about the details of a policy. If you were to build a single chatbot experience offering both insurance policy information and a tool to submit claims, the chatbot is easily confused. To solve this, you can use an LLM to decide based on the first question which department the user wants to talk to and send the question through the right combination of tools and prompts.

Another use case where intelligent routing can be useful is when you need to sort incoming feedback from users into categories with specific issue templates on Github. You can give the LLM the original issue text along with a set of examples to teach the LLM how to sort the issues. Although different from routing user questions to the right bot, issue sorting is still a routing use case.

Finally, intelligent routing can be used to optimize costs of your LLM. You can use a small language model as a router to determine whether the question is a simple or complex question and route the question to either a smaller language model or a larger language model depending on the complexity. You do incur an extra call to a language model, but it could help you shave off quite a sizable amount off your next creditcard bill, improve the response quality and speed at which users get a response.

## Introducing to the process framework

In the previous chapter we built our workflow by hand. But as our workflows become more complex when we add intelligent routing for example it's important to build them on top of a more robust foundation. A workflow engine provides better options for robustness and scaling workflows.

There are a lot of options available in the marketplace right now. I've personally used [Camunda][CAMUNDA], [Dapr Workflow][DAPR], and [Prefect][PREFECT] a lot in my work. Each of these products offer the same set of features:

- A resilient workflow engine that can handle transient errors.
- A solution to handle long-running workflows with versioning.
- A management dashboard to track running workflow instances.

While the last item on this list isn't always useful when you have short-running workflows, it's an important feature for long-running workflows with human interaction.

While all of these products are great, they have one problem in common: You need to add yet another layer of moving parts on top of Semantic Kernel. That's why the developers of the Semantic Kernel team came up with the idea of providing a workflow engine out of the box tailored for LLM-based workflows.

A> **The Semantic Kernel process framework is still in preview!**  
A> While the base API of Semantic Kernel has been stable for a while, the API interface for the process framework shows some churn. I don't recommend running the process framework in a production scenario unless you plan for extra maintenance due to changes in the API. Using the workflow engine offered in Dapr is a better option if you need to plan for a more stable environment for now.

The process framework in Semantic Kernel has two core components:

- Steps allow you to define logic for a single step in a process.
- Processes are used to combine steps into a workflow. A process controls the data flow between steps and what steps are connected.

Semantic Kernel process framework relies on a runtime to host the processes. The process you define with the steps and process components need to be hosted on one of the available runtimes to be functional. Currently, Semantic Kernel has three options for running workflows:

- **The local runtime** provides no resiliency or scaling options and is meant for debugging workflows locally.
- **The [Dapr][DAPR] runtime** is better suited for production scenarios. Dapr offers scaling and resiliency options not found in the local runtime. The Dapr process framework runtime is contained in the `Microsoft.SemanticKernel.Runtime.Dapr` package.

The process framework features a couple more useful options that we'll cover after taking a look at setting it up.

### Installing the process framework

You can use the Semantic Kernel process framework from console applications and ASP.NET Core applications. To demonstrate how the process framework works, we'll use ASP.NET Core as the basis.

To install the process framework you need to add a package called `Microsoft.SemanticKernel.Process.Core` and one of the runtime packages to your ASP.NET Core application. For now, we'll go with the local runtime. You can install the local runtime by adding a reference to the `Microsoft.SemanticKernel.Process.LocalRuntime` package.

If you're adding the package via .NET CLI, I recommend using the following commands:

```bash
dotnet add package Microsoft.SemanticKernel.Process.Core --allow-prerelease
dotnet add package Microsoft.SemanticKernel.Process.LocalRuntime --allow-prerelease
```

The extra `allow-prerelease` flag allows the .NET CLI to install the alpha version of the packages. Without this flag you will probably run into issues where the CLI straight-up refuses to work.

Once you have the packages configured in the application, you only need to configure the `Kernel` object and AI connectors as we've done before in [#s](#setting-up-semantic-kernel).

### Writing the process steps

Creating a basic process in the Semantic Kernel process framework starts by building steps. A step is implemented as a class derived from `KernelProcessStep`. The following code demonstrates a basic kernel process step implementation:

```csharp
public class GetNameStep: KernelProcessStep
{
    [KernelFunction]
    public string GetName()
    {
        return "Willem";
    }    
}
```

In the process step implementation you'll need to add a method marked with `[KernelFunction]` to add logic to the step. Kernel process step methods have a few rules to them:

- You can add a `Kernel` argument to the method to gain access to a kernel instance. You can use this to invoke prompts or get access to plugins.
- You can add a `KernelProcessStepContext` argument to the method to emit events from your step logic. We'll cover emitting events when we get to [#s](#making-decisions-with-sk-process) and cover how to make a decision in a process.
- You can have one argument containing the input data for your workflow. This argument
  can be of any type as long as it is serializable to JSON. This is because of underlying runtime requirements. Dapr for example needs to serialize input data to JSON when calling a step in your process implementation.
- The method can return a JSON serializable value directly or return a `Task<T>` where the `T` type argument is a JSON serializable type. This makes it possible to make async calls from your step logic.

I recommend placing steps into a `Steps` namespace in your application and process implementations in a `Processes` namespace. This makes it easier to distinguish between process related components and other components in your application. When your processes become more complex, you'll find yourself creating more helper classes and services to maintain testability.

The code in the previous sample only returns a name to generate a greeting for. I've created a second step in the [sample code][BASIC_SAMPLE_CODE] to generate a greeting and print the greeting to the terminal. The code for this second step looks like this:

```csharp
public class GenerateGreetingStep: KernelProcessStep
{
    [KernelFunction]
    public void GenerateGreeting(Kernel kernel, string name)
    {
        kernel.Data["GreetingMessage"] = $"Hello, {name}";
    }
}
```

In this second step we use the kernel to store the outcome of the step.
Currently, there's no way to send data from inside the process as a return value of the process. To work around this problem you need to use the `Data` dictionary of the kernel. It's not ideal, and I hope they'll fix this in an upcoming release of the process framework.

Now that we have two steps, let's wire them up into a basic process.

### Wiring up the process

Wiring up steps into a process can be done by using the `ProcessBuilder` class. The following code demonstrates how to wire up our two-step process:

```csharp
public class GreetingProcess
{
    private KernelProcess _process;
    
    public GreetingProcess()
    {
        var processBuilder = new ProcessBuilder("GreetingProcess");

        var getNameStep = processBuilder
            .AddStepFromType<GetNameStep>();

        var generateGreetingStep = processBuilder
            .AddStepFromType<GenerateGreetingStep>();

        processBuilder
            .OnInputEvent("StartProcess")
            .SendEventTo(new(getNameStep));

        getNameStep
            .OnFunctionResult()
            .SendEventTo(new(generateGreetingStep));

        _process = processBuilder.Build();
    }

    public async Task<string> StartAsync(Kernel kernel)
    {
        await _process.StartAsync(kernel, new KernelProcessEvent 
        { 
            Id = "StartProcess" 
        });

        var result = kernel.Data["GreetingMessage"] as string;

        return result;
    }
}
```

This code performs the following steps:

1. First, we create a new class `GreetingProcess` as a container for the process logic.
2. Next, in the constructor we create a new instance of the `ProcessBuilder` class.
3. Then, we register the step definitions for the process.
4. After that, we define an input event for the process to start the process.
5. Next, we emit an event to the second step when the first step finishes.
6. Finally, we define a `StartAsync` method that runs the process and returns the `GreetingMessage` we stored in the second step of the process.

The Semantic Kernel process framework uses events whenever it needs to send or receive data. The input event we defined is fired when we call `StartAsync` and causes the `GetName` method in `GetNameStep` to be called.

The `OnFunctionResult` definition in the process generates an event that we capture by executing the `GenerateGreeting` method in the `GenerateGreetingStep` class.

Using events to transport data is important, because this allows the runtime to move the process steps to different machines and talk to them via HTTP or even gRPC. The event data is serialized to JSON, sent over to where the step is executed and then deserialized and used.

You can use the process in your web application using the following code:

```csharp
app.MapGet("/greeting", async (Kernel kernel) =>
{
    var process = new GreetingProcess();
    return await process.StartAsync(kernel);
});
```

The basic process we just built uses a single method in each step, but that's not a requirement. You can define multiple kernel functions per step. This makes it possible to have multiple variations of the same process step in one class. Definine multiple kernel functions in one step class can be useful if you need to have a variant of the step for the first time it's executed and another variant for the executions after that.

If you decide to define multiple kernel functions in one step class, you need to be more specific when routing data in the process definition. Instead of calling `OnFunctionResult()` you need to call `OnFunctionResult(functionName: "GetName")` so the process knows which variant of the step you're using. The function name should match the name of the method without a possible `Async` postfix if you're building an async method.

The same technique we just discussed should also be applied to `SendEventTo`. You need to add a `functionName` argument to route the data to the correct variant of your process step.

### Visualizing the process using mermaid diagrams

As your process becomes more complex, you'll find that it becomes harder to see what the exact flow is in the process. Luckily for us, Microsoft thought of this and added a `ToMermaid` method to the final process instance we created in the constructor of our process. Mermaid is a text-based diagramming syntax with [an online tool][MERMAID_TOOL] that allows you to turn the mermaid format into a PNG image.

I made a habit of wiring the `ToMermaid` method in my process definitions so I can call it to output the graph definition of my process in the mermaid format. It's a life saver when you get to building workflows with more complex decision making steps or loops. The following code demonstrates how to do this:

```csharp
public class GreetingProcess
{
    private KernelProcess _process;
    
    public GreetingProcess()
    {
        // ... Process builder logic
    }

    public async Task StartAsync(Kernel kernel)
    {
        // ... The process start logic
    }

    public string ToMermaid()
    {
        return _process.ToMermaid();
    }
}
```

You can call the `ToMermaid` method from a unit-test and store the output in a text file so you can more easily copy it over to the online mermaid tool. I sometimes add some logic to the `Program.cs` file of my application to export the content to the terminal there. Just make sure you remove it before committing to git!

When you generate a mermaid file for the process we just built, you will get the the visualization in the online Mermaid tool as shown in [#s](#process-visualization).

{#process-visualization}
![Mermaid diagram for the basic process](process-mermaid-visualization.png)

Now that we've seen the basics of building a process in Semantic Kernel we need to discuss how to route to different steps in the workflow based on a condition in a step.

{#making-decisions-with-sk-process}
## Making decisions in a Semantic Kernel process

In many workflow engines you have a dedicated decision step that uses input and an expression to invoke one workflow step or another. In Semantic Kernel we need to build a decision making step ourselves.

### Using events to route data through the workflow

The following code demonstrates how to build a step that emits two different events based on the input provided to the step:

```csharp
public class MakeDecisionStep: KernelProcessStep
{
    [KernelFunction]
    public async Task MakeDecisionAsync(
        KernelProcessStepContext context, int randomValue)
    {
        if (randomValue > 10)
        {
            await context.EmitEventAsync("HighOutcome", randomValue);
        }
        else
        {
            await context.EmitEventAsync("LowOutcome", randomValue);
        }
    }
}
```

This process step contains the following logic:

1. First, we check if the input argument is higher than 10.
2. If the input is higher than 10, we emit the "HighOutcome" event.
3. If the input is lower or equal to 10, we emit the "LowOutcome" event.

We can use these events to execute different steps in the workflow. Let's take a look
at the process logic:

```csharp
public class RandomDecisionMakingProcess
{
    private readonly KernelProcess _process;

    public RandomDecisionMakingProcess()
    {
        var processBuilder = new ProcessBuilder("RandomDecisionMaking");
        
        var generateRandomNumberStep = processBuilder.
            AddStepFromType<GenerateRandomNumberStep>();

        var makeDecisionStep = processBuilder
            .AddStepFromType<MakeDecisionStep>();

        var handleLowOutcomeStep = processBuilder
            .AddStepFromType<HandleLowOutcomeStep>();

        var handleHighOutcomeStep = processBuilder.
            AddStepFromType<HandleHighOutcomeStep>();
        
        processBuilder
            .OnInputEvent("StartProcess")
            .SendEventTo(new(generateRandomNumberStep));

        generateRandomNumberStep
            .OnFunctionResult()
            .SendEventTo(new(makeDecisionStep));
        
        makeDecisionStep
            .OnEvent("HighOutcome")
            .SendEventTo(new(handleHighOutcomeStep));

        makeDecisionStep
            .OnEvent("LowOutcome")
            .SendEventTo(new(handleLowOutcomeStep));
        
        _process = processBuilder.Build();
    }

    // ... Rest is omitted for now
}
```

In the process logic we use the `OnFunctionResult` method to route the output
of the `GenerateRandomNumberStep` to the `MakeDecisionStep`. From there we no longer use the `OnFunctionResult` method to route the data. Instead, we capture the `HighOutcome` event and route the data of the event to the `ProcessHighOutcomeStep`. We also capture the `LowOutcome` event to route it to the `ProcessLowOutcomeStep`.

If you're interested in the rest of the code for this process, make sure to check out [the sample code][DECISION_PROCESS_SAMPLE] for this chapter.

## Building an intelligent request routing workflow

So far in this chapter we've only covered how to build processes in Semantic Kernel. While useful, it doesn't address the topic of intelligent request routing. It's time we address that by looking at how we can use an LLM to route requests based on their complexity.

We need to perform a couple of steps to build an intelligent model router using the process framework:

1. First, we need to configure multiple LLMs in our application. A bigger model for answering complex questions and a smaller model for the more straightforward questions.
2. Next, we need to create two steps to address the complex and more straightforward questions using the correct models.
3. Then, we need to make a decision making step that routes requests based on their complexity.
4. Finally, we need to wire up the process.

Let's start by configuring multiple LLMs with Semantic Kernel.

### Configuring multiple AI connectors

So far we've only focused on using a single LLM with our application but Semantic Kernel supports registering multiple LLMs by setting a service identifier for each of the registered models.

The following code demonstrates how to configure the kernel with multiple models in ASP.NET Core:

```csharp
builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        builder.Configuration["LanguageModel:ComplexCompletionModel"]!,
        builder.Configuration["LanguageModel:Endpoint"]!,
        builder.Configuration["LanguageModel:ApiKey"]!,
        serviceId: "complexPrompts")
    .AddAzureOpenAIChatCompletion(
        builder.Configuration["LanguageModel:BasicCompletionModel"]!,
        builder.Configuration["LanguageModel:Endpoint"]!,
        builder.Configuration["LanguageModel:ApiKey"]!,
        serviceId: "basicPrompts");
```

We specify one chat completion model with a `complexPrompts` service identifier and the other with a `basicPrompts` service identifier. The complex requests we'll route to GPT-4o while the more basic questions we'll route to GPT-4o-mini.

After configuring the models, we need to make sure we can use both models.

### Creating steps for straightforward and complex prompts

Let's first create a step that can handle straightforward questions by using the chat completion service marked with the `basicRequests` service identifier.

```csharp
public class HandleBasicPromptStep: KernelProcessStep
{
    [KernelFunction]
    public async Task HandlePromptAsync(Kernel kernel, string prompt)
    {
        var completionService = kernel.GetRequiredService<IChatCompletionService>();
        var chatHistory = new ChatHistory();
        
        chatHistory.AddSystemMessage(EmbeddedResource.Read("instructions.txt"));
        chatHistory.AddUserMessage(prompt);

        var response = await completionService.GetChatMessageContentsAsync(
            chatHistory,new AzureOpenAIPromptExecutionSettings()
        {
            ServiceId = "basicPrompts",
        });
        
        // NOTE: The response is a list of choices that you could request in the past.
        // Now, there's only one choice no matter what model you're using!
        
        kernel.Data["ResponseContent"] = response[0].Content;
    }
}
```

This step contains the following logic:

1. First, we implement a new step class for basic prompts.
2. Next, we create a method `HandlePromptAsync` that accepts a prompt
3. Then, in the `HandlePromptAsync` method we request the `IChatCompletionService`.
4. After, we create a chat history with system instructions and the user's prompt.
5. Next, we invoke the completion service providing prompt execution settings that point the completion service to the correct service identifier.
6. Finally, we return the response by putting it in the kernel data dictionary.

The logic for handling complex prompts is similar to how we handle straightforward prompts. The difference is in the service identifier.

```csharp
public class HandleComplexPromptStep: KernelProcessStep
{
    [KernelFunction]
    public async Task HandlePromptAsync(Kernel kernel, string prompt)
    {
        var completionService = kernel.GetRequiredService<IChatCompletionService>();
        var chatHistory = new ChatHistory();
        
        chatHistory.AddSystemMessage(EmbeddedResource.Read("instructions.txt"));
        chatHistory.AddUserMessage(prompt);

        var response = await completionService.GetChatMessageContentsAsync(
            chatHistory,new AzureOpenAIPromptExecutionSettings()
        {
            ServiceId = "complexPrompts",
        });
        
        // NOTE: The response is a list of choices that you could request in the past.
        // Now, there's only one choice no matter what model you're using!

        kernel.Data["ResponseContent"] = response[0].Content;
    }
}
```

I've copied the logic here for simplicity and because this allows me to customize the behavior for complex prompts. You can use inheritance to create different implementations of the class, but it doesn't make much sense to me because conceptually I'm doing different things here.

Let's discuss how we're going to decide to route a prompt to the basic or the complex prompt handling steps.

### Routing prompts based on their complexity

Deciding where a user's prompt should go is a type of classification task that's easily handled by almost any LLM with sufficient capacity. After some experimentation I came up with the following prompt to route requests based on their complexity

~~~text
You are a routing agent responsible for deciding whether a user message should be handled by the powerful GPT-4o model or the lightweight GPT-4o-mini model.

Use the following logic:

1. If the user input:
   - Is long (more than 100 words), or
   - Includes technical content, code, math, or complex instructions, or
   - Requires reasoning, step-by-step planning, or detailed analysis,
   → route to **GPT-4o**.

2. If the user input:
   - Is short (less than 100 words), and
   - Is a straightforward query, casual chat, or small task like summarizing, translating, or answering trivia,
   → route to **GPT-4o-mini**.

Your output must be in this exact JSON format:

~~~json
{
  "model": "gpt-4o" | "gpt-4o-mini",
  "reason": "Short explanation of why this model was chosen"
}
```
~~~



## Summary

[DAPR]: https://dapr.io/
[CAMUNDA]: https://camunda.com/
[PREFECT]: https://www.prefect.io/
[BASIC_SAMPLE_CODE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-10/csharp/Chapter10.BasicProcess/
[MERMAID_TOOL]: https://mermaid.live/
[DECISION_PROCESS_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-10/csharp/Chapter10.DecisionMakingProcess/