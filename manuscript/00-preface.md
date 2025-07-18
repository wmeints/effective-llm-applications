{sample: true}
# Preface

## Why I wrote this book

This book came about because I needed a way to copy my knowledge to my colleagues. I almost built a custom AI agent to answer all my colleagues' questions, but it felt like giving McDonald's food to people who deserve healthy food for long-term happiness. So instead of giving quick, hallucinated together answers, I decided that my colleagues and you, as a reader, are better off with a book. I'm also gaining from this, because I structure my thoughts and get a chance to go back and improve myself.

I've been using Large Language Models (LLMs) for quite a while now and discovered how to use them to build intelligent applications by failing a lot. I can't protect anyone from making their own mistakes, but I can show what I learned and explain what worked for me.

It's challenging to write a book in this age of AI and the internet because the book is old when it hits the printing press. To combat this, I took a different approach: I'm writing and releasing it in chunks. Some chunks are lovely to read when they hit your mailbox, while others may be slightly incomplete. Please forgive my writing style and enjoy the fast-paced release cycle of the book.

## Who this book is for

I wrote this book for developers who want to use an LLM in their application to solve
specific challenges that can't be solved with normal program logic. All examples in the
book are in C# because I think you can find plenty of Python examples online but not
enough in a more enterprise-oriented language like C#.

## What this book covers

- **[#s, #t](#understanding-llms)**, introduces large language models (LLMs) through the
  lens of my personal experience, explaining what they are, their capabilities, and why
  they're transforming how we build applications. This foundational chapter is essential
  for understanding the core concepts, and terminology that will help you make informed
  decisions about using LLMs in your projects, even if you're completely new to working
  with these models.

- **[#s, #t](#essential-llmops-knowledge)**, explains the basic operations involved in
  building and hosting LLM-based applications. This chapter provides important
  information you'll need to know to appreciate the design patterns that we cover in
  later chapters.

- **[#s, #t](#getting-started-with-semantic-kernel)**, covers Semantic Kernel, a
  framework from Microsoft that simplifies building applications with LLMs by providing
  tools, abstractions, and patterns for integrating AI capabilities into your projects.
  This chapter walks you through setting up your development environment and creating
  your first Semantic Kernel project, giving you hands-on experience with the framework
  we'll build upon throughout the rest of the book.

- **[#s, #t](#the-art-and-nonsense-of-prompt-engineering)**, dives into the art of
  prompt engineering, teaching you how to craft effective prompts that get reliable,
  high-quality responses from LLMs by covering key concepts like temperature, templates,
  and advanced techniques. This chapter is crucial because the ability to write good
  prompts is the foundation of working with LLMs - without this skill, you'll struggle
  to get consistent results no matter what frameworks or patterns you use.

- **[#s, #t](#prompt-testing-and-monitoring)**, takes you through the steps needed to
  test and monitor interactions with LLMs in your application. This chapter helps you
  understand the importance of testing and monitoring prompts to make sure your
  application remains operational in the long run.

- **[#s, #t](#enhancing-llms-with-tools)**, explores how to enhance LLMs by giving them
  access to external tools, showing you how to build custom tools, integrate APIs, and
  manage memory and context to create more capable AI systems. LLMs become dramatically
  more powerful when they can interact with external tools and data - understanding
  these patterns will let you build AI assistants that can take actual actions and work
  with real-world data.

- **[#s, #t](#retrieval-augmented-generation)**, shows you how to improve your LLM
  applications by grounding the responses in your own data using Retrieval Augmented
  Generation (RAG), going from basic vector embeddings all the way to building a working
  domain-specific chatbot. If you want your LLM applications to give accurate responses
  based on your company's documents, internal knowledge, or any specific dataset, this
  chapter is essential since it teaches you the complete RAG architecture from
  preprocessing to efficient retrieval and context integration.

- **[#s, #t](#working-with-structured-output)**, teaches you how to get structured output from LLMs, enabling you to reliably integrate AI responses into your applications. This chapter helps you bridge the gap between AI capabilities and your existing codebase. Mastering this skill will let you build reliable AI features that work seamlessly with the other components in your applications.

- **[#s, #t](#prompt-chaining-workflows)**, delves into the essential pattern of prompt chaining, showing you how to break down complex tasks into manageable sequences of prompts that build upon each other's outputs to refine the output.

- **[#s, #t](#intelligent-request-routing)**, teaches you how to build systems that intelligently route requests through a workflow by applying a reasoning prompt with logic. This chapter helps you build more flexible workflows using LLMs.

- **[#s, #t](#working-with-agents)**, helps you understand agents and how to build them with Semantic Kernel. We'll also look at combining multiple agents to solve even more complex problems. Although agents and multi-agent systems are relatively new, this chapter will help you build a solid foundation for the future whatever it may look like with agents.

## System requirements

This book contains samples in C# to demonstrate various patterns. Please make sure you
have the following tools available on your system:

- [.NET SDK 9.0 or Higher](https://dot.net/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) or a similar containerization tool.
- [Visual Studio Code](https://code.visualstudio.com) or a similar code editing tool.
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) for working with Azure OpenAI.

## Running the sample code in this book

This book comes with samples on GitHub. You can find the repository at
[https://github.com/wmeints/effective-llm-applications/](https://github.com/wmeints/effective-llm-applications/).
You'll find the samples in the `samples` folder in the repository.

Each sample comes with a README.md file that explains how to run the sample and the
requirements for the sample. I've made sure you can run the sample with either Azure
OpenAI or the regular OpenAI service when you follow the instructions in the included
README.md file.

As an added bonus, I've made sure to include samples for Java and Python too. So if you
are reading this book but don't work with C#, you can still try out the samples.

## Feedback and issues

If you have any feedback or issues with the book, please create an issue in the [GitHub
repository][BOOK_REPO]. I'll do my best to address the issue as soon as possible.

[BOOK_REPO]: https://github.com/wmeints/effective-llm-applications/
