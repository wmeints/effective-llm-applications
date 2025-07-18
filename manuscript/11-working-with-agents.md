{#working-with-agents}
# Working with agents

Throughout the book, we learned how to use Semantic Kernels with large language models to implement various scenarios. In this final chapter, we'll examine building agents with Semantic Kernel.

At the end of this final chapter, you will have learned how to create agents with Semantic Kernel and how to integrate them into a multi-agent system.

We'll cover the following topics:

- What is and isn't an agent
- When to use an agent
- Building an agent with Semantic Kernel
- Building multi-agent systems with Semantic Kernel
- Testing agents and multi-agent systems
- Security practices when working with agents

## What is and isn't an agent

The concept of an AI agent in relation to LLMs is flexible. Thanks to the hype that has been going on since roughly 2022, we now have two definitions depending on whether you want to sell software as a service or just explain what an agent is from a technical perspective.

### The product marketing definition of an agent

The definition of product marketing is very broad and involves everything from running a simple prompt and a workflow to building highly complex interactions with an LLM. On the other hand, the definition of agents from a product marketing perspective is limited. It only focuses on using LLMs.

While agents can use prompts and workflows, using a prompt or a workflow doesn't make your application an agent.

### The scientific definition of an agent

The more scientific approach to agents goes back much farther than using an LLM. You can find agents in a subset of the AI field called reinforcement learning that teaches computers to perform a task by letting an agent interact with an environment and teaching it based on feedback given by the environment. Agents in reinforcement learning are trained by allowing them to practice on a task thousands to millions of times. You'll likely see them train thousands of times on simple tasks and millions of times on more complex tasks like picking products from a conveyor belt.

Symbolic reasoning agents existed before reinforcement learning. In this form of AI, we give the computer symbols and logical rules and then let it figure out how to solve a problem. The symbols can be values retrieved from a database, and humans invent the rules.

Siri, the well-known digital assistant from Apple, is a sample of a symbolic agent. It uses speech-to-text to translate audio to text, extracts entities and intent as symbols, and then uses logical rules to perform tasks like setting a timer or sending an email. After executing the requested task, you typically get feedback by converting templated responses to speech. It's a well-designed symbolic agent, but its technique is a little dated.

Agents have a few key characteristics that make them what they are:

- They're autonomous
- They're goal-driven
- They're connected

All agents are made to operate without a lot of human input while they're running. You can have agents that run entirely independently once you start them. However, some agents require human feedback to ensure safe operation and high-quality results.

Agents typically work towards a single goal. For example, when you ask Siri to set a timer, Siri starts the agent to set a timer. It will figure out everything else based on information from the environment it's running in. An agent you want to use to write test cases to validate your software aims to write a test for a specific piece of code.

Finally, agents are given ways to interact with their environment. There are two ways in which an agent interacts with their environment. The agent can read information from the environment needed to generate a plan for reaching the goal. The agent can also perform actions to manipulate the environment to reach the goal. Depending on how well the agent is trained, it will have an easy or hard time achieving its goal.

### The role of large language models in agents

Looking at the scientific approach to agents, you can see that they aren't necessarily bound to an LLM. There are other approaches to reaching a goal for agents. So why are we using LLMs with agents, then?

To understand the power of using an LLM to build an agent, we should first look at an agent model. I've found the model used to explain a reinforcement learning agent quite useful for learning about LLM-based agents.

Consider the structure of an agent as displayed in [#s](#agent-structure).

{#agent-structure}
![The structure of an agent](agent-structure.png)

The agent forms the core of the system and will interact with its working environment. The cycle starts by gathering state from the environment and deciding the next action to perform. The agent decides what the best action is and takes the action. After the agent completes the action, it receives new state information and feedback on how useful the action was to achieve the goal we set for it.

This cycle of state, action, and feedback continues until the agent reaches a stop condition. The stop condition could be that we achieved the goal or that we reached a terminal state. For example, we tried a hundred times and still weren't able to complete the task.

In reinforcement learning, we use a policy or a model to determine the next best action. This model is typically a neural network that predicts which of the possible actions is the best. The neural network takes in information from the environment to make the choice.

LLMs are neural networks, too, and they could help the agent plan actions. However, they don't predict which action is best; instead, they generate text. To make an LLM work in an agent, we need to shuffle the mental model of a reinforcement learning agent.

The state the agent receives could be the chat history, which contains an initial prompt setting the goal and then a list of messages describing what happened before and the feedback the agent received.
An agent's action could be a tool call response from the LLM. We can let the agent execute the tool call and feed the tool's output into the conversation history as new state information and feedback for the agent.

Using an LLM as the model for an agent saves you a lot of work. You don't need to train a neural network with millions of samples for just one task. The LLM is pretrained on all sorts of functions, making it quite useful as the core of an agent.

Depending on the task, you may need to introduce more complex tricks to gather the correct information for the agent. For example, in the paper "[Large Language Models Play StarCraft II: Benchmarks and A Chain of Summarization Approach](https://arxiv.org/pdf/2312.11865), they queue up information over time before calling the LLM with a summary of what happened in the game - seeing how powerful a foundational technique like an LLM is incredible.

While an LLM is pretrained on many tasks, it depends on what task you're trying to complete with an agent and how much success you'll have using an LLM. Complex tasks that can be expressed as text are great candidates for solving with an LLM-based agent. Spatial tasks are probably going to give you challenges.

### The structure of an LLM-based agent

In the previous section, we learned that reinforcement learning and LLM-based agents are similar when you approach them from a scientific point of view. However, the scientific approach doesn't help much in programming an agent. There's a better representation of an LLM-based agent from a programming perspective, as shown in [#s](#agent-from-programming-perspective)

{#agent-from-programming-perspective}
![An agent from a programming perspective](agent-from-programming-perspective.png)

In the context of Semantic Kernel and LLMs in general, an agent is a component in an application that has access to a list of tools and interacts with an LLM. The agent receives an initial set of instructions that sets the goal and kicks off the agent process. The agent can use memory to keep track of previous actions to help it achieve the goal set in the instructions.

In [#s](#getting-started-with-semantic-kernel), we covered how Semantic Kernel implements a loop to make it possible to call multiple tools when you submit a prompt to the kernel. This loop is the core of how an agent works in Semantic Kernel. An agent's workflow is shown in [#s](#agent-processing-loop).

{#agent-processing-loop}
![The agent processing loop]()

The loop starts with instructions and an initial prompt indicating the goal we want to achieve. With this initial set of instructions, the agent calls the LLM and receives a response. When the agent gets a tool call, it invokes the tool and stores the response in its internal memory. After completing a tool call, the agent moves to the beginning of the loop and calls the LLM again with the tool's output and the chat history. If the response is a regular chat message, the loop stops.

As you may remember from [#s](#enhancing-llms-with-tools), we can implement tools for the agent as C# functions or Semantic Kernel plugins.

You can use tools to give agents access to information through a vector index. This is essentially including the RAG design pattern we discussed in [#s](#retrieval-augmented-generation).

You can also use existing MCP (Model Context Protocol) servers to integrate your agent with websites like GitHub or tools like Google Drive. We haven't covered MCP in this book, but you can learn more about this protocol [in the documentation](https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/adding-mcp-plugins?pivots=programming-language-csharp).

The agent's memory component can be used as a key/value store to store information for the longer term. For this, you can use a typical database with a vector index. Semantic Kernel supports many variations since you can use the same storage for implementing the RAG pattern.

Before we start implementing an agent with Semantic Kernel, it's important to understand the relationship between prompts and agents because prompts play an essential role in how agents work.

### The role of instructions in an LLM-based agent

LLM-based agents work primarily off prompts and the chat history. So it's essential to build a prompt suitable for an agent.

Remember from [#s](#the-art-and-nonsense-of-prompt-engineering) that there are 5 essential aspects when it comes to constructing a high-quality prompt:

1. Provide clear direction
2. Specify what you want as the output
3. Add context and samples to the prompt
4. Keep the prompt focused on one goal
5. Tune the output with hyperparameters

These principles still apply to building instructions for an agent, but you need to tweak each of these five aspects a bit to make them suitable for an agent.

When you provide direction to an agent, promote producing a chain of thought. For example, you can give the agent a step-by-step plan as the following prompt shows:

```text
You are a Feature File Generator Agent. Your primary purpose is to help create
comprehensive, well-structured feature files based on project documentation
and user requirements.

## Your Capabilities

### TODO Management

- Create, complete, remove, and list TODO items to track your planning tasks
- Use TODO items to break down complex feature file generation
  into manageable steps
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

1. Start by reading the provided work item description. Identifying
   scenarios that need to be recorded. Use the best practices for
   feature files to help you identify useful scenarios.
2. Record a TODO item for each of the scenarios you identified.
3. Go over the recorded TODO items for each of the scenarios and perform
   the following steps:
   - Identify useful examples from the reference documentation for
     the scenario
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

1. A feature should focus on user behavior. List goals as part
   of the feature description but don't use a separate heading.
2. A scenario should have a clear and descriptive name.
3. Keep scenarios focused on a single user behavior.
4. Keep scenarios independent and deterministic.
5. Use background steps wisely. Use them only for common 
   steps that need to be executed for all scenarios in the feature file.
6. Limit the size of scenarios to keep them clear.
7. Avoid technical jargon in the scenarios.
```

This prompt provides the following information to the agent:

- First, it describes the goal of the agent.
- Then, it describes the agent's capabilities and the associated tools.
- Next, it gives the agent a step-by-step approach to the problem. You should include a plan to clarify what the agent should do. If you can't use a fixed plan, it's a great idea to instruct the agent to start by setting up a plan before executing it. Being explicit about what you expect of the agent greatly improves the results.
- Finally, the prompt contains a description of the output and how to approach specific
 sub-problems.

The prompt is very detailed, which will help you get the best results from the LLM. I recommend checking the prompt frequently for quality issues. I noticed that the models will get better over time if you follow the plan, so you may need to tweak the prompt a few times.

### Which model to use for building agents

We covered different LLM providers in [#s](#understanding-llms); there are many to choose from. By the time this book is a year old, I don't think I can list all of them anymore. And choosing a model for building agents doesn't get much easier.

Agents work better with LLMs that are trained on agentic tasks. As logical as it sounds to use a model trained on agentic tasks, you may wonder which model to use.

Agentic tasks are usually tasks where the model needs to follow a series of steps or generate a plan and then follow that plan. Following a plan requires some form of reasoning skills.

The best option is to use a reasoning model for your agent to ensure that the LLM was trained on agentic tasks. For example, GPT-4.1 and the Orion series from OpenAI (o1, o2, o3, etc.) are explicitly trained on agentic tasks. However, you should also consider using the LLMs from Anthropic. The Claude 4 models from Anthropic perform well too.

Note that reasoning models are more expensive, so if you want to save some money, you may want to consider implementing the patterns from [#s](#intelligent-request-routing).

## When to use an agent

A lot of people will tell you that you should use an agent. However, an agent is only good at a subset of problems that you will encounter. There's a reason why I include so many other patterns in this book and only cover agents last. Agents are new, and we don't know exactly what limitations we'll run into. Also, agents are more expensive than a single prompt.

I've created the following table to help you decide between using an agent, a workflow, or a single prompt.

| Use Case                                               | Prompt | Workflow | Agent |
| ------------------------------------------------------ | ------ | -------- | ----- |
| One-shot tasks (e.g summarization)                     | v      | -        | -     |
| Deterministic multi-step tasks                         | -      | v        | -     |
| Open-ended problem solving                             | -      | -        | v     |
| Tasks involving state/memory                           | -      | v        | v     |
| Goal-directed behavior (planning, retries, correction) | -      | -        | v     |

I recommend aiming to solve problems with the simplest possible approach. If you can do something deterministic, you'll have a much easier time testing and debugging your application.

## Agent use cases

After seeing the table from the previous section, you may still wonder, "What are concrete use cases where agents shine?" Well, at least one place where agents shine is in writing code. You don't need to look far for agents in software engineering.

Github Copilot has Agent Mode, which is an implementation of an agent. It can access your code base and write files, just to name a few tools it can use. Like GitHub Copilot, I recommend looking at Cursor, Windsurf, and Claude Code. All of these tools offer excellent agent implementations. All these agents work with the same principle we covered earlier in the chapter, so while complex, writing a coding agent yourself is possible.

Another excellent use case for agents is textual content creation. Agents are really good at creating project plans based on client documentation and requirements.

Another IT-related use case is an agent that can resolve issues with your cloud computing environment. Imagine that an application generates an alert after it shuts down due to a problem with storage or a database. An agent could pick up this alert, read the logs from your environment, and then use the APIs in your cloud environment to fix the problem for you. Of course, this is one of the more dangerous use cases of an agent, and you should be cautious when you want to implement this scenario.

{#building-an-agent}
## Building an agent with Semantic Kernel

Now that we have covered what an agent is, when to use one, and some useful use cases, it's time to examine how Semantic Kernel can help you build agents.

We'll build an agent that can write a feature file to help you turn a generic description into a set of Behavior-driven development (BDD) scenarios. I built this agent as an experiment to understand how to take the idea of a coding agent in the direction of requirements engineering.

You can follow along with the sample with [the code][CHAPTER_SAMPLE_CODE] included for this chapter. In the book, I'll cover the key steps needed to create an agent.

### Setting up an agent project

While you can add an agent to console applications, desktop applications, and server applications, in this chapter, we'll build an agent in a console application.

We'll use the core agents package `Microsoft.SemanticKernel.Agents.Core`. This package contains the core abstractions needed to build an agent.

Make sure your application includes a chat completion model. It's also a good idea to configure an embedding model in case you want to search for information semantically.

Learn more about configuring chat completion and embedding models in [#s](#setting-up-semantic-kernel). The sample code for this chapter also includes the setup code for the language model, so you can use that as a starting point if you want to build your own agent.

### Creating an agent class

There are multiple ways to build an agent in Semantic Kernel. You can use an online service to host your agent or make a ChatCompletionAgent, which integrates well with any cloud-based LLM provider or one of the open-source models on your local machine. 

OpenAI, Google, and Azure all offer services to build agents. These services are useful when you don't want to store an agent's chat history manually. In many cases, cloud-based agent services offer additional tools. For example, OpenAI allows you to use the code interpreter. It lets the agent write Python code and execute it in a sandbox environment. If you're using the ChatCompletionAgent, you'll have to build this functionality yourself.

Although the online services offer helpful tools to make it easier to build an agent, there's something to be said for manually storing the chat history. You can access it from your application and analyze it if the agent makes a mistake. Also, it allows you to control where you deploy your agent. A ChatCompletionAgent is easily moved between various environments and LLM providers, while an Azure OpenAI agent only works on Azure.

For this chapter, we'll create a ChatCompletionAgent. The code for creating a new ChatCompletionAgent looks like this: 

```csharp
var instructions = EmbeddedResource.Read(
    "Prompts.AgentInstructions.md");

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

1. First, we load the agent's instructions from an embedded resource. The instructions you write for the agent are used as the system prompt for the LLM.
2. Next, we create the settings used when invoking the LLM. You can control the temperature, presence penalty, and other hyperparameters for the LLM. If you want your agent to use tools, consider setting the function choice behavior to automatic.
3. Finally, we can create a new `ChatCompletionAgent instance. We connect the necessary settings and the application's kernel instance to the agent.

The kernel instance we use should contain the tools we want to provide to the agent. You can implement tools using the mechanisms discussed in [#s](#enhancing-llms-with-tools).

Most of the agent's power comes from the combination of high-quality instructions and well-defined tools. So it's worth spending time describing the tools as well as you can and testing the prompt to ensure the agent can execute your plan.

### Invoking the agent with a description of a feature

Calling the agent with a task slightly differs from using the ChatCompletionService we covered earlier in [#s](#executing-your-first-prompt). The code needed to call an agent looks like this:

```csharp
var chatHistory = new ChatHistory();
var agentThread = new ChatHistoryAgentThread(_chatHistory);

chatHistory.AddUserMessage(workItemDescription);

var response = agent.InvokeAsync(agentThread);
```

This code performs the following steps:

1. First, we create an empty chat history object to store the conversation history.
2. Next, we create an agent thread based on the chat history.
3. Then, we add the work item description to the chat history.
4. Finally, we invoke the agent with the agent thread.

All the agent's interactions are stored in the chat history object. 
You can store the chat history as a JSON object in your database or iterate over the messages and store them individually.

If you want the user to provide follow-up instructions, you should append them to the chat history and then invoke the agent again with the agent thread that wraps the chat history.

In the sample code for this chapter, I chose to build a full agent with tools, so the code will look more complicated than shown here. One key difference is that I wrapped the `ChatCompletionAgent` in a `FeatureFileGenerator` class that automatically retries invoking the agent when we receive a quota exceeded error from the LLM.

Also, the agent in the sample code for this chapter uses streaming. The streaming logic looks similar to how we've used streaming before in the book. Here's what the code for streaming an agent response looks like:

```csharp
public async IAsyncEnumerable<string> InvokeAsync(string prompt)
{
    _chatHistory.AddUserMessage(prompt);

    var responseStream = _retryPipeline.ExecuteEnumerableAsync(
        context => _agent.InvokeStreamingAsync(_agentThread));

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

1. First, we add the prompt to the active chat history.
2. Then, we call the agent with `InvokeStreamingAsync`.
3. Next, we iterate over the returned response object and extract the message content for each chunk the agent returned.

The response type here is an `IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>>` object, which is a mouthful if you want to tell your colleagues about it. This enumerable is an asynchronous stream of agent response items containing chunks related to a single chat message.

The `AgentResponseItem` includes the message content and metadata to associate the response stream with a specific agent and thread. We don't need this extra information right now, but it will be helpful when building multi-agent solutions later on.

Retrying agent invocations with streaming is a rather annoying affair, as `Polly`, the de facto resilience library for .NET, has no built-in support for retrying operations that return an `IAsyncEnumerable<T>`. I made a basic helper to solve this issue:

```csharp
public static class PollyStreamingExtensions
{
    public static async IAsyncEnumerable<TItem> ExecuteEnumerableAsync<TItem>(
        this ResiliencePipeline policy,
        Func<CancellationToken, IAsyncEnumerable<TItem>> action,
        [EnumeratorCancellation] CancellationToken cancellationToken = 
            default)
    {
        var (enumerator, movedNext) = await policy.ExecuteAsync(
            async (ct) =>
            {
                var asyncEnumerable = action(ct);
                var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator();

                return (asyncEnumerator, 
                    await asyncEnumerator.MoveNextAsync(ct));
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

This `ExecuteEnumerableAsync` extension method takes in a function that returns an `IAsyncEnumerable<T>` object. It will try to grab the first item from the stream. If that fails, it will trigger the resilience pipeline logic. Otherwise, it will continue to move through the stream. I assumed it would work until the end, once you start iterating over the stream.

Once you have a working base structure for the agent, it's important to provide it with tools to perform tasks.

### Connecting the agent to other content in your project

As you probably remember from [#s](#enhancing-llms-with-tools) you can write tools and connect them to the `Kernel` as plugins. Providing tools to an agent works in the same way.

The kernel instance you connect to the agent should contain the tools you want it to use. It's also important that you provide the right settings for `FunctionChoiceBehavior` to enable automatic tool use in the agent.

In the sample code for this chapter, I grouped the tools for the agent in two plugins:

1. `FileSystemPlugin,` which allows the agent to browse for files in a specific directory. I limited the plugin to having read access to my file system in a directory I configured during the application's startup.
2. `FeatureFilePlugin` allows the agent to read and write feature file content. I limited the feature file plugin to one file that I configured during the agent's startup. This ensures that the agent can't be pushed into overwriting content in other places on the file system.
3. `TodoItemsPlugin` gives the agent limited memory functionality so it can track TODO items that are part of the feature file generation plan. I'm saving the content to a `.agent/todo-items.json` file so you can see what the agent is doing.

When providing tools to your agent, you must be aware that you don't have much control over what the agent does with those tools. I recommend limiting the tools as much as possible so you don't end up deleting the root file system or sending information to people that's not supposed to be sent.

Another great tip is to use a filter to ask the user for confirmation before invoking a tool. You can learn more about using tool invocation filters in [#s](#applying-filters-to-functions). Don't try to limit what users can do in the agent instructions, because it's too easy for people to break your agent with prompt injection.

You can also use tool invocation filters to report progress to the user. Often, you'll see tools like GitHub Copilot report tool usage in the user interface. Tool invocation filters are a helpful tool for building similar functionality with Semantic Kernel.

### Getting structured output from the agent

So far, we've covered how to chat to an agent, who can then use tools to perform tasks. This doesn't need structured output, but you can certainly combine generating structured output with agents. In many cases, I prefer to get structured output if I don't need to get follow-up input from the user.

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

First, we define the prompt invocation settings with a response format to get the structured output. Then, we invoke the agent as usual, providing a new set of KernelArguments specifying the prompt invocation settings we created.

{#building-multi-agent-systems}
## Building multi-agent systems with Semantic Kernel

In some cases, a single agent will have a hard time completing a whole scenario. If you have an agent who can write feature files, they're not very good at reviewing them simultaneously because the instructions will be watered down too much. It's better to use multiple agents if you need to solve a problem requiring two or more tasks.

Working with multiple agents in Semantic Kernel is still in preview, and the API is rather unstable right now, so I'll cover it briefly in this book. I may revisit this section to provide a more detailed approach to working with agents.

### Multi-agent patterns

Semantic Kernel offers multiple patterns to orchestrate agents:

- **Concurrent agents**: Broadcast a task to multiple agents simultaneously and collect the results when they finish their work. Concurrent agents are typically used to complete multiple independent tasks at once.
- **Sequential**: Take a task, process it with one agent, and then hand the result to the next agent in the sequence. If you wish, this hand-off can be repeated multiple times to refine the results with more than two agents.
- **Group chat**: Have multiple agents work together in a group chat to brainstorm on a task. This is useful for content creation. For example, when you have a research agent, a writing agent, and a reviewing agent working together on a blog post.
- **Hand-off**: One agent orchestrates the conversation and hands off specialist tasks to other sub-agents.

As work progresses on the multi-agent patterns in Semantic Kernel, you'll find more and more patterns on [the documentation website](https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-orchestration/?pivots=programming-language-csharp).

## Testing agents

Testing LLM-based applications requires a specific approach, as we've covered in [#s](#prompt-testing-and-monitoring). Agents require a similar approach, which we'll cover here. We'll focus on the following areas.

- Unit-testing agents
- Testing agent behavior
- Improving agents with tracing

Let's start with unit-test strategies that work well for testing agents.

When unit-testing agents, it's essential to establish what you can unit-test and what parts of the agent should be covered with other kinds of tests. The tools used by the agent can often be unit-tested well. The agent behavior is challenging to unit-test as the LLM doesn't provide stable outputs.

I recommend using prompt testing as covered in [#s](#model-based-testing) to validate agent behavior. I prefer the following approach to prompt testing an agent:

First, I run a specific task through the agent with a curated set of test data for the agent's tools. Combining test input with curated content should help get the correct results.

Then, I take the output and use prompt testing to validate that it is what I expect it to be. This can be challenging if the output is a call to a tool. In that case, it's essential to validate the output sent to the tool. You can inject a specific filter into the agent to capture and verify the tool call.

Of course, you need to include failure scenarios in your testing. Here are some ideas for testing failure scenarios:

Ask yourself what happens if a specific tool is unavailable. Can the agent use other tools? I often encounter surprising behavior when the expected tool isn't available or throws an error. Agents will try to use different tools to solve the problem, often leading to some surprising problems.

While testing an agent that was supposed to write tests and then run them to get feedback to improve those tests, the agent had shell access and a specific tool for running tests, but the agent wouldn't use the testing tool because the tool failed. Instead, it started constructing shell commands to run a test runner. Which it successfully was able to do. That looked okay, until the output of the tests wasn't helpful for the agent, as it couldn't improve the test cases.

Another interesting failure scenario is this: What will happen if the agent runs out of context window space in the LLM? Sometimes, I forget to include some way to summarize the conversation. I also had an interesting case where the summarization led to failures because the agent didn't know what to do anymore. The key point to remember here is not to summarize your plan too much.

As you can imagine, testing an agent with tools requires you to think of many complicated scenarios. It's pretty hard to get those right the first time. Therefore, I recommend investing in telemetry.

### Improving agents with tracing

In [#s](#collecting-test-data), we discussed how to collect LLM input and output for testing purposes. Since you can't think of every possible interaction in your agent, creating a setup where you can collect a whole conversation with tool interactions to analyze it later to build test scenarios is a great idea.

As tempting as it sounds, I don't collect this data on production environments. It's too risky because of privacy and security concerns. However, if someone reports a problem, I contact them and ask them to redo the scenario on a specific test environment where we can collect data. One of two things can happen:

1. They reproduce the problem on the test environment, and you have test data to work with.
2. They don't reproduce the problem and learn how to solve their scenario differently.

If you sit down next to the person who experienced the problem while they reproduced it on your test environment, you can help them work around the problem or find out how they got into trouble. Collaborating on an issue is often a great experience for you and the user, so I highly recommend it.

## Security practices when working with agents

As fun and powerful as agents are, considerable risks are involved in building agents. The lack of control over the agent's behavior is annoying when you try to test the agent, but it proves a real security problem when you want to run it in production.

Therefore, I recommend taking extra precautions in addition to the security measures I covered in the various chapters of this book.

### Limit access to your systems

Ensure your agent can't perform tasks that it should be able to. An agent should have limited permissions when calling internal API endpoints in an organization. Let the agent call internal API endpoints on behalf of the user. If a user has permission to a system, then it's okay for the agent to access the same information as the user. In all other cases, you don't want to accidentally expose information to the user that they shouldn't have access to.

It's also important to let the user approve dangerous actions before they're performed. You can use a tool call filter to filter calls to dangerous tools. In a terminal application, this is easily done by asking for input from the user. If you're working in a web environment, this gets more complicated.

You can use SignalR in a web frontend to solve this. In SignalR, you can have the server call logic on the client as long as the client is connected to the hub through a secure connection. Usually, this is used to send notifications, but you can also ask the user for input. When the user provides positive confirmation, you can continue the tool call.

If you plan to allow your agent to run shell commands, I recommend doing so inside a container. You can use a Docker container for this purpose. When correctly configured, you can let the agent run commands inside the container without it being able to escape the container's boundaries. An interesting article on this topic is in the [Semantic Kernel documentation](https://learn.microsoft.com/en-us/azure/container-apps/sessions-tutorial-semantic-kernel).

### Be aware of data poisoning

Depending on the type of agent you're building, you may have tools that download data from systems you can't fully trust. For example, if you're building an agent that researches content online, the agent may pull in content from the Internet that contains dangerous content.

The content the agent reads from the internet is kept in the context transmitted to the LLM. If that content contains text that is, in fact, a prompt injection attack, you'll end up with an almost invisible prompt injection attack. The user may not even notice that his session is being poisoned.

I recommend using content filters to validate content coming back from search and download tools before letting that data enter the LLM's context. In [#s](#llmops-user-safety), we covered using content safety tools for general user safety. These tools work great for this scenario, too.

### Limit the autonomy of the agents

Agents can run in circles quite frequently, like a dog chasing its tail. This effect gets worse when you combine multiple agents. Limiting the number of iterations an agent can take before terminating it or involving human oversight is a good idea.

At the time of writing, Semantic Kernel's problem is that you can't limit
the number of tool calls before stopping the agent. You must write a tool invocation filter to limit the number of tool calls you make. I hope they will fix that in a future version, as it is an essential protection against abuse of the agent.

## Summary

In this final chapter, we looked at agents and their value to LLM-based applications over other patterns we covered in previous chapters. We learned that agents can be helpful when you want to solve problems that require flexible planning and in cases where we can only define a set of rules and a goal. Testing agents is challenging and requires strategies different from regular software and basic prompts. Finally, we covered that it is essential to constrain your agents so you don't run into high credit card bills or broken systems.

If you want to learn more about agents, I recommend reading the paper [AI Agents vs Agentic-AI: A Conceptual Taxonomy, Applications and Challenges](https://arxiv.org/abs/2505.10468). It provides a great explanation of the taxonomy around agents and gives you a glimpse of what the future may hold for agentic AI.

[CHAPTER_SAMPLE_CODE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-11/csharp/