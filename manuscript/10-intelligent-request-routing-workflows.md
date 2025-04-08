{#intelligent-request-routing}
# Intelligent Request Routing Workflows

In the previous chapter we covered prompt chaining and how this helps get more accurate responses from an LLM. In this chapter we're exploring how to use intelligent request routing to build more complex LLM interactions.

We're going to use the LLM as an intent-based router for chat applications and workflows. By the end of this chapter you know wow to write a prompt that can classify input and integrate that prompt in a chat application and a workflow.

We'll cover the following topics:

- Why use intelligent routing in a workflow
- Building an intelligent request routing workflow in Semantic Kernel
- Using intelligent request routing in a chat scenario in Semantic Kernel

## Why use intelligent routing in a workflow

While most LLM-based workflows will focus on using prompts to process content you can use LLMs in other ways too. You can use an LLM to route requests based on their content.

This sounds rather abstract from this one sentence, so let me explain what this looks like in practice.

Imagine you're building a chatbot solution for the website for a health insurance company. You need people to be able to ask questions about policies offered by the insurance company, but you also want people to submit insurance claims through the website. Both use cases require a different approach and a different set of tools for the chatbot. You don't want the chatbot to be able to submit an insurance claim when someone asks about the details of a policy. If you were to build a single chatbot experience offering both insurance policy information and a tool to submit claims, the chatbot is easily confused. To solve this, you can use an LLM to decide based on the first question which department the user wants to talk to and send the question through the right combination of tools and prompts.

Another use case where intelligent routing can be useful is when you need to sort incoming feedback from users into categories with specific issue templates on Github. You can give the LLM the original issue text along with a set of examples to teach the LLM how to sort the issues. Although different from routing user questions to the right bot, issue sorting is still a routing use case.

Finally, intelligent routing can be used to optimize costs of your LLM. You can use a small language model as a router to determine whether the question is a simple or complex question and route the question to either a smaller language model or a larger language model depending on the complexity. You do incur an extra call to a language model, but it could help you shave off quite a sizable amount off your next creditcard bill.

For the remainder of this chapter we'll focus on designing and building two types of intelligent request routing solutions:

- Routing Github issues to specific functional areas in a Github repository.
- A chat solution with multiple "agents" for product catalog information and customer service.

We'll start by looking at routing Github issues to specific functional areas.

## Building an intelligent request routing workflow

## Using intelligent request routing in a chat scenario

## Summary
