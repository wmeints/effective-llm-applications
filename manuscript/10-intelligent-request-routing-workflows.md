{#intelligent-request-routing}
# Intelligent Request Routing Workflows

In the previous chapter, we covered prompt chaining and how it helps get more accurate responses from an LLM. In this chapter, we're exploring how to use intelligent request routing to build more complex LLM interactions.

We will use the LLM as an intent-based router within workflows. By the end of this chapter, you know how to write a prompt that can classify input and integrate that prompt into a workflow.

We'll cover the following topics:

- Why use intelligent routing in a workflow
- Building an intelligent request routing workflow in Semantic Kernel
- Using intelligent request routing in a chat scenario in Semantic Kernel

Let's discuss why you would use an intelligent router in your workflow.

## Why use intelligent routing in a workflow

While most LLM-based workflows focus on using prompts to process content, you can use LLMs in other ways, too. For example, you can use an LLM to route requests based on their content. I've come across three interesting use cases.

Imagine you're building a chatbot solution for the website of a health insurance company. You need people to be able to ask questions about policies offered by the insurance company, but you also want people to submit insurance claims through the website. Both use cases require a different set of instructions and a different set of tools for the chatbot. You don't want the chatbot to be able to submit an insurance claim when someone asks about the details of a policy. If you were to build a single chatbot experience offering both insurance policy information and a tool to submit claims, the chatbot would be easily confused. To solve this, you can use an LLM to decide, based on the first question, which department the user wants to talk to and send the question to the right "department" with the proper instructions and tools.

Another practical use case for intelligent routing is sorting incoming user feedback into categories with specific issue templates on Github. You can give the LLM the original issue text and examples to teach the LLM how to sort the issues. Although different from routing user questions to the right bot, issue sorting is still a routing use case.

Finally, you can use intelligent routing to optimize the costs and CO2 emissions of your LLM. You can use a small language model as a router to determine whether the question is complex or straightforward and route the question to either a smaller or a larger language model, depending on the complexity. You incur an extra call to a language model, but it could help you shave off a pretty sizable amount off your next credit card bill, improving the response quality and speed at which users get a response. The cost reduction isn't much when you have few users, but it does bring up impressive savings when you have a larger user base.

## Introducing to the process framework

In the previous chapter, we built our workflow by hand. However, as our workflows become more complex when we add intelligent routing, for example, building them on top of a more robust foundation is essential. A workflow engine provides better robustness and scaling options.

There are a lot of options available in the marketplace right now. I've personally used [Camunda][CAMUNDA] and [Dapr Workflow][DAPR]. Each of these products offers the same set of functional characteristics from a technical perspective:

- A resilient workflow engine that can handle transient errors.
- A solution to handle long-running workflows with versioning.
- A management dashboard to track running workflow instances.

While the last item on this list isn't always valuable when you have short-running workflows, it's an essential feature for long-running workflows with human interaction. It can also be helpful if you have to debug a failing workflow.

Beyond these characteristics, these products vary a lot. Camunda, for example, focuses more on building business processes with a standardized modeling language. Camunda is also more focused on enterprise-level workflows. Dapr doesn't have a nice modeling language. Instead, it focuses on developer productivity in distributed applications. It also doesn't have a dashboard. You need to build that yourself.

While both of these products are great, they have one problem in common: You need to add yet another layer of moving parts on top of Semantic Kernel. That's why the Semantic Kernel team's developers came up with the idea of providing a workflow engine out of the box tailored for LLM-based workflows.

A> **The Semantic Kernel process framework is still in preview!**  
A> While the base API of Semantic Kernel has been stable for a while, the API interface for the process framework shows some churn. I don't recommend running the process framework in a production scenario unless you plan for extra maintenance due to changes in the API. Using the workflow engine offered in Dapr is a better option if you need to prepare for a more stable environment.

The process framework in Semantic Kernel has two core components:

- Steps allow you to define logic for a single step in a process.
- Processes are used to combine steps into a workflow. A process controls the data flow between steps and what steps are connected.

Semantic Kernel process framework relies on a runtime to host the processes.  Currently, Semantic Kernel has three options for running workflows:

- **The local runtime** provides no resiliency or scaling options and is meant for debugging and testing workflows on your development box.
- **The [Dapr] [DAPR] runtime** is better suited for production scenarios. Dapr offers scaling and resiliency options that are not found in the local runtime. The `Microsoft.SemanticKernel.Runtime.Dapr` package contains the Dapr implementation of the workflow runtime.

The process framework features a couple more useful options, which we'll cover after we set it up.

### Installing the process framework

You can use the Semantic Kernel process framework from Console and ASP.NET Core applications. We'll use ASP.NET Core as the basis to demonstrate how the process framework works.

To install the process framework, you must add a package called `Microsoft.SemanticKernel.Process.Core` and one of the runtime packages to your ASP.NET Core application. For now, we'll go with the local runtime. You can install the local runtime by referencing the `Microsoft.SemanticKernel.Process.LocalRuntime` package.

If you're adding the package via .NET CLI, I recommend using the following commands:

```bash
dotnet add package Microsoft.SemanticKernel.Process.Core --allow-prerelease
dotnet add package Microsoft.SemanticKernel.Process.LocalRuntime --allow-prerelease
```

The extra `--allow-prerelease` flag is required because we're installing an alpha release.

Once the packages are configured in the application, you only need to configure the `Kernel` object and AI connectors as we've done before in [#s](#setting-up-semantic-kernel).

### Writing the process steps

Creating a basic Semantic Kernel process framework starts with building steps. A step is implemented as a class derived from `KernelProcessStep`. The following code demonstrates a basic kernel process step implementation:

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

In the process step implementation, you'll need to add a method marked with `[KernelFunction]` to add logic to the step. Kernel process step methods have a few rules to them:

- You can add a `Kernel` argument to the method to access a kernel instance. You can use this to invoke prompts or get access to plugins.
- You can add a `KernelProcessStepContext` argument to the method to emit events from your step logic to introduce a more complex control flow. We'll cover emitting events when we get to [#s](#making-decisions-with-sk-process) and how to make a decision in a process.
- You can have one argument containing the input data for your workflow. This argument can be of any type as long as it is serializable to JSON. As you can scale workflows across multiple machines, you must send and receive data across network boundaries.
- The method can return a JSON serializable value directly or a `Task<T>` where the `T` type argument is a JSON serializable type defining the result of the task.

I recommend placing steps in a `Steps` namespace in your application and process implementations in a `Processes` namespace. This makes it easier to distinguish between process-related components and other components in your application. As your processes become more complex, you'll find yourself creating more helper classes and services to maintain testability.

The code in the previous sample only returns a name to generate a greeting. I've created a second step in the [sample code][BASIC_SAMPLE_CODE] to generate a greeting and print the greeting to the terminal. The code for this second step looks like this:

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

In this second step, we use the kernel to store the outcome of the step.
Currently, the process framework can't send data from inside the process as a return value. To work around this problem, you need to use the kernel's `Data` dictionary. It's not ideal, and I hope they fix this in an upcoming release of the process framework.

Now that we have two steps let's wire them into a basic process.

{#process-wiring}
### Wiring up the process

Wiring up steps into a process can be done using the `ProcessBuilder` class. The following code demonstrates how to wire up our two-step process:

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

1. First, we create a new class, `GreetingProcess`, as a container for the processing logic.
2. Next, we create a new instance of the `ProcessBuilder` class in the constructor.
3. Then, we register the step definitions for the process.
4. After that, we define an input event for the process to start the process.
5. Next, we emit an event to the second step when the first step finishes.
6. Finally, we define a `StartAsync` method that runs the process and returns the `GreetingMessage` we stored in the second step of the process.

The Semantic Kernel process framework uses events to send or receive data. The input event we defined is fired when we call `StartAsync`, which causes the `GetName` method in `GetNameStep` to be called.

The `OnFunctionResult` definition generates an event we capture by executing the `GenerateGreeting` method in the `GenerateGreetingStep` class.

You can use the process in your web application using the following code:

```csharp
app.MapGet("/greeting", async (Kernel kernel) =>
{
    var process = new GreetingProcess();
    return await process.StartAsync(kernel);
});
```

The basic process we built uses a single method in each step, but that's not required. You can define multiple kernel functions per step. Defining multiple functions in a step class makes it possible to have multiple variations of the same process step in one class. Defining multiple kernel functions in one step class can be helpful if you need to have a variant of the step for the first time it's executed and another variant for the executions after that in case you're building a loop.

If you define multiple kernel functions in a step class, you must be more specific when routing data in the process definition. Instead of calling `OnFunctionResult()` you need to call `OnFunctionResult(functionName: "<FunctionName>")` The function name should match the name of the method without a possible `Async` postfix if you're building an async method.

If you need to send data to a specific function with `SendEventTo`, you'll need to use the same construction as with `OnFunctionResult.`

### Visualizing the process using mermaid diagrams

As your process becomes more complex, you'll find it harder to see the exact flow in the process. Luckily for us, the Semantic Kernel team thought of this and added a `ToMermaid` method to the final process instance we created in the constructor of our process. Mermaid is a text-based diagramming syntax with [an online tool][MERMAID_TOOL] that allows you to turn the mermaid format into a PNG image.

I habitually wire the `ToMermaid` method in my process definitions so I can call it to output the graph definition of my process in the mermaid format. Building workflows with more complex decision-making steps or loops is a lifesaver. The following code demonstrates how to do this:

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

You can call the `ToMermaid` method from a unit test and store the output in a text file to more easily copy it to the online mermaid tool. I sometimes add some logic to my application's `Program.cs` file to export the content to the terminal there. Just make sure you remove it before committing to git!

When you generate a mermaid file for the process we built, you will get the visualization in the online Mermaid tool, as shown in [#s](#process-visualization).

{#process-visualization}
![Mermaid diagram for the basic process](process-mermaid-visualization.png)

Now that we've seen the basics of building a process in Semantic Kernel, we need to discuss how to route to different steps in the workflow based on a condition in a step.

{#making-decisions-with-sk-process}
## Making decisions in a Semantic Kernel process

In many workflow engines, you have a dedicated decision step that uses input and an expression to invoke one workflow step or another. In Semantic Kernel, we need to build a decision-making step ourselves.

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
2. If the input exceeds 10, we emit the "HighOutcome" event.
3. If the input is lower or equal to 10, we emit the "LowOutcome" event.

We can use these events to execute different steps in the workflow. Let's take a look at the process logic:

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

In the processing logic, we use the `OnFunctionResult` method to route the output
of the `GenerateRandomNumberStep` to the `MakeDecisionStep`. From there, we no longer use the `OnFunctionResult` method to route the data. Instead, we capture the `HighOutcome` event and route the event's data to the `ProcessHighOutcomeStep`. We also capture the `LowOutcome` event to route it to the `ProcessLowOutcomeStep`.

If you're interested in the rest of the code for this process, check out [the sample code][DECISION_PROCESS_SAMPLE] for this chapter.

## Building an intelligent request routing workflow

So far in this chapter, we've only covered how to build processes in Semantic Kernel. While useful, this doesn't address the topic of intelligent request routing. It's time we addressed that by looking at how we can use an LLM to route requests based on their complexity.

We need to perform four steps to build an intelligent model router using the process framework:

1. First, we need to configure multiple LLMs in our application—a larger model for answering complex questions and a smaller one for more straightforward questions.
2. Next, we need to create two steps using the correct models to address the complex and more straightforward questions.
3. After that, we need to add a decision-making step that routes requests based on their complexity.
4. Finally, we need to wire up the process so that the input is passed through the decision step and sent to the correct model steps.

Let's start by configuring multiple LLMs with Semantic Kernel.

### Configuring multiple AI connectors

So far, we've only focused on using a single LLM with our application. Still, Semantic Kernel supports registering multiple LLMs by setting a service identifier for each registered model.

The following code demonstrates how to configure the kernel with multiple models in ASP.NET Core. You can add this to the `Program.cs` file of your ASP.NET core application:

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

We specify one chat completion model with a `complexPrompts` service identifier and the other with a `basicPrompts` service identifier. We'll route to GPT-4o for the complex requests, while for the more basic questions, we'll route to GPT-4o-mini.

After configuring the models, we must ensure we can use both models.

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
        
        // NOTE: The response has a list of choices. In the past you could
        // ask for multiple responses. This is no longer the case, but the
        // Semantic Kernel still has this for stability reasons.
        
        kernel.Data["ResponseContent"] = response[0].Content;
    }
}
```

This step contains the following logic:

1. First, we implement a new step class for basic prompts.
2. Next, we create a method `HandlePromptAsync` that accepts a prompt
3. After that, in the `HandlePromptAsync` method, we request the `IChatCompletionService.`
4. After, we create a chat history with system instructions and the user's prompt.
5. Next, we invoke the completion service, providing prompt execution settings that point the completion service to the correct service identifier.
6. Finally, we return the response by putting it in the kernel data dictionary.

The logic for handling complex prompts is similar to straightforward prompts. The difference is in the service identifier.

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
        
        // NOTE: The response has a list of choices. In the past you could
        // ask for multiple responses. This is no longer the case, but the
        // Semantic Kernel still has this for stability reasons.

        kernel.Data["ResponseContent"] = response[0].Content;
    }
}
```

I've copied the logic here for simplicity and because this allows me to customize the behavior for complex prompts. For example, we could use a different set of hyperparameters for the prompt. You can use inheritance to create different implementations of the class, but it doesn't make much sense because I'm doing other things here conceptually.

Let's discuss how we will route a prompt to the basic or the complex prompt handling steps.

### Routing prompts based on their complexity

Deciding where a user's prompt should go is a classification task that any LLM easily handles with sufficient capacity. After some experimentation, I came up with the following prompt to route requests based on their complexity:

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

```json
{
  "model": "gpt-4o" | "gpt-4o-mini",
  "reason": "Short explanation of why this model was chosen"
}
```
~~~

In this prompt, I'm giving examples of what makes a request suitable for one or the other model. Then, I ask the model to generate output in JSON format so that I can process the response more easily.

Remember that we can use the `ResponseFormat` property on the prompt execution settings class to force the LLM to generate a JSON response. The following code demonstrates how to build the routing step:

```csharp
public class RoutePromptStep : KernelProcessStep
{
    [KernelFunction]
    public async Task RoutePromptAsync(
        Kernel kernel, KernelProcessStepContext context, string prompt)
    {
        var classificationResult = await ClassifyPromptAsync(kernel, prompt);

        if (classificationResult == "gpt-4o")
        {
            await context.EmitEventAsync("HandleComplexPrompt", prompt);
        }
        else if (classificationResult == "gpt-4o-mini")
        {
            await context.EmitEventAsync("HandleBasicPrompt", prompt);
        }
    }

    // ... The rest of the process step.
}
```

This code invokes the `ClassifyPromptAsync` method to determine which model to use. The `ClassifyPromptAsync` method will use the prompt we discussed earlier. When we've found out which model to use, we can route the prompt to the right LLM by emitting the appropriate event.

The method for classifying the user's prompt looks like this:

```csharp
private async Task<string> ClassifyPromptAsync(Kernel kernel, string prompt)
{
    var promptTemplate = kernel.CreateFunctionFromPromptYaml(
        EmbeddedResource.Read("routing-prompt.yml"));

    var executionSettings = new AzureOpenAIPromptExecutionSettings
    {
        Temperature = 0.1,
        ResponseFormat = typeof(RequestRoutingResponseData)
    };

    var response = await promptTemplate.InvokeAsync(
        kernel, new KernelArguments(executionSettings)
        {
            ["prompt"] = prompt
        });

    var responseData = JsonSerializer
        .Deserialize<RequestRoutingResponseData>(
            response.GetValue<string>()!);

    return responseData!.Model;
}
```

This code performs the following steps:

1. First, we load the prompt template stored in a YAML file embedded in the application. I created a class `EmbeddedResource` to load the prompt file from the executable.
2. Next, we configure the prompt execution settings to get the correct response format. I specified a low temperature to help reduce the variation in model names returned.
3. Then, we execute the prompt with the execution settings specifying the user's prompt
 as input for the classification prompt. The output is a JSON payload deserialized to a C# object.
4. Finally, we return the response to the process step for routing the user's prompt to the right model.

The `RoutePromptStep` uses events to decide where to route the data. We must capture these events and forward the prompt to the correct model step. The code for this looks like this:

```csharp
public class AnswerQuestionProcess
{
    private KernelProcess _process;

    public AnswerQuestionProcess()
    {
        var builder = new ProcessBuilder("AnswerQuestion");

        var basicQuestionStep = builder.AddStepFromType<HandleBasicPromptStep>();
        var complexQuestionStep = builder.AddStepFromType<HandleComplexPromptStep>();
        var routingStep = builder.AddStepFromType<RoutePromptStep>();

        builder.OnInputEvent("StartProcess").SendEventTo(new(routingStep));

        routingStep.OnEvent("HandleBasicPrompt")
            .SendEventTo(new(basicQuestionStep));

        routingStep.OnEvent("HandleComplexPrompt")
            .SendEventTo(new(complexQuestionStep));

        _process = builder.Build();
    }

    public async Task<string?> ExecuteAsync(Kernel kernel, string prompt)
    {
        await _process.StartAsync(kernel, new KernelProcessEvent
        {
            Id = "StartProcess",
            Data = prompt
        });

        return kernel.Data["ResponseContent"] as string;
    }
}
```

The process's layout is similar to what we've seen before. We can start the process from an input event, providing it with a prompt. The input data is routed to the routing step we built. From the routing step, we then use the `OnEvent` method to route the prompt to the correct step based on the event raised by the routing step.

## Things to consider when using intelligent request routing

If you're planning on using multiple language models in chat scenarios, you'll need a slightly different approach from what we discussed in this chapter. Typical chat scenarios feature streaming responses to provide a better experience for the application's users, which invalidates using a workflow for chat scenarios.

While intelligent request routing has advantages in cases where an immediate response isn't needed, I wouldn't recommend using a large model to route requests for chat scenarios. You can get much lower latency using a small language model. Even better: Let the user decide if you can.

Users will want to use the biggest possible model for all requests in a chatbot. However, there are ways to help move users towards smaller models to reduce costs and CO2 emissions. For example, you can visualize the costs of a prompt for various models. Make sure to use something that the user of your application finds interesting. In one project, I considered showing the users how many bottles of water it would take to complete a request with a model. Showing the impact of using an LLM helped users consider the environment a little more often.

## Summary

This chapter covered how to build intelligent request routing with Semantic Kernel workflows. The workflow approach that Semantic Kernel offers adds robustness in case the LLM fails to respond in time and it's well worth checking out if you're building an application that doesn't involve chat.

In the next chapter, we'll examine building agents with Semantic Kernel to solve more complex problems with a single agent or a team of agents.

[DAPR]: https://dapr.io/
[CAMUNDA]: https://camunda.com/
[BASIC_SAMPLE_CODE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-10/csharp/Chapter10.BasicProcess/
[MERMAID_TOOL]: https://mermaid.live/
[DECISION_PROCESS_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-10/csharp/Chapter10.DecisionMakingProcess/