# Understanding Large Language Models

I remember the first time I realized just how useful large language models could
be. Like many developers, I initially tried ChatGPT almost as a joke – I didn't expect
it to work as well as it did. I had this piece of code that needed unit tests, and on a
whim, I copy-pasted it into ChatGPT and asked it to write the tests for me. What
happened next surprised me. Within seconds, I got back working code that needed only
minor tweaks. The overall structure was spot-on, and it saved me 30 minutes of work.
While 30 minutes doesn't sound like a lot of time saved, it does add up and this is
the worst-case scenario at a time when I didn't quite understand how an LLM worked.

Here's the thing – that interaction looked like magic, but really wasn't. It was the
result of careful engineering, good quality prompt design, and an understanding of what
LLMs can (and can't) do well. That's what this chapter is all about: demystifying these
powerful tools so you can put them to work in your own applications.

You've probably heard the buzz around LLMs. Maybe you've played with ChatGPT, Claude,
or other AI assistants. Maybe you're skeptical about the hype, or maybe you're excited
about the possibilities. Whatever your starting point, this chapter will give you the
solid foundation you need to move beyond simple interactions and start building real
applications with LLMs.

We'll start with the basics – what LLMs are and why they matter – but we won't get
bogged down in the theoretical. Instead, we'll focus on the practical: how these models
work in the real world, what they're good at, and where they fall short. I'll share
stories from my own journey with LLMs, including the failures and breakthroughs that
taught me the most.

By the end of this chapter, you'll understand:

- The key concepts and terminology you need to work effectively with LLMs
- How to evaluate different models and choose the right one for your needs
- Practical considerations for building production applications
- Real-world use cases and applications that go beyond the obvious

Let's dive in and explore the fascinating world of large language models – not as
magical black boxes, but as practical tools we can understand, control, and use to
solve real problems.

One quick note before we begin: throughout this chapter, I'll be sharing code examples
and practical tips based on my experience. While the specific models and APIs might
evolve, the underlying principles and patterns will remain relevant. I encourage you to
run the examples yourself and experiment with different approaches. That's how you'll
truly internalize these concepts and make them your own.

## What are LLMs and why they matter

Let's start with the basics. A Large Language Model (LLM) is a neural network trained on
massive amounts of data to understand and generate human-like text.

### A brief history

Language models aren't new at all. We've been trying to understand human language for
a very long time. Early attempts involved pattern matching using regular expressions.
Probably well-known for the fact that you only write them once and then never change
them because they're hard to understand and quite inflexible.

Later we introduced clunky chatbots that were capable of matching patterns to understand
intent using machine learning. These bots still had fixed responses and were unable to
understand intent when the input was even slightly off.

The real game-changers started appearing around 2017. Before that we had language models
that were rather specialized and limited in performance. Don't get me wrong, tools like
[SpaCy](https://spacy.io/) were amazing at the time for what we could do.

### The breakthrough moment

Everything changed with the introduction of the transformer neural network architecture.
The paper ["Attention is all you need"](https://arxiv.org/abs/1706.03762) demonstrated
a completely new way of processing language. I won't bore you with the technical
details, but here is why it matters: Previous models would process text word-by-word
and weren't able to look at the context as a whole. The transformer architecture instead
looks at the entire context at once, understanding the relationship between words
regardless of their position.

Think about how you understand this sentence: "The developer copy-pasted the code into
ChatGPT to generate unit-tests for it.". You automatically know "it" refers to the code,
not the tool ChatGPT. Transformers can make these connections too, and they can do it at
scale.

### How LLMs Work

At their core, LLMs predict what comes next based on what they've seen before. When you
give an LLM a prompt, it's not searching through a database for answers. Instead, it's
using its understanding of patterns to generate responses word by word.

Here's a simplified view of what happens:

1. Your input gets broken down into tokens (pieces of words or characters)
2. The model looks at these tokens and their relationships using attention mechanisms
3. It predicts the most likely next token based on its training
4. This process repeats until it generates a complete response

The magic happens in how these models handle context. The attention mechanism helps
predict the combined context of input and (the to be generated) output. When you feed
tokens into the model, the attention mechanism state is updated with the current
understanding of the context that we're working on. Based on this information the model
can now more reliably predict the next likely token.

### Why This Matters for Software Development

You might be thinking, "Okay, cool technology, but why should I care as a developer?"
Here's why:

First, LLMs are changing how we write code. They're not just glorified autocomplete –
they can understand intent. When I'm working on a new feature, I can describe what I
want in plain English, and an LLM can help me scaffold the code, suggest test cases,
or point out potential issues.

So even if you're not building AI applications yourself, it will change how your tools work
and how quickly and effectively you can write code.

Second, and this is what this book is about, they're enabling new types of applications.
Think about all the tasks that were too complex or expensive to automate before because
they required understanding natural language. Now we can build applications that can:

- Generate human-like responses to customer inquiries
- Upgrade code bases from deprecated frameworks or old languages to more modern equivalents
- Translate raw input from field reports into a coherent and actionable management summary
- Help review a document that you wrote by providing useful suggestions to improve it

And we're just getting started!

### The Current Reality

Let's be clear though – LLMs aren't magic. They have real limitations. They can provide
you with some pretty surprising responses, the response can look right but contain
the wrong information, and you need to apply careful engineering to use LLMs in
production systems.

That's exactly why understanding how they work is so crucial. When you know what's
happening under the hood, you can:

- Design better prompts that get more reliable results
- Build safeguards against common failure modes
- Create hybrid systems that combine LLM capabilities with traditional programming
- Make informed decisions about when (and when not) to use LLMs

In the next sections, we'll dive deeper into these practical aspects. But first, let me
share some real examples from my own journey with LLMs – including the mistakes I made
so you don't have to repeat them.

## My journey with LLMs

My journey with LLMs started with a healthy dose of "this isn't going to work". Like
many of you didn't quite understand the impact of this new technology.

That unit testing experiment changed everything. Not because the code was perfect
(it wasn't), but because it showed me that LLMs could understand context and generate
meaningful output that was actually useful. This wasn't just pattern matching or
template filling – it was something qualitatively different.

### Early experiments

Of course, once I saw what LLMs could do I went a little overboard with ChatGPT. I tried
to use it for everything text related. I even messed up two blog posts on my website
with the negative feedback to show for it. It turns out, LLMs produce flavorless and
pretty mediocre content. They were trained to represent the average of what language
has to offer. And that's, well, pretty average and flavourless.

I tried using LLMs for coding too, as I am a developer. I wrote a full application using
only AI. And it's used in production today. But it was quite hard to get there. The LLM
quite frequently steered into the wall with weird layouts and useless unit-tests. I
haven't bothered measuring how quickly I built the application. But I have a feeling
that I was quicker, but I was also less satisfied with the result, because I feel that
writing code is a skill I'm proud of.

After learning about open source models I figured I should give that a try too. It
turned out to be very slow even on a beefy Intel Core i9 machine with a huge graphics
card. I quickly found out that you need an aweful lot of power to run an LLM on our
own machine and in the cloud.

There are plenty more experiences where I found the boundaries of what LLMs can do, but
let me finish with one final example. I tried using an LLM to upgrade program code from
a low-code solution to Typescript without human supervision. We quickly had to put in
human supervision, because the results were horrible!

### Key lessons learned

You might wonder, why am I telling you my experiences? There are three key lessons that
I want you to keep in mind while reading this book:

1. Be specific in what you ask from the LLM. Don't just ask for an article about LLMs,
   provide specific instructions.

2. Always review and understand the output of the LLM. Don't let your users use the output
   of the LLM unseen. The output will be wrong in all the weird ways you've never thought of.

3. Break big problems down into small problems. Instead of asking the LLM to perform
   10 steps, ask it for just one step. It will be easier for the LLM to perform and
   easier for you to debug.

4. Keep track of the context yourself and provide it in focused chunks. LLMs have
   limited input and output space, so they can't keep track of a complete book or
   even a blog post.

### Evolution of my understanding

After the initial rollercoaster ride with LLMs my understanding of them evolved. I stopped
seeing them as a silver bullet that can solve all my language related problems and started
seeing them as a powerful pattern-matching engine capable of transforming text.

One breakthrough moment was realizing that LLMs excel at clearly defined tasks that involve
matching a pattern in source text and transforming it into other text. If you can find a clear
pattern in the input yourself, and you can clearly define the target structure, it's
likely that an LLM is a good solution to your problem. The less clear the problem statement
is, the more issues you'll experience.

If you want to build an effective LLM application you'll want to provide a good balance
between human and machine. Human oversight is essential when using an LLM. Throughout
the rest of the book you will find that I'm using interaction patterns that promote
human oversight, because it's necessary and improves the experience by a lot.

### How I integrated LLMs into real projects

Clearly defined problems and human oversight are important when you view an LLM-based
application from a functional perspective. From a technical perspective, it's important
that you think about applying LLMs as a software engineering problem with an AI aspect
rather than a pure AI project.

Here are three reasons why you should use a software engineering approach:

- LLMs will behave better when you follow a structured approach. The more structure, the better.
- LLM behavior changes when providers push new versions of the models, automated testing is your friend.
- LLMs are slow, you will need to find ways to provide delayed responses to the user.
- The API endpoints of LLM providers often break, so you'll need to have solid error handling.

Throughout the rest of the book, I will share the strategies that I applied to help me get
the most out of my LLM-based applications.

### Moving forward

These experiences shaped how I approach LLM integration today. Instead of asking
"Can an LLM do this?", I ask "How can an LLM help with this specific aspect of the
problem?" This shift in perspective has been crucial for building practical,
reliable systems.

In the next section, we'll look at the current LLM landscape and how you can choose the
right models for your projects. We'll build on these lessons learned to help you make
informed decisions about which LLMs to use and how to use them effectively.

## The current LLM landscape

After sharing my journey, let's look at the tools available to you right now. The LLM
landscape is incredibly dynamic, with new models and capabilities being developed
frequently. I'll focus on what's practically useful for building applications.

### Overview of major LLM providers

It's good to know that the most powerful models right now are made available by a few
major LLM providers. There are open source options too, but these are generally less
powerful and require more engineering effort to use. Having said that, I highly recommend
you give them a try, because they offer other benefits you can't get from major LLM providers.

#### OpenAI

They've been at the forefront with their GPT series. What I love about OpenAI is their
API's reliability. The downside? They can be expensive for large-scale applications,
and their terms of service are quite restrictive. Also, it takes a while before newer
models become available through the API. And if they become available, it takes a while
before you can push a decent amount of tokens through the API, because the rate limits
are pretty low.

#### Anthropic

Their Claude model has impressed me with its ability to handle complex instructions and
longer context windows. I've found it particularly good at tasks requiring careful
reasoning, like code review and technical writing. The pricing is competitive
with OpenAI, and their terms of service are generally more business-friendly.

Anthropic is notorious for their rate limits. They often cap out and you'll end up with
random overload errors. It's important to have a good contract with them or plan for
a backup.

Sadly though, you can't use Anthropic directly with Semantic Kernel at this time, so
you'll not find much about the models from Anthropic in this book.

#### Meta

With LLaMA and its variants, Meta has shaken up the field by releasing powerful
open-source models. While you'll need more technical expertise to use these effectively,
they offer flexibility that proprietary models can't match.

LLaMA models are available through the major cloud providers, but you can also run them
on your own machine if you have a good enough GPU, for example a RTX4080 has no problem
running these models. If you want to give this a try, I recommend taking a look at
[Ollama](https://ollama.com/).

#### Google

Their PaLM API and Gemini models are interesting contenders that I personally haven't
had much experience with. However, if you're in the Google space, then they are a great
option and relatively easy to configure these days. I've found their documentation
particularly developer-friendly.

### Model comparison

Let's break down the model types that I've worked with to give you an understanding
of what to expect from each model type.

#### GPT series by OpenAI

- **GPT-4o:** The king of the hill when it comes to the GPT models. This model is powerful
  enough for most complex tasks. It's a general purpose model, useful for code as well
  as more generic text based tasks. This model supports generating images too.

- **GPT-4o mini:** The smaller brother of the GPT-4o model is a lot faster while still
  providing plenty of capabilities. I generally try this model first and only switch
  to the bigger and slower GPT-4o model when tests fail too frequently.

Recently, OpenAI started work on a new series of models called the Orion models. These
models focus on reasoning capabilties, and generally lack the general purpose features
that the GPT series has. Currently, there are two Orion-type models:

- **o1:** This is the biggest model so far from OpenAI. It's at the top of the
  benchmarks right now for biology, physics, and chemistry tasks. However, you're paying
  a premium for something that you can solve using the patterns in this book in combination
  with a less expensive model. Proceed with caution.

- **o1-mini:** Is the smaller version with capabilities somewhere between GPT-4o and o1.
  This model lacks a lot of the general knowledge included in GTP-4o and o1, so it will
  be useful for specific reasoning tasks but not much else.

You can find a full listing of the OpenAI models on [their website][OPENAI_MODELS].

#### Claude models by Anthropic

Claude models come in three varieties, just as with the GPT series, you can choose a smaller,
less capable, but faster and cheaper model depending on what your use-case is.

- **Haiku:** The fastest model from Anthropic. It lacks the capability to process
  images, but otherwise this model provides a very fast response with good quality
  reasoning capabilities.

- **Sonnet:** This is the GPT-4o equivalent from Anthropic. It's most useful for writing
  code and long form content. My personal experience with it that it is quite good at
  following more complex instructions, and it will stick to your style instructions
  quite well.

- **Opus:** This is the bigger brother of the Sonnet model. At the time of writing it
  hasn't been updated since 2023, and I expect it will be a while before they update it.
  It is slightly stronger than Sonnet at complex reasoning tasks.

As with the OpenAI models, I recommend testing your prompts and going with the cheaper
model first before attempting the same task with a bigger model. I've found that the
Sonnet model is overall your best option right now.

#### LLaMA and open-source alternatives

There are many open-source options available at the moment, and it is hard to keep up
with all the development progress. I've found that overall the commercial offerings
provide a great all round experience. I generally start with the commercial models when
I'm exploring a use case. When I understand a use case well enough, I will give an
open-source model a try to see whether we can lower the price point of our solution.

Having said that, I've had great experiences with these models:

- **LLaMA 3.3:** A general purpose model offered by Meta through [Hugging
  Face][HUGGINGFACE_LLAMA]. This model comes in two types, a pretrained variant that you
  can fine-tune yourself to perform specific tasks, and an instruction tuned model that
  Meta recommend to use for chat applications. But don't let yourself be limited by what
  Meta says, because the instruction tuned version is quite useful for non-chat purposes
  as well, as long as you have an instruction based use case.

- **Mistral:** The Mistral model by the identically named company is a very fast
  open-source LLM that is mostly used for chat purposes. This model is generally less
  capable than the LLaMA model variants, but its speed makes up for that. This model is
  also hosted on [HuggingFace][HUGGINGFACE_MISTRAL] and comes in many fine-tuned
  variants.

- **Gemma2:** was published by Google in February 2024 and is trained using a
  [teacher/student technique][HUGGINGFACE_GEMMA2]. The training
  technique itself looks very interesting, but I've found that Gemma2 isn't quite as
  good for many of the tasks that I worked on as the other models in the open-source
  space.

- **Phi 4:** Is a new model that was [introduced by Microsoft in december
  2024][PHI4_ANNOUNCEMENT]. It has a similar size compared to the Mistral and Gemma2
  models, 14 billion parameters but shows higher performance in [the
  benchmarks][PHI4_BENCHMARKS]. While this doesn't tell the whole story, I think it's
  worth trying this model when you are looking for a smaller open-source model.

### Making practical choices

It's important to remember that the most powerful model isn't always the best option. I
follow this general workflow when developing an LLM-based application that can be quite
helpful if you're just starting out:

1. First, I choose a general purpose model based on the cloud provider I'm working with.
   Most of the time my clients already have a contract with either Microsoft Azure or
   AWS to host their solutions. I use the existing environment to prototype the
   solution.

2. After the initial prototype, I'll look at data privacy requirements and intellectual
   property issues that the solution may have. Depending on these requirements I will
   determine the engineering effort and contractual effort we need to undertake to make
   the solution production viable. Usually, I'm the person who talks about the technical
   requirements while one of our legal people looks into contracts.

3. After the initial prototype and requirements gathering, we'll deploy the solution to
   production for a smaller group of people to gather initial user feedback and monitor
   for performance and costs. Based on this information I decide whether we should go
   back and optimize the solution or replace the model with something else.

4. Once the solution is optimized and running in production for the general population
   of an organization, I'll keep monitoring in production for sudden changes in quality
   of the responses or performance. We regularly test and update the models to improve
   the overall performance of the solution.

This general workflow has helped me quite well over the past few years to deploy
solutions in production. In the next section we'll dive into key concepts and
terminology to use LLMs effectively. Understanding these fundamentals will help you make
better decisions during the development process of your LLM-based applications.

## Key concepts and terminology

- Essential terminology
  - Tokens and tokenization
  - Context window
  - Temperature and sampling
  - Few-shot learning
  - Zero-shot capabilities
- Core concepts
  - Prompt engineering
  - Fine-tuning vs. prompt engineering
  - Embeddings
  - Vector similarity
  - Attention mechanisms

## Practical considerations for working with LLMs

### Cost management strategies

One of the key factors that influences LLM-based application is the cost management aspect of these solutions.
The world of LLM providers is constantly shifting. But there are two major directions right now for running an LLM:

- You can use a cloud based LLM through OpenAI, Microsoft, Google, or AWS with token-based pricing.
- Or you can host an open-source model yourself.

If you're looking for powerful general purpose models you'll have to talk to one of the big tech companies.
They offer token-based pricing which generally makes development quite cheap. The costs can stack up pretty high depending
on how your solution is used. Cloud has the advantage here because operational costs are lower, you don't have to maintain
the infrastructure as much as you do with an on-premises solution.

I've found that running open-source models on-premises has a higher upfront cost because of the hardware that you need
to purchase. And the write-off on the hardware is generally pretty high, because of the speed of innovation that happens
in the GPU space. The operational costs are also higher because you now need to maintain your own infrastructure.

I can only recommend running on-premises if you can host the model on end-user machines like Copilot+ laptops and Macbooks.
Lucky for us, there's the option to run many of the open-source models in the cloud. This offers the best of both worlds.
You can pay per token, and you have lower maintenance costs.

- Rate limiting and quotas
- API reliability and fallbacks
- Security considerations
  - Data privacy
  - Prompt injection
  - Output validation
- Performance optimization
  - Caching strategies
  - Batch processing
  - Response streaming
- Testing strategies
  - Unit testing with LLMs
  - Integration testing
  - Prompt testing

## Real-world applications and use cases

- Content generation
  - Documentation
  - Marketing copy
  - Code generation
- Text analysis
  - Sentiment analysis
  - Entity extraction
  - Classification
- Conversational AI
  - Customer support
  - Virtual assistants
  - Knowledge base integration
- Code assistance
  - Code completion
  - Code review
  - Documentation generation
- Business process automation
  - Email processing
  - Document analysis
  - Report generation

## Summary

- Recap of key points
- Preview of next chapter
- Action items for readers

## Exercises

- Hands-on experiments with different LLMs
- Prompt engineering exercises
- Cost calculation scenarios
- Security analysis practice

[OPENAI_MODELS]: https://platform.openai.com/docs/models#models-overview
[HUGGINGFACE_LLAMA]: https://huggingface.co/meta-llama/Llama-3.3-70B-Instruct
[HUGGINGFACE_MISTRAL]: https://huggingface.co/mistralai/Mistral-Nemo-Instruct-2407
[HUGGINGFACE_GEMMA2]: https://huggingface.co/blog/gemma2
[PHI4_ANNOUNCEMENT]: https://techcommunity.microsoft.com/blog/aiplatformblog/introducing-phi-4-microsoft%E2%80%99s-newest-small-language-model-specializing-in-comple/4357090
[PHI4_BENCHMARKS]: https://www.microsoft.com/en-us/research/uploads/prod/2024/12/P4TechReport.pdf
