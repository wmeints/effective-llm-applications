{#the-art-and-nonsense-of-prompt-engineering}
# The art and nonsense of prompt engineering

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

At the end of this chapter you'll understand how to write effective prompt templates
for your LLM-based application. You'll also understand the difference between chat
and non-chat use cases when it comes to using an LLM.

Let's get started by exploring why prompt engineering is so popular.

## Why are prompts important for effective LLM-Based applications?

Prompt engineering is super popular today because it's the cornerstone of using an LLM.
Let's first get a good understanding of why prompt engineering is such an important
concept when working on LLM-based applications.

### Why prompt engineering matters

Large Language Models can process language and produce language. Prompts are the only
method to control an LLM. But there's something positively weird about using prompts.
When you work with ChatGPT you're likely going write a prompt that looks like this:

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

For a prompt to be effective, we need to use the attention mechanism and the embeddings
to our advantage. Funny enough it's the weirdest combinations of words that let the
embeddings and attention mechanism of the LLM work best. The clearer the pattern, the
better the output.

### Common misconceptions about prompts

There are a few common misconceptions about prompts that I want to address before we
dive into the rest of prompt engineering.

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
exactly what you'd expect from a machine. And although the models have been updated
since the release of the paper, I still see this behavior in our production
applications. [#s](#context-window-curve) shows the attention span of a typical LLM.
Input that's at the start and end of a prompt is likely to get picked up better by the
LLM than content in the middle of the prompt.

![Context window attention curve](context-window-curve.png)

Longer and more complicated prompts can work, but if you find that the LLM doesn't give
you the desired response, it's important to understand the context window curve and
adjust your application accordingly.

#### You can rely on the general knowledge captured in the LLM

LLMs are trained on a lot of data. As a result you can get a recipe for apple pie that
looks very reasonable. But you can't rely on the general knowledge captured in the LLM.
There's a high statistical chance that you get a response that looks reasonable, but the
LLM may lie to you very convincingly. After all, we're just matching a pattern. The LLM
doesn't remember facts, it just predicts the next token based on the pattern it saw.

There's a lot that can go sideways with prompts. But if you understand how to approach
prompt engineering it is very helpful in building effective LLM-based applications.

{#prompt-principles}
## The 5 basics of a good prompt

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

That leaves us with 5 basic principles that I know from experience work well for
prompts:

- Provide clear direction
- Specify the output format for the prompt
- Add context and samples to the prompt
- Keep the prompt focused on one task
- Tune your prompt with hyperparameters

Let's go through them one by one.

### Provide clear direction

Take a step back and think about how the LLM works. Remember that the LLM is a pattern
matching machine that uses embeddings (modelling relationships between words in a
sentence) and an attention mechanism (modeling the context of the input and output
tokens) to produce useful results. The LLM is only as good as the pattern it can match.

Vague instructions aren't going to work. You may get lucky, but you're more likely to
get too much variation in responses as your application is used by more users. And
that's okay for chat applications like ChatGPT, but it's not okay for business
applications where users expect high quality results.

There are two things that are helpful for generating a pattern that the LLM can match.
First, it's helpful to provide the LLM with a clear context. For example, you can give it
a persona to provide the context. Or you can tell it what project/task you're working on.
Second, it's important to provide the LLM with detailed instructions about
the desired output and the style of the output.

The context helps set the embeddings and attention mechanism of the LLM so it starts
searching for output tokens in the right word space. For example, the persona of a chef
will likely lead the LLM in the direction of food, recipes, cooking techniques, and so
on. A copywriter persona primes the embeddings and attention mechanism in the direction
of marketing, text writing, and publishing. In the case of the copywriter you'll
probably also need to provide for what sort of medium the text is intended. Copywriting
is different for a blog post than it is for a book.

The detailed instructions help the LLM to follow a specific style pattern. For example,
if you ask for short responses, it's much more likely for the LLM to produce short
outputs.

### Specify the output format for the prompt

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

Note that the response contains the recipe in between Markdown fenced code block
delimiters. We can now use a regular expression to extract the recipe from the response.

While this looks like an excellent way of working with LLMs in use cases outside of
chat, there are better ways to handle structured outputs. You can instruct the LLM to
generate JSON results or even call a function to output the generated code. Both of
these options provide much more control over the output format. We'll cover structured
output in greater detail in chapter 8.

Let's look at how you can help the model with samples next.

### Add samples to the prompt

Any LLM is capable of reproducing patterns it learned from the large body of text it was
trained on. But that body of text hopefully doesn't contain internal documents of the
client or company you work for. So if you need to answer questions about internal
information, you're going to have to tell the LLM about the information.

We'll cover Retrieval Augmented Generation in greater depth in chapter 7. But for now,
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

You can consider the context heading in the prompt as the samples that the model can
use to generate a useful response. It's highly likely that the LLM will reproduce the
content you provided because of the talking parrot effect.

If your sample is incorrect, vague, or non-existent, then the LLM can't
match the pattern, and you'll get an answer that doesn't make much sense or is plain
misleading. So it's important to make sure you only inject information that's safe,
and sensible.

Adding samples to a prompt helps establish a good pattern for the LLM to follow. You'll
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

While adding multiple samples to the prompt is helpful, adding multiple tasks to a
prompt isn't. Let's take a look.

### Keep the prompt focused on one task

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
more text. Chain-of-thought prompts lack control in many cases. You will need to address
this if you want to provide stable results to your users.

Chain-of-thought prompts work with tools like ChatGPT, because you've got nothing better
there. You can't program the workflow in ChatGPT.

For LLM-based applications I recommend skipping chain-of-thought prompts. Instead, I
recommend splitting the task into multiple prompts and using logic or one of the design
patterns in the remaining chapters of this book to come to a complete result.

### Tune your prompt with hyperparameters

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
final layer of a LLM performs a softmax calculation. The neural network doesn't produce
words, it produces numbers that we have to translate into numbers. The numbers it
produces are typically negative and range from 0 to values well into -200 range. The
softmax output layer turns these values into a probability distribution. Each position
in the output layer represents a possible output token (words, numbers, etc.) . Each of
the possible output tokens is assigned a value adding up to a total of one. The higher
the value, the more likely the token is to be the next token in the output.

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

LLMs don't just use Top-P sampling. The inventors of these types of models found
that Top-P sampling isn't enough to get natural text. So they decided to add temperature
to the mix. Temperature controls the shape of the probability distribution used to
perform Top-P sampling.

Here's how temperature works. Before performing the Top-P sampling, the raw scores for
the candidate tokens are divided by the temperature value. After that the softmax
function is applied to determine the probabilities for the candidate tokens. In
[#s](#temperature-effects) you can see this in action with a before and after.

{#temperature-effects}
![The effect of temperature on the probability distribution](scaled-probability-comparison.png)

The higher the temperature, the softer the curve is. It takes longer for the cummulative
probability function to reach the threshold thus more candidate tokens make it into the
selection. Conversely, the lower the temperature, the harder the curve is. It takes less
time to reach the threshold.

Ideally you'd want to see the distribution of each of the tokens in the output to make
an informed decision about the temperature and Top-P values, but you can't do that. So
tuning the temperature is a bit of an arcane art.

At this point I understand that you're probably thinking that this is a lot to get
through. And it is. I've included an interactive notebook that demonstrates Top-P
sampling and temperature scaling in the [GitHub repository][TOP_P_SAMPLE]. You can run
the notebook inside Visual Studio Code to explore the effects of temperature and Top-P
sampling to get a better understanding of these concepts.

#### Presence Penalty

As you can probably tell by now, the sampling techniques used in LLMs are complex. And
it gets even more complex. Let me add one more variable to the mix, presence penalty. A
positive presence penalty reduces the probability value of a token that was seen before
in the output. This forces candidate tokens lower in the selection ranking for the Top-P
sampling process. It is less likely that you see the same token in the output if you
provide a positive value for the presence penalty.

#### Frequency Penalty

Frequency penalty is the last hyperparameter we need to discuss. This hyperparameter
looks similar to the presence penalty hyperparameter but uses a different approach.
Instead of applying a flat penalty to tokens that occurred before in the output,
frequency penalty applies a penalty to the probability of tokens that frequently appear
in the output. The more often a token appears in the output, the higher the penalty and
the less likely the token is to be selected during the Top-P sampling process.

#### What to choose for each of the hyper parameters

I know that this is a lot to take in so let me give you some direction on what to choose
for each of the parameters.

Coding requires a more repetitive and boring output to be effective. So you want to
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

Once you've written a good quality prompt, you may want to keep it around for longer.
For this it's nice to have some sort of templating system in place. Let's take a look at
what Semantic Kernel has to offer.

{#prompt-templates}
## Writing prompt templates for reusability

Writing your prompts inline with other C# code is never a good plan. It's hard to read,
hard to maintain, and you can't reuse it. That's why Semantic Kernel offers a way to
write prompt templates using a variety of templating languages:

- Semantic Kernel Templates: The internal format developed by Microsoft.
- Handlebars: A popular templating language that's available for a wide range of
  programming languages.
- Liquid: An alternative to handlebars that's also available for a number of programming
  languages.

The prompt templating feature in Semantic Kernel is quite powerful. Let's take a look at
the internal templating engine first.

### Creating a prompt template in Semantic Kernel

The Semantic Kernel templating language is a text based language. You can write a basic
prompt template like this:

```text
Help me cook something nice, give me a recipe for {{ $dish }}
```

In this template, we ask a basic recipe for a dish. Dish is a variable identified by
`{{ \$dish }}`. We can fill this variable later when we invoke the prompt.

You can invoke the prompt template using the following code:

```csharp
var promptTemplate = File.ReadAllText(
    Path.Join(Directory.GetCurrentDirectory(), "prompt.txt")
);

var result = await kernel.InvokePromptAsync(promptTemplate,
    arguments: new KernelArguments
    {
        ["dish"] = "pizza"
    },
    templateFormat: "semantic-kernel");

Console.WriteLine(result);
```

Let's go over this code to understand what's happening:

1. First, we load a prompt file from disk using the standard .NET I/O functions.
2. Then, we call the `InvokePromptAsync` method on the kernel instance to execute the
   prompt template providing `arguments`, and the `templateFormat`.
3. Finally, we print the result to the console.

We're using a kernel instance in the code sample as described in
[#s](#setting-up-semantic-kernel). You can find the full source code for this sample in
the [GitHub repository][SK_TEMPLATE_SAMPLE].

You can pass in values for the variables in the template by providing a `KernelArguments` object. 
This object is a special type of dictionary that stores both values for use in the template and
a set of hyperparameters used when executing the prompt.

We haven't specified any hyperparameters with the prompt, but you can add those as part
of the arguments for the prompt. You can modify the call to `InvokePromptAsync` to
include execution settings for the LLM provider you're using. The following code
fragment demonstrates this:

```csharp
var promptTemplate = File.ReadAllText(
    Path.Join(Directory.GetCurrentDirectory(), "prompt.txt")
);

var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    MaxTokens = 1000,
    Temperature = 0.5,
    TopP = 0.98,
    FrequencyPenalty = 0.0,
    PresencePenalty = 0.0
};

var result = await kernel.InvokePromptAsync(promptTemplate,
    arguments: new KernelArguments(executionSettings)
    {
        ["dish"] = "pizza",
    },
    templateFormat: "semantic-kernel");
```

In this code snippet we've added execution settings for the Azure OpenAI LLM provider.
The settings are passed to the `KernelArguments` object to specify the hyperparameters
for the prompt.

Semantic Kernel Prompt Templates have limited functionality. For example, you can't use
any loops in the prompt template. If you need more advanced functionality
you can use Handlebars or Liquid templates as an alternative. In the next chapter we'll
look at using Handlebars templates.

### Using Handlebars as an alternative templating language

You can use Handlebars templates through a separate package
`Microsoft.SemanticKernel.PromptTemplates.Handlebars`. This package provides a
Handlebars template engine that you can use to write more complex prompts.

For example, if you want to use a Handlebars template to generate a recipe for a dish
and pair it with a list of ingredients that you have in the fridge, you could write a
template like this:

```handlebars
Help me cook something nice, give me a recipe for {{ dish }}.
Use the ingredients I have in the fridge: 

{{#each ingredients}}
    {{ . }}
{{/each}}
```

A variable in Handlebars is identified by `{{ variable_name }}`. Since handlebars
supports loops, we can use the `#each` statement to loop over the ingredients list.
Handlebars uses helpers to implement the more advanced functionality like loops. You can
find a full explanation of the syntax in the [Handlebars
documentation][HB_MANUAL]. You can use many of the helpers
described in the manual, except for any that need to render templates from other files.

You can invoke the Handlebars template using the following code:

```csharp
var promptTemplate = File.ReadAllText(
    Path.Join(Directory.GetCurrentDirectory(), "prompt.txt")
);

var result = await kernel.InvokePromptAsync(promptTemplate,
    arguments: new KernelArguments
    {
        ["dish"] = "pizza",
        ["ingredients"] = new List<string> 
        { 
            "pepperoni",
            "mozarella",
            "spinach" 
        }
    },
    templateFormat: "handlebars",
    promptTemplateFactory: new HandlebarsPromptTemplateFactory());

Console.WriteLine(result);
```

We perform the following steps in the code:

1. First, we load the handlebars template from disk.
2. Then, we call `InvokePromptAsync` on the kernel instance to execute the prompt
   template specifying that we're rendering a Handlebars template. We must also specify
   that the kernel should use the `HandlebarsPromptTemplateFactory` to render the
   template.
3. Finally, we print the result of the prompt template to the terminal.

Handlebars templates aren't supported out of the box, so you need to provide the correct
template factory instance using the `promptTemplateFactory` argument.

You can find the full source for this sample in the [GitHub
repository][HB_TEMPLATE_SAMPLE], including instructions on how to run the sample
yourself.

### Maximizing reuse of prompts in your application

When you've tried the samples and debugged them in VSCode or your favorite IDE, you'll
have noticed that the output of `InvokePromptAsync` is a `FunctionResult`. Prompts in
Semantic Kernel are turned into callable C# functions called Kernel Functions.

Why would Semantic Kernel do this? Compiling prompts down to program functions helps
make the prompts reusable as program logic. You can store compiled prompts in your
program logic reducing the amount of code you need to run a prompt.

If you find that you use the same prompts over and over in your program it's helpful to
upgrade the prompts from the coding pattern we used in the previous sections to a
reusable kernel function. Let me show you how to create a kernel function from a prompt
with the following code:

```csharp
var promptTemplate = File.ReadAllText(
    Path.Join(Directory.GetCurrentDirectory(), "prompt.txt")
);

var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    MaxTokens = 1200,
    Temperature = 0.5,
    TopP = 1.0,
    FrequencyPenalty = 0.0,
    PresencePenalty = 0.0
};

var prompt = kernel.CreateFunctionFromPrompt(
    promptTemplate, templateFormat: "handlebars",
    promptTemplateFactory: new HandlebarsPromptTemplateFactory(),
    executionSettings: executionSettings);

var result = await kernel.InvokeAsync(prompt, new KernelArguments
{
    ["dish"] = "pizza",
    ["ingredients"] = new List<string>
    {
        "pepperoni",
        "mozzarella",
        "spinach"
    }
});
```

Let's go through this code to understand the differences from earlier code samples:

1. In the first step, we load the prompt template from disk.
2. Next, we specify the execution settings we want to use for the prompt.
3. Then, we call `CreateFunctionFromPrompt` instead of invoking the prompt directly. The
   input for the function contains the prompt template, the prompt template factory we
   want to use, and the execution settings.
4. Finally, we can invoke the new function using `InvokeAsync` on the `kernel` object
   and pass the arguments to the function.

In the sample we store the function in a `prompt` variable. In production code you can
store the prompt as a private variable of a class that serves as a wrapper around the
Semantic Kernel code.

In chapter 6, we'll explore other patterns to efficiently make reusable prompts
available to your application.

I've made sure that the code for building a kernel function is available in the [GitHub
repository][KF_SAMPLE] so you can explore it in greater depth if you want.

Kernel functions are nice step towards fully reusable prompts. But there's one more
step you can take if you want to make your business logic more readable.

### Using YAML-based prompt configuration

As prompts come with additional settings you can consider storing the prompt
configuration with the prompt in a dedicated file. In Semantic Kernel you can use YAML
files to do this. Let me demonstrate what the YAML format for a prompt looks like:

```yaml
name: GenerateRecipe
description: Generates a recipe based on ingredients in your fridge
template: |
  Help me cook something nice, give me a recipe for {{ dish }}. 
  Use the ingredients I have in the fridge: 

  {{#each ingredients}}
      {{ . }}
  {{/each}}
template_format: handlebars
input_variables:
  - name: dish
    description: The name of the dish you want to make
    is_required: true
  - name: ingredients
    description: A list of ingredient names you have in the fridge
    is_required: true
execution_settings:
  default:
    top_p: 0.98
    temperature: 0.7
    presence_penalty: 0.0
    frequency_penalty: 0.0
    max_tokens: 1200
```

There's a lot to unpack here. Let's go over the important properties:

1. `name` Determines the name of the kernel function that we'll create from the YAML
   file.
2. `template` Contains the prompt template for the prompt. This is the same as the
   prompt template we used in the previous sections.
3. `template_format` Specifies the template format we're using for the YAML prompt.
4. `input_variables` Describe the expected input data for the prompt.
5. `execution_settings` Describe the hyperparameters for the prompt.

Depending on your use case you'll want to configure more settings. For a full
description of the YAML format, you can refer to the [Semantic Kernel
documentation](https://learn.microsoft.com/en-us/semantic-kernel/concepts/prompts/yaml-schema).

To use the YAML based prompt files, you'll need to add the
`Microsoft.SemanticKernel.Yaml` package to your project. After you've added it to your
project, you can load the YAML based prompt with the following code:

```csharp

var promptTemplate = File.ReadAllText(
    Path.Join(Directory.GetCurrentDirectory(), "prompt.yaml")
);

var prompt = kernel.CreateFunctionFromPromptYaml(
    promptTemplate, 
    new HandlebarsPromptTemplateFactory());
```

In this code fragment we load the prompt YAML file from disk and then use the
`CreateFunctionFromPromptYaml` method on the kernel to create a kernel function from the
YAML file. As the Handlebars format we've used for the prompt template isn't readily
available, we have to explicitly tell Semantic Kernel that we want to use it for the
prompt template.

The `prompt` variable now contains a kernel function that you can use in your
application.

I think the YAML format is interesting because it provides a way to store prompts with
their execution settings in a single file. I find the `execution_settings` the best
option of this format, because you can configure different execution settings depending on
the LLM provider you're using.

Let me explain why having multiple execution settings is useful with a bit more detail.
Remember from [#s](#llomops-failover-strategies) that it can be useful to have a
failover option in your application. Using the YAML file you can specify multiple
execution settings for different LLM providers.

To support the failover scenario, we need to modify the YAML file to include extra
execution settings:

```yaml
name: GenerateRecipe
description: Generates a recipe based on ingredients in your fridge
template: |
  Help me cook something nice, give me a recipe for {{ dish }}.
  Use the ingredients I have in the fridge: 

  {{#each ingredients}}
      {{ . }}
  {{/each}}
template_format: handlebars
input_variables:
  - name: dish
    description: The name of the dish you want to make
    is_required: true
  - name: ingredients
    description: A list of ingredient names you have in the fridge
    is_required: true
execution_settings:
  default:
    top_p: 0.98
    temperature: 0.7
    presence_penalty: 0.0
    frequency_penalty: 0.0
    max_tokens: 1200
  azure_openai:
    top_p: 0.9
    temperature: 0.7
    presence_penalty: 0.0
    frequency_penalty: 0.0
    max_tokens: 1200
```

The first set of execution settings specify defaults for all LLM providers. The second
set of execution settings are only valid for an LLM provider we registered with a service
ID `azure_openai`.

When creating a kernel, we can add a chat completion service with a Service ID. Here's
the code to do so:

```csharp
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!,
        serviceId: "azure_openai"
    )
    .Build();
```

Now when we want to execute a YAML based prompt with the Azure OpenAI LLM provider, we
can use the following code:

```csharp
var serviceSelection = new PromptExecutionSettings()
{
    ServiceId = "azure_openai"
};

var result = await kernel.InvokeAsync(prompt,
    arguments: new KernelArguments(serviceSelection)
    {
        ["dish"] = "pizza",
        ["ingredients"] = new List<string>
        {
            "pepperoni",
            "mozarella",
            "spinach" 
        }
    });
```

In this code we stack an additional set of execution settings on top of the ones we
specified in the YAML file. You only need to specify the name of the LLM provider you
want to use in the `ServiceId` property of the `PromptExecutionSettings` object and pass
the execution settings into the arguments of the `InvokeAsync` method.

Switching between LLM provider is now as simple as setting a different value for the
`ServiceId` property in the execution settings.

The YAML format does have some limitations. I've found that the YAML format in general
is sensitive to mismatches in spaces and tabs. And editing the text of the template in
the YAML file can be a bit cumbersome. But it's nice to have the option to store prompts
in a single file with multiple sets of excution settings.

**Note:** Working with multiple LLM providers is experimental at the time
of writing. You will need to add `<NoWarn>SKEXP0001</NoWarn>` to the `PropertyGroup`
section of your project file to suppress the build error telling you that the feature is
experimental.

## Using the chat history to your advantage

In [#s](#prompt-templates) we discussed how to use single prompts with Semantic Kernel.
While this is useful for non-chat based scenarios I think it's important to also discuss
chat-based scenarios. When talking about chat-based scenarios you have to keep in mind
that you don't necessarily need a chat user interface. We're strictly talking about
a list of prompts and responses that form a conversation.

You'll need a different approach when working with chat history. We're no longer dealing
with a scenario where we need to generate a single response. Instead, we'll build a
conversation that can have multiple turns and use that as the central unit of content to
work with.

Let's take a look at the chat history object first as this will be the central unit
we'll work with. You can build a conversation using the following piece of code:

```csharp
var chatHistory = new ChatHistory();

chatHistory.AddSystemMessage(
  "You're a digital chef help me cook. Your name is Flora."
);

chatHistory.AddUserMessage(
  "Hi, I'd like a nice recipe for a french style apple pie"
);
```

This code performs the following steps:

1. First, we'll create a new chat history object.
2. Then, we add a new system message containing instructions for the assistant.
3. Finally, we add the first user message asking for an apple pie recipe.

The system instructions make sure that the LLM generates responses in line with the goal
of the application. The system instructions typically also provide instructions
regarding the style of communication and things that we don't like to see in the
response.

You can have three different types of messages in the chat history:

1. System - Reserved for providing system-level instructions that should not be visible
   to a user.
2. User - Content submitted by the user
3. Assistant - Content generated by the LLM

You can add as many messages as you like to the chat history. Once we have a chat
history object we can use it to generate a response using the `IChatCompletionService`.
The following code fragment demonstrates how this works:

```csharp
var chatCompletionService = 
    kernel.Services.GetService<IChatCompletionService>();

var response = await chatCompletionService!
  .GetChatMessageContentsAsync(chatHistory);

Console.WriteLine(response[0].Content);
```

The code fragment performs the following steps:

1. First, we obtain an instance of the `IChatCompletionService` from the kernel.
2. After that, we ask the kernel to return a chat message based on the chat history we
   provided.
3. Finally, we print the content of the first chat message that's returned.

Where invoking a prompt returns just one result, the chat completion service returns
an array of results. Don't get fooled by this array. You can't get multiple results
anymore from Semantic Kernel as many of the LLM providers no longer support
returning multiple results for the same prompt. I expect they'll remove this feature
soon after writing this.

It's good to know that the chat history is serializable to JSON. So you can store it in
a database in serialized form and reuse it later to continue a conversation. Nearly all
databases support storing JSON objects these days so it's quite easy to save the history
of a conversation with a user in a way that you can query it or use it later.

The internal flow of the `IChatCompletionService` is the same is it is for executing
prompts (see [#s](#semantic-kernel-architecture)). The main difference here is that
you're dealing with a longer prompt shaped like a conversation. Filters, and functions
work for chat oriented applications as well as single prompts. But the main goal of the
`IChatCompletionService` is to build chat-oriented applications.

Generating a response for a longer conversation takes a lot of time because you need to
transfer the whole chat history to the model that's going to process it token by token
to produce a response. This is a slow process. It's good to know that Semantic Kernel
supports streaming responses to prompts and in chat scenarios.

You can ask `IChatCompletionService` to stream the response by calling
`GetStreamingChatMessageContentsAsync` instead of `GetChatMessageContentsAsync`. The
streaming method returns an `IAsyncEnumerable` object that you can iterate over using
`await foreach` as the following code demonstrates:

```csharp
var chatCompletionService = kernel.Services
    .GetService<IChatCompletionService>();

var responseIterator = chatCompletionService!
    .GetStreamingChatMessageContentsAsync(chatHistory);

await foreach (var token in responseIterator)
{
    Console.Write(token.Content);
}
```

I've included the full source code in the [GitHub Repository][STREAMING_SAMPLE].

Note that as conversations get longer you'll run out of context space, and you'll need
to decide how to keep the chat history within acceptable limits. It is up to you to
decide how you want to handle this. You can choose to truncate older messages from
the history or summarize the earlier messages to keep a shorter version around.

In my experience it works well to truncate older messages from the chat history
until you have a consistent set of messages within your desired context window size.
I've never had to resort to summarization of the history. This is because in chat
scenarios older messages become less relevant as the conversation goes on. In some cases
I've seen the LLM flip out because I had unrelated content in the chat history. So it's
good to keep the chat history focused on the task at hand.

You can use code from the [Microsoft Semantic Kernel GitHub repository][TRUNCATION_SAMPLE]
as a good starting point to implement truncation logic in your application.

Working with a chat history is a bit more complex than working with a single prompt. But
it's essential when you're building an assistant-like use case. It's good to know that
you can mix and match prompts and chat in such a scenario. We'll explore this in the
chapter 6 when we look at using functions with Semantic Kernel.

Prompt engineering is an art, and it can be dangerous if you're not careful. Let's
look at how you can protect your prompts from abuse.

## Security considerations when using prompts

In [#s](#llmops-application-security) we discussed the importance of a layered defense.
The layered defense starts with the prompt execution proces. We'll need to make sure
that the input and output of the prompt are filtered for any unwanted content. We also
need to make sure that we reduce the risk of prompt injection.

### Filtering executable code from prompts and the model output

One attack vector that's impacting prompt engineering is called prompt injection. This
class of attack is used by malicious actors to inject content into the prompt that
produces output that's executed by the application in unwanted ways.

For example, you can construct prompts that produce harmful javascript that your browser
is going to execute if you don't filter out the executable code from the response.

By default, the template engines used in Semantic Kernel escape javascript, CSS, and
script tags in the prompt input and output. This way you're somewhat protected against
abuse if you're rendering content in the browser. However, if you're going to use the
output of a prompt as code to be executed in the context of your application you should
be aware that it can't be trusted. I wouldn't do it at all, but if you have to, be sure
to limit what that code has access to.

Keep in mind that filtering the input and output of the LLM is an important step, but 
will not be enough to protect your application. We'll cover more steps in chapter 6 as
we look at using functions with Semantic Kernel.

Fully understanding just how dangerous prompt injection is requires a deep dive into the
security and is beyond the scope of the book. However, I recommend reading the [MITRE
Atlas][MITRE] and the [OWASP Top 10 for LLMs][OWASP_LLM] to get a better understanding
of how hackers are abusing prompts these days.

### Filtering PII from the prompt and the model output

Many organizations want to make sure that they're not exposing personal information to
the LLM because they're worried that the LLM provider may store that information. This
is a valid concern, although many LLM providers have provisions in their contract that
handle this concern quite well.

If you have to filter PII from the input of the prompt you can use a filter to do so.
Let me show you a quick example of how to build a filter for this purpose:

```csharp
public class PIIFilter: IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(
      PromptRenderContext context, 
      Func<PromptRenderContext, Task> next)
    {
        // This function is called when the prompt is rendered. This is where
        // we can filter the contents of the prompt before it's submitted.
        
        var renderedPrompt = context.RenderedPrompt;
        
        //TODO: Filter the prompt contents
        
        // Replace the original prompt with the filtered prompt.
        context.RenderedPrompt = renderedPrompt; 
        await next(context);
    }
}
```

In this code we perform the following steps:

1. First, we create a class that implements the `IPromptRenderFilter` interface.
2. Next, we implement the `OnPromptRenderAsync` to filter the rendered prompt.

To use the filter, we can configure it in the kernel using the following code:

```csharp
kernel.PromptRenderFilters.Add(new PIIFilter());
```

Note that I haven't included any actual PII filtering code in the filter. If you're
interested in learning how to use one of the popular PII filtering tools, please find
the documentation for each of them here:

1. [Azure PII Detection][AZ_PII_DETECTION]
2. [Google de-identification][GA_PII_DETECTION]
3. [AWS PII detection][AWS_PII_DETECTION]

I recommend getting rid of PII as soon as it enters the application if you have to.
There's less chance of it leaking anywhere the sooner you remove it. But sadly, I can't
tell you how to apply any of these services specifically, because I'm not building your
application.

## Summary

In this chapter we've looked at the art and nonsense of prompt engineering. We discussed
the important role prompts play in LLM-based applications and how to write effective
prompts. We also covered the importance of testing and monitoring your prompts so you
can ensure that your application will still work in 6 months time.

In the next chapter, we'll look at testing and monitoring prompts in your
LLM-based application.

[CONTEXT_WINDOW_PAPER]: https://arxiv.org/abs/2307.03172
[LLM_ALIGNMENT]: https://medium.com/@madalina.lupu.d/align-llms-with-reinforcement-learning-from-human-feedback-595d61f160d5
[TOP_P_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/notebooks
[SK_TEMPLATE_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-04/csharp/Chapter4.SemanticKernelTemplates
[HB_TEMPLATE_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-04/csharp/Chapter4.HandleBarsTemplates
[KF_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-04/csharp/Chapter4.KernelFunctionPrompts
[HB_MANUAL]: https://handlebarsjs.com/guide/
[STREAMING_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-04/csharp/Chapter4.StreamingChatCompletions
[TRUNCATION_SAMPLE]: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/MultipleProviders_ChatHistoryReducer.cs
[AZ_PII_DETECTION]: https://learn.microsoft.com/en-us/azure/ai-services/language-service/personally-identifiable-information/how-to-call
[GA_PII_DETECTION]: https://cloud.google.com/sensitive-data-protection/docs/deidentify-sensitive-data
[AWS_PII_DETECTION]: https://docs.aws.amazon.com/comprehend/latest/dg/how-pii.html
[OWASP_LLM]: https://owasp.org/www-project-top-10-for-large-language-model-applications/
[MITRE]: https://atlas.mitre.org/matrices/ATLAS
