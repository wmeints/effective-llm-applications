{#working-with-agents}
# Working with agents

Throughout the book we learned how to use Semantic Kernel with Large Language Models to implement various scenarios. In this final chapter we'll look at building agents with Semantic Kernel. 

At the end of this final chapter you'll have learned how to create agents with Semantic Kernel and how to integrate them into a multi-agent system.

We'll cover the following topics:

- What is and isn't an agent
- When to use an agent
- Building an agent with Semantic Kernel
- Building multi-agent systems with Semantic Kernel
- Testing agents and multi-agent systems
- Security practices when working with agents

## What is and isn't an agent

The concept of an AI agent in relation to LLMs is flexible depending on how you look at it. Thanks to the hype that has been going on since roughly 2022 we now have two definitions depending on whether you want to sell software as a service or just want to explain what an agent is from a technical perspective.

### The product marketing definition of an agent

The product marketing definition is on one hand very broad and involves everything from running a simple prompt, a workflow, up to building highly complex interactions with an LLM. On the other hand, the definition of agents from a product marketing perspective is rather limited. It only focuses on using LLMs.

While agents can use prompts and workflows using a prompt or a workflow doesn't make your application an agent.

### The scientific definition of an agent

The more scientific approach to agents goes back much farther than using an LLM. You can find agents in a subset of the AI field called reinforcement learning that teaches computers to perform a task by letting an agent interact with an environment and teaching it based on feedback given by the environment. Agents in reinforcement learning are trained by letting them practice on a task thousands to millions of times. Where you'll like see them train thousands of times on simple tasks and millions of times on more complex tasks like picking products from a conveyor belt.

Before the use of agents in reinforcement learning there were symbolic reasoning agents. Symbolic reasoning is a form of AI where we give the computer symbols and logical rules and then let it figure out how to solve a problem. The symbols can be values retrieved from a database, and the rules are invented by humans.

Siri, the well-known digital assistant from Apple is a sample of a symbolic agent. It uses speech to text to translate audio to text, extracts entities and intent as symbols and then uses logical rules to perform tasks like setting a timer or sending an email. After executing the requested task you typicall get feedback by converting templated responses to speech. It's a well designed symbolic agent, but it's technique is a little dated these days.

Agents have a few key characteristics that make them what they are:

- They're autonomous
- They're goal-driven
- They're connected

All agents are made to operate without giving at a lot of human input while they're running. You can have agents that run completely on their own once you start them. But there are also agents that require some human feedback to ensure safe operation and high quality results.

Agents typically work towards a single goal. For example, when you ask Siri to set a timer, you start the agent with the goal of setting a timer. It will figure out everything else based on information from the environment it's running in. An agent that you want to use for writing test cases to validate your software has the goal to write a test for a specific piece of code.

Finally, agents are given ways to interact with their environment. There are two ways in which an agent interacts with their environment. The agent can read information from the environment needed to generate a plan for reaching the goal. The agent can also perform actions to manipulate the environment to reach the goal. Depending on how well the agent is trained it will have an easy or hard time achieving its goal.

### The role of large language models in agents

Looking at the scientific approach to agents you can see that agents aren't necessarily bound to an LLM. There are other approaches to reaching a goal for agents. So why are we using LLMs with agents then?

To understand the power of using an LLM to build an agent we should first look at a model of an agent. I've found the model used to explain a reinforcement learning agent quite useful to learn about LLM-based agents.

Consider the structure of an agent as displayed in [#s](#agent-structure).

{#agent-structure}
![The structure of an agent](agent-structure.png)

The agent forms the core of the system and it will interact with the environment it is working in. The cycle starts by gathering state from the environment and then deciding what should be the next action to perform. The agent decides what the best action is and takes the action. After the action is completed, the agent receives new state information and feedback on how useful the action was to achieve the goal we set for it.

This cycle of state, action, feedback continues until the agent reaches a stop condition. The stop condition could be  that we achieved the goal or that we reached a terminal state, for example, we tried a hundred times and still weren't able to complete the task.

In reinforcement learning we use a policy or a model to determine the next best action. This model is typically a neural network that predicts which of the possible actions is the best. The neural network takes in information from the environment to make the choice.

LLMs are neural networks too and they could be used to help the agent to complete tasks, except that they don't predict which one of the actions is best. Instead they generate text. To make an LLM work in an agent we need to shuffle the mental model of a reinforcement learning agent.

- The state the agent receives could be the chat history containing an initial prompt setting the goal and then a list messages describing what happened before and the feedback the agent received.
- The action an agent takes could be a tool call response from the LLM. We can let the agent execute the tool call and feed the output of the tool into the conversation history as new state information and as feedback for the agent.

Using an LLM as the model for an agent saves you a lot of work. You don't need to train a neural network with millions of samples for just one task. The LLM is pretrained on all sorts of tasks making it quite useful as the core of an agent.

Depending on the task you may need to introduce more complex tricks to gather the right information for the agent. For example in the paper "[Large Language Models Play StarCraft II: Benchmarks and A Chain of Summarization Approach](https://arxiv.org/pdf/2312.11865)" they queue up information over time before calling the LLM with a summary of what happened in the game. It's amazing to see how powerful a foundational technique like an LLM really is.

While an LLM is pretrained on a lot of tasks, it depends on what task you're trying to complete with an agent how much success you'll have using an LLM. Complex tasks that can be expressed as text are a great candidate to solve with an LLM-based agent. Spatial tasks are probably going to give you challenges.

### The structure of an LLM-based agent

In the previous section we learned that reinforcement learning and LLM-based agents are similar when you approach them from a scientific point of view. However, I found that the scientific approach doesn't help much in programming an agent. There's a better representation of an LLM-based agent from a programming perspective as shown in [#s](#agent-from-programming-perspective)

{#agent-from-programming-perspective}
![An agent from a programming perspective](agent-from-programming-perspective.png)

In the context of Semantic Kernel and LLMs in general, an agent is a component in an application that has access to a list of tools and interacts with an LLM. The agent receives an initial set of instructions that sets the goal and kicks off the agent process. The agent can use memory to keep track of previous actions to help it achieve the goal set in the instructions.

In [#s](#getting-started-with-semantic-kernel) we covered how Semantic Kernel implements a loop to make it possible to call multiple tools when you submit a prompt to the kernel. This loop is the core of how an agent works in Semantic Kernel. The workflow of an agent is shown in [#s](#agent-processing-loop).

{#agent-processing-loop}
![The agent processing loop]()

The loop starts with a set of instructions and an initial prompt indicating the goal we want to achieve. With this initial set of instructions, the agent calls the LLM and receives a response. When the agent receives a tool call, it invokes the tool and stores the response in its internal memory. After completing a tool call, the agent moves to the beginning of the loop and calls the LLM again with the output of the tool and the chat history. If the response is a regular chat message, the loop stops.

As you may remember from [#s](#enhancing-llms-with-tools) we can implement tools for the agent as C# functions or Semantic Kernel plugins.

By using tools you can give agents access to information through the use of a vector index. You're essentially including the RAG design pattern we discussed in [#s](#retrieval-augmented-generation).

You can also use existing MCP (Model Context Protocol) servers to integrate your agent with websites like Github or tools like Google Drive. We haven't covered MCP in this book but you can learn more about this protocol [in the documentation](https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/adding-mcp-plugins?pivots=programming-language-csharp).

The memory component of the agent can be used as a key/value store to keep information for the longer term. You can use a typical database with a vector index for this. Semantic Kernel has support for many variations since you can use the same storage for implementing the RAG pattern.

Before we start implementing an agent with Semantic Kernel it's important to understand the relationship between prompts and agents because prompts play an essential role in how agents work.

### The role of instructions in an LLM-based agent

LLM-based agents work primarily off prompts and the chat history. So it's important to build a prompt that's suitable for an agent.

Remember from [#s](#the-art-and-nonsense-of-prompt-engineering) that there are 5 important aspects when it comes to constructing a high quality prompt:

1. Provide clear direction
2. Specify what you want as the output
3. Add context and samples to the prompt
4. Keep the prompt focused on one goal
5. Tune the output with hyperparameters

These principles still apply to building instructions for an agent, but you need to tweak each of these five aspects a bit to make them suitable for an agent.

When you provide direction to an agent, make sure to promote producing a chain of thought. For example, you can give the agent a step-by-step plan as the following prompt shows:

```text
You are a Feature File Generator Agent. Your primary purpose is to help create
comprehensive, well-structured feature files based on project documentation and user
requirements.

## Your Capabilities

### TODO Management

- Create, complete, remove, and list TODO items to track your planning tasks
- Use TODO items to break down complex feature file generation into manageable steps
- Always create a plan before starting to write feature files

### Feature File Operations

- Read, write, and modify feature files using Gherkin syntax
- Insert content at specific lines, append content, replace
  existing content, or remove content

### Documentation Access

- Read project documentation files to understand requirements and context
- Search for specific information across documentation files
- List available documentation files

## The process

1. Start by reading the provided work item description. Identifying scenarios
   that need to be recorded. Use the best practices for feature files to
   help you identify useful scenarios.
2. Record a TODO item for each of the scenarios you identified.
3. Go over the recorded TODO items for each of the scenarios and perform
   the following steps:
   - Identify useful examples from the reference documentation for the scenario
   - Write down the steps for the scenario using the examples
   - Read through the scenario and make sure it is as complete as possible
   - Mark the TODO item for the scenario as completed
4. Read through the whole feature file and identify any missing scenarios
   adding them to the feature file
5. Read through the TODO list and make sure all tasks are completed.

After completing the feature file, use the validation steps to
provide information to the developer about the quality of
the work you just performed.

## Validation steps

- Go over the feature file to review the contents of the file.
- Score the file on the following aspects with a score of 1-10.
  - The readability for a business user
  - How hard it is to automate the feature file for the developer
  - How complete the feature file is

## What a scenario should look like

Make sure to write scenarios using these guidelines:

1. A feature should focus on user behavior. List goals as part of the feature description but don't use a separate heading.
2. A scenario should have a clear and descriptive name.
3. Keep scenarios focused on a single user behavior.
4. Keep scenarios independent and deterministic.
5. Use background steps wisely. Use them only for common steps that need to be executed for all scenarios in the feature file.
6. Limit the size of scenarios to keep them clear.
7. Avoid technical jargon in the scenarios.
```

This prompt provides the following information to the agent:

- First, it describes the goal of the agent.
- Then, it describes the capabilities of the agent and the tools associated.
- Next, it gives the agent a step-by-step approach to the problem. If you can, you should definitely include a plan in your intruction to provide clarity about what the agent should do. If you can't use a fixed plan, it's a great idea to instruct the agent to start by setting up a plan before executing it. Being explicit about what you expect of the agent greatly improves the results.
- Finally, the prompt contains a description of the output and how to approach specific
  sub-problems.

There is a lot of detail in the prompt as this will help get the best results from the  LLM. I recommend checking the prompt frequently for quality issues. I noticed that over time the models will get better following the plan, so you may need to tweak the prompt a few times.

### Which model to use for building agents

We covered different LLM providers in [#s](#understanding-llms), there are a lot to choose from. I don't think I can list all of them anymore by the time this book is a year old. And it doesn't get much easier to choose a model when it comes to agents.

Agents work better with LLMs that were trained on agentic tasks. This sounds logical, but how do you know if your agent is going to work with a particular model? And what's an agentic task?

Agentic tasks are usually tasks where the model needs to follow a series of steps or generate a plan and then follow that plan. This requires some form of reasoning skills.

The best option is to use a reasoning model for your agent so that you can be sure that it was trained on agentic tasks. For example, GPT-4.1 and the Orion series from OpenAI (o1, o2, o3, etc.) are trained specifically on agentic tasks. But you should also consider using the LLMs from Anthropic. The Claude 4 models from Anthropic perform really well too.

Note that reasoning models are more expensive so you may want to consider implementing the patterns from [#s](#intelligent-request-routing) if you want to save some money.

## When to use an agent

A lot of people will tell you that you should use an agent. But an agent is only good at a subset of problems that you will encounter. There's a reason why I include so many other patterns in this book and only cover agents last. Agents are new, and we don't know what limitations we'll run into exactly. Also, agents are more expensive than a single prompt.

To help you choose between using an agent, a workflow or a single prompt, I've created the following table.

| Use Case                                               | Prompt | Workflow | Agent |
| ------------------------------------------------------ | ------ | -------- | ----- |
| One-shot tasks (e.g summarization)                     | v      | -        | -     |
| Deterministic multi-step tasks                         | -      | v        | -     |
| Open-ended problem solving                             | -      | -        | v     |
| Tasks involving state/memory                           | -      | v        | v     |
| Goal-directed behavior (planning, retries, correction) | -      | -        | v     |

I recommend aiming to solve problems with the simplest possible approach. So if you can something that's deterministic you'll have a much easier time testing and debugging your application.

## Agent use cases

When talking about agents you may be wondering, what are good use cases for agents? Well, there's one place where agents shine and that's in writing code. You don't need to look far for agents in software engineering.

Github Copilot has Agent Mode that's an implementation of an agent. It can access your code base and write files just to name a few tools it can use. Similar to Github Copilot I recommend looking at Cursor, Windsurf and Claude Code. All of these tools offer excellent implementations of agents. All these agents work with the same principle we covered earlier in the chapter, so while complex, it is possible to write a software engineering agent yourself.

Another great use case for agents is content creation. Writing a blog posts requires researching and refining the content in multiple passes. You can do this yourself, or you can write an agent to do this for you. I'll leave this one up to you as this is one of those scenarios that's a great exercise after you've finished this book.

Another more IT related use case is an agent that can resolve issues with your cloud computing environment. Imagine that an alert is generated by an application after it shutdown due to a problem with storage or a database. An agent could pick up this alert, read the logs from your environment and then use the APIs in your cloud environment to fix the problem for you. Of course this is one of the more dangerous use cases of an agent and you should be cautious when you want to implement this scenario.

{#building-an-agent}
## Building an agent with Semantic Kernel

Now that we have covered what an agent is, when to use one, and some useful use cases, it's time to take a look at how Semantic Kernel help you build agents.

We'll build an agent that can write a feature file to help you turn a generic description into a set of Behavior-driven development (BDD) scenarios. I built this agent as an experiment to understand how to take the idea of a coding agent in the direction of requirements engineering.

You can follow along with the sample with [the code][CHAPTER_SAMPLE_CODE] included for this chapter. I'll cover the key steps needed to create an agent here in the book.

### Setting up an agent project

While you can add an agent to console applications, desktop application and server application For the purpose of this chapter, we'll build an agent in a console application.

We'll use the core agents package called `Microsoft.SemanticKernel.Agents.Core`. This package contains the core abstractions that we need to build an agent.

Make sure your application includes a chat completion model and it's also a good idea to configure an embedding model in case you want to semantically search for information.
You can learn more about configuring chat completion and embedding models in [#s](#setting-up-semantic-kernel). The sample code for this chapter includes the setup code for the language model as well.

### Creating an agent class

There are multiple ways to build an agent in Semantic Kernel. You can use an online service to host your agent or you can build a ChatCompletionAgent which integrates well with any cloud-based LLM provider or one of the open-source models on your local machine. 

OpenAI, Google, and Azure all offer services to build agents. These services are useful when you don't want to manually store the chat history of an agent. In many cases the cloud-based agent services offer additional tools. For example, OpenAI allows you to use the code interpreter. It let's the agent write Python code and execute in a sandbox environment. If you're using the ChatCompletionAgent you'll have to build this functionality yourself.

Although the online services offer useful tools to make it easier to build an agent, there's something to be said for manually storing the chat history. You can access it from your application and analyze it in case the agent makes a mistake. Also, it allows you to control where you deploy your agent. A ChatCompletionAgent is easily moved between various environments and LLM providers while an Azure OpenAI agent only works on Azure.

For the purpose of this chapter, we'll create a ChatCompletionAgent. The code for creating a new ChatCompletionAgent looks like this: 

```csharp
var instructions = EmbeddedResource.Read("Prompts.AgentInstructions.md");

var executionOptions = new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    Temperature = 0.7,
    MaxTokens = 8096,
};

_agent = new ChatCompletionAgent
{
    Instructions = instructions,
    Name = "FeatureFileGenerator",
    Kernel = kernel,
    Arguments = new KernelArguments(executionOptions),
};
```

In this code, we perform the following steps:

1. First, we load the instructions for the agent from an embedded resource. Instructions you write for the agent are used as the system prompt for the LLM.
2. Next, we create the settings used when invoking the LLM. You can control the temperature, presence penalty, and other hyper parameters for the LLM. If you want your agent to use tools, you should consider setting the function choice behavior to automatic.
3. Finally, we can create a new instance of the `ChatCompletionAgent`. We connect the necessary settings and the kernel instance of the application to the agent.

The kernel instance we use should contain the tools we want to provide to the agent. You can implement tools using the mechanisms we dicussed in [#s](#enhancing-llms-with-tools).

Most of the power of the agent comes from the combination of high quality instructions and well defined tools. So it's worth spending time describing the tools as well as you can and testing the prompt to make sure the agent is capable of executing your plan.

### Invoking the agent with a description of a feature

Calling the agent with a task works slightly differently from using the ChatCompletionService we covered earlier in [#s](#executing-your-first-prompt). The code needed to call an agent looks like this:

```csharp
var chatHistory = new ChatHistory();
var agentThread = new ChatHistoryAgentThread(_chatHistory);

chatHistory.AddUserMessage(workItemDescription);

var response = agent.InvokeAsync(agentThread);
```

This code performs the following step:

1. First we create an empty chat history object to store the conversation history.
2. Next, we create an agent thread based off the chat history.
3. Then, we add the work item description, to the chat history.
4. Finally, we invoke the agent with the agent thread.

All the interactions that the agent performs, are stored in the chat history object. 
You can store the chat history as a JSON object in your database or iterate over the messages and store them individually.

If you want the user to provide follow up instructions, you should append them to the chat history and than invoke the agent again with the agent thread that wraps the chat history.

In the sample code for this chapter I choose to build a full agent with tools, so the code in the sample will look more complicated than shown here. One of the key differences is that I wrapped the `ChatCompletionAgent` in a `FeatureFileGenerator` class that automatically will retry invoking the agent when we receive a quota exceeded error from the LLM. 

Also, the agent in the sample code for this chapter uses streaming. This looks similar to how we've used streaming before in the book. Here's what the code for streaming an agent response looks like:

```csharp
public async IAsyncEnumerable<string> InvokeAsync(string prompt)
{
    _chatHistory.AddUserMessage(prompt);

    var responseStream = _retryPipeline.ExecuteEnumerableAsync(context => _agent.InvokeStreamingAsync(_agentThread));

    await foreach (var chunk in responseStream)
    {
        if (chunk.Message is not null && chunk.Message.Content is not null)
        {
            yield return chunk.Message.Content;
        }
    }
}
```

This code uses the retry pipeline that I created for the sample code. It performs the following steps:

1. First, we add the prompt to the currently active chat history.
2. Then, we invoke call the agent with `InvokeStreamingAsync`.
3. Next, we iterate over the returned response object and extract the message content for each of the chunks returned by the agent.

The response type here is a `IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>>` object. Which is a mouthful if you want to tell your colleagues about it. Basically, this is an asynchronous enumerable stream of agent response items containing chunks related to a single chat message.

The `AgentResponseItem` includes the message content as well as metadata needed to associate the response stream with a specific agent and thread. We don't need this extra information right now, but it will play a role when you're building multi-agent solutions later on.

Retrying agent invocations with streaming is a rather annoying affair as `Polly`, the defacto resilience library for .NET has no built-in support for retrying operations that return an `IAsyncEnumerable<T>`. I made a basic helper to solve this issue:

```csharp
public static class PollyStreamingExtensions
{
    public static async IAsyncEnumerable<TItem> ExecuteEnumerableAsync<TItem>(
        this ResiliencePipeline policy,
        Func<CancellationToken, IAsyncEnumerable<TItem>> action,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var (enumerator, movedNext) = await policy.ExecuteAsync(
            async (ct) =>
            {
                var asyncEnumerable = action(ct);
                var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator();

                return (asyncEnumerator, await asyncEnumerator.MoveNextAsync(ct));
            },
            cancellationToken);

        if (movedNext)
        {
            do
            {
                yield return enumerator.Current;
            }
            while (await enumerator.MoveNextAsync(cancellationToken));
        }
    }
}
```

This `ExecuteEnumerableAsync` extension method takes in a function that returns an `IAsyncEnumerable<T>` object. It will try to grab the first item from the stream. If that fails, it will trigger the resilience pipeline logic. Otherwise, it will continue to move through the stream. I made the assumption that once you start iterating over the stream it will continue to work until the end.

Once you have a working base structure for the agent it's important to provide tools to the agent so it can perform tasks. 

### Connecting the agent to other content in your project

As you probably remember from [#s](#enhancing-llms-with-tools) you can write tools and connect them to the `Kernel` as plugins. Providing tools to an agent works in the same way.

The kernel instance you connect to the agent should contain the tools that you want the agent to use. It's also important that you provide the right settings for `FunctionChoiceBehavior`, to enable automatic tool use in the agent.

In the sample code for this chapter I grouped the tools for the agent in two plugins:

1. `FileSystemPlugin` which allows the agent to browse for files in a specific directory. I limited the plugin to only have read access to my file system in a directory I configure during startup of the application.
2. `FeatureFilePlugin` which allows the agent to read and write feature file content. This is limited to one file that I configured during the startup of the agent. This ensures that the agent can't be pushed into overwriting content in other places on the file system.
3. `TodoItemsPlugin` which gives the agent limited memory functionality so it can track TODO items that are part of the feature file generation plan. I'm saving the content to a `.agent/todo-items.json` file so you can see what the agent is doing.

When providing tools to your agent you must be aware that you don't have a lot of control over what the agent is going to do with those tools. I recommend limiting the tools as much as possible so you don't end up deleting the root file system or sending information to people that's not supposed to be sent.

Another great tip is to use a filter to ask the user for confirmation before invoking a tool. You can learn more about using tool invocation filters in [#s](#applying-filters-to-functions). Don't try to limit what users can do in the agent instructions, because it's just too easy for people to break your agent with prompt injection.

You can use tool invocation filters too for reportign progress to the user. Often you'll see tools like Github Copilot report tool usage in the user interface. Tool invocation filters are a helpful tool to build similar functionality with Semantic Kernel.

### Getting structured output from the agent

So far we've covered how to chat to an agent which can then use tools to perform tasks. This doesn't need structured output. But you can certainly combine generating structured output with agents. In many cases I prefer to get structured output if I don't need to get follow-up input from the user.

If you want to get structured output from an agent, you'll need to modify the execution settings that you provide to the agent. The following code demonstrates this:

```csharp
var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    ResponseFormat = typeof(MyResponseType),
};

_agent.InvokeAsync(_agentThread, new AgentInvokeOptions
{

    KernelArguments = new KernelArguments(executionSettings)
});
```

First, we define the prompt invocation settings with a response format to get the structured output. Then, we invoke the agent as normal, providing a new set of KernelArguments specifying the prompt invocation settings we created.

### Building stateful agents with a backing service

{#building-multi-agent-systems}
## Building multi-agent systems with Semantic Kernel

There are cases where a single agent will have a hard time completing a full scenario. If you have an agent that can write feature files it's not very good at reviewing them at the same time because the instructions will be watered down too much. It's better to use multiple agents if you need to solve a problem that requires two or more tasks to be completed.

To write and review BDD feature files we can use a competitive agents pattern. One agent will write the feature file and a second agent reviews the feature file providing feedback so the first agent can improve it. There are other patterns that you can use. We'll cover these in [#s](#multi-agent-patterns).

For now we'll work on extending the scenario from [#s](building-an-agent) with a second agent that we'll use to review the generated BDD feature file.

### Reviewing the feature file with a second agent

### Coordinating behavior between the writer and the reviewer agents

## Testing agents and multi-agent systems

As if testing prompts isn't difficult enough, agents are harder to test and require even more specific testing strategies to deal with the fact that they're rather dynamic.

We'll cover three layers of testing and monitoring to help you understand how you can get the best results when you need to test your agent:

- Unit-testing agents
- Testing agent behavior
- Improving agents with tracing

Let's start with unit-test strategies that work well for testing agents.

### Unit-testing agents

- Agents themselves are hard to test. But there's not a lot of code to test.
- Focus on testing tools rather than the agent itself.

### Testing agent behavior

- Define a scenario that you want the agent to complete
- Run the agent, and let it complete the scenario
- Make sure you use predefined responses from tools to limit where the scenario leads
- Include failure scenarios to make sure your agent stops or recovers correctly

### Dealing with multi-agent systems

- Make sure you test the code that controls agent choices
- Test agents separately, testing them together requires a lot of work
- Run smoke tests to validate the agents working together

### Improving agents with tracing

- Make sure to set up monitoring as discussed in [#s](#prompt-testing-and-monitoring).
- Use the monitoring to understand how people are using your agent and derive test scenarios from this information.

{#multi-agent-patterns}
## Interaction patterns for multi-agent systems

In [#s](#building-multi-agent-systems) we used the competitive agents to review feature files after they were written by a requirements engineering agent. There are other patterns that are useful for other use cases.

When it comes to multi-agent systems it's important to understand that you'll have to deal with multiple autonomous agents communicating with eachother. This requires you to make choices about interaction and coordination styles.

### interaction styles for multi-agent systems

There are three coordination styles that are important for multi-agent systems:

1. Competitive agents
2. Cooperative agents
3. Hierarchical agents

Agents can give eachother feedback and in essence work against eachother with the ultimate goal of improving the produced end result. 

## Security practices when working with agents

### Limit access to your systems

- Limit what your agent can do without human supervision.
- Base everything of least privilege principles, this will reduce the blast radius
- Make sure you limit access to a system in time when performing dangerous operations. I recommend asking for human approval, then give the agent an access token with the elevated permissions, and limit that token in time so that the token is less likely to be abused.

### Be aware of data poisoning

- When your agent reads documentation or websites its easy for a hacker to inject specific instructions into the documentation or the website your agent is reading to essentially poison the agent. You don't see the effects of this poisoning before it's too late. I recommend limiting information to stuff you control. And if you can't provide 100% control over the context information I recommend using content filtering that detects poisonous content before it's used by the agent.

### Limit the autonomy of the agents

- Agents can run in circles and they do that quite frequently like a dog chasing its tail. This effect gets worse when you combine multiple agents. It's a good idea to limit the number of iterations an agent can take before terminating the agent or involving human oversight.

### Use content filters and validate input

- Agents can produce very dangerous content especially if they send this content to other agents without human oversight. I recommend including content filters for every operation and tuning the filters to be a little more strict than you normally would in chat use cases.

### Provide human oversight

- Don't leave your agent unattended. Make sure you let humans review dangerous actions before executing them. It's also a great idea to let a human review generated content before posting it somewhere.

## Summary

In this final chapter we looked at agents and what value they bring to LLM-based applications over other patterns that we covered in previous chapters. We learned that agents can be helpful when you want to solve problems that require flexible planning and in cases where we can only define a set of rules and a goal. We also looked at building an agent and combining it with other agents to solve more complex problems in your LLM-based applications. We discovered that testing agents is challenging and requires strategies that are different from regular software and basic prompts. Finally, we covered that it is important to constrain your agents so you don't run into high creditcard bills or broken systems.

If you're interested in learning more about agents, I recommend reading the paper [AI Agents vs Agentic-AI: A Conceptual Taxonomy, Applications and Challenges](https://arxiv.org/abs/2505.10468). It provides a great explanation of the taxonomy around agents and gives you a glimpse of what the future may hold for agentic AI.

[CHAPTER_SAMPLE_CODE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-11/csharp/