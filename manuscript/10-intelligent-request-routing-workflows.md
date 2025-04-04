{#intelligent-request-routing}
# Intelligent Request Routing Workflows

In the previous chapter we covered prompt chaining and how this helps get more accurate responses from an LLM. In this chapter we're exploring how to use intelligent request routing to build more complex LLM interactions.

We're going to use the LLM as an intent-based router for chat applications and workflows. By the end of this chapter you know wow to write a prompt that can classify input and integrate that prompt in a chat application and a workflow.

We'll cover the following topics:

- Why use intelligent routing in a workflow
- Building an intelligent request routing workflow in Semantic Kernel
- Using intelligent request routing in a chat scenario in Semantic Kernel

## Why use intelligent routing in a workflow

While most LLM-based workflows will focus on using prompts to process content you can use LLMs in other ways too. For example, when you need to decide what sort of issue the user is submitting to your helpdesk so you can route that issue to the correct product team.

Another use case where intelligent routing can be useful is when building a chat interface for a eCommerce website. Instead of adding multiple chatbots, one for each specific area in your website, you can have one chat interface that has an intelligent router behind it to route requests to specific chat bots.

You can only use a limited amount of tools within the scope of one chat bot. Also, remember that the more focused the prompt the better the results are. This goes for system instructions too. If you have generic instructions that cover multiple functional areas in your website you'll find that the chat bot isn't going to answer as well as it would when focusing it on customer service or the product catalog.

The intelligent request router can help you direct requests towards a specific chat focused on what the customer wants to talk about.

For the remainder of this chapter we'll focus on designing and building two types of intelligent request routing solutions:

- Routing Github issues to specific functional areas
- A chat solution with multiple "agents" for product catalog information and customer service

We'll start by looking at routing issues to specific functional teams using Semantic Kernel.

## Building an intelligent request routing workflow in Semantic Kernel

## Using intelligent request routing in a chat scenario in Semantic Kernel

## Summary
