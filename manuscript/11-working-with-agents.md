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

### The scientific definition of an agent

The more scientific approach to agents goes back much farther than using an LLM. You can find agents in a subset of the AI field called reinforcement learning that teaches computers to perform a task by letting an agent interact with an environment and teaching it based on feedback given by the environment.

Before the use of agents in reinforcement learning there were symbolic reasoning agents. Symbolic reasoning is a form of AI where we give the computer symbols and logical rules and then let it figure out how to solve a problem. The symbols can be values retrieved from a database, and the rules are invented by humans.

Siri, the well-known agent from Apple is a sample of a symbolic agent. It uses speech to text to translate audio to text, extracts entities and intent as symbols and then uses logical rules to perform tasks like setting a timer or sending an email.

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

It turns out that reinforcement learning agents and LLM-based agents have some overlap. Depending on what task you're trying to complete with an agent you may have more success using an LLM because it is much better trained at completing certain tasks where you would otherwise have to train a completely new neural network.

### The structure of an LLM-based agent

In the previous section we learned that reinforcement learning and LLM-based agents are similar in how you can approach them from a scientific point of view. However, I found that the scientific approach doesn't help much in programming an agent. There's a better representation of an LLM-based agent from a programming perspective as shown in [#s](#agent-from-programming-perspective)

{#agent-from-programming-perspective}
![An agent from a programming perspective](agent-from-programming-perspective.png)

In the context of Semantic Kernel and LLMs in general, an agent is a component in an application that has access to a list of tools and interacts with an LLM. The agent receives an initial set of instructions that sets the goal and kicks off the agent process. The agent can use memory to keep track of previous actions to help it achieve the goal set in the instructions.

In [#s](...) we covered how Semantic Kernel implements a loop to make it possible to call multiple tools when you submit a prompt to the kernel. This loop is the core of how an agent works in Semantic Kernel. The workflow of an agent is shown in [#s](#agent-processing-loop).

{#agent-processing-loop}
![The agent processing loop]

The loop starts with a set of instructions and an initial prompt indicating the goal we want to achieve. With this initial set of instructions, the agent calls the LLM and receives a response. When the agent receives a tool call, it invokes the tool and stores the response in its internal memory. After completing a tool call, the agent moves to the beginning of the loop and calls the LLM again with the output of the tool and the chat history. If the response is a regular chat message, the loop stops.

As you may remember from [#s](#enhancing-llms-with-tools) we can implement tools for the agent as C# functions or Semantic Kernel plugins. 

By using tools you can give agents access to information through the use of a vector index. You're essentially including the RAG design pattern we discussed in [#s](#retrieval-augmented-generation).

You can also use existing MCP (Model Context Protocol) servers to integrate your agent with websites like Github or tools like Google Drive. We haven't covered MCP in this book but you can learn more about this protocol [in the documentation](https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/adding-mcp-plugins?pivots=programming-language-csharp).

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

This prompt does two things: First, it gives the agent a step-by-step approach to the problem. Next, it tells it to read the instructions and follow them step by step. This is called [a chain-of-thought prompt](https://www.promptingguide.ai/techniques/cot). The LLM can't think step by step, but it can emulate this behavior by producing output tokens that look like they're a chain of thought. And because the LLM produces a lot more detailed information it is capable of using that information to choose the right tool to cool at the end of the sequence.

## When to use an agent

## Building an agent with Semantic Kernel

## Building multi-agent systems with Semantic Kernel

## Testing agents and multi-agent systems

## Security practices when working with agents

## Summary

In this final chapter we looked at agents and what value they bring to LLM-based applications over other patterns that we covered in previous chapters. We learned that agents can be helpful when you want to solve problems that require flexible planning and in cases where we can only define a set of rules and a goal. We also looked at building an agent and combining it with other agents to solve more complex problems in your LLM-based applications. We discovered that testing agents is challenging and requires strategies that are different from regular software and basic prompts. Finally, we covered that it is important to constrain your agents so you don't run into high creditcard bills or broken systems.

If you're interested in learning more about agents, I recommend reading the paper [AI Agents vs Agentic-AI: A Conceptual Taxonomy, Applications and Challenges](https://arxiv.org/abs/2505.10468). It provides a great explanation of the taxonomy around agents and gives you a glimpse of what the future may hold for agentic AI.