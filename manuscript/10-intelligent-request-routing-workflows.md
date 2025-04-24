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

For the remainder of this chapter we'll focus on designing and building two types of intelligent request routing solutions:

- Routing Github issues to specific functional areas in a Github repository.
- A chat solution with multiple "agents" for product catalog information and customer service.

In the previous chapter we've built our workflow by hand. It helps to understand how to split a complex task into separate steps, but it is lacking some important features to make the workflow more robust. There is a solution though: The Semantic Kernel process framework.

## Introducing to the process framework

When it comes to chaining prompts together it's a great idea to use a workflow engine to make handling transient errors, human-machine interaction, and versioning of long-running workflows easier.

There are a lot of options available in the marketplace right now. I've personally used [Camunda][CAMUNDA], [Dapr Workflow][DAPR], and [Prefect][PREFECT] a lot in my work. Each of these products offer the same set of features:

- A resilient workflow engine that can handle transient errors.
- A solution to handle long-running workflows with versioning.
- A management dashboard to track running workflow instances.

While the last item on this list isn't always useful when you have short-running workflows, it's a life saver for long-running workflows with human interaction.

While all of these products are great, they have one problem in common: You need to add yet another layer of moving parts on top of Semantic Kernel. That's why the developers of the Semantic Kernel team came up with the idea of providing a workflow engine out of the box tailored for LLM-based workflows.

> The Semantic Kernel process framework is still in preview!  
> While the base API of Semantic Kernel has been stable for a while, the API interface for the process framework shows some churn. I don't recommend running the process framework in a production scenario unless you plan for extra maintenance due to changes in the API. A workflow engine like the one offered in Dapr is a better option if you need to plan for a more stable environment.

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
    public void GenerateGreeting(string name)
    {
        Console.WriteLine("$Hello, ${name}!");
    }
}
```

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

    public async Task StartAsync(Kernel kernel)
    {
        await _process.StartAsync(kernel, new KernelProcessEvent 
        { 
            Id = "StartProcess" 
        });
    }
}
```

This code performs the following steps:

1. First, we create a new class `GreetingProcess` as a container for the process logic.
2. Next, in the constructor we create a new instance of the `ProcessBuilder` class.
3. Then, we register the step definitions for the process.
4. After that, we define an input event for the process to start the process.
5. Next, we emit an event to the second step when the first step finishes.

The Semantic Kernel process framework uses events whenever it needs to send or receive data. The input event we defined is fired when we call `StartAsync` and causes the `GetName` method in `GetNameStep` to be called. 

The `OnFunctionResult` definition in the process generates an event that we capture by executing the `GenerateGreeting` method in the `GenerateGreetingStep` class.

Using events to transport data is important, because this allows the runtime to move the process steps to different machines and talk to them via HTTP or even gRPC. The event data is serialized to JSON, sent over to where the step is executed and then deserialized and used.

The basic process we just built uses a single method in each step, but that's not a requirement. You can define multiple kernel functions per step. This makes it possible to have multiple variations of the same process step in one class. Definine multiple kernel functions in one step class can be useful if you need to have a variant of the step for the first time it's executed and another variant for the executions after that.

If you decide to define multiple kernel functions in one step class, you need to be more specific when routing data in the process definition. Instead of calling `OnFunctionResult()` you need to call `OnFunctionResult(functionName: "GetName")` so the process knows which variant of the step you're using. The function name should match the name of the method without a possible `Async` postfix if you're building an async method.

The same technique we just discussed should also be applied to `SendEventTo`. You need to add a `functionName` argument to route the data to the correct variant of your process step.

### Visualizing the process using mermaid diagrams

As your process becomes more complex, you'll find that it becomes harder to see what's the exact flow in the process. Luckily for us, Microsoft thought of this and added a `ToMermaid` method to the final process instance we created in the constructor of our process. Mermaid is a text-based diagramming syntax with [an online tool][MERMAID_TOOL] that allows you to turn the mermaid format into a PNG image.

I made a habit of wiring the `ToMermaid` method in my process definitions so I can call it to output the graph definition of my process in the mermaid format. The following code demonstrates how to do this:

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

{#making-decisions-with-sk-process}
## Making decisions in a Semantic Kernel process

### Using events to route data through the workflow

### Using state in workflow steps to base your decisions on

## Building an intelligent request routing workflow

## Summary

[DAPR]: https://dapr.io/
[CAMUNDA]: https://camunda.com/
[PREFECT]: https://www.prefect.io/
[BASIC_SAMPLE_CODE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-10/csharp/Chapter10.BasicProcess/
[MERMAID_TOOL]: https://mermaid.live/