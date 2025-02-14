{#the-art-and-nonsense-of-prompt-engineering}
# The art and nonsense of prompt engineering

In [#s](#getting-started-with-semantic-kernel), we got the first chance to work with Semantic Kernel to execute a basic prompt. Prompts are the most important part of working with large language models.

I've seen so many posts on social media about prompt engineering that it almost made no sense to me to discuss it. It feels as if everyone already knows everything about prompt engineering. But when I look at the tips and tricks people share and compare them to what I've seen in production applications, I'm worried we're forgetting something important.

LLMs are powerful tools, but they're also susceptible to the input they receive. Prompt engineering is a very unnatural use of language. We're not writing for humans; we're writing for a statistical model, providing us with all sorts of strange challenges.

In this chapter, we'll dive deeper into the art and science of prompt engineering. We'll cover the following topics:

- Why are prompts necessary for effective LLM-based applications?
- The five basics of a good prompt
- Using prompt templates for reusability
- Prompt testing and iterating on your prompts
- Monitoring prompt interactions in production
- Security considerations when using prompts

At the end of this chapter, you'll understand how to write effective, prompt templates for your LLM-based application. You'll also understand the difference between chat and non-chat use cases when using an LLM.

Let's get started by exploring why prompt engineering is so popular.

## Why are prompts necessary for effective LLM-based applications?

Prompt engineering is super popular today because it's the cornerstone of using an LLM. Let's first understand why prompt engineering is such an important concept when working on LLM-based applications.

### Why prompt engineering matters

Large Language Models can process language and produce language. Prompts are the only method to control an LLM. But there's something positively weird about using prompts. When you work with ChatGPT, you're likely going write a prompt that looks like this:

```text
You're a world-class chef, and you're preparing a meal.
Provide a recipe for apple pie please.
```

This sounds like a typical sentence to many of you when talking to ChatGPT. But it's unnatural. If I'm talking to a chef, I don't need to establish that they're a world-class chef. They already know about that or are just a good chef, or maybe even my grandma. I don't need to encourage their natural behavior.  The second thing that stands out to me is this. Why am I saying please to a machine? What does that get me?

Some people even resort to offering money to the LLM or threatening it with termination for better results. But that's not how LLMs work.

The apple pie prompt works because we establish a pattern for the model to work with to produce the best possible output tokens. Remember from [#s](#understanding-llms) that an LLM can only predict the next most likely output token. Sampling is also used to make it sound natural. We're dealing with a neural network that's a mathematical concept pattern-matching input to get the correct output. That means we need to think about prompts not as standard sentences but as computer instructions.

For a prompt to be effective, we must use the attention mechanism and the embeddings to our advantage. Funny enough, the weirdest combinations of words let the embeddings and attention mechanism of the LLM work best—the more precise the pattern, the better the output.

### Common misconceptions about prompts

I want to address a few common misconceptions about prompts before we discuss the rest of prompt engineering.

#### Prompts are static

While you can reuse prompts over time, you'll have to account for the fact that LLMs get updated and retrained periodically. A prompt that worked before will likely break because the LLM was updated. Prompts that may have worked with Claude will produce different results with GPT-4o because they used a different training set.

{#context-window-limits}
#### Complicated prompts yield better results

Some people will tell you to write complicated prompts to get better results. But you will often get better results by splitting complex tasks into smaller tasks and running them separately. This is because of how the context window of the LLM works. From [this paper][CONTEXT_WINDOW_PAPER], we can learn that LLMs have an attention span that isn't exactly what you'd expect from a machine. Although the models have been updated since the paper's release, I still see this behavior in our production applications. [#s](#context-window-curve) shows the attention span of a typical LLM. Input at the start and end of a prompt is likely to get picked up better by the LLM than content in the middle of the prompt.

{#context-window-curve} 
![Context window attention curve](context-window-curve.png)

Longer and more complicated prompts can work, but if the LLM doesn't give you the desired response, it's essential to understand the context window curve and adjust your application accordingly.

#### You can rely on the general knowledge captured in the LLM

LLMs are trained on a lot of data. As a result, you can get a recipe for apple pie that looks very reasonable. But you can't rely on the general knowledge captured in the LLM. There's a high statistical chance of getting a reasonable response, but the LLM may lie to you convincingly. After all, we're just matching a pattern. The LLM doesn't remember facts; it just predicts the next token based on the pattern it saw.

There's a lot that can go sideways with prompts. However, understanding prompt engineering can be very helpful in building effective LLM-based applications.

{#prompt-principles} 
## The five basics of a good prompt

Writing a good prompt is hard because you have to think like the LLM you're working with. If you want to be close to 100% effective, you'll need to know:

- What data the LLM was trained with so you know which language (Dutch, English, Spanish, Chinese, etc.)
it understands best.
- What tasks was the LLM trained on to write the prompt using the same structure and style so the pattern is as straightforward as possible?
- How was the LLM [aligned][LLM_ALIGNMENT], so you know the style of text the LLM can produce?

Sadly, you can't know these three facts as hard facts. There are benchmarks, but I've found that they only provide a biased guideline, as many LLM providers are likely to game them. You'll have to guess what works best because the datasets used to train LLMs aren't open-source. They're a well-guarded secret by the companies that build them.

That leaves us with five basic principles that I know from experience work well for prompts:

- Provide clear direction
- Specify the output format for the prompt
- Add context and samples to the prompt
- Keep the prompt focused on one task
- Tune your prompt with hyperparameters

Let's go through them one by one.

### Provide clear direction

Take a step back and think about how the LLM works. Remember that the LLM is a pattern-matching machine that uses embeddings (modeling patterns of relationships between words in a sentence) and an attention mechanism (modeling patterns from the context of the prompt and the response) to produce results. The LLM is only as good as the pattern it can match.

Vague instructions aren't going to work. You may get lucky, but you're more likely to get too much variation in response as more users use your application. And that's okay for chat applications like ChatGPT, but it's not for business applications where users expect high-quality results.

Two things help generate a pattern that the LLM can match. First, it's beneficial to provide the LLM with a clear context. For example, you can give it a persona to provide the context or tell it what project/task you're working on. Second, it's essential to provide the LLM with detailed instructions about the response's content, style, and structure.

The context helps set the embeddings and attention mechanism of the LLM, so it starts searching for output tokens in the correct word space. For example, the persona of a chef will likely lead the LLM in the direction of food, recipes, cooking techniques, and so on. A copywriter persona primes the embeddings and attention mechanism in the direction of marketing, text writing, and publishing. In the case of the copywriter, you'll also need to provide the medium in which the text is intended. Copywriting is different for a blog post than it is for a book.

The detailed instructions help the LLM follow a specific style pattern. For example, if you ask for short responses, the LLM is likelier to produce short outputs.

### Specify the output format for the prompt

Next, we have to consider the prompt's output format. In chat applications, it's often enough not to specify the output format at all. LLMs are trained as chatbots, and they will produce conversational output. But if you want to use the LLM for other tasks, it's essential to tell the LLM precisely what output format you want.

For example, you can tell the LLM to output a list of items or a table or to put a specific portion of the output in a fenced Markdown code block. This outputs the results of the prompt between \`\`\` and \`\`\` so it's easily parsable by your application.

Let me demonstrate by asking the LLM to give me the apple pie recipe we used at the start of the chapter but put it in a code block so I can easily extract it later.

Here's the prompt:

```text
You're a world-class chef, and you're preparing a meal. 
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
- 6 cups (about five large) apples, peeled, cored, and thinly sliced
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

Note that the response contains the recipe between Markdown fenced code block delimiters. We can now extract the recipe from the response using a regular expression.

While this looks like an excellent way of working with LLMs in use cases outside of chat, there are better ways to handle structured outputs. You can instruct the LLM to generate JSON results or even call a function to output the generated code. Both of these options provide much more control over the output format. We'll cover structured output in greater detail in chapter 8.

Next, let's look at how you can help the model with samples.

{#few-shot-learning}
### Add samples to the prompt

Any LLM can reproduce patterns learned from the large body of text on which it was trained. But that body of text hopefully doesn't contain internal documents of the client or company you work for. So, if you need to answer questions about internal information, you must tell the LLM about the information.

We'll cover Retrieval Augmented Generation in greater depth in chapter 7. But for now, it's essential to understand that the LLM can't remember facts. It's a talking parrot that reproduces patterns it has seen before. This looks like a limitation, but you can turn this into a powerful trait.

For example, I want to answer a question about the company's policy on remote work. In that case, I can first look up documents related to the question by performing a similarity search using a search engine. The documents I can put into the prompt as additional context and then instruct the LLM to answer the question. A prompt for answering questions typically will look like this:

```text
You're a digital assistant for our company's HR department. 
Please answer the employee's question based on the content 
provided as context.

## Context

<The found fragments about the policy on remote work>

## Question

<The employee question>
```

You can consider the context heading in the prompt as samples that the model can use to generate a helpful response. Because of the talking parrot effect, the LLM will likely reproduce the content you provided.

If your sample is incorrect, vague, or non-existent, then the LLM can't match the pattern, and you'll get an answer that doesn't make much sense or is plain misleading. So it's essential to ensure you only inject safe and sensible information.

Adding samples to a prompt helps establish a good pattern for the LLM. You'll see many people add examples of their desired results to a prompt. Adding examples to your prompt is called one-shot learning or few-shot learning if you add more than one example of the expected output. Let's look at an example to understand what that looks like:

~~~text
You're a world-class chef, and you're preparing a meal. Provide a recipe for apple pie please. Output the result in JSON; use the following example as a guide.

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

We'll get a completely different response from the previous sample when we run the prompt. The LLM will now generate a JSON object for your recipe.

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

In this case, we received a good response with just one sample in the prompt. If one sample doesn't help, don't be afraid to add more samples to help the LLM match your desired pattern. Providing enough variety in the samples will help with the quality of the output.

While adding multiple samples to the prompt is helpful, adding multiple tasks isn't. Let's take a look.

### Keep the prompt focused on one task

LLMs are good at many things but not at performing multiple tasks in one prompt. You'll get better results if you focus the prompt on one task.

Getting a recipe for apple pie is a single task. Answering a question can still be considered one task. Generating an article about the powerful features of an LLM sounds like one task but, at the very least, involves multiple steps.

If a prompt generates a vague answer even with the previous tips, you likely have a complex task requiring multiple steps. In that case, you'll want to split the prompt into various prompts for each task step.

While LLMs have a large context window and are capable of following steps, they're not very good at it. According to the author, you can find many so-called chain-of-thought prompts on the Internet that produce fantastic results.

For example, I can improve my apple pie recipe prompt by adding one sentence:

```text
You're a world-class chef. You are preparing a meal.
Give me a recipe for apple pie. Take it step by step.
```

The one sentence "take it step by step" turns the prompt into a chain of thought. I'm asking the LLM to generate a more detailed response. Because it's generating a more detailed response, I will likely get something that resonates better with my expectations.

Let's return to what I wrote at the start of [#s](prompt-principles): We're dealing with a pattern-matching machine. So, what happens here? I provide a pattern that causes the LLM to follow a more detailed style. Because it follows a more detailed style, the attention mechanism is influenced as the response gets longer. The result is that it generates a complete description and no longer skips steps because, in a more detailed description of apple pie, that is statistically the most likely thing.

The recipe looks better, but it doesn't have to be correct. You need to verify the LLM's output here. It will be harder to validate because you need to look at more text. Chain-of-thought prompts lack control in many cases. You must address this to provide stable results to your users.

Chain-of-thought prompts work with tools like ChatGPT because there is nothing better there. However, you can't program the workflow in ChatGPT.

For LLM-based applications, I recommend skipping chain-of-thought prompts. Instead, I recommend splitting the task into multiple prompts and using logic or one of the design patterns in the remaining chapters of this book to come to a complete result.

### Tune your prompt with hyperparameters

The last basic principle of a good prompt is to tune it with hyperparameters. In the previous sections, we've only looked at the prompt's text, but that's only one part of the equation.

Remember from [#s](#llm-output-sampling) that the LLM uses sampling to make the output look natural. Several parameters control the sampling, which you can set when using a tool like Semantic Kernel. These parameters are called hyperparameters; depending on the provider, you may encounter different variations on these parameters. Here are the most important ones:

- Top-P
- Temperature
- Presence Penalty
- Frequency Penalty

This part is more advanced. But it's important to understand these parameters and how they interact with the LLM so you know what value to choose for various tasks to get good-quality responses.

#### Top-P

Let's start with the Top-P setting, which refers to how LLMs select the output token that should come next in the response. The neural network of the LLM doesn't predict words; it predicts numbers that are then translated to tokens (words and parts of words) using a dictionary lookup. The LLM translates the numbers in the output layer to a probability distribution to select the next output token in the response. Each possible token is assigned a value representing the likelihood that it will be the next token. The higher the value, the more likely the token is to be the next token in the output.

We could select the token with the highest probability as the output from this probability distribution. However, this leads to repetitive and boring output. We can fix this by using a smarter sampling technique.

All modern LLMs use Top-P sampling. Here's how it works: First, we establish a threshold for the cumulative probability. We then sort the tokens from highest to lowest probability and start collecting tokens, adding their probabilities to a cumulative probability. We collect tokens until the cumulative probability value reaches the threshold. The process is demonstrated in [#s](#top-p-sampling).

{#top-p-sampling} 
![Top-P sampling](probability-visualization.png)

For the samples in [#s](#top-p-sampling), we've established a threshold of 0.9. The tokens with the highest probability are selected until the cumulative probability reaches 0.9. The tokens with the lowest probability are removed from the distribution.

Once we've selected the candidate tokens in the distribution, we randomly select one of them as the output, resulting in a more varied output.

The key thing to remember here is that a higher Top-P results in more tokens being selected as candidates. A lower value will result in a more focused, repetitive output. This is ideal for generating code but less so for marketing content where you want greater variety.

#### Temperature

LLMs don't just use Top-P sampling. The inventors of these types of models found that Top-P sampling isn't enough to get natural text. So, they decided to add temperature to the mix. Temperature controls the shape of the probability distribution used to perform Top-P sampling.

Here's how temperature works. Before performing the Top-P sampling, the raw values for the candidate tokens are divided by the temperature value. After that, the probability distribution is determined as usual. In [#s](#temperature-effects), you can see this in action with a before and after.

{#temperature-effects} 
![The effect of temperature on the probability distribution](scaled-probability-comparison.png)

The higher the temperature, the softer the curve is. It takes longer for the cumulative probability function to reach the threshold, so more candidate tokens make it into the selection. Conversely, the lower the temperature, the harder the curve is. It takes less time to reach the threshold.

At this point, I understand that you're probably thinking that this is a lot to get through. And it is. I've included an interactive notebook that demonstrates Top-P sampling and temperature scaling in the [GitHub repository][TOP_P_SAMPLE]. You can run the notebook inside Visual Studio Code to explore the effects of temperature and Top-P sampling and better understand these concepts.

#### Presence Penalty

As you can tell by now, the sampling techniques used in LLMs are complex. And it gets even more complicated. Let me add one more variable to the mix: presence penalty. A positive presence penalty reduces the probability value of a token seen before in the output. This forces candidate tokens lower in the selection ranking for the Top-P sampling process. It is less likely that you see the same token in the output if you provide a positive value for the presence penalty.

#### Frequency Penalty

The frequency penalty is the last hyperparameter we need to discuss. This hyperparameter looks similar to the presence penalty hyperparameter but uses a different approach. Instead of applying a flat penalty to tokens that occurred before in the output, a frequency penalty applies a penalty to the probability of tokens that frequently appear in the output. The more often a token appears in the output, the higher the penalty and the less likely the token is to be selected during the Top-P sampling process.

#### What to choose for each of the hyperparameters

I know this is a lot to take in, so let me give you some direction on what to choose for each parameter.

Coding requires a more repetitive and boring output to be effective, so you want to choose a lower value for Top-P and a lower value for temperature. The presence penalty and frequency penalty should be set to 0 because we don't want to apply any penalties. Coding is repetitive by nature.

I recommend setting the Top-P value higher and applying a higher temperature for general-purpose tasks like generating marketing content. The presence penalty and frequency penalty can help make the text more varied. Don't go overboard, though; a low value is often enough. Setting the presence and frequency penalty to a high value leads to fascinating but often nonsensical output.

You must test your prompts with various inputs and settings to establish what works for most cases. Because the probability distribution values ultimately depend on your prompt's content, you likely need to adjust the hyperparameters a little bit based on that.

Once you've written a good-quality prompt, you may want to keep it around for longer. For this, it's nice to have some sort of templating system in place. Let's take a look at what Semantic Kernel has to offer.

{#prompt-templates} 
## Writing prompt templates for reusability

Writing your prompts in line with other C# codes is never a good plan. It's hard to read and maintain, and you can't reuse it. That's why Semantic Kernel offers a way to write prompt templates using a variety of templating languages:

- Semantic Kernel Templates: The internal format developed by Microsoft.
- Handlebars: A popular templating language that is available for many
programming languages.
- Liquid: An alternative to handlebars that is also available for several programming languages.

The prompt templating feature in Semantic Kernel is quite powerful. Let's first examine the internal templating engine.

### Creating a prompt template in Semantic Kernel

The Semantic Kernel templating language is a text-based language. You can write a basic prompt template like this:

```text
Help me cook something nice, give me a recipe for {{ $dish }}
```

In this template, we ask for a basic recipe for a dish. Dish is a variable identified by `{{ \$dish }}`. We can fill this variable later when we invoke the prompt.

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

The code sample uses a kernel instance as described in [#s](#setting-up-semantic-kernel). The full source code for this sample is in the [GitHub repository][SK_TEMPLATE_SAMPLE].

You can pass in values for the variables in the template by providing a `KernelArguments` object. This object is a special type of dictionary that stores both values for use in the template and a set of hyperparameters used when executing the prompt.

We haven't specified any hyperparameters with the prompt, but you can add those as part of the arguments for the prompt. You can modify the call to `InvokePromptAsync` to include execution settings for your LLM provider. The following code fragment demonstrates this:

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

In this code snippet, we've added execution settings for the Azure OpenAI LLM provider. The settings are passed to the `KernelArguments` object to specify the prompt's hyperparameters.

Semantic Kernel Prompt Templates have limited functionality. For example, you can't use any loops in the prompt template. You can use Handlebars or Liquid templates as an alternative if you need more advanced functionality. In the next chapter, we'll look at using Handlebars templates.

### Using Handlebars as an alternative templating language

You can use Handlebars templates through a separate package, `Microsoft.SemanticKernel.PromptTemplates.Handlebars`. This package provides a Handlebars template engine for writing more complex prompts.

For example, if you want to use a Handlebars template to generate a recipe for a dish and pair it with a list of ingredients that you have in the fridge, you could write a template like this:

```handlebars
Help me cook something nice, give me a recipe for {{ dish }}.
Use the ingredients I have in the fridge: 

{{#each ingredients}}
    {{ . }}
{{/each}}
```

A variable in Handlebars is identified by `{{ variable_name }}`. Since handlebars support loops, we can use the `#each` statement to loop over the ingredients list. You can refer to the current item in the loop using `.` as a variable. Handlebars use helpers to implement more advanced functionality like loops. You can find a full explanation of the syntax in the [Handlebars documentation][HB_MANUAL]. You can use many of the helpers described in the manual, except for any that need to render templates from other files.

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
            "mozzarella",
            "spinach" 
        }
    },
    templateFormat: "handlebars",
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);

Console.WriteLine(result);
```

We perform the following steps in the code:

1. First, we load the handlebars template from the disk.
2. Then, we call `InvokePromptAsync` on the kernel instance to execute the prompt
template specifying that we're rendering a Handlebars template. We must also specify that the kernel should use the `HandlebarsPromptTemplateFactory` to render the template.
3. Finally, we print the result of the prompt template to the terminal.

Handlebars templates aren't supported out of the box, so you must provide the correct template factory instance using the `promptTemplateFactory` argument.

The full source for this sample is in the [GitHub repository][HB_TEMPLATE_SAMPLE], which includes instructions on how to run it yourself.

{#reusable-prompts}
### Maximizing the reuse of prompts in your application

When you've tried the samples and debugged them in VSCode or your favorite IDE, you'll have noticed that the output of `InvokePromptAsync` is a `FunctionResult`. Prompts in Semantic Kernel are turned into callable C# functions called Kernel Functions.

Why would Semantic Kernel do this? Compiling prompts down to program functions helps make the prompts reusable as program logic. You can store compiled prompts in your program logic, reducing the amount of code you need to run a prompt.

If you use the same prompts repeatedly in your program, it's helpful to upgrade the prompts from the coding pattern we used in the previous sections to a reusable kernel function. Let me show you how to create a kernel function from a prompt with the following code:

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

1. First, we load the prompt template from the disk.
2. Next, we specify the execution settings we want to use for the prompt.
3. Then, we call `CreateFunctionFromPrompt` instead of invoking the prompt directly. The
input for the function contains the prompt template, the prompt template factory we want to use, and the execution settings.
4. Finally, we can invoke the new function using `InvokeAsync` on the `kernel` object
and pass the arguments to the function.

In the sample, we store the function in a `prompt` variable. In production code, you can store the prompt as a private variable of a class that wraps around the Semantic Kernel code.

In [#s](#enhancing-llms-with-tools), we'll explore other patterns for efficiently making reusable prompts available to your application.

I've made sure that the code for building a kernel function is available in the [GitHub repository][KF_SAMPLE] so you can explore it in greater depth.

Kernel functions are a nice step towards fully reusable prompts. But if you want to make your business logic more readable, there's one more step.

{#yaml-based-prompts} ### Using YAML-based prompt configuration

As prompts come with additional settings, consider storing the prompt configuration with the prompt in a dedicated file. In Semantic Kernel, you can use YAML files. Let me demonstrate what the YAML format for a prompt looks like:

```yaml
name: GenerateRecipe
description: Generates a recipe based on ingredients in your fridge
template: |
    Help me cook something nice, give me a recipe for {{ dish }}. 
    Use the ingredients I have in the fridge: 

    {{#each ingredients}}
    - {{ . }}
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

There's a lot to unpack here. Let's go over the essential properties:

1. `name` Determines the name of the kernel function that we'll create from the YAML
file.
2. `template` Contains the prompt template for the prompt. This is the same as the prompt template we used in the previous sections.
3. `template_format` Specifies the template format we use for the YAML prompt.
4. `input_variables` Describe the expected input data for the prompt.
5. `execution_settings` Describe the hyperparameters for the prompt.

Depending on your use case, you'll want to configure more settings. For a complete description of the YAML format, you can refer to the [Semantic Kernel documentation](https://learn.microsoft.com/en-us/semantic-kernel/concepts/prompts/yaml-schema).

You'll need to add the `Microsoft.SemanticKernel.Yaml` package to your project to use the YAML-based prompt files. After you've added it to your project, you can load the YAML-based prompt with the following code:

```csharp

var promptTemplate = File.ReadAllText(
    Path.Join(Directory.GetCurrentDirectory(), "prompt.yaml")
);

var prompt = kernel.CreateFunctionFromPromptYaml(
    promptTemplate, 
    new HandlebarsPromptTemplateFactory());
```

In this code fragment, we load the prompt YAML file from the disk and then use the `CreateFunctionFromPromptYaml` method on the kernel to create a kernel function from the YAML file. As the Handlebars format we've used for the prompt template isn't readily available, we have to explicitly tell Semantic Kernel that we want to use it for the prompt template.

The `prompt` variable now contains a kernel function you can use in your application.

The YAML format is interesting because it allows you to store prompts with their execution settings in a single file. I find the `execution_settings` option the best option for this format because you can configure different execution settings depending on the LLM provider you're using.

Let me explain why having multiple execution settings is helpful with a bit more detail. Remember from [#s](#llomops-failover-strategies) that having a failover option in your application can be beneficial. Using the YAML file, you can specify multiple execution settings for different LLM providers.

To support the failover scenario, we need to modify the YAML file to include extra execution settings:

```yaml
name: GenerateRecipe
description: Generates a recipe based on ingredients in your fridge
template: |
   Help me cook something nice, give me a recipe for {{ dish }}.
   Use the ingredients I have in the fridge: 

   {{#each ingredients}}
   - {{ . }}
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

The first set of execution settings specifies defaults for all LLM providers. The second set of execution settings is only valid for an LLM provider we registered with a service ID of `azure_openai`.

When creating a kernel, we can add a chat completion service with a Service ID. Here's the code to do so:

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

Now, when we want to execute a YAML-based prompt with the Azure OpenAI LLM provider, we can use the following code:

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
            "mozzarella",
            "spinach" 
        }
    });
```

In this code, we stack an additional set of execution settings on top of the ones specified in the YAML file. You only need to select the name of the LLM provider you want to use in the `ServiceId` property of the `PromptExecutionSettings` object and pass the execution settings into the arguments of the `InvokeAsync` method.

Switching between LLM providers is now as simple as setting a different value for the `ServiceId` property in the execution settings.

The YAML format does have some limitations. I've found that it is sensitive to mismatches in spaces and tabs. Editing the template text in the YAML file can be a bit cumbersome. But it's nice to have the option to store prompts in a single file with multiple sets of execution settings.

Working with multiple LLM providers is experimental at the time of writing. You must add `<NoWarn>SKEXP0001</NoWarn>` to your project file's `PropertyGroup` section to suppress the build error telling you that the feature is experimental. The following code shows how to turn off the warning:

```xml
<PropertyGroup>
    <NoWarn>SKEXP0001</NoWarn>
</PropertyGroup>
```

{#working-with-chat-completion}
## Using the chat history to your advantage

In [#s](#prompt-templates), we discussed using single prompts with Semantic Kernel. While this is useful for non-chat-based scenarios, I think it's important to also discuss chat-based scenarios. When talking about chat-based scenarios, you have to keep in mind that you don't necessarily need a chat user interface. We're strictly talking about a list of prompts and responses that form a conversation.

You'll need a different approach when working with chat history. We're no longer dealing with a scenario where we must generate a single response. Instead, we'll build a conversation that can have multiple turns and use that as the central unit of content to work with.

Let's look at the chat history object first, as this will be the central unit we'll work with. You can build a conversation using the following piece of code:

```csharp
var chatHistory = new ChatHistory();

chatHistory.AddSystemMessage(
    "You're a digital chef; help me cook. Your name is Flora."
);

chatHistory.AddUserMessage(
    "Hi, I'd like a nice recipe for a French-style apple pie."
);
```

This code performs the following steps:

1. First, we'll create a new chat history object.
2. Then, we add a new system message containing instructions for the assistant.
3. Finally, we add the first user message asking for an apple pie recipe.

The system instructions ensure that the LLM generates responses that are in line with the application's goal. They typically also provide instructions regarding the style of communication and things that we don't like to see in the response.

You can have three different types of messages in the chat history:

1. System - Reserved for providing system-level instructions that should not be visible to a user.
2. User - Content submitted by the user
3. Assistant - Content generated by the LLM

You can add as many messages as you like to the chat history. Once we have a chat history object, we can use it to generate a response using the `IChatCompletionService`. The following code fragment demonstrates how this works:

```csharp
var chatCompletionService = 
    kernel.Services.GetService<IChatCompletionService>();

var response = await chatCompletionService!
    .GetChatMessageContentsAsync(chatHistory);

Console.WriteLine(response[0].Content);
```

The code fragment performs the following steps:

1. First, we obtain an instance of the `IChatCompletionService` from the kernel.
2. After that, we ask the kernel to return a chat message based on our provided chat history.
3. Finally, we print the content of the returned first chat message.

Where invoking a prompt returns just one result, the chat completion service returns an array of results. Don't get fooled by this array. You can't get multiple results anymore from Semantic Kernel, as many of the LLM providers no longer support returning various results for the same prompt. I expect they'll remove this feature soon after writing this.

You can serialize the chat history to JSON. So, you can store it in a database in serialized form and reuse it later to continue a conversation. However, I recommend you don't do this. For one, the chat history includes the system prompt, and you may want to update it later to provide extra instructions. Also, since the LLM executes the chat history content, you open the door to poisoning chat history data with malicious data that gets executed without your supervision.

The internal flow of the `IChatCompletionService` is the same as it is for executing prompts (see [#s](#semantic-kernel-architecture)). The main difference is that you're dealing with a more extended prompt shaped like a conversation. Filters and functions work for chat-oriented applications as well as single prompts. However, the main goal of the `IChatCompletionService` is to build chat-oriented applications.

Generating a response for a more extended conversation takes a lot of time because you need to transfer the whole chat history to the model, which will process it token by the token to produce a response. This is a slow process. It's good that Semantic Kernel supports streaming responses to prompts and chat scenarios.

You can ask `IChatCompletionService` to stream the response by calling `GetStreamingChatMessageContentsAsync` instead of `GetChatMessageContentsAsync.` The streaming method returns an `IAsyncEnumerable` object that you can iterate over using `await foreach` as the following code demonstrates:

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

I've included the complete source code in the [GitHub Repository][STREAMING_SAMPLE].

Note that as conversations get longer, you'll run out of context space and need to decide how to keep the chat history within acceptable limits. You can truncate older messages from the history or summarize the earlier ones to keep a shorter version around.

In my experience, it works well to truncate older messages from the chat history until you have a consistent set of messages within your desired context window size. I've never had to resort to summarizing the history. This is because, in chat scenarios, older messages become less relevant as the conversation continues. Sometimes, I've seen the LLM flip out because I had unrelated content in the chat history. So, it's good to keep the chat history focused on the task at hand.

Semantic Kernel includes two components to help you manage the length of the chat history:

- `ChatHistoryTruncationReducer` truncates older messages that aren't essential to the LLM.
- `ChatHistorySummarizationReducer` summarizes the non-essential chat messages.
You can learn more about using these history reducers in [the product documentation][HISTORY_REDUCERS]

Working with a chat history is more complex than a single prompt. But it's essential when you're building an assistant-like use case. Knowing you can mix and match prompts and chat in such a scenario is good. We'll explore this in #[s](#enhancing-llms-with-tools) when we look at using functions with Semantic Kernel.

Prompt engineering is an art, but it can also be dangerous if you're not careful. Let's examine how to protect your prompts from abuse.

## Security considerations when using prompts

In [#s](#llmops-application-security), we discussed the importance of a layered defense. The layered defense starts with the prompt execution process. We'll need to filter the prompt's input and output for unwanted content and reduce the risk of prompt injection.

### Filtering executable code from prompts and the model output

One attack vector impacting prompt engineering is prompt injection. Malicious actors use this class of attack to inject content into the prompt, producing output that the application executes in unwanted ways.

For example, you can construct prompts that produce harmful javascript that your browser will execute if you don't filter out the executable code from the response.

By default, the template engines used in Semantic Kernel escape javascript, CSS, and script tags in the prompt input and output. This way, you're somewhat protected against abuse if you render content in the browser. However, if you're going to use the output of a prompt as code to be executed in the context of your application, you should be aware that it can't be trusted. I wouldn't do it at all, but if you have to, limit what that code has access to.

Keep in mind that filtering the LLM's input and output is an important step, but it will not be enough to protect your application. We'll cover more steps in [#s](#enhancing-llms-with-tools) as we look at using functions with the Semantic Kernel.

Fully understanding just how dangerous prompt injection is requires a deep dive into security and is beyond the book's scope. However, I recommend reading the [MITRE Atlas][MITRE] and the [OWASP Top 10 for LLMs][OWASP_LLM] to get a better understanding of how hackers are abusing prompts these days.

### Filtering PII from the prompt and the model output

Many organizations want to ensure they're not exposing personal information to the LLM because they're worried that the LLM provider may store that information. This is a valid concern, although many LLM providers have provisions that handle this concern quite well in their contract.

If you have to filter PII from the input of the prompt, you can use a filter to do so. Let me show you a quick example of how to build a filter for this purpose:

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

In this code, we perform the following steps:

1. First, we create a class that implements the `IPromptRenderFilter` interface.
2. Next, we implement the `OnPromptRenderAsync` to filter the rendered prompt.

To use the filter, we can configure it in the kernel using the following code:

```csharp
kernel.PromptRenderFilters.Add(new PIIFilter());
```

Note that I haven't included any actual PII filtering code in the filter. If you're interested in learning how to use one of the popular PII filtering tools, please find the documentation for each of them here:

1. [Azure PII Detection][AZ_PII_DETECTION]
2. [Google de-identification][GA_PII_DETECTION]
3. [AWS PII detection][AWS_PII_DETECTION]

I recommend getting rid of PII as soon as it enters the application. The sooner you remove it, the less likely it is to leak anywhere.

## Summary

In this chapter, we've examined the art and nonsense of prompt engineering. We discussed the important role prompts play in LLM-based applications and how to write effective and safe prompts.

In the next chapter, we'll look at testing and monitoring prompts in your LLM-based application.

## Running the samples in this chapter

Check out the samples for this chapter in the [Github repository][GH_SAMPLE_DIR]. Each sample has a README file containing system requirements and instructions on how to run the sample.

[CONTEXT_WINDOW_PAPER]: https://arxiv.org/abs/2307.03172
[LLM_ALIGNMENT]: https://medium.com/@madalina.lupu.d/align-llms-with-reinforcement-learning-from-human-feedback-595d61f160d5
[TOP_P_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/notebooks
[SK_TEMPLATE_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-04/csharp/Chapter4.SemanticKernelTemplates
[HB_TEMPLATE_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-04/csharp/Chapter4.HandleBarsTemplates
[KF_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-04/csharp/Chapter4.KernelFunctionPrompts
[HB_MANUAL]: https://handlebarsjs.com/guide/
[STREAMING_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-04/csharp/Chapter4.StreamingChatCompletions
[HISTORY_REDUCERS]: https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/chat-history?pivots=programming-language-csharp#chat-history-reduction
[AZ_PII_DETECTION]: https://learn.microsoft.com/en-us/azure/ai-services/language-service/personally-identifiable-information/how-to-call
[GA_PII_DETECTION]: https://cloud.google.com/sensitive-data-protection/docs/deidentify-sensitive-data
[AWS_PII_DETECTION]: https://docs.aws.amazon.com/comprehend/latest/dg/how-pii.html
[OWASP_LLM]: https://owasp.org/www-project-top-10-for-large-language-model-applications/
[MITRE]: https://atlas.mitre.org/matrices/ATLAS/
[GH_SAMPLE_DIR]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-04/