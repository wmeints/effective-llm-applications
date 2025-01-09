# Preface

## Why I wrote this book

This book came about because I needed a way to copy my knowledge to my colleagues.

I've been using Large Language Models (LLMs) for quite a while now and discovered how to
use them to build intelligent applications by failing a lot. I don't think I can protect
anyone from making their own mistakes, but I can show what I learned and explain what
worked for me.

## Who this book is for

I wrote this book for developers who want to use an LLM in their application to solve
specific challenges that can't be solved with normal program logic. All examples in the
book are in C# because I think you can find plenty of Python examples online but not
enough in a more enterprise-oriented language like C#.

## What this book covers

- **[#f](#understanding-llms)**, introduces large language models
  (LLMs) through the lens of my personal experience, explaining what they are, their
  capabilities, and why they're transforming how we build applications. This
  foundational chapter is essential for understanding the core concepts, and terminology
  that will help you make informed decisions about using LLMs in your projects, even if
  you're completely new to working with these models.

- **[#f](#essential-llmops-knowledge)**, explains the basic operations involved in
  building and hosting LLM-based applications. This chapter provides important
  information you'll need to know to appreciate the design patterns that we cover in
  later chapters.

- **[#f](#getting-started-with-semantic-kernel)**, covers Semantic Kernel, a framework
  from Microsoft that simplifies building applications with LLMs by providing tools,
  abstractions, and patterns for integrating AI capabilities into your projects. This
  chapter walks you through setting up your development environment and creating your
  first Semantic Kernel project, giving you hands-on experience with the framework we'll
  build upon throughout the rest of the book.

- **Chapter 4, "The Art of Prompt Engineering"**, dives into the art of prompt engineering,
  teaching you how to craft effective prompts that get reliable, high-quality responses
  from LLMs by covering key concepts like temperature, templates, and advanced techniques.
  This chapter is crucial because the ability to write good prompts is the foundation of
  working with LLMs - without this skill, you'll struggle to get consistent results no
  matter what frameworks or patterns you use.

- **Chapter 5, "Enhancing LLMs With Tools"**, explores how to enhance LLMs by giving them
  access to external tools and skills, showing you how to build custom tools, integrate
  APIs, and manage memory and context to create more capable AI systems. LLMs become
  dramatically more powerful when they can interact with external tools and data -
  understanding these patterns will let you build AI assistants that can take actual
  actions and work with real-world data.

- **Chapter 6, "Retrieval Augmented Generation (RAG)"**, shows you how to improve your LLM
  applications by grounding the responses in your own data using Retrieval Augmented
  Generation (RAG), going from basic vector embeddings all the way to building a working
  domain-specific chatbot. If you want your LLM applications to give accurate responses
  based on your company's documents, internal knowledge, or any specific dataset, this
  chapter is essential since it teaches you the complete RAG architecture from
  preprocessing to efficient retrieval and context integration.

- **Chapter 7, "Working with structured output"**, teaches you how to get structured output
  from LLMs, enabling you to reliably integrate AI responses into your applications.
  This chapter helps you bridge the gap between AI capabilities and your existing
  codebase. Mastering this skill will let you build reliable AI features that work
  seamlessly with the other components in your applications.

- **Chapter 8, "Prompt Chaining Workflows"**, delves into the essential pattern of prompt
  chaining, showing you how to break down complex tasks into manageable sequences of
  prompts that build upon each other's outputs to refine the output.

- **Chapter 9, "Intelligent Request Routing Workflows"**, teaches you how to build systems
  that intelligently route requests through a workflow by applying a reasoning prompt
  with logic. This chapter helps you build more flexible workflows using LLMs.

- In **Chapter 10, "LLM Orchestration Workflows"**, you'll learn how to design and implement
  robust LLM orchestration workflows that can handle complex document processing tasks
  at scale, including key patterns for state handling, and error recovery. This chapter
  is essential for anyone building production-grade LLM applications that need to
  reliably process large volumes of requests while maintaining observability and
  performance - skills that become critical as your LLM applications grow beyond simple
  single-request implementations.

- **Chapter 11, "Artist and Critic Workflows"**, explores the powerful "artist and critic"
  pattern where LLMs are used both to generate content and to critically evaluate and
  refine that content through iterative feedback loops. This chapter helps you
  understand how you can build self-improving workflows for content creation and code
  generation or any other scenario where output quality is important.

- **Chapter 12, "Building Basic Agents"**, introduces the fundamentals of building
  autonomous agents with LLMs. It covers the core loop of observation, reasoning, and
  action along with essential patterns for managing agent memory and state. This chapter
  helps you understand the role of agents and when the use an agent in your application.

- **Chapter 13, "Building Multi-Agent Teams"**, tackles the advanced topic of building
  multi-agent teams, exploring how to coordinate multiple AI agents working together
  with defined roles, communication protocols, and strategies for solving complex tasks.
  Although the multi-agent pattern is still in its infancy, reading this chapter will
  help you understand where current generative AI research is moving towards.

## System requirements

This book contains samples in C# to demonstrate various patterns. Please make sure you
have the following tools available on your system:

- [.NET SDK 9.0 or Higher](https://dot.net/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) or a similar containerization tool.
- [Visual Studio Code](https://code.visualstudio.com) or a similar code editing tool.

## Running the sample code in this book

This book comes with samples on GitHub. You can find the repository at
[https://github.com/wmeints/effective-llm-applications/](https://github.com/wmeints/effective-llm-applications/).
You'll find the samples in the `samples` folder in the repository.

Each sample comes with a README.md file that explains how to run the sample and the
requirements for the sample. I've made sure you can run the sample with either Azure
OpenAI or the regular OpenAI service when you follow the instrucitons in the included
README.md file.

As an added bonus, I've made sure to include samples for Java and Python too. So if you
are reading this book but don't work with C#, you can still try out the samples.

## Feedback and issues

If you have any feedback or issues with the book, please create an issue in the [GitHub
repository][BOOK_REPO]. I'll do my best to address the issue as soon as possible.

[BOOK_REPO]: https://github.com/wmeints/effective-llm-applications/
