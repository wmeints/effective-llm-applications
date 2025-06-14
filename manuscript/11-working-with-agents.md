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
![The agent processing loop]

The loop starts with a set of instructions and an initial prompt indicating the goal we want to achieve. With this initial set of instructions, the agent calls the LLM and receives a response. When the agent receives a tool call, it invokes the tool and stores the response in its internal memory. After completing a tool call, the agent moves to the beginning of the loop and calls the LLM again with the output of the tool and the chat history. If the response is a regular chat message, the loop stops.

As you may remember from [#s](#enhancing-llms-with-tools) we can implement tools for the agent as C# functions or Semantic Kernel plugins.

By using tools you can give agents access to information through the use of a vector index. You're essentially including the RAG design pattern we discussed in [#s](#retrieval-augmented-generation).

You can also use existing MCP (Model Context Protocol) servers to integrate your agent with websites like Github or tools like Google Drive. We haven't covered MCP in this book but you can learn more about this protocol [in the documentation](https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/adding-mcp-plugins?pivots=programming-language-csharp).

The memory component of the agent can be used as a key/value store to keep information for the longer term. You can use a typical database with a vector index for this. Semantic Kernel has support for many variations since you can use the same storage for implementing the RAG pattern.

Before we start implementing an agent with Semantic Kernel it's important to understand the relationship between prompts and agents because prompts play an essential role in how agents work.

### The role of instructions in an LLM-based agent

LLM-based agents work primarily off prompts and the chat history. So it's important to build a prompt that suitable for an agent.

Remember from [#s](#the-art-and-nonsense-of-prompt-engineering) that there are 5 things important when it comes to constructing a prompt:

1. Provide clear direction
2. Specify what you want as the output
3. Add context and samples to the prompt
4. Keep the prompt focused on one task
5. Tune the output with hyperparameters

These principles still apply to building instructions for an agent, but you need to tweak each of these five aspects a bit to make them suitable for an agent.

When you provide direction to an agent, make sure to promote producing a chain of thought. For example, you can give the agent a step-by-step plan as the following prompt shows:

```text
TODO: Prompt
```

This prompt does two things:

- First, it gives the agent a step-by-step approach to the problem. If you can, you should definitely include a plan in your intruction to provide clarity about what the agent should do. If you can't use a fixed plan, it's a great idea to instruct the agent to start by setting up a plan before executing it. Being explicit about what you expect of the agent greatly improves the results.
- Next, the prompt tells the agent to read the plan and follow them step by step. This is called [a chain-of-thought prompt](https://www.promptingguide.ai/techniques/cot). The LLM can't think step by step, but it can emulate this behavior by producing output tokens that look like they're a chain of thought. And because the LLM produces a lot more detailed information that steers the attention mechanism in the right direction.

### Which model to use for building agents

We covered different LLM providers in [#s](#understanding-llms), there are a lot to choose from. I don't think I can list all of them anymore by the time this book is a year old. And it doesn't get much easier to choose a model when it comes to agents.

Agents work better with LLMs that were trained on chain-of-thought tasks and other agentic tasks. This sounds logical, but how do you know if your agent is going to work with a particular model?

The best option is to use a reasoning model for your agent. For example, GPT-4.1 and the Orion series from OpenAI (o1, o2, o3, etc.) are trained specifically on agentic tasks. But you should also consider using the LLMs from Anthropic. The models from Anthropic perform really well in tools like Github Copilot Agent Mode.

Note that reasoning models are more expensive so you may want to consider implementing the patterns from [#s](#intelligent-request-routing) if you want to save some money.

## When to use an agent

A lot of people will tell you that you should use an agent. But an agent is only good at a subset of problems that you will encounter. There's a reason why I include so many other patterns in this book and only cover agents last. Agents are new, and we don't know what limitations we'll run in exactly. Also, agents are more expensive than a single prompt.

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

Github Copilot has Agent Mode that's an implementation of an agent. It can access your code base and write files just to name a few tools it can use. Similar to Github Copilot I recommend looking at Cursor and Windsurf. Both offer excellent implementations of agents. All these agents work with the same principle we covered earlier in the chapter, so while complex, it is possible to write a software engineering agent yourself.

Another great use case for agents is content creation. Writing a blog posts requires researching and refining the content in multiple passes. You can do this yourself, or you can write an agent to do this for you. I'll leave this one up to you as this is one of those scenarios that's a great exercise after you've finished this book.

Another more IT related use case is an agent that can resolve issues with your cloud computing environment. Imagine that an alert is generated by an application after it shutdown due to a problem with storage or a database. An agent could pick up this alert, read the logs from your environment and then use the APIs in your cloud environment to fix the problem for you. Of course this is one of the more dangerous use cases of an agent and you should probably take caution when you want to implement this scenario.

{#building-an-agent}
## Building an agent with Semantic Kernel

Now that we have covered what an agent is, when to use one, and some sample cases, it's time to take a look at how Semantic Kernel help you build agents.

We'll build an agent that can write a feature file to help you turn a generic description into a set of Behavior-driven development (BDD) scenarios. I built this agent as an experiment to understand how to take the idea of a coding agent in the direction of requirements engineering.

You can follow along with the sample with [the code][CHAPTER_SAMPLE_CODE] included for this chapter. I'll cover the key steps needed to create an agent here in the book.

### Setting up an agent project

While you can add an agent to console applications, desktop application and server application I recommend building agents as part of an ASP.NET core application so you can have long-running processes separately from where your user is working.

Depending on the complexity of a task it can take a very long time for an agent to complete work. Running an agent task on your laptop generally isn't a great experience since the agent stops working when you close your laptop.

Also, distributing fixes to many machines when you're experimenting with agents is a bad idea. It's better to split the frontend from the backend logic.

You'll need an extra package in your ASP.NET Core application to build an agent with Semantic Kernel. The core agents package is called `Microsoft.SemanticKernel.Agents.Core` and offers the basic abstractions to build agents.

Make sure your application includes a chat completion model and it's also a good idea to configure an embedding model in case you want to semantically search for information.
You can learn more about configuring chat completion and embedding models in [#s](#setting-up-semantic-kernel).

### Creating an agent class

### Invoking the agent with a description of a feature

### Connecting the agent to other content in your project

### Getting structured output from the agent

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
## Useful multi-agent patterns

In [#s](#building-multi-agent-systems) we used the competitive agents pattern to review feature files after they were written by a requirements engineering agent. There are other patterns that are useful for other use cases.

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