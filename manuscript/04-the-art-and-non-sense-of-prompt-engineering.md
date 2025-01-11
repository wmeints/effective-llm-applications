# the Art and Nonsense of Prompt Engineering

In chapter 3 we got the first chance to work with Semantic Kernel to execute a basic
prompt. Prompts are arguably the most important part of working with large language
models.

I've seen so many posts on social media about prompt engineering it almost made no sense
to me to talk about prompt engineering. It feels as if everyone already knows everything
about prompt engineering. But when I look at the tips and tricks people share, and
compare that to what I've seen in production applications, I'm worried that we're
forgetting something important.

LLMs are powerful tools, but they're also very sensitive to the input they receive.
Prompt engineering is a very unnatural use of language. We're not writing for humans,
we're writing for a statistical model. And that provides us with all sorts of strange
challenges.

In this chapter we'll dive deeper into the art of prompt engineering and uncover
some of the nonsensical things about writing prompts. We'll cover the following topics:

- Why are prompts important for effective LLM-based applications?
- The 6 basics of a good prompt
- Using prompt templates for reusability
- Prompt testing and iterating on your prompts
- Monitoring prompt interactions in production
- Security considerations when using prompts

At the end of this chapter you'll have learned how to write effective prompt templates
with system instructions and how to make sure your prompts still work after 6 months.

## Why Are Prompts Important for Effective LLM-Based Applications?

Let's first get a good understanding of why prompt engineering is such an important
concept when working on LLM-based applications.

### Why Prompt Engineering Matters

Large Language Models can process language and produce language. So it makes sense that
we need to use language to interact with them. But there's something positively weird
about LLMs. When you work with ChatGPT you're likely going write a prompt that looks
like this:

```plaintext
You're a world-class chef and you're preparing a meal.
Provide a recipe for apple pie please.
```

To many of you this sounds like a normal sentence when talking to ChatGPT. But it's
unnatural. If I'm talking to a chef, I don't need to establish the fact that they're a
world-class chef. They already know about that, or they are just a good chef, or maybe
even my grandma. I don't need to encourage their natural behavior.  The second thing
that stands out to me is this. Why am I saying please to a machine? What does that get
me?

