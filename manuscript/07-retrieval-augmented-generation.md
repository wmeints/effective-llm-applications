{#retrieval-augmented-generation}
# Retrieval augmented generation

In previous chapters we focused on learning the basic building blocks of an LLM-based application. We covered prompting, the basics of LLMs, testing, and using tools. It's time to move on to more advanced topics. In this chapter and the ones after this chapter we'll shift our focus towards common patterns that you can use to build more elaborate LLM-based applications.

In this chapter you'll learn how to use Retrieval Augmented Generation as a way to answer questions based on internal knowledge not yet encoded in the LLM. You'll learn how to index documents for retrieval, and how to use indexed content in the LLMs response.

We'll cover the following topics:

- What is Retrieval Augmented Generation (RAG)
- Building an end-to-end RAG pipeline with Semantic Kernel
- Testing the RAG pipeline
- Optimizing retrieval for RAG
- Variations on the RAG pattern

Let's get started by discussing what Retrieval Augmented Generation (RAG) is and learn what components are involved in this pattern.

## What is Retrieval Augmented Generation (RAG)

Retrieval Augmented Generation is a pattern where you use information from an external source as extra context information when generating a response to a prompt. You're likely going to use RAG to answer questions from users. [#s](#rag-pattern-architecture) shows the structure of the RAG pattern.

{#rag-pattern-architecture}
![RAG pattern architecture](rag-pattern-architecture.png)

The RAG pattern has two main components:

1. First, there's the retrieval portion, which is responsible for finding relevant information based on the prompt that was entered.
2. Next, there's the generation portion, which is responsible for generating the final response to the prompt.

The basics of the RAG pattern aren't overly involved but you can find a lot of variants on the internet of it. That's the beauty of the pattern. You can adapt and extend it to your needs.

Let's take a closer look at the retrieval component of the RAG pattern to understand how to approach retrieving information to answer questions.

{#retrieval-component-architecture}
### Retrieval component architecture

To retrieve information we'll need to build a retrieval component. The retrieval component of the RAG pattern is usually made out of two subcomponents. You need a method to process information into a format that's easy for an LLM to answer questions with, and you need a method for the application to retrieve the preprocessed information. [#s](#retrieval-architecture) shows the details of the retrieval portion of the RAG pattern.

{#retrieval-architecture}
![Retrieval component of the RAG pattern](retrieval-architecture.png)

The retrieval component of the RAG pattern is best described with a practical use case. Imagine you need to explain to how to dissamble a washing machine to a mechanic. You could give the whole manual to the mechanic and let them read it. But that's not efficient because it takes quite a long time. It's nicer when the mechanic can ask a question like: "How do I remove the electrical board from the front of the Miele W1 washing machine?" And have the LLM answer that question with specific instructions on how to do that, but nothing more.

We could give the LLM the full manual of the washing machine and let it figure out the answer. But that doesn't work very well, it might be even worse than the mechanic reading the manual. First, the manual might not fit in the context window of the LLM. Next, the LLM might not be able to grab the right information, because of the limitations in what the LLM can focus on within its context window as we discussed in [#s](#context-window-limits).

If you want a clear answer, it's important to give the LLM focused information that matches the question as closely as possible. And this is what you should solve in the retrieval portion of the RAG pattern.

Ideally you want to preprocess the washing machine manual in such a way that you end up with chunks of information that clearly describe how to perform a specific task. That way the LLM can almost copy-paste the information to the response.

I'm sorry to say though that it takes quite a bit of effort to preprocess content in the most ideal way. If you want perfection you're going to need an army of people that enter the information by hand. That's not feasible for most projects. So you'll have to settle for a less ideal solution. But that's not a huge problem, because in many cases you can get away with a less ideal solution.

To preprocess content for retrieval you can start by splitting the text in the input documents into chunks of reasonable size. For example, you can create chunks of around 1000 tokens each. You can then store those chunks in a suitable database for retrieval.

### Storing information for retrieval

For example, you could use a regular search engine to store the chunks. Elastic Search is one of the most widely used search engines that does an excellent job finding documents based on a query.

Regular search engines will preprocess the chunks into a collection of search terms. They remove stopwords, lowercase the content, and split long words to increase the likelihood of a match during search. When you send a query it also gets preprocessed into search terms and is matched those terms against the documents in the database. To find a document, one or more search terms must match the text in the document. If you use a synonym or make a spelling mistake, the search match will be less precise.

That's why you will find that most people will use a vector database to store chunked content because it provides better search capabilities when compared to regular search engines.

A vector database turns the chunks into embedding vectors storing them alongside the original data. When you send a search query the vector database translates the query into an embedding vector too and matches it against the embedding vectors of the documents in the database. The documents whose vectors are closest to the query vector are returned by the search engine. Vector search uses cosine similarity to determine how close vectors are to each other. Imagine an arrow for the document and another for the query. The cosine similarity is the angle between the two arrows. [#s](#cosine-similarity) demonstrates this principle.

{#cosine-similarity}
![Cosine similarity](cosine-similarity.png)

The advantage of the vector based search method is that you can have spelling mistakes, use synonyms, or even use different words to describe the same thing. As long as the text in the document is similar to the test in the query, the vector search will find it, because the embedding vectors will be similar.

### Retrieving relevant information

Storing chunked content in a vector database is a good start for most use cases and will get you a long way. But there are a few details that we need to discuss about retrieving information from a vector database or search engine.

To generate a response, you'll need to first retrieve relevant information. You can use the user's prompt to find relevant chunks in the database. It is likely that one of the chunks you retrieve contains the answer to the question. But in many cases the answer is spread across multiple chunks. So it's important to retrieve more than one chunk when performing a search and inject all chunks into the prompt.

It's also important to make sure that chunks are coherent. For example, you don't want a chunk to contain half sentences or end with half a word. So while splitting content into chunks is a good solution, it's a little more complex than just slicing content into pieces. In [#s](#end-to-end-rag-pipeline-implementation) we'll discuss how to implement chunking with Semantic Kernel in greater detail.

Balancing the amount of chunks to retrieve, the size of the chunks, and general  content quality will determine the success of your RAG implementation. I've found that the LLM you use to generate the final answer doesn't have a lot of influence on the quality of the answers. If you find that you're getting weird results from a RAG implementation, it's highly likely you're having issues with the data quality in the vector database or search engine.

Having said that, if you don't inject the retrieved information in the right spot, you'll still end up with bad quality responses. So it's important to make sure that you use the retrieved information in the right way.

### Generating responses

In general, the generation component of the RAG pattern is straightforward. You take the information retrieved earlier and inject it into the prompt to generate a response.

However, based on the kind of solution you're building, you'll need a different approach to get the information into the prompt.

In chatbot scenarios, you'll want to implement the RAG pattern as a tool, and include the output of the tool in the chat history as a tool response. Using a tool gives the LLM-based application the flexibility to just give a straight answer without considering extra content from an external source.

For example, one of the chatbots I built can answer questions from a knowledge base for building software. It can also generate pieces of text for marketing purposes. It would be strange to mix the technical information about building software with general marketing content. Unless of course you're writing marketing content about building software. It's nice that by using a tool we have the flexibility to consider marketing on its own or combine it with technical information.

In non-chat scenarios, you'll want to create a specialized prompt to help guide the LLM in the right direction. The trick here is to use few-shot learning to help the LLM generate the right response as we discussed in [#s](#few-shot-learning). A typical prompt for answering question looks like this:

```text
You're answering questions about washing machines for technical support.
Please use the information in the context section to answer the question.
If you don't know the answer, just say so. Don't make up answers.

## Context

{{technical_documentation}}

## Question

{{question}}
```

In this prompt, you can inject the relevant chunks you found in the database and the question from the user. The output of the LLM will be a response to the question with possibly some extra information. The extra information is usually not a problem, but if it is, you can add one extra line: `## Answer` that will help the LLM focus on just the answer.

Let's look at how to put the theory into practice with Semantic Kernel by building an end-to-end pipeline.

{#end-to-end-rag-pipeline-implementation}
## Building an end-to-end RAG pipeline with Semantic Kernel

There are a lot of solutions for implementing RAG systems online, and it's easy to get confused by the fancy implementations that are out there. But the basics of the RAG pattern are straighforward. To show you how the theory from the previous section fits together, we'll build a RAG implementation for the content of this book. Since I wrote the content for the book in markdown, it's a great example on how to process content into a sensible format for answering questions.

To implement the RAG pattern in Semantic Kernel we'll need to integrate a few components as shown in [#s](#end-to-end-rag-pipeline).

{#end-to-end-rag-pipeline}
![End-to-end RAG pipeline in Semantic Kernel](end-to-end-rag-pipeline.png)

We'll need to configure the following pieces:

1. First, we need to build a data model `TextUnit` for the data we want retrieve.
2. Then, we need to build `ContextIndexer` that will process raw content into the `TextUnit` instances and store them in a `IVectorStore`.
3. Finally, we need to build the `QuestionAnsweringTool` that uses the `IVectorStore` in combination with the `Kernel` to answer questions.

We'll use a straightforward workflow with a single prompt to explore the RAG pattern to help you understand the basics. Many of the components in the workflow are the same for chat-based scenarios. We'll discuss how to use many of the components in the basic RAG pattern implementation with a chatbot after the initial setup.

Let's take a look at the project structure for RAG first.

### Setting up the project structure

Let's start by setting up the project structure. For the scenarion in this chapter, we'll assume you're building a web application. You can create a new web application using the following command:

```bash
dotnet new web -n Chapter7.RetrievalAugmentedGeneration
```

Next we need to add a few packages by running the following commands from the project directory we just created:

```bash
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.SemanticKernel.Connectors.Qdrant --prerelease
```

The first package adds the basic components for Semantic Kernel to the web application.
The second package adds vector store support based on a Postgres database. You can include other connectors as well, but for now, we'll stick to Qdrant as one of the more common vector stores.

After setting up the packages, we'll need to modify the `Program.cs` file in the project to include basic configuration for Semantic Kernel. The file should look like this:

```csharp
var builder = WebApplication.CreateBuilder(args);

var kernelBuilder = builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["LanguageModel:CompletionModel"]!,
        endpoint: builder.Configuration["LanguageModel:Endpoint"]!,
        apiKey: builder.Configuration["LanguageModel:ApiKey"]!
    )
    .AddAzureOpenAITextEmbeddingGeneration(
        deploymentName: builder.Configuration["LanguageModel:EmbeddingModel"]!,
        endpoint: builder.Configuration["LanguageModel:Endpoint"]!,
        apiKey: builder.Configuration["LanguageModel:ApiKey"]!
    );

builder.Services.AddSingleton<IVectorStore>(
    sp => new QdrantVectorStore(new QdrantClient("localhost")));

builder.Services.AddTransient<ContentIndexer>();
builder.Services.AddTransient<QuestionAnsweringTool>();

var app = builder.Build();

app.MapGet("/", () => "Hello world!");

app.Run();
```

This code configures Semantic Kernel with a new vector store. It performs the following steps:

1. First, we create a new web application builder.
2. Next, we configure the `Kernel` service with the Azure OpenAI connector.
3. Then, we configure the vector store for the application.
4. After that, we configure the content indexer and question answering tool.
5. Next, we build the web application and map a basic endpoint.

At this point, you can verify the application by starting it by running the following command in a terminal from the project directory:

```bash
dotnet run
```

You should see a notification that the web application is running. The terminal also lists the HTTP port the server is listening on. You can verify that the application works by navigating to the URL mentioned in the terminal output.

Now that we have the basic structure of the application, let's move on to the next step, creating the data model for the text data.

### Building a data model for retrieval

Many of the vector databases you can use with Semantic Kernel have a very basic structure to them. You can usually store a record that's identified by a key storing an embedding vector and some additional metadata.

In Semantic Kernel, you'll need to create a specific class to represent data you're storing in a vector store. Semantic Kernel uses the term vector store to represent what is a database that can store vector data. This can be a pure vector database or a relational database with support for storing vector data. If you're planning on using a regular database to store vector data you need to be aware that you can't combine the data structures offered by Semantic Kernel with otehr relational data although the database may support it.

The data model for our RAG implemention that we're building is formed by the `TextUnit` class. A text unit in our application represents a chunked fragment of content extracted from a markdown file. The code sample shows the structure of the class:

```csharp
using Microsoft.Extensions.VectorData;

public class TextUnit
{
    [VectorStoreRecordKey]
    public ulong Id { get; set; }

    [VectorStoreRecordData]
    public string OriginalFileName { get; set; } = default!;

    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string Content { get; set; } = default!;

    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
```

A vector store record in Semantic Kernel requires a unique key and a vector data field. The identifier for the `TextUnit` is a unique number, marked with the `[VectorStoreRecordKey]` attribute. The vector data field has to be of type `ReadOnlyMemory<float>` and is marked with the `[VectorStoreRecordVector]` attribute. Depending on the kind of embedding model you're going to use to generate embeddings, you need to specify a different value for the embedding size. We're using the `text-embedding-v3-small` model by OpenAI which has an embedding size of 1536. Which means that each piece of text is represented by a vector with 1536 dimensions.

The embedding size is usually found in the manual of the LLM provider that offers the embedding model you're using. Although it's smart to use an embedding model from the LLM provider that you're using for the LLM, it's not required. Using an embedding model from another provider or using an open-source embedding model does require extra maintenance while it may not add extra value in terms of higher quality search results.

Let's move on to the next step, building the content indexer.

### Preprocessing content into vector data

The content indexer is responsible for preprocessing raw content into vector store records. As we discussed in [#s](#retrieval-component-architecture), we can preprocess content in any way we wish, but for the sample, we need to keep things pragmatic, because we're working with a book that's long form content. We could try to process the content in a chapter per section, but we could end up with chunks that are either very long or very short.

To keep things simple, we'll split the content into chunks of 1000 tokens each. This way we can be sure that we have a good balance between the size of the chunks and the amount of chunks we have. The code sample shows the structure of the `ContentIndexer` class:

```csharp
public class ContentIndexer(
    IVectorStore vectorStore,
    ITextEmbeddingGenerationService embeddingGenerator,
{
    public async Task ProcessContentAsync()
    {
        ulong currentIdentifier = 1L;
        
        var files = Directory.GetFiles(
            "Content", "*.md", SearchOption.AllDirectories);

        var collection = vectorStore.GetCollection<ulong, TextUnit>("content");
        await collection.CreateCollectionIfNotExistsAsync();

        foreach (var file in files)
        {
            var lines = await File.ReadAllLinesAsync(file);

            var chunks = TextChunker.SplitMarkdownParagraphs(
                lines, maxTokensPerParagraph: 1000);

            foreach (var chunk in chunks)
            {
                var embedding = 
                    await embeddingGenerator.GenerateEmbeddingAsync(chunk);

                var textUnit = new TextUnit
                {
                    Content = chunk,
                    Embedding = embedding,
                    OriginalFileName = file,
                    Id = currentIdentifier++
                };

                await collection.UpsertAsync(textUnit);
            }
        }
    }
}
```

There's a lot going on in the `ContentIndexer` class, so let's break it down:

1. First, we create a new class `ContentIndexer` with dependencies on the vector store and the embedding generator.
2. Next, we create a new method to process content called `ProcessContentAsync`.
3. Then, we list all markdown files in the `Content` directory.
4. After that, we ensure we have a collection called `content` to store the processed `TextUnit` instances in.
5. Next, we loop over all markdown files and create chunks of 1000 tokens each.
6. Then, we generate an embedding vector for the chunks.
7. After, we create a new `TextUnit` instance and store it in the vector store.

The `ContentIndexer` class is a basic implementation of a content processing tool. I highly recommend looking at applying a retry mechanism for the embedding generation. You'll also want to make sure that you can reprocess a file if you're running into a transient error. Nothing worse than having to start over the whole indexing process because of a single failure.

Now that we have the content indexer, let's finish up the RAG pipeline by building the question answering tool.

### Using the vector store with a prompt

The generation component of the RAG pattern implementation we're working on is formed by the `QuestionAnsweringTool`. This class is a C# class that looks like this:

```csharp
public class QuestionAnsweringTool(
    Kernel kernel, IVectorStore vectorStore,
    ITextEmbeddingGenerationService embeddingGenerator)
{
    public async Task<string> AnswerAsync(string question)
    {
        // Content for the method
    }
}
```

Let's go over this code step by step:

1. First, we're creating a class that depends on the vector store we used earlier, the embedding generator, and the kernel we need to use for generating output.
2. Next, we create a new method `AnswerAsync` that takes a question as input and produces an answer as output.

The code for the `AnswerAsync` method looks like this:

```csharp
var collection = vectorStore.GetCollection<ulong, TextUnit>("content");

var questionEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(
    question);

var searchOptions = new VectorSearchOptions
{
    Top = 3,
};

var searchResponse = await collection.VectorizedSearchAsync(
    questionEmbedding, searchOptions);

var fragments = new List<TextUnit>();

await foreach (var fragment in searchResponse.Results)
{
    fragments.Add(fragment.Record);
}

var promptTemplateContent = File.ReadAllText("Prompts/answer-question.yaml");

var promptTemplate = kernel.CreateFunctionFromPromptYaml(
    promptTemplateContent, new HandlebarsPromptTemplateFactory());

var response = await promptTemplate.InvokeAsync(kernel, new KernelArguments
{
    ["question"] = question,
    ["fragments"] = fragments
});

return response.GetValue<string>()!;
```

In the method we're performing the following steps:

1. First, we'll lookup the collection containing the preprocessed text units in the vector store. We'll use this collection to search for relevant text units.
2. Next, we generate an embedding vector for the question using the same embedding model we used to generate embedding vectors for the text units.
3. Then, we perform a cosine similarity search to find the most relevant text units for the question. We'll ask for three text units to be returned.
4. Next, we process the results into a list of text units to be inserted into the prompt.
5. Then, we load up the answer-question.yaml prompt from the file system.
6. Finally, we execute the prompt with the retrieved text and the user question and return the output.

You may be wondering, what does the prompt look like? The prompt is stored in a YAML file following the structure we discussed earlier in [#s](#prompt-templates). The content of the `answer-question.yaml` file looks like this:

```yaml
name: answer_question
template: |
  You're a helpful assistant supporting me by answering questions 
  about the book building effective llm-based applications with
  semantic kernel. Answer the question using the provided context.
  If you don't know the answer, say so, don't make up 
  answers.

  ## Context

  {{#each fragments}}
  {{ .Content }}

  {{/each}}

  ## Question

  {{question}}
template_format: handlebars
input_variables:
  - name: fragments
    description: The topic you want to discuss in the blogpost.
    is_required: true
  - name: question
    description: The question you want to ask about the topic.
    is_required: true  
execution_settings:
  default:
    top_p: 0.98
    temperature: 0.7
    presence_penalty: 0.0
    frequency_penalty: 0.0
    max_tokens: 1200
```

In this prompt we're telling the LLM that we're answering questions about this book. WE then provide the fragments using a foreach loop rendering the `Content` property of the `TextUnit` class. Finally, we provide the question that we need an answer to.

The rest of the file lists the input variables for the prompt and the execution settings for the LLM.

To use the question answering tool, we can hook up the required components in the `Program.cs` file of the project:

```csharp
// Setup logic for semantic kernel

builder.Services.AddTransient<ContentIndexer>();
builder.Services.AddTransient<QuestionAnsweringTool>();

var app = builder.Build();

app.MapGet("/answer", async (
    [FromServices] QuestionAnsweringTool tool, [FromQuery] string question) =>
{
    return await tool.AnswerAsync(question);
});

var scope = app.Services.CreateScope();
var indexer = scope.ServiceProvider.GetRequiredService<ContentIndexer>();

await indexer.ProcessContentAsync();

// The rest of Program.cs
```

The code in this fragment needs to be added right after you're configuring the kernel in the service collection. The code performs the following steps:

1. First, we register the content indexer and answering tool so we can use both.
2. Next, we build the web application and map a new endpoint `/answer` that takes a query parameter `question`. The endpoint uses the `QuestionAnsweringTool` to generate a response to the question.
3. Then, we create a new scope to get the content indexer from the service provider.
4. Finally, we process the content to store it in the vector store.

The rest of the `Program.cs` file remains the same.

You can now run the application and ask a question from the browser by navigating to
`http://localhost:<port>/answer?question=What+is+the+RAG+pattern`. Make sure to check the port matches the one shown in the terminal when you start the application.

The application should return a response to the question you asked based on the content of the book.

As this is one of the bigger samples in this book, I recommend checking out the [complete source code][SAMPLE_SOURCE_1] on GitHub. It contains all the instructions required to configure the project and run it on your machine. I've included an extra [Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/) script to help you configure Azure OpenAI for the project.

Using the RAG pattern with a prompt is one of the easiest ways to get started with prototyping a solution on your business content. But most of you will probably build a chatbot on top of internal business content, so let's explore how to apply the RAG pattern in the context of a chat solution as well.

### Using the vector store as a tool

Applying the RAG pattern in a chat solution follows many of the same steps involved in implementing the RAG pattern with a prompt, so I'm not going to repeat all the steps.

The major difference is that in the context of a chatbot you'll want to connect the retrieval component as a tool for the LLM. This way you let the LLM decide if it's necessary to look up information needed to generate a proper response.

You can implement the RAG pattern as a tool using the following code:

```csharp
public class QuestionAnsweringBot(
    Kernel kernel, IVectorStore vectorStore,
    ITextEmbeddingGenerationService embeddingGenerator,
    IChatCompletionService chatCompletions)
{
    public async Task<string> GenerateResponse(string prompt)
    {
        var textCollection = vectorStore.GetCollection<ulong, TextUnit>("content");

        var textSearch = new VectorStoreTextSearch<TextUnit>(
            textCollection,
            embeddingGenerator,
            new TextUnitStringMapper(),
            new TextUnitTextSearchResultMapper());

        var searchFunction = textSearch.CreateGetTextSearchResults();

        kernel.Plugins.AddFromFunctions("SearchPlugin", [searchFunction]);

        var chatHistory = new ChatHistory();

        chatHistory.AddSystemMessage(
            "You're a friendly assistant. Your name is Ricardo");

        chatHistory.AddUserMessage(prompt);

        var executionSettings = new AzureOpenAIPromptExecutionSettings
        {
            Temperature = 0.6,
            FrequencyPenalty = 0.0,
            PresencePenalty = 0.0,
            MaxTokens = 2500,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var response = await chatCompletions.GetChatMessageContentAsync(
            chatHistory, executionSettings, kernel);

        return response.Content!;
    }
}
```

You can consider the `QuestionAnsweringBot` class to be the central service for the chatbot. It uses the vector store, embedding generation service, and the kernel to generate responses. The class performs the following steps:

1. First, we retrieve the collection of preprocessed text units from the vector store.
2. Next, we create a new `VectorTextSearch` instance that connects the collection with two essential components, a string mapper `TextUnitStringMapper` and a search result mapper `TextUnitTextSearchResultMapper`.
3. After that, we connect the vector search component to the kernel as a plugin with a single function, `CreateGetTextSearchResults`.

The rest of the code is similar to the code we used earlier in [#s](#working-with-chat-completion). We'll create a new chat history object with the necessary messages and ask Semantic Kernel to generate a response. If you ask a question, it's highly likely that the LLM will use the search plugin to find the answer to the question. Make sure you set the right function invocation settings or the search is never performed.

We need to discuss an important detail about this setup. In the previous section we manually translated `TextUnit` instances to strings in the prompt. This time we need a different solution for this. This is where the `TextUnitStringMapper` plays an important role.

When you use the `VectorStoreTextSearch` class as a tool, you need to provide a way to inject search results into the chat history of the conversation as a string. The `TextUnitStringMapper` class is responsible for this translation. The class looks like this:

```csharp
public class TextUnitStringMapper : ITextSearchStringMapper
{
    public string MapFromResultToString(object result)
    {
        if (result is TextUnit textUnit)
        {
            return textUnit.Content;
        }

        throw new ArgumentException("Invalid result object");
    }
}
```

The mapping class in this sample is very basic returning the content of the retrieved text unit. However, you can extend the class to do much more. For example, you can return a structure that's going to help the LLM generate citations for the found sources. For example, you can create an implementation like this:

```csharp
public class CitationsTextUnitStringMapper : ITextSearchStringMapper
{
    public string MapFromResultToString(object result)
    {
        if (result is TextUnit textUnit)
        {
            var outputBuilder = new StringBuilder();

            outputBuilder.AppendLine($"Name: {textUnit.Id}");
            outputBuilder.AppendLine($"Value: {textUnit.Content}");
            outputBuilder.AppendLine($"Link: {textUnit.OriginalFileName}");

            return outputBuilder.ToString();
        }

        throw new ArgumentException("Invalid result object");
    }
}
```

The `CitationsTextUnitStringMapper` class returns a string with the name, value, and link to the original file for the text unit. If you change the system message to tell the LLM to generate citations, you can use this mapper to generate the citations for the found sources. For example, you can change the content of the `AddSystemMessage` call to include this system message.

```text
You're a friendly assistant. When answering questions, include citations to the relevant information where it is referenced in the response.
```

Keep in mind that instructions like this one aren't going to force the LLM to do the right thing. You may end up with citations that aren't really citations. It's important to test the system and see if it's generating the right content. If it's not you may need to adjust the system message.

As an alternative to generating citations through instructions you can also use a `IFunctionInvocationFilter` to capture the output of the search tool and display links to the found sources in the user interface separately. The following code demonstrates how to build such a filter:

```csharp
public class CitationCapturingFilter : IFunctionInvocationFilter
{
    public List<TextSearchResult> Captures { get; } = new();

    public async Task OnFunctionInvocationAsync(
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, Task> next)
    {
        await next(context);

        if (context.Function.PluginName == "SearchPlugin" &&
            context.Function.Name == "GetTextSearchResults")
        {
            var results = context.Result.GetValue<List<TextSearchResult>>()!;
            Captures.AddRange(results);
        }
    }
}
```

This function filter performs the following steps:

1. First, it invokes the function as usual.
2. Then, when it detects that the search plugin is invoked, it captures the search results and stores them in the `Captures` list.

You can integrate this filter into the bot using the following code:

```csharp
public class QuestionAnsweringBot(
    Kernel kernel, IVectorStore vectorStore,
    ITextEmbeddingGenerationService embeddingGenerator,
    IChatCompletionService chatCompletions)
{
    public async Task<string> GenerateResponse(string prompt)
    {
        var textCollection = vectorStore.GetCollection<ulong, TextUnit>("content");

        var textSearch = new VectorStoreTextSearch<TextUnit>(
            textCollection,
            embeddingGenerator,
            new TextUnitStringMapper(),
            new TextUnitTextSearchResultMapper());

        var searchFunction = textSearch.CreateGetTextSearchResults();

        kernel.Plugins.AddFromFunctions("SearchPlugin", [searchFunction]);
  
        var citationsFilter = new CitationCapturingFilter();
        kernel.FunctionInvocationFilters.Add(citationsFilter);

        // ... Rest of the code
    }
}
```

Let's go over the code to understand the modifications compared to the original
version of this code: The start of the method is the same as before, but we've added a new filter right after configuring the text search plugin.

In previous code fragments we already were using the `TextUnitTextSearchResultMapper`. Let's discuss what this mapper does, because it's necessary here to make the function filter work. The class looks like this:

```csharp
public class TextUnitTextSearchResultMapper : ITextSearchResultMapper
{
    public TextSearchResult MapFromResultToTextSearchResult(object result)
    {
        if (result is TextUnit textUnit)
        {
            return new TextSearchResult(value: textUnit.Content)
            {
                Link = textUnit.OriginalFileName,
                Name = textUnit.Id.ToString()
            };
        }

        throw new ArgumentException("Invalid result object");
    }
}
```

In this class we perform the following steps:

1. First, we implement the `ISearchResultMapper` interface creating the `MapFromResultToTextSearchResult` method.
2. Then, in the method we check if the result is a `TextUnit` instance.
3. Next, we create a new `TextSearchResult` instance with the content of the text unit including the identifier, link, and value for the search result.

With the additional filter you can be sure that you're capturing the search results that were found by the vector search instead of relying on the LLM to choose whether something was used in the response. You may get some false positives still, because the LLM may not actually use a result that you retrieved.

Implementing a RAG pattern takes effort to get right, and it's not going to be 100% perfect all the time. You have to make a choice here, do you want to have a response where the LLM makes up sources? Or do you want to have a response with a separate set of citations that may not be included in the actual response.

Whatever you choose, I recommend testing the various parts of your RAG implementation to make sure you achieve the highest possible quality. Let's discuss how to approach testing the RAG pattern in the next section.

## Testing the RAG pipeline

## Optimizing retrieval for RAG

## Variations on the RAG pattern

- Using graphs for retrieval (graphrag: https://microsoft.github.io/graphrag/)
- Reranking

[SAMPLE_SOURCE_1]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-07/Chapter7.RetrievalAugmentedGeneration/