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

In this chapter we'll dive deeper into the art and science of prompt engineering things
about writing prompts. We'll cover the following topics:

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

```text
You're a world-class chef and you're preparing a meal.
Provide a recipe for apple pie please.
```

To many of you this sounds like a normal sentence when talking to ChatGPT. But it's
unnatural. If I'm talking to a chef, I don't need to establish the fact that they're a
world-class chef. They already know about that, or they are just a good chef, or maybe
even my grandma. I don't need to encourage their natural behavior.  The second thing
that stands out to me is this. Why am I saying please to a machine? What does that get
me?

Some people even resort to offering money to the LLM or threatening it with termination
in hopes to get better results. But that's not how LLMs work at all.

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
dive into the rest of prompt engineering. These misconceptions can harm your success
when building an LLM-based application.

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
There's a high statistical chance that you get a response that looks reasonable, but the
LLM may lie to you very convincingly. After all, we're just matching a pattern and the
LLM doesn't remember facts, it just predicts the next token based on the pattern it saw.

There's a lot that can go sideways with prompts. But if you understand how to approach
prompt engineering it is very helpful in building effective LLM-based applications.

{#prompt-principles}
## the 6 Basics of a Good Prompt

Writing a good prompt is hard, because you have to think like the LLM you're working
with. If you want to be close to 100% effective, you'll need to know:

- What data the LLM was trained with so you know which language (Dutch, English, etc.)
  it understands best.
- What tasks the LLM was trained on to write the prompt using the same structure and
  style, so the pattern is as clear as possible.
- How the LLM was [aligned][LLM_ALIGNMENT], so you know the style of text the LLM is
  capable of producing.

Sadly, you can't know any of these three things as hard facts. There are benchmarks, but
I've found that they only provide a biased guideline as many LLM providers are likely to
game them. You'll have to guess what works best, because the datasets used to train LLMs
aren't open-source. They're a well guarded secret by the companies that build them.
Because without the data they wouldn't make money.

That leaves us with 6 basic principles that I know from experience work well for
prompts:

- Provide clear direction
- Specify the output format for the prompt
- Add context and samples to the prompt
- Keep the prompt focused on one task
- Tune your prompt with hyperparameters

Let's go through them one by one.

### Provide Clear Direction

Take a step back and think about how the LLM works. Remember that the LLM is a pattern
matching machine that uses embeddings (modelling relationships between words in a
sentence) and an attention mechanism (modeling the context of the input and output
tokens) to produce useful results. The LLM is only as good as the pattern it can match.

Vague instructions aren't going to work. You may get lucky, but you're more likely to
get too much variation in responses as your application is used by more users. And
that's okay for chat applications like ChatGPT, but it's not okay for business
applications where users expect high quality results.

There are two things that are helpful for generating a pattern that the LLM can match.
First, it's helpful to provide the LLM with a persona. For example, a chef, or a
copywriter. Second, it's important to provide the LLM with detailed instructions about
the desired output and the style of the output.

The persona helps prime the embeddings and attention mechanism of the LLM so it starts
searching for output tokens in the right word space. For example, the persona of a chef
will likely lead the LLM in the direction of food, recipes, cooking techniques, and so
on. A copywriter persona primes the embeddings and attention mechanism in the direction
of marketing, text writing, and publishing. In the case of the copywriter you'll
probably also need to provide for what sort of medium the text is intended. Copywriting
is different for a blog post than it is for a book.

The detailed instructions help the LLM to follow a specific style pattern. For example,
if you ask for short responses, it's much more likely for the LLM to produce short
outputs.

### Specify the Output Format for the Prompt

Next, we have to think about the output format for the prompt. In chat applications it's
often enough to not specify the output format at all. LLMs are trained as chatbots, and
they will produce output that's conversational. But if you want to use the LLM for other
tasks, it's important to tell the LLM specifically what output format you want.

For example, you can tell the LLM to output a list of items, a table, or to put a
specific portion of the output in a fenced Markdown code block. This outputs the results
of the prompt between \`\`\` and \`\`\` so it's easily parsable by your application.

Let me demonstrate by asking the LLM to give me the apple pie recipe we used at the
start of the chapter, but to put the recipe in a code block, so I can easily extract it
later.

Here's the prompt:

```text
You're a world-class chef and you're preparing a meal. 
Provide a recipe for apple pie please.
Put the recipe in a fenced markdown block.
```

GPT-4o will respond with something along these lines:

~~~text
Certainly! Here's a classic apple pie recipe in a fenced markdown block:

```markdown
# Classic Apple Pie Recipe

## Ingredients
### For the Pie Crust:
- 2 ½ cups (310g) all-purpose flour
- 1 tsp salt
- 1 tsp granulated sugar
- 1 cup (230g) unsalted butter, cold and cubed
- 6–8 tbsp ice water

### For the Apple Filling:
- 6 cups (about 5 large) apples, peeled, cored, and thinly sliced
- ¾ cup (150g) granulated sugar
- ¼ cup (55g) brown sugar
- 2 tbsp all-purpose flour
- 1 tbsp lemon juice
- 1 tsp ground cinnamon
- ¼ tsp ground nutmeg
- ¼ tsp ground allspice
- Pinch of salt

### For Assembly:
- 1 egg, beaten (for egg wash)
- 1 tbsp granulated sugar (for sprinkling, optional)

... remainder of the response
```
~~~

Note that the response contains the recipe in between Markdown code block delimiters. We
can now use a regular expression to extract the recipe from the response.

While this looks like an excellent way of working with LLMs in use cases outside of
chat, there are better ways to handle structured outputs. You can instruct the LLM to
generate JSON results or even call a function to output the generated code. Both of
these options provide much more control over the output format. We'll cover structured
output in greater detail in chapter 7.

I'll keep repeating this throughout the book. We're controlling a pattern matching
machine. Telling it what output format you want is only putting in the right pattern
for what you want to achieve.

### Add Context and samples to the Prompt

Any LLM is capable of reproducing patterns it learned from the large body of text it was
trained on. But that body of text hopefully doesn't contain internal documents of the
client or company you work for. So if you need to answer questions about internal
information, you're going to have to tell the LLM about the information.

We'll cover Retrieval Augmented Generation in greater depth in chapter 6. But for now,
it's important to understand that the LLM can't remember facts. It's a talking parrot
that reproduces patterns it has seen before. This looks like a limitation, but you can
turn this into a powerful trait.

For example, if I want to answer a question about the company's policy on remote work, I
can first look up documents related to the question by performing a similarity search
using a search engine. The documents I find, I can put into the prompt as additional
context, and then instruct the LLM to answer the question. A prompt for answering
questions typically will look like this:

```text
You're a digital assistant for the HR department of our company. 
Please answer the question of the employee based on the content 
provided as context.

## Context

<The found fragments about the policy on remote work>

## Question

<The employee question>
```

It's highly likely that the LLM will reproduce the sample content you provided as
context in the prompt. There are of course downsides to this. If your context
information is incorrect, vague, or non-existent, then the LLM can't match the pattern,
and you'll get an answer that doesn't make much sense or is plain misleading.

Adding context to a prompt helps establish a good pattern for the LLM to follow. You'll
see a lot of people add examples of the results they want to a prompt. Adding examples
to your prompt is called one-shot learning or few-shot learning if you add more than one
example of the expected output. Let's look at an example to understand what that
looks like:

~~~text
You're a world-class chef and you're preparing a meal. Provide
a recipe for apple pie please. Output the result in JSON, use the
following example as a guide.

## Example

```
{
  "title": "The recipe title",
  "ingredients": [
    { "unit": "tbsp", "amount": 1, "name": "the name of the ingredient" },
    { "unit": "grams", "amount": 12, "name": "another ingredient" },
  ],
  "instructions": "The instructions to prepare the recipe"
}
```

~~~

When we run the prompt, we'll get a completely different response from the previous
sample. The LLM will now generate a JSON object for your recipe.

~~~text
Certainly! Here's a recipe for a classic apple pie in JSON format:

```json
{
  \"title\": \"Classic Apple Pie\",
  \"ingredients\": [
    { \"unit\": \"cups\", \"amount\": 2.5, \"name\": \"all-purpose flour\" },
    { \"unit\": \"tbsp\", \"amount\": 1, \"name\": \"granulated sugar\" },
    { \"unit\": \"tsp\", \"amount\": 1, \"name\": \"salt\" },
    { \"unit\": \"cups\", \"amount\": 1, \"name\": \"unsalted butter, cold and diced\" },
    { \"unit\": \"tbsp\", \"amount\": 6, \"name\": \"ice water\" },
    { \"unit\": \"lbs\", \"amount\": 2, \"name\": \"apples\" },
    { \"unit\": \"cups\", \"amount\": 0.75, \"name\": \"granulated sugar\" },
    { \"unit\": \"tbsp\", \"amount\": 2, \"name\": \"all-purpose flour\" },
    { \"unit\": \"tsp\", \"amount\": 1, \"name\": \"ground cinnamon\" },
    { \"unit\": \"pinch\", \"amount\": 1, \"name\": \"ground nutmeg\" },
    { \"unit\": \"tbsp\", \"amount\": 1, \"name\": \"lemon juice\" },
    { \"unit\": \"tsp\", \"amount\": 1, \"name\": \"vanilla extract\" },
    { \"unit\": \"tbsp\", \"amount\": 2, \"name\": \"unsalted butter (for filling)\" },
    { \"unit\": \"tbsp\", \"amount\": 1, \"name\": \"milk (for brushing crust)\" },
    { \"unit\": \"tbsp\", \"amount\": 1, \"name\": \"granulated sugar\" }
  ],
  // ... Remainder of the response
}
```
~~~

In this case we received a good response with just one sample in the prompt. If you find
that one sample doesn't help, don't be afraid to add more samples to help the LLM match
your desired patern. Providing enough variety in the samples will help with the quality
of the output.

### Keep the Prompt Focused on One Task

LLMs are good at a lot of things, but they're not good at performing multiple tasks in
one prompt. You'll get better results if you can focus the prompt on one task.

Getting a recipe for apple pie is a single task. Answering a question can still be
considered one task. Generating an article about the powerful features of an LLM sounds
like one task, but at the very least involves multiple steps.

If you find that a prompt generates a vague answer even with the previous tips, it's
likely you have a complex task that requires multiple steps to complete. In that case,
you'll want to split the prompt into multiple separate prompts for each step of the
task.

While LLMs have a large context window, and they are capable of following steps, they're
not very good at it. You can find many so-called chain-of-thought prompts on the
internet that produce fantastic results according to the author.

For example, I can improve my apple pie recipe prompt by adding one sentence:

```text
You're a world-class chef. You are preparing a meal.
Give me a recipe for apple pie. Take it step by step.
```

The one sentence "take it step by step" turns the prompt into a chain-of-thought prompt.
I'm asking the LLM to generate a more detailed response. And because it's generating a
more detailed response, I'm likely to get something that resonates better with my
expectations.

Let's go back to what I wrote at the start of [#s](prompt-principles): We're dealing
with a pattern matching machine. So, what happens here? I am providing a pattern that
causes the LLM to follow a more detailed style. Because it's following a more detailed
style, the attention mechanism is influenced as the response gets longer. The end result
is that it generates a more complete description and no longer skips steps, because in a
more detailed description of apple pie, that's what is statistically the most likely
thing.

Note, the recipe looks better, but it doesn't have to be correct. You need to verify the
output of the LLM here. And it will be harder to validate, because you need to look at
more text. Chain-of-thought prompts lack control. You will need to address this if you
want to provide stable results to your users.

Chain-of-thought prompts work with tools like ChatGPT, because you've got nothing better
there. You can't program the workflow in ChatGPT.

For LLM-based applications I recommend skipping chain-of-thought prompts. Instead, I
recommend splitting the task into multiple prompts and using logic or one of the design
patterns in the remaining chapters of this book to come to a complete result.

### Tune Your Prompt With Hyperparameters

The last basic principle of a good prompt is to tune your prompt with hyperparameters.
In the previous sections we've only looked at the text of the prompt. But that's only
one part of the equation.

Remember from [#s](#llm-output-sampling) that the LLM uses sampling to make the output
look natural. The sampling is controlled by a number of parameters that you can set when
you use a tool like Semantic Kernel. These parameters are called hyperparameters,
depending on the provider you may encounter different variations on these parameters.
Here are the most important ones:

- Top-P
- Temperature
- Presence Penalty
- Frequency Penalty

This part is definitely a bit more advanced. But it's important to understand these
parameters and how they interact with the LLM so you know what value to choose for
various tasks to get good quality responses.

#### Top-P

The important bit to understand about an LLM in relation output sampling is this. The
final layer of a LLM performs a softmax calculation. The neural network predicts output
values between -1 and +1. The softmax calculation turns these values into a probability
distribution. Each of the possible output tokens (words, numbers, etc.) is assigned a
value adding up to a total of one. The higher the value, the more likely the token is to
be the next token in the output.

From this probability distribution we could simply select the token with the highest
probability as the output. However, this leads to repetitive and boring output. We can
fix this by using a smarter sampling technique.

All modern LLMs use Top-P sampling. This is how that works. First, we establish a
threshold for the cummulative probability. We then sort the tokens from highest to
lowest probability and then start collecting tokens adding their probabilities to a
cummulative probability. We collect tokens until the cummulative probability value
reaches the threshold. The process is demonstrated in [#s](#top-p-sampling).

{#top-p-sampling}
![Top-P sampling](probability-visualization.png)

For the samples in [#s](#top-p-sampling) we've established a threshold of 0.9. The
tokens with the highest probability are selected until the cummulative probability
reaches 0.9. The tokens with the lowest probability are removed from the distribution.

Once we've selected the candidate tokens in the distribution we randomly select one of
the tokens from the distribution as the output. This results in a more varied output.

The key thing to remember here is that a higher Top-P results in more tokens to be
selected as candidates. A lower value will result in a more focused by repetitive
output. This is ideal for generating code but less so for marketing content where you
want greater variety.

#### Temperature

LLMs don't just use Top-P sampling. The people who invented these types of models found
that Top-P sampling isn't enough to get natural text. So they decided to add temperature
to the mix. Temperature controls the softness of the probability distribution used to
perform Top-P sampling.

Before performing the cummulative probability based selection, the raw scores for the
candidate tokens are divided by the temperature value. After that the softmax function
is applied to determine the probabilities for the candidate tokens. In
[#s](#temperature-effects) you can see this in action with a before and after.

{#temperature-effects}
![The effect of temperature on the probability distribution](scaled-probability-comparison.png)

The higher the temperature, the softer the curve is. It takes longer for the cummulative
probability function to reach the threshold thus more candidate tokens make it into the
selection. Conversely, the lower the temperature, the harder the curve is. It takes less
time to reach the threshold.

Ideally you'd want to see the distribution of each of the tokens in the output to make
an informed decision about the temperature and Top-P values, but you can't do that. I'm
not 100% sure why this information isn't available, but my guess is that it would expose
too many details of the internal to the user of the LLM.

#### Presence Penalty

As you can probably tell by now, the sampling techniques used in LLMs are complex. And
it gets's worse. Let me add one more variable to the mix, presence penalty. A positive
presence penalty reduces the probability value of a token that was seen before in the
output. This forces candidate tokens lower in the selection ranking for the Top-P
sampling process. It is less likely that you see the same token in the output if you
provide a positive value for the presence penalty.

#### Frequency Penalty

The last hyperparameter we'll discuss is the frequency penalty. This penalty looks
similar to the presence penalty parameter but uses a different approach. Instead of
applying a flat penalty to tokens that occurred before in the output, frequency penalty
applies a penalty to the probability of tokens that frequently appear in the output. The
more often a token appears in the output, the higher the penalty and the less likely the
token is to be selected during the Top-P sampling process.

#### What to choose for each of the parameters

I know that this is a lot to take in and apply effectively. So let me give you some
direction on what to choose for each of the parameters.

Coding requires a more repetitive and boring output to be effective. So you'd want to
choose a lower value for Top-P and a lower value for temperature. The presence penalty
and frequency penalty should be set to 0, because we don't want to apply any penalties.
Coding is repetitive by nature.

For general purpose tasks like generating marketing content I recommend setting the
Top-P value higher and apply a higher temperature. The presence penalty and frequency
penalty can be useful to make the text more varied. Don't go overboard though, a low
value is often enough. Setting the presence penalty and frequency penalty to a high
value leads to fascinating but often nonsensical output.

It's important that you test your prompts with a variety of inputs and settings to
establish what works for the majority of cases. Because the values for the probability
distribution ultimately depend on the content of your prompt, and it's very likely that
you need to adjust the hyperparameters a little bit based on that.

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