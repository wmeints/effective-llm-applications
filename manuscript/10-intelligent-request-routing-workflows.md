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

While all of these products are great, they have one problem in common: You need to add yet another layer of moving parts on top of Semantic Kernel. That's why Semantic Kernel came up with the idea of providing a workflow engine out of the box.

> The Semantic Kernel process framework is still in preview! This means that anything can change, and if you want to run in production with lower maintenance costs, you should skip it and use one of the other workflow runtimes I mentioned.

The process framework in Semantic Kernel has two core components:

- Steps allow you to define logic for a single step in a process.
- Processes are used to combine steps into a workflow. A process controls the data flow between steps and what steps are connected.

Semantic Kernel process framework relies on a runtime to host the processes. The process you define with the steps and process components need to be hosted on one of the available runtimes to be functional. Currently, Semantic Kernel has three options for running workflows:

- **The local runtime** provides no resiliency or scaling options and is meant for debugging workflows locally.
- **The [Orleans][ORLEANS] runtime** provides resiliency by using Orleans to run the workflow. It maps the steps to individual runtime units called grains that can be hosted on multiple machines.
- **The [Dapr][DAPR] runtime** is an alternative to Orleans and better suited for Kubernetes-based hosting scenarios. Dapr offers similar scaling and resiliency options as Orleans.

### Installing the process framework

### Writing the process steps

### Wiring up the process

### Visualizing the process using mermaid diagrams

## Making decisions in a Semantic Kernel-based workflow

### Using events to route data through the workflow

### Using state in workflow steps to base your decisions on

## Building an intelligent request routing workflow

## Summary

[ORLEANS]: https://learn.microsoft.com/en-us/dotnet/orleans/overview
[DAPR]: https://dapr.io/
[CAMUNDA]: https://camunda.com/
[PREFECT]: https://www.prefect.io/