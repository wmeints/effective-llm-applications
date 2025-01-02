# Understanding Large Language Models

I remember the first time I realized just how useful large language models could
be. Like many developers, I initially tried ChatGPT almost as a joke – I didn't expect
it to work as well as it did. I had this piece of code that needed unit tests, and on a
whim, I copy-pasted it into ChatGPT and asked it to write the tests for me. What
happened next surprised me. Within seconds, I got back working code that needed only
minor tweaks. The overall structure was spot-on, and it saved me 30 minutes of work.
While 30 minutes doesn't sound like a lot of time saved, it does add up and this is
the worst-case scenario at a time when I didn't quite understand how an LLM worked.

That interaction looked like magic, but really wasn't. It was the result of careful
engineering, good quality prompt design, and an understanding of what LLMs can (and
can't) do well. That's what this chapter is all about: demystifying these powerful tools
so you can put them to work in your own applications.

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

- What LLMs are and why they're changing the way we solve problems
- The key concepts and terminology you need to work effectively with LLMs
- The current LLM landscape from a commercial and open-source perspective
- Practical considerations for building production applications
- Real-world use cases and applications that go beyond the obvious

Let's dive in and explore the world of large language models – not as magical black
boxes, but as practical tools we can understand, control, and use to solve real
problems.

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
give an LLM input text, it's not searching through a database for answers. Instead, it's
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
want in plain English, and an LLM can help me scaffold the code, suggest test cases, or
point out potential issues.

So even if you're not building AI applications yourself, it will change how your tools
work and how quickly and effectively you can write code.

Second, and this is what this book is about, they're enabling new types of applications.
Think about all the tasks that were too complex or expensive to automate before because
they required understanding natural language. Now we can build applications that can:

- Generate human-like responses to customer inquiries
- Upgrade code bases from deprecated frameworks or old languages to more modern
  equivalents
- Translate raw input from field reports into a coherent and actionable management
  summary
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
many of you I didn't quite understand the impact of this new technology.

The unit testing experiment changed everything for me. Not because it solved all my
unit-testing needs, but because it showed me that LLMs could understand context and
generate meaningful output that was actually useful. This wasn't just pattern matching
or template filling – it was something completely different.

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
that I was quicker, but I was also less satisfied with the result, because writing great
code is a skill I'm proud of.

After learning about open source LLMs I figured I should give that a try too. It
turned out to be very slow even on a beefy Intel Core i9 machine with a huge graphics
card. I quickly found out that you need an aweful lot of power to run an LLM on our
own machine and in the cloud. And with a price tag of 3500 euros for a decent machine
it's not something you want to do for a hobby project.

There are plenty more experiences where I found the boundaries of what LLMs can do, but
let me finish with one final example. I tried using an LLM to upgrade program code from
a low-code solution to Typescript without human supervision. We quickly had to put in
human supervision, because without that it wasn't going to help us.

### Key lessons learned

You might wonder, why am I telling you my experiences? There are three key lessons that
I want you to keep in mind while reading this book:

1. Be specific in what you ask from the LLM. Don't just ask for an article about LLMs,
   provide specific instructions.

2. Always review and understand the output of the LLM. Don't let your users use the
   output of the LLM unseen. The output will be wrong in all the weird ways you've never
   thought of.

3. Break big problems down into small problems. Instead of asking the LLM to perform 10
   steps, ask it for just one step. It will be easier for the LLM to perform and easier
   for you to debug.

4. Keep track of the context yourself and provide it in focused chunks. LLMs have
   limited input and output space, so they can't keep track of a complete book or even a
   blog post.

### Evolution of my understanding

After the initial rollercoaster ride with LLMs my understanding of them evolved. I
stopped seeing them as a silver bullet that can solve all my language related problems
and started seeing them as a powerful pattern-matching engine capable of transforming
text.

One important moment was realizing that LLMs excel at clearly defined tasks that
involve matching a pattern in source text and transforming it into other text. If you
can find a clear pattern in the input yourself, and you can clearly define the target
structure, it's likely that an LLM is a good solution to your problem. The less clear
the problem statement is, the more issues you'll experience.

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

- LLMs will behave better when you follow a structured approach. The more structure, the
  better.
- LLM behavior changes when providers push new versions of the models, automated testing
  is your friend.
- LLMs are slow, you will need to find ways to provide delayed responses to the user.
- The API endpoints of LLM providers often break, so you'll need to have solid error
  handling.

Throughout the rest of the book, I will share the strategies that I applied to help me
get the most out of my LLM-based applications.

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
major LLM providers. There are open-source options too, but these are generally less
powerful and require more engineering effort to use. Having said that, I highly
recommend you give them a try, because they offer other benefits you can't get from
major LLM providers, especially the smaller models.

#### OpenAI

OpenAI has been at the forefront with their GPT series. What I love about OpenAI is
their API's reliability. The downside? They can be expensive for large-scale
applications, and their terms of service are quite restrictive. Also, it takes a while
before newer models become available through the API. And if they become available, it
takes a while before you can push a decent amount of input through the API, because the
rate limits are pretty low.

#### Anthropic

Anthropic's Claude model has impressed me with its ability to handle complex
instructions and longer context windows. I've found it particularly good at tasks
requiring careful reasoning, like code review and technical writing. The pricing is
competitive with OpenAI, and their terms of service are generally more
business-friendly.

Anthropic is notorious for their rate limits. They often cap out when people in the
United States are online and working, and you'll end up with random overload errors.
It's important to have a good contract with them or plan for a backup.

Sadly though, you can't use Anthropic directly with Semantic Kernel at this time, so
you'll not find much about the models from Anthropic in this book. I recommend keeping
an eye out fore updates from Microsoft on this, because I noticed that they have
a few issues open on their GitHub repository that they're working on.

#### Meta

With LLaMA and its variants, Meta has shaken up the field by releasing powerful
open-source models. While you'll need more technical expertise to use these effectively,
they offer flexibility that proprietary models can't match.

LLaMA models are available through the major cloud providers, but you can also run them
on your own machine if you have a good enough GPU, for example a RTX4080 has no problem
running these models. If you want to give this a try, I recommend taking a look at
[Ollama](https://ollama.com/).

#### Google

Google's PaLM API and Gemini models are interesting contenders that I personally haven't
had much experience with. However, if you're in the Google space, then they are a great
option and relatively easy to configure these days through their portal. I've found
their documentation particularly developer-friendly.

### Model comparison

With the major providers covered, let's break down the model the important model types
that I've worked with to give you an understanding of what to expect from each model
type.

#### GPT series by OpenAI

- **GPT-4o:** The king of the hill when it comes to the GPT models. This model is
  powerful enough for most complex tasks. It's a general purpose model, useful for code
  as well as more generic text based tasks. This model supports generating images too.

- **GPT-4o mini:** The smaller brother of the GPT-4o model is a lot faster while still
  providing plenty of capabilities. I generally try this model first and only switch to
  the bigger and slower GPT-4o model when tests fail too frequently.

Recently, OpenAI started work on a new series of models called the Orion models. These
models focus on reasoning capabilties, and generally lack the general purpose features
that the GPT series has. Currently, there are two Orion-type models:

- **o1:** This is the biggest model so far from OpenAI. It's at the top of the
  benchmarks right now for biology, physics, and chemistry tasks. However, you're paying
  a premium for something that you can solve using the patterns in this book in
  combination with a less expensive model. Proceed with caution.

- **o1-mini:** Is the smaller version with capabilities somewhere between GPT-4o and o1.
  This model lacks a lot of the general knowledge included in GTP-4o and o1, so it will
  be useful for specific reasoning tasks but not much else.

You can find a full listing of the OpenAI models on [their website][OPENAI_MODELS].

#### Claude models by Anthropic

Claude models come in three varieties, just as with the GPT series, you can choose a
smaller, less capable, but faster and cheaper model depending on what your use-case is.

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

#### Gemini models by Google

Google was a little late to the game, and is often considered the underdog in the LLM
landscape. They too offer various models that work well for different types of tasks.
Here are the two most recent models offered by them.

- **Gemini 1.5 Pro:** This model is the most powerful model from Google. It's a general
  purpose model focusing on reasoning tasks. It works quite well if you have a task
  that requires the model to generate output that has to meet specific constraints. I've
  found that this model works less for generating content like blog posts or marketing
  materials.

- **Gemini 2.0 Flash:** Is the fastest model offered by Google. It's much faster than
  the pro model, but offers less reasoning capabilities. It's a great model for
  chat applications that need to be fast, and only need to answer questions.

Looking at various tests and benchmarks, I've found that the Gemini models are trained
mostly on instruction and constraint solving tasks. They score lower on creative writing
tasks and coding tasks.

#### LLaMA and open-source alternatives

While the commercial models are powerful, the open-source models are catching up fast.
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

It's important to remember that the most powerful model aren't always the best option. I
follow this general workflow when developing an LLM-based application that can be quite
helpful if you're just starting out:

1. First, I choose a general purpose model based on the cloud provider I'm working with.
   Most of the time my clients already have a contract with either Microsoft Azure or
   AWS to host their solutions. I use the existing environment to prototype the
   solution.  

2. After the initial prototype, I'll look at data privacy requirements that the solution
   may have. Depending on these requirements I will determine the engineering effort and
   contractual effort we need to undertake to make the solution production viable.
   Usually, I'm the person who talks about the technical requirements while one of our
   legal people looks into contracts.  

3. After the initial prototype and requirements gathering, I'll deploy the solution to
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

## Key Concepts and Terminology

Before we dive into building applications with LLMs, let's cover some essential concepts
you'll need to understand. Don't worry if some of these seem abstract at first – we'll
put them into practice throughout the rest of the book.

### Essential Terminology

#### Tokens and Tokenization

Before an LLM can process your text, it needs to break it down into tokens. Think of
tokens as the building blocks the model uses to understand text. A token can be:

- A complete word
- Part of a word
- A number
- A special character
- A delimiter

The process works like this: your text gets split into tokens, which are then converted
into numbers using the model's vocabulary. This conversion is necessary because the
transformer-based neural network that powers the LLM can only process numbers, not raw
text.

For example, the word "tokenization" might be split like this:

```plaintext
"tokenization" -> ["token", "ization"]
```

When the model generates a response, the process happens in reverse – tokens are
converted back into text. This is why sometimes you might see slightly odd word splits
in responses, especially with technical terms or rare words.

#### Embeddings

At the input side of almost all LLMs is something called an embedding layer. This
component turns tokens into dense vectors that capture the semantic meaning of the text.
These are called embeddings or embedding vectors. Embedding vectors are interesting
because they can represent the relationships between words in a mathematical space.

The embedding layer isn't just a random part of the model – it's trained on vast amounts
of text to understand how words relate to each other based on how they are used. Think
of it as a map where similar words or concepts are located close to each other.

You'll work directly with embeddings later when we implement the Retrieval Augmented
Generation (RAG) pattern in Chapter 5. For now, just know that they're important for how
LLMs understand text.

#### Context Window

Every LLM has a limit to how much text it can consider at once – this is called the
context window. It's essentially the model's short-term memory, including both your
input and its output.

Context windows have grown significantly:

- Most modern commercial models handle 100K-250K tokens
- This translates to roughly 100K words on average
- Open source models typically have smaller windows due to their compact size

You can find the exact size in the model's documentation (often called a model card).
Managing this context window effectively becomes crucial when building applications, as
we'll see in later chapters.

#### Output Sampling and Temperature

LLMs aim to produce human-like text, and one way they do this is through output
sampling. When generating each token, the model doesn't just pick the most likely
option, it samples from a distribution of possibilities.

Temperature is your main control over this sampling process:

- Low temperature (0.1-0.3): More focused, deterministic responses
- High temperature (0.7-0.9): More creative, varied output

Here's how I typically set temperature:

- Code generation: 0.2 (we want precision)
- Content creation: 0.7 (we want creativity)
- Factual responses: 0.1 (we want consistency)

While temperature is the most common setting you'll adjust, there are other sampling
parameters available. I recommend checking out [this article][PARAMETER_EXPLANATION] for
a deeper dive into all the options.

#### Few-shot Learning

Sometimes the best way to get what you want from an LLM is to show it examples. This is
called few-shot learning, and it comes in two flavors:

**One-shot Learning**  
You provide a single example:

```plaintext
Input: "The pizza was cold"
Output: Negative sentiment

Now classify: "The service was excellent"
```

**Few-shot Learning**  
You provide multiple examples:

```plaintext
Input: "The pizza was cold"
Output: Negative sentiment

Input: "The atmosphere was lovely"
Output: Positive sentiment

Now classify: "The service was excellent"
```

Often, one good example is enough, but complex tasks might need more.

#### Zero-shot Capabilities

Modern LLMs are so well-trained that they can often perform tasks without any examples.
This is called the zero-shot capability of a model. You just describe what you want:

```plaintext
Classify the sentiment of this review: "The service was excellent"
```

While I often start with zero-shot for simplicity, I'm not afraid to add examples if the
results aren't quite what I need. The key is being flexible and pragmatic about which
approach you use.

In the next section, we'll look at core concepts for working with these
models.

### Core Concepts

#### Prompt Engineering

The input you give to an LLM is called a prompt. Think of it as instructions that tell
the model what you want it to do. A prompt contains both the task description and any
context the model needs to generate the right output.

Here's a simple example:

```plaintext
Bad prompt:
"Write a function that validates email addresses."

Good prompt:
"Write a C# function that validates email addresses. The function should:

- Use regular expressions for validation
- Return a boolean indicating if the email is valid
- Handle common edge cases like missing @ symbols"
```

You'll find many websites promoting "proven" prompt patterns, often with claims like:

- "Always start with 'You are an expert...'"
- "Use this exact format for best results..."
- "Include these specific phrases..."

I've learned the hard way that while these patterns might work initially, they often
break in unexpected ways. What works better is:

- Testing your prompts thoroughly
- Adjusting temperature and other parameters
- Building robust error handling around the LLM
- Iterating based on real usage

We'll dive deep into practical prompt engineering in Chapter 3.

#### Fine-tuning vs. Prompt Engineering

You might hear people talk about fine-tuning models for specific domains. Let me share
my perspective on this.

Fine-tuning means taking a base model (like GPT-4) and training it further on your
specific data to make it better at particular tasks. While this sounds appealing, there
are several reasons I rarely recommend it:

**Why People Consider Fine-tuning:**

- Make the model more specialized for specific tasks and domains
- Improve performance for domain-specific language
- Ensure consistent outputs

**Why I Usually Avoid It:**

1. Cost
   - Significant computing resources required
   - High monetary investment
   - Ongoing maintenance costs

2. Complexity
   - Requires extensive training data
   - Technical expertise needed
   - Time-consuming process

3. Trade-offs
   - Lose general capabilities
   - Limited flexibility
   - Can be overkill for most use cases

Instead of fine-tuning, I recommend looking at alternatives like Retrieval Augmented
Generation (RAG). Here's a practical example:

Let's say you're building a chatbot to answer questions about your company's products.
Instead of fine-tuning a model on your product documentation, you could:

1. Store your documentation in a vector database
2. Search for relevant information when a question comes in
3. Include that information in your prompt as context
4. Let the LLM generate an answer based on the provided context

This approach is:

- More flexible (easy to update documentation)
- Cost-effective (no training required)
- Faster to implement
- More maintainable

Throughout this book, we'll explore patterns like RAG that give you the control you need
without the complexity of fine-tuning. In Chapter 5, we'll implement a complete RAG
system so you can see these benefits firsthand.

In the next section, we'll look at practical considerations for working with these
models, building on these fundamental concepts to create reliable applications.

## Practical Considerations for Working with LLMs

There are quite a few things that you need to account for when building LLM-based
applications. This book is full of those things, but I've found there are three
cross-cutting concepts that I always keep in mind when building LLM-based applications.
These are the lessons I've learned while building applications in production, and
they'll help you avoid some of the nastier problems that I ran into.

I think about these things as the downsides to building an LLM-based application. It's
inspiring to work with language models, but they come with a cost.

### Cost management

LLMs are resource-intensive, both in terms of computing power and API costs. If you
don't need an LLM or can achieve the same result with a smaller model, I recommend doing
so. And if you still decide to build an LLM-based application I highly recommend
monitoring API usage and costs closely.

### Security

Using an LLM opens up new security risks. For example, people will try to steal your
application's internal instructions. They will also try to abuse your application
through prompt injection. I've seen this happen in the wild, and it's not pretty. Make
sure you're aware of these risks and have safeguards in place.

### Performance

LLM-based applications can be slow, especially if you're going to build more complicated
interactions with LLMs. Throughout the rest of the book you'll find strategies and
patterns to help work around the performance issues that you'll encounter. Consider this
your first warning that you'll need to think about how to compensate for slow responses.

We'll dive deeper into these topics in chapter two when we explore LLMOps essentials.
For now, let's look at some real-world applications and use cases to inspire you with
what's possible.

## Real-world applications and use case

While social media is full of posts about using LLMs for marketing content or personal
task automation, I've found the real potential goes far beyond these common examples.
Let me share some real-world cases I've worked on that showcase what's possible when you
think bigger.

### Generating Targeted Reports from Technical Information

One organization I worked with was running charity projects with donor support. They had
a challenge: their field reports were too technical and detailed for donors to digest
easily. Here's how we solved it:

The situation: Field reports contained crucial project information, but they were
written in technical language that donors struggled to understand. The organization
needed donor-friendly summaries but couldn't afford the time to manually rewrite each
report.

We built a solution combining two key patterns: Retrieval Augmented Generation (RAG) and
prompt chaining. The RAG pattern helped us find relevant information matching donor
questions, while the prompt chain helped us rewrite the technical content in a
donor-friendly style that matched the organization's voice.

A key learning here was the importance of human review. We had to build features showing
where information came from because reviewers wouldn't trust the system without this
transparency. It's a great example of how LLMs work best when they augment human
capabilities rather than trying to replace them entirely.

### AI-Powered Knowledge Sharing Through Interviews

Another interesting case involved Info Support itself, the organization I work for. We
were struggling with knowledge sharing. We had a common problem: experts who were too
busy to write articles about their innovations, leading to repeated problem-solving
across projects.

Instead of asking people to write articles, we flipped the script. We built a chat-based
system where the LLM interviewed people about their work, asking progressively deeper
questions to understand the topic fully. The system then transformed these conversations
into structured articles.

One interesting discovery was how well users responded to being interviewed by an AI.
The interaction felt natural without falling into the [uncanny
valley](https://en.wikipedia.org/wiki/Uncanny_valley). However, we learned that letting
the LLM fully control the interview flow was tricky. We eventually replaced our complex
prompt-based decision-making with a simpler function that tracked question count to
manage interview length.

### Modernizing Legacy Code Bases

One of my favorite projects involved upgrading legacy web forms from XML to TypeScript.
The organization had so many forms that manual conversion would have taken years.

We approached this by building a batch pipeline that combined traditional XML parsing
with LLM-powered code generation. Rather than asking the LLM to handle everything, we
broke the problem into manageable chunks, using multiple refinement prompts to improve
the generated code quality.

This project taught us a crucial lesson: LLMs work best when combined with traditional
programming logic. While they're great at understanding and translating code patterns,
they struggle with handling large amounts of complex code all at once. Breaking the
problem down into smaller pieces and using traditional parsing where appropriate gave us
much better results.

### We're Only Just Starting

These cases represent just the beginning of what's possible with LLMs. While many people
start their LLM journey with personal automation through tools like ChatGPT, these
examples show how you can scale up to automate more complex team-based workflows.

It's important to remember that we're in the early days of this technology. The patterns
I share in this book work well today, but I expect them to evolve significantly over the
next few years. What excites me most is that we're just starting to understand what's
possible.

As we move forward in this book, we'll explore various design patterns in detail,
showing you how to implement them in your own projects. But first, let's make sure you
have a solid foundation by discussing operating LLMs in production in the next chapter.

## Summary

In this chapter, we've learned a brief history of Large Language Models and how they
work from a conceptual level. We've explored the key concepts and terminology to help
you understand and work with LLMs in the next chapters. Finally, I've shared with you my
personal journey to help you understand the practical considerations and real-world
applications of LLMs.

In the next chapter we'll talk about more practical considerations that you need to keep
in mind when building LLM-based applications when we discuss the essentials of LLMOps.

## Further reading

Here are some resources if you want to learn more about the inner workings of LLMs:

- [How LLMs think](https://towardsdatascience.com/how-llms-think-d8754a79017d) - An
  interesting article exploring some of the math behind LLMs in an attempt to understand
  how and why they work.
- [Attention is all you need](https://arxiv.org/abs/1706.03762) - The original paper
  that sparked the transformer revolution.

[OPENAI_MODELS]: https://platform.openai.com/docs/models#models-overview
[HUGGINGFACE_LLAMA]: https://huggingface.co/meta-llama/Llama-3.3-70B-Instruct
[HUGGINGFACE_MISTRAL]: https://huggingface.co/mistralai/Mistral-Nemo-Instruct-2407
[HUGGINGFACE_GEMMA2]: https://huggingface.co/blog/gemma2
[PHI4_ANNOUNCEMENT]: https://techcommunity.microsoft.com/blog/aiplatformblog/introducing-phi-4-microsoft%E2%80%99s-newest-small-language-model-specializing-in-comple/4357090
[PHI4_BENCHMARKS]: https://www.microsoft.com/en-us/research/uploads/prod/2024/12/P4TechReport.pdf
[PARAMETER_EXPLANATION]: https://medium.com/@albert_88839/large-language-model-settings-temperature-top-p-and-max-tokens-1a0b54dcb25e