The apple pie prompt works because we establish a pattern for the model to work with so
that it produces the best possible output tokens. Remember from
[#s](#understanding-llms) that an LLM can only predict the next most-likely output
token. And it uses sampling to make it sound natural. We're dealing with a neural
network that's a mathmatical concept pattern matching input so that we get, hopefully,
the right output. That means we need to think about prompts not as normal sentences, but
as computer instructions.

For a prompt to be effective, we need to prime the attention mechanism and use the
embeddings to our advantage. That's why prompts are an important part of working with
LLMs. And that's why the unnatural way of speaking to an LLM is so effective.

### Common Misconceptions About Prompts

There are a few common misconceptions about prompts that I want to address before we
dive into the art of prompt engineering. These misconceptions can be misleading and can
harm your success when building an LLM-based application.

#### Prompt engineering is a job

Prompt engineering isn't a job. It's a skill that's part of another job like programming
or copywriting. Prompt engineering is effective for tools like ChatGPT, but you have to
remember that ChatGPT is more than just a prompt interface. A lot of the logic that
makes the tool work is hidden from you. If you want to move beyond basic chat interfaces
it's important to understand the other parts of building LLM-based applications.

#### Prompts are static

While you can reuse prompts over time, you'll have to account for the fact that LLMs get
updated and retrained now and then. A prompt that worked before is likely going to
break at some point because the LLM was updated. Prompts that may have worked with
Claude will produce different results with GPT-4o, because they used a different
training set.

#### Complicated prompts yield better results

Some people will tell you to write complicated prompts to get better results. But you
will often get better results by splitting complex tasks into smaller tasks and running
them separately. This is because of how the context window of the LLM works. From [this
paper][CONTEXT_WINDOW_PAPER] we can learn that LLMs have an attention span that isn't
exactly what you'd expect from a machine. [#s](#context-window-curve) shows the
attention span of a typical LLM. Input that's at the start and end of a prompt is likely
to get picked up better by the LLM than content in the middle of the prompt.

![Context window attention curve](context-window-curve.png)

Longer and more complicated prompts can work, but if you find that the LLM doesn't give
you the desired response, it's important to understand the context window curve and
adjust your application accordingly.

#### You can rely on the general knowledge captured in the LLM

LLMs are trained on a lot of data. As a result you can get a recipe for apple pie that
looks very reasonable. But you can't rely on the general knowledge captured in the LLM.
There's a high statistical chance that you get a response that looks reasonable, but it
may lie to you very convincingly. After all, we're just matching a pattern and the LLM
doesn't remember facts, it just predicts the next token.

There's a lot that can go sideways with prompts. But if you understand how to approach
prompt engineering it is very helpful in building effective LLM-based applications.

## the 6 Basics of a Good Prompt

Writing a good prompt is hard, because you have to think like the LLM you're working
with. If you want to be close to 100% effective, you'll need to know:

- What data the LLM was trained to understand which language (Dutch, English, etc.) it
  understands best.
- What tasks the LLM was trained on to write the prompt using the same structure and
  style, so the pattern is as clear as possible.
- How the LLM was [aligned][LLM_ALIGNMENT], so you know the style of text the LLM is
  capable of producing.

Sadly, you can't know any of these three things as hard facts. There are benchmarks, but
I've found that they only provide a biased guideline. In your daily work you have to
guess what works best, because the datasets used to train LLMs aren't open-source.
They're a well guarded secret by the companies that build them. Because without the data
they wouldn't make money.

That leaves us with 6 basic principles that I know from experience work well for
prompts:

- Provide clear direction
- Specify the output format for the prompt
- Add context and samples to the prompt
- Keep the prompt focused on one task
- Tune your prompt with hyperparameters

Let's go through them one by one.

### Provide Clear Direction

The first thing you need to do when writing a prompt is to think about the goal of the
prompt. What's the task you want to accomplish. Then you need to write down clear and
consise instructions for the LLM to follow.

There are two things that are important here. First, it's helpful to provide the LLM
with a persona. For example, a chef, or a copywriter. Second, it's important to provide
the LLM with detailed instructions about the desired output and the style of the output.

The persona helps prime the embeddings and attention mechanism of the LLM so it starts
searching for output tokens in the right space. For example, the persona chef will
likely lead the LLM in the direction of food, recipes, cooking techniques, and so on. A
copywriter persona primes the embeddings and attention mechanism in the direction of
marketing, text writing, and publishing. In the case of the copywriter you'll probably
also need to provide for what sort of medium the text is intended. Copywriting is
different for a blog post than it is for a book.

The detailed instructions help the LLM to follow a specific style pattern. For example,
if you ask for short responses, it's much more likely for the LLM to produce short
outputs.

### Specify the Output Format for the Prompt

We haven't covered structured outputs just yet, but it's important to understand that
you can tell the LLM what sort of output you want. This is critical if you want to use
the LLM for use cases beyond chat interface. But it is also helpful in chat interfaces
to provide output that's easily digestible by a user.

### Add Context and samples to the Prompt

For example, it can be helpful to first establish the context of the conversation by
setting a role for the LLM. Prompts that start with "You're a <job-title> and your task
is to <task description>" can be very effective. Alternatively, you can provide context
by specifying a prompt like "We're working on <project-name> that's about
<project-description>". Both approaches to establishing context can be helpful to prime
the attention mechanism and embeddings in the LLM.

### Keep the Prompt Focused on One Task

### Tune Your Prompt With Hyperparameters

## Using Prompt Templates for Reusability

- Store your prompts in source control so you can version them.
- Write prompts in a separate prompt file for easier editing and reviewing.
- Use placeholders to make your prompts more flexible.
- Options for prompt writing, semantic kernel templates, or handlebars.

## Using the chat history to your advantage

- A prompt doesn't have to be one sequence of text. You can use the chat history object to provide more context to the model.
- The most important use of the chat history is to provide a set of system instructions
  to the model. These instructions provide a base direction for the model to follow
  while the prompt provides instructions to achieve a specific goal.

## Prompt Testing and Iterating on Your Prompts

- Use a separate test suite for prompt testing.
- Test for properties in the text instead of relying on the exact output.
- Use multiple input samples to verify your prompt.

## Monitoring Prompt Interactions in Production

- Use application insights to collect telemetry data on prompt interactions.
- Export data from application insights and use it in your tests.

## Security Considerations When Using Prompts

In [#s](#llmops-application-security) we discussed the importance of a layered defense.
The layered defense starts with the prompt execution proces. We'll need to make sure
that the input and output of the prompt are filtered for any unwanted content. We also
need to make sure that we reduce the risk of prompt injection.

### Filtering PII From the Input

### Protecting Yourself Against Prompt Injection

### Filtering Harmful Content From the Output

## Summary

In this chapter we've looked at the art and nonsense of prompt engineering. We discussed
the important role prompts play in LLM-based applications and how to write effective
prompts. We also covered the importance of testing and monitoring your prompts so you
can ensure that your application will still work in 6 months time.

In the next chapter, we'll extend the capabilities of the LLM with functions and use prompts
to interact with them.

[CONTEXT_WINDOW_PAPER]: https://arxiv.org/abs/2307.03172
[LLM_ALIGNMENT]: https://medium.com/@madalina.lupu.d/align-llms-with-reinforcement-learning-from-human-feedback-595d61f160d5