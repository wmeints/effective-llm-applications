{#retrieval-augmented-generation}
# Retrieval augmented generation

In previous chapters, we focused on learning the basic building blocks of an LLM-based application. We covered the basics of LLMs, prompting, testing, and using tools. Now, it's time to move on to more advanced topics. In this chapter and the ones after it, we'll focus on common patterns you can use to build more elaborate LLM-based applications. We'll use the knowledge learned in previous chapters to help implement the patterns.

In this chapter, you'll learn how to use Retrieval Augmented Generation to answer questions based on internal knowledge that has not previously been trained in the LLM. You'll learn how to index documents for retrieval and use indexed content in the LLMs response.

We'll cover the following topics:

- What is Retrieval Augmented Generation (RAG)
- Building an end-to-end RAG pipeline with Semantic Kernel
- A practical approach to validating the RAG pipeline
- Evaluating the RAG pattern with user research
- Variations on the RAG pattern

Let's start by discussing Retrieval Augmented Generation (RAG) and learning what components are involved in this pattern.

## What is Retrieval Augmented Generation (RAG)

Retrieval Augmented Generation (RAG) is a pattern in which you use information from an external source as extra context information when generating a response to a prompt.

The RAG pattern is a form of in-context learning we discussed in [#s](#few-shot-learning). You can use RAG for many scenarios. But you're most likely using RAG to answer users' questions. [#s](#rag-pattern-architecture) shows the structure of the RAG pattern.

{#rag-pattern-architecture}
![RAG pattern architecture](rag-pattern-architecture.png)

The RAG pattern has two main components:

1. The retrieval portion is responsible for finding relevant information based on the entered prompt.
2. The generation portion is responsible for generating the final response to the prompt.

The basics of the RAG pattern aren't overly involved, but you can find many variants on the Internet. That's the beauty of the pattern: You can adapt and extend it to your needs.

Let's examine the retrieval component of the RAG pattern more closely to understand how to retrieve information to answer questions.

{#retrieval-component-architecture}
### Retrieval component architecture

The retrieval component of the RAG pattern is usually made out of two subcomponents. You need a method to process information into a format that's easy for an LLM to answer questions with, and you need a method for the application to retrieve the preprocessed data. [#s](#retrieval-architecture) shows the details of the retrieval portion of the RAG pattern.

{#retrieval-architecture}
![Retrieval component of the RAG pattern](retrieval-architecture.png)

Let's explore the retrieval component of the RAG pattern with a practical use case. Imagine you must explain how to disassemble a washing machine to a mechanic. You could give the whole manual to the mechanic and let them read it. But that's not efficient because it takes quite a long time. It's nicer when the mechanic can ask, "How do I remove the electrical board from the front of the Miele W1 washing machine?" and the LLM answers with specific instructions on how to do that, but nothing more.

We could give the LLM the whole washing machine manual and let it determine the answer. But that doesn't work very well; it might be even worse than the mechanic reading the manual. First, the manual might not fit in the LLM's context window. Next, the LLM might be unable to grab the correct information because of the limitations in what it can focus on within its context window, as we discussed in [#s](#context-window-limits).

If you want a clear answer, providing the LLM with more focused information that matches the question as closely as possible is essential. And this is what you should solve in the retrieval portion of the RAG pattern.

Ideally, you want to preprocess the washing machine manual to end up with chunks of information that clearly describe how to perform a specific task. That way, the LLM can almost copy-paste the information into the response. Remember that the LLM can only match patterns and generate tokens based on the pattern.

Preprocessing content optimally takes a lot of effort. If you want perfection, you will need an army of people who enter the information by hand. That's not feasible for most projects, so you'll have to settle for a less ideal solution. But that's not a massive problem because, in many cases, you can get away with a less optimal solution. Thanks to the tranformative power of the LLM.

To preprocess content for retrieval, you can start by splitting the text in the input documents into chunks of reasonable size. For example, you can create chunks of around 1000 tokens each. You can then store those chunks in a suitable database for retrieval.

### Storing information for retrieval

There are many options for storing the extracted content chunks. For example, you could store the searchable content using a regular search engine. [Elastic Search][ES] is one of the most widely used search engines and does an excellent job finding documents based on a query.

Regular search engines will preprocess the chunks into a collection of search terms. They remove stopwords, lowercase the content, and split long words to increase the likelihood of a match during the search. When you send a query, it also gets preprocessed into search terms and matches those terms against the documents in the database. To find a document, one or more search terms must match the text in the document. If you use a synonym or make a spelling mistake, the search match will be less precise.

That's why most people will use a vector database to store chunked Content: it semantically matches the query against the documents which can handle things like synonyms, acronyms, and other hard to parse language constructs.

A vector database stores chunks based on the embedding vector representations for the chunks. To store content in a vector database for search, you first translate the content into an embedding vector representing the semantic meaning of the content using an embedding model. After creating the embedding vector, you can store it with the original content and metadata in the vector database. When you want to search, you send an embedding vector made with the same embedding representing your query to the vector database and match it against the embedding vectors of the documents in the database. The vector database returns the documents with embedding vectors closest to the query embedding vector. Vector search uses cosine similarity to determine how close vectors are to each other. Imagine an arrow for the document and another for the query. The cosine similarity is the angle between the two arrows. [#s](#cosine-similarity) demonstrates this principle.

{#cosine-similarity}
![Cosine similarity](cosine-similarity.png)

The advantage of the vector-based search method is that you can make spelling mistakes, use synonyms, or even use different words to describe the same thing. As long as the text in the document is semantically similar to the text in the query, you will find it because the embedding vectors will be closely related.

### Retrieving relevant information

Storing chunked content in a vector database is a good start for most use cases and will get you a long way. However, there are a few details that we need to discuss when retrieving information from a vector database.

To generate a response, you'll need first to retrieve relevant information. You can use the user's prompt to find relevant chunks in the database. One of the chunks you retrieve likely contains the answer to the question. But in many cases, the answer is spread across multiple chunks. So, it's essential to recover more than one chunk when performing a search and inject all chunks into the prompt.

It's also essential to make sure that chunks are coherent. For example, you don't want a chunk to contain half sentences or end with half a word. So, while splitting Content into chunks is a good solution, it's more complex than just slicing Content into pieces. In [#s](#end-to-end-rag-pipeline-implementation), we'll discuss how to implement chunking to ensure you end up with sensible chunks.

Balancing the amount of chunks to retrieve, the size of the chunks, and general content quality will determine the success of your RAG implementation. The LLM you use to generate the final answer doesn't influence the quality of the answers as much as the retrieval approach you choose. If you're getting weird results from a RAG implementation, you're likely having issues with the data quality in the vector database.

Having said that, if you don't inject the retrieved information in the right spot, you'll still end up with bad-quality responses. So, it's essential to ensure you use the retrieved information correctly.

### Generating responses

In general, the generation component of the RAG pattern is straightforward. You take the information retrieved earlier and inject it into the prompt to generate a response.

However, depending on the solution you're building, you'll need a different approach to getting the information into the prompt.

In chatbot scenarios, you'll want to implement the RAG pattern as a tool and include the tool's output in the chat history as a tool response. Using a tool gives the LLM-based application the flexibility to provide a straight answer without considering extra Content from an external source.

For example, one of the chatbots I built can answer questions from a knowledge base about building software and generate text for marketing purposes. It would be strange to mix technical information about building software with general marketing content—unless you're writing marketing content about building software. It's nice that by using a tool, we can consider marketing on its own or combine it with technical information.

In non-chat scenarios, you'll want to create a specialized prompt to help guide the LLM in the right direction. The trick here is to use in-context learning to help the LLM generate the correct response, as discussed earlier in [#s](#few-shot-learning). A typical prompt for answering a question looks like this:

```text
You're answering questions about washing machines for technical support.
Please use the information in the context section to answer the question.
If you don't know the answer, say so. Don't make up answers.

## Context

{{technical_documentation}}

## Question

{{question}}
```

In this prompt, you can inject the relevant chunks you found in the database and the user's question. The output of the LLM will be a response to the question with some extra information. The additional information is usually not a problem, but if it is, you can add one extra line: `## Answer` that will help the LLM focus on just the answer.

Let's look at how to implement the theory with Semantic Kernel by building an end-to-end pipeline.

{#end-to-end-rag-pipeline-implementation}
## Building an end-to-end RAG pipeline with Semantic Kernel

There are a lot of solutions for implementing the RAG pattern online, and it's easy to get confused by the fancy implementations out there. But the basics of the RAG pattern are straightforward. To show you how the theory from the previous section fits together, we'll build an RAG implementation for the content of this book. Since I wrote the content for the book in markdown, it's a great example of how to process content in a sensible format for answering questions.

To implement the RAG pattern in Semantic Kernel, we'll need to integrate a few components, as shown in [#s](#end-to-end-rag-pipeline).

{#end-to-end-rag-pipeline}
![End-to-end RAG pipeline in Semantic Kernel](end-to-end-rag-pipeline.png)

We'll need to configure the following pieces:

1. First, we need to build a data model, `TextUnit`, for the data we want to retrieve.
2. Then, we need to build a `ContextIndexer` that will process raw content into `TextUnit` instances and store it in an `VectorStore`
3. Finally, we need to build the `QuestionAnsweringTool` that uses the `VectorStore` in combination with the `Kernel` to answer questions.

We'll use a straightforward workflow with a single prompt to explore the RAG pattern to help you understand the basics. Many of the components in the workflow are the same for chat-based scenarios. We'll discuss how to use many of the components in the basic RAG pattern implementation with a chatbot after the initial setup.

Let's first take a look at the RAG project structure.

### Setting up the project structure

For the scenario in this chapter, we'll assume you're building a web application. You can create a new web application using the following command:

```bash
dotnet new web -n Chapter7.RetrievalAugmentedGeneration
```

Next, we need to add a few packages by running the following commands from the project directory we just created:

```bash
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.SemanticKernel.Connectors.Qdrant --prerelease
```

The first package adds the essential components for Semantic Kernel to the web application.
The second package adds vector store support based on a Postgres database. You can also include other connectors, but for now, we'll stick to Qdrant as one of the more common vector stores.

After setting up the packages, we'll need to modify the `Program.cs` file in the project to include basic configuration for Semantic Kernel. The file should look like this:

```csharp
var builder = WebApplication.CreateBuilder(args);

var kernelBuilder = builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["LanguageModel:CompletionModel"]!,
        endpoint: builder.Configuration["LanguageModel:Endpoint"]!,
        apiKey: builder.Configuration["LanguageModel:ApiKey"]!
    )
    .AddAzureOpenAIEmbeddingGenerator(
        deploymentName: builder.Configuration["LanguageModel:EmbeddingModel"]!,
        endpoint: builder.Configuration["LanguageModel:Endpoint"]!,
        apiKey: builder.Configuration["LanguageModel:ApiKey"]!
    );

builder.Services.AddQdrantVectorStore("localhost");

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
4. After, we configure the content indexer and question-answering tool.
5. Finally, we build the web application and map a basic endpoint.

At this point, you can verify the application by starting it by running the following command in a terminal from the project directory:

```bash
dotnet run
```

You should see a notification that the web application is running. The terminal also lists the HTTP port the server is listening on. You can verify that the application works by navigating to the URL mentioned in the terminal output.

Now that we have the application's basic structure, let's move on to the next step: creating the data model for the text data.

### Building a data model for retrieval

Many vector databases you can use with Semantic Kernel have a fundamental structure. You can usually store a record identified by a key. The record stores an embedding vector and some additional metadata that must be serializable to JSON.

In Semantic Kernel, you must create a specific class to represent the data in a vector store. Semantic Kernel uses the term vector store to describe a database that can store vector data. This can be a pure vector database or a relational database with support for storing vector data. If you're planning on using a regular database to store vector data you need to be aware that you can't combine the data structures offered by Semantic Kernel with other relational data processing such as Entity Framework Core although the database may support it.

The `TextUnit` class forms the data model for our RAG implementation. A text unit in our application represents a chunked fragment of Content extracted from a markdown file. The code sample shows the structure of the class:

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

public class TextUnit
{
    [VectorStoreKey]
    public ulong Id { get; set; }

    [VectorStoreData]
    [TextSearchResultName]
    public string OriginalFileName { get; set; } = default!;

    [VectorStoreData(IsFullTextIndexed = true)]
    [TextSearchResultValue]
    public string Content { get; set; } = default!;

    [VectorStoreVector(
        1536, 
        DistanceFunction = DistanceFunction.CosineSimilarity, 
        IndexKind = IndexKind.Hnsw
    )]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
```

A vector store record in Semantic Kernel requires a unique key and a vector data field. The identifier for the `TextUnit` is a unique number marked with the `[VectorStoreKey]` attribute. The vector data field has to be of type `ReadOnlyMemory<float>` and is marked with the `[VectorStoreVector]` attribute. Depending on the embedding model you will use to generate embeddings, you need to specify a different value for the embedding size. We're using the `text-embedding-3-small` model by OpenAI, which has an embedding size of 1536, meaning that each text piece is represented by a vector with 1536 dimensions.

To retrieve the value of a chunk for the LLM, we need to specify which property contains the original text we indexed. We can use the `[TextSearchResultValue]` attribute for this purpose. In addition to the result value, we can also request the filename using the `[TextSearchResultName]` attribute. The name is useful when citing the source for an answer.

The embedding size is usually found in the manual of the LLM provider that offers the embedding model you're using. Although it's wise to use an embedding model from the LLM provider that you're using for the LLM, it's not required. Using an embedding model from another provider or an open-source embedding model requires extra maintenance and may not add additional value in terms of higher-quality search results.

Let's move on to the next step, building the content indexer.

### Preprocessing Content into vector data

The content indexer is responsible for preprocessing raw content into vector store records. As we discussed in [#s](#retrieval-component-architecture), we can preprocess content in any way we wish, but for the sample, we need to keep things pragmatic because we're working with a book that's long-form content. We could process the content in a chapter per section, but we could end up with chunks that are either very long or very short.

We'll split the content into approximately 1000 tokens each to simplify things. This way, we can be sure that we have a good balance between the size of the chunks and the amount of chunks we have. The code sample shows the structure of the `ContentIndexer` class:

```csharp
public class ContentIndexer(
    VectorStore vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    ILogger<ContentIndexer> logger)
{
    public async Task ProcessContentAsync()
    {
        ulong currentIdentifier = 1L;
        
        var files = Directory.GetFiles("Content", "*.md",
            SearchOption.AllDirectories);

        var textUnits = new List<TextUnit>();

        var collection = vectorStore.GetCollection<ulong, TextUnit>("content");

        await collection.EnsureCollectionExistsAsync();

        foreach (var file in files)
        {
            var lines = await File.ReadAllLinesAsync(file);

            var chunks = TextChunker.SplitMarkdownParagraphs(
                lines, maxTokensPerParagraph: 1000);

            foreach (var chunk in chunks)
            {
                var embedding = await embeddingGenerator.GenerateAsync(chunk);

                var textUnit = new TextUnit
                {
                    Content = chunk,
                    Embedding = embedding.Vector,
                    OriginalFileName = file,
                    Id = currentIdentifier++
                };

                await collection.UpsertAsync(textUnit);
            }
        }
    }
}
```

A lot is going on in the `ContentIndexer` class, so let's break it down:

1. First, we create a new class, `ContentIndexer,` which depends on the vector store and the embedding generator.
2. Next, we create a new method to process content called `ProcessContentAsync`
3. Then, we list all markdown files in the `Content` directory.
4. After that, we ensure we have a collection called `content` to store the processed `TextUnit` instances in
5. Next, we loop over all markdown files and create chunks of 1000 tokens each.
6. Then, we generate an embedding vector for the chunks.
7. After, we create a new `TextUnit` instance and store it in the vector store.

The `ContentIndexer` class is a basic implementation of a content processing tool. I recommend applying a retry mechanism for embedding generation. You'll also want to make sure that you can reprocess a file if you encounter a transient error. There is nothing worse than having to start over the whole indexing process because of a single failure.

Now that we have the content indexer let's finish up the RAG pipeline by building the question-answering tool.

{#using-the-vector-store-with-a-prompt}
### Using the vector store with a prompt

The generation component of the RAG pattern implementation we're working on is formed by the `QuestionAnsweringTool`. This class is a C# class that looks like this:

```csharp
public class QuestionAnsweringTool(
    Kernel kernel, VectorStore vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
{
    public async Task<string> AnswerAsync(string question)
    {
        //Content for the method
    }
}
```

Let's go over this code step by step:

1. First, we're creating a class that depends on the vector store we used earlier, the embedding generator, and the kernel we need to use for generating output.
2. Next, we create a new method, `AnswerAsync`, that takes a question as input and produces an answer as output.

The code for the `AnswerAsync` method looks like this:

```csharp
var collection = vectorStore.GetCollection<ulong, TextUnit>("Content");

var questionEmbedding = await embeddingGenerator.GenerateAsync(
    question);

var searchResponse = collection.SearchAsync(
    questionEmbedding, searchOptions);

var fragments = new List<TextUnit>();

await foreach (var fragment in searchResponse)
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

In the method, we're performing the following steps:

1. First, we'll lookup the collection containing the preprocessed text units in the vector store. We'll use this collection to search for relevant text units.
2. Next, we generate an embedding vector for the question using the same embedding model we used to create embedding vectors for the text units.
3. Then, we perform a cosine similarity search to find the most relevant text units for the question. We'll ask for three text units to be returned.
4. Next, we process the results into a list of text units to be inserted into the prompt.
5. Then, we load up the `answer-question.yaml` prompt from the file system.
6. Finally, we execute the prompt with the retrieved text and the user question and return the output.

You may be wondering what the prompt looks like. The prompt is stored in a YAML file following the structure discussed earlier in [#s](#prompt-templates). The content of the `answer-question.yaml` file looks like this:

```yaml
name: answer_question
template: |
  You're a helpful assistant, supporting me by answering questions 
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
    description: The topic you want to discuss in the blog post.
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

In this prompt, we're telling the LLM we're answering questions about this book. We then provide the fragments using a `foreach` loop rendering the `Content` property of the `TextUnit` class. Finally, we give the question that we need an answer to.

The rest of the file lists the input variables for the prompt and the execution settings for the LLM.

To use the question-answering tool, we can hook up the required components in the `Program.cs` file of the project:

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

The code in this fragment must be added after configuring the kernel in the service collection. The code performs the following steps:

1. First, we register the content indexer and answering tool so we can use both.
2. Next, we build the web application and map a new endpoint `/answer` that takes a query parameter `question`. The endpoint uses the `QuestionAnsweringTool` to generate a response to the question.
3. Then, we create a new scope to get the Content indexer from the service provider.
4. Finally, we process the Content to store it in the vector store.

The rest of the `Program.cs` file remains the same.

You can now run the application and ask a question from the browser by navigating to
`http://localhost:<port>/answer?question=What+is+RAG`. Ensure the port matches the one shown in the terminal when you start the application.

The application should return a response to the question you asked based on the content of the book.

As this is one of the bigger samples in this book, check out the [complete source code][SAMPLE_SOURCE_1] on GitHub. It contains all the instructions required to configure and run the project on your machine. I've included an extra [Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/) script to help you configure Azure OpenAI for the project.

Using the RAG pattern with a prompt is one of the easiest ways to get started with prototyping a solution for your business content. But most of you will probably build a chatbot on top of internal business content, so let's explore how to apply the RAG pattern in the context of a chat solution.

### Using the vector store as a tool

Applying the RAG pattern in a chat solution follows many of the same steps involved in implementing it with a prompt, so let me focus on the steps that are different from a prompt-based RAG implementation.

The major difference is that in the context of a chatbot, you'll want to connect the retrieval component as a tool for the LLM. This way, you let the LLM decide if it's necessary to look up information needed to generate a proper response.

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

        kernel.Plugins.AddFromObject(
            textSearch.CreateWithSearch("SearchPlugin"));

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
2. Next, we create a new `VectorTextSearch` instance that connects the collection with two essential components, a string mapper `TextUnitStringMapper` and a search result mapper `TextUnitTextSearchResultMapper`
3. After that, we connect the vector search component to the kernel as a plugin with a single function, `CreateGetTextSearchResults`

The rest of the code is similar to the code we used earlier in [#s](#working-with-chat-completion). We'll create a new chat history object with the necessary messages and ask Semantic Kernel to generate a response. If you ask a question, it's highly likely that the LLM will use the search plugin to find the answer. Ensure you set the right function invocation settings or the search will never be performed.

We need to discuss important details about this setup. In the previous section, we manually translated `TextUnit` instances into strings in the prompt. This time, we need a different solution. This is where the `TextUnitStringMapper` plays an important role.

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

The mapping class in this sample is elementary, returning the content of the retrieved text unit. However, you can extend the class to do much more. For example, you can return a structure that will help the LLM generate citations for the found sources. For example, you can create an implementation as shown in the following code:

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
You're a friendly assistant. When answering questions, include citations 
to the relevant information where it is referenced in the response.
```

Remember that instructions like this don't force the LLM to do the right thing. You may end up with citations that aren't citations. Testing the system and seeing if it's generating the right content is essential. If it's not, you may need to adjust the system message.

As an alternative to generating citations through instructions you can also use a `IFunctionInvocationFilter` to capture the output of the search tool and display links to the found sources in the user interface. The following code demonstrates how to build such a filter:

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
    Kernel kernel, VectorStore vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
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

        kernel.Plugins.AddFromObject(
            textSearch.CreateWithSearch("SearchPlugin"));
  
        var citationsFilter = new CitationCapturingFilter();
            kernel.FunctionInvocationFilters.Add(citationsFilter);

        // ... Rest of the code
    }
}
```

Let's go over the code to understand the modifications compared to the original
version of this code: The start of the method is the same as before, but we've added a new filter right after configuring the text search plugin.

In previous code fragments, we were already using the `TextUnitTextSearchResultMapper`. Let's discuss what this mapper does because it's necessary here to make the function filter work. The class looks like this:

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

In this class, we perform the following steps:

1. First, we implement the `ISearchResultMapper` interface, creating the `MapFromResultToTextSearchResult` method.
2. Then, in the method, we check if the result is a `TextUnit` instance.
3. Next, we create a new `TextSearchResult` instance with the Content of the text unit, including the search result's identifier, link, and value.

With the additional filter, you can be sure that you're capturing the search results found by the vector search instead of relying on the LLM to choose whether something was used in the response. However, you may still get some false positives because the LLM may not actually use a result that you retrieved.

Implementing a RAG pattern takes effort to get right, and it will not always be 100% perfect. You have to make a choice here: Do you want to have a response where the LLM makes up sources? Or do you want to have a response with a separate set of citations that may not be included in the actual response?

Whatever you choose, I recommend spending some time to establish a good test strategy for the various parts of your RAG implementation to ensure the highest possible quality. In the next section, we will discuss how to approach testing the RAG pattern.

## A practical approach to validating the RAG pipeline

Testing the RAG pattern can be quite complicated. [#s](#rag-evaluation-controls) shows the various aspects of the RAG pattern we discussed and the quality controls available to validate them. It's a lot to get through.

{#rag-evaluation-controls}
![RAG pattern evaluation controls](rag-evaluation-controls.png)

I could go through the process step-by-step, explaining all the small details of how to validate each component in the RAG pattern. But I've found this to be very unpractical. I would much rather leave you with a workflow that you can extend if you
need a more detailed approach.

In the next section, we'll look at a basic pipeline to evaluate the RAG pipeline we built earlier in the chapter. Let's get started with an overview of the validation process.

### Overview of the validation process

When testing an RAG pipeline, I like measuring performance with one or two metrics that matter the most to me. For example, I think it's important that the LLM's final answer usess as much content from the source documents as possible. We can measure this using the faithfulness metric.

The people who made [Ragas][RAGAS_LIBRARY] invented the faithfulness metric. It's a model-based metric that measures whether an answer could have come from the information we retrieved from the vector database. It's a very interesting approach to validating a RAG system and, luckily, not too hard to implement.

We need three steps to determine the faithfulness of the RAG pipeline.

1. First, we must create a validation dataset containing questions we can ask.
2. Next, we need to generate output based on the validation questions, recording the answer, the retrieved context information, and the question we asked.
3. Finally, we can measure the faithfulness of the responses.

Let's start by writing code to generate validation data for the RAG pipeline.

### Generating validation data

Generating synthetic data is a great way to get validation questions fast. You can gather questions from users or monitoring data, but interviewing users takes up a lot of time, and gathering data from the monitoring system is only feasible if you're running in production. I also found that gathering information from the monitoring system can take a lot of time because you must differentiate between random prompts and actual RAG-related questions. If you have more than 10 users, it will be tough to make this distinction.

Let's look at how to generate synthetic questions to help you get started validating your RAG pipeline. To create synthetic questions, you can use the following prompt:

```text
Help prepare QnA pairs with short answers for a topic by
extracting them from the given text. Make sure to write detailed
questions about the given text with a detailed answer.
Output must list the specified number of QnA pairs in JSON.

Text:
<|text_start|>
{{context}}
<|text_end|>

Output with {{count}} QnA pairs with detailed answers:  
```

In this prompt, we instruct the LLM to generate questions and answers for a given piece of context information. We then give it the context information we want to base the questions on and finally hint that we want a specific number of QnA pairs.

The code to execute the prompt looks like this:

```csharp
var prompt = kernel.CreateFunctionFromPromptYaml(
    EmbeddedResource.Read("Prompts.LongAnswerQuestion.yaml"), 
    new HandlebarsPromptTemplateFactory());

var promptExecutionSettings = 
    new OpenAIPromptExecutionSettings
    {
        ResponseFormat = typeof(QuestionGeneratorResult)
    };

var promptExecution = await prompt.InvokeAsync(
    kernel, new KernelArguments(promptExecutionSettings)
    {
        ["context"] = content,
        ["count"] = numberOfQuestions
    });

var responseData = JsonSerializer.Deserialize<QuestionGeneratorResult>(
    promptExecution.GetValue<string>()!);
```

In this code, we perform the following steps:

1. First, we load the prompt as a kernel function
2. Next, we create prompt execution settings to specify the answer structure we expect from the LLM.
3. Then, we execute the prompt with a piece of context information and the number of questions we want generated.
4. Finally, we deserialize the content using the JSON serializer into the question generator result.

The `QuestionGeneratorResult` class looks like this:

```csharp
public class QuestionGeneratorResult
{
    public List<QuestionAnswerPair> QuestionAnswerPairs { get; set; } = new();
}

public class QuestionAnswerPair
{
    public string Question { get; set; } = default!;
    public string Answer { get; set; } = default!;
}
```

We haven't discussed structured output yet, but here's the critical part. In the C# sample code, we're setting the `ResponseFormat` property to the `QuestionGeneratorResult` class. Setting the `ResponseFormat` property tells the LLM to generate specific JSON output that matches the configured class.

The code that runs this prompt is part of a bigger program that performs a couple of steps:

1. First, the program chunks the content as we would typically when indexing content in the vector database.
2. Next, the program iterates over each generated chunk and runs the prompt with each chunk's content.

I recommend storing the used context, the question, and the answer in the dataset so you can verify the quality of the generated dataset later on.

The full source code showing how to generate the validation dataset is stored in the [GitHub repository][GEN_VAL_DATA_SAMPLE]. Feel free to take a look and use it for your own projects. The sample code contains several variations on the question generation prompt. I've included one to generate true/false statements and one to generate short answers.

Now that we have a validation dataset let's generate responses that we can use to measure the RAG system's faithfulness.

### Generating test samples

For this step in the evaluation workflow, we'll use the code we wrote in [#s](#using-the-vector-store-with-a-prompt) to implement a sample RAG pipeline and generate test samples by modifying it.

The main program code looks like this:

```csharp

using var reader = new StreamReader(File.OpenRead("Input/validation-data.csv"));
using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

var validationQuestionRecords = csv.GetRecords<ValidationDataRecord>().ToList();
var testSampleRecords = new List<TestSampleRecord>();

var embeddingGenerator = 
    kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>()

var answeringTool = new QuestionAnsweringTool(
    kernel,
    vectorStore,
    embeddingGenerator);

foreach (var record in validationQuestionRecords.SelectRandom(25))
{
    var result = await answeringTool.AnswerAsync(record.Question);

    var testSampleRecord = new TestSampleRecord(
        record.Question, result.Response, result.Context);

    testSampleRecords.Add(testSampleRecord);
}

using var outputStream = File.OpenWrite("test-samples.json");

await JsonSerializer.SerializeAsync(
    outputStream, testSampleRecords,
    new JsonSerializerOptions
    {
        WriteIndented = true
    });
```

In this code, we perform the following steps:

1. First, we load the validation dataset we created earlier.
2. Next, we use the `QuestionAnsweringTool` to generate a response for each question in the dataset. We feed an embedding generator, the kernel, and vector store to it.
3. Finally, we store the question, the answer, and the used context in a JSON file.

The code for this step of the validation workflow can also be found on [GitHub][GH_TEST_SAMPLES]. It includes more details about setting up the kernel instance and code to handle content indexing, similar to how we did it in [#s](#using-the-vector-store-with-a-prompt).

With the test samples generated, we can write some Python code to measure the faithfulness of the RAG pipelines using Ragas.

### Measuring faithfulness

The following fragment shows the code for measuring the faithfulness metric. It is very similar to the code we used to generate the input for this fragment.

```csharp
var inputStream = File.OpenRead("Input/test-samples.json");

var records = await JsonSerializer.DeserializeAsync<List<TestSampleRecord>(
    inputStream);

var promptTemplate = kernel.CreateFunctionFromPromptYaml(
    EmbeddedResource.Read("Prompts.faithfulness-metric.yaml"),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory());

var promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
{
    ResponseFormat = typeof(FaithfulnessMetricResult)
};

var results = new List<FaithfulnessMetricResult>();

foreach (var record in records)
{
    var result = await promptTemplate.InvokeAsync(
        kernel, new KernelArguments(promptExecutionSettings)
        {
            ["context"] = record.Context,
            ["statement"] = record.Answer,
        });

    var resultData = JsonSerializer.Deserialize<FaithfulnessMetricResult>(
        result.ToString());

    results.Add(resultData!);
}

var faithfulCount = results.Count(r => r.Faithful);
var totalCount = results.Count;

var percentage = (double)faithfulCount / totalCount * 100;
```

In this code, we perform the following steps:

1. First, we load the test samples we generated earlier.
2. Next, we use the `faithfulness-metric` prompt to measure the faithfulness of the RAG pipeline for each test sample found in the input file.
3. Finally, we calculate the score for the faithfulness metric.

The code for this step of the validation workflow can also be found on [GitHub][GH_FM_SAMPLE]. It includes more details about setting up the faithfulness metric evaluation step, similar to how we did it in [#s](#using-the-vector-store-with-a-prompt).

Let's look at the prompt used to calculate the faithfulness of an answer. It looks like this:

```handlebars
Your task is to judge whether the statement is faithful to the context provided. 
You must respond with true if the statement can be directly inferred 
from the context or false when the statement can't be directly inferred
from the context.

## Context

{{#each context}}
{{ .Content }}
{{/each}}

## Statement

{{statement}}
```

This prompt is similar to the one we used to answer questions in [#s](#using-the-vector-store-with-a-prompt). It contains the context information retrieved from the vector database. In the question-answering prompt, we inserted the question from the user; however, in the validation prompt, we need to insert the answer that the LLM gave to match that against the context information.

The prompt's output is either true when the response is faithful to the context or false when the LLM can't derive the statement from the context.

As with all model-based validations, it's important to remember that you don't always get a correct answer. The faithfulness validation may not work in all cases.

If you're interested in how the faithfulness metric came to be, I recommend looking at the [Ragas][RAGAS_LIBRARY] documentation for this metric. It outlines the formulas and general ideas behind the metric quite well.

Once you have validated the RAG pattern implementation with the faithfulness metric, you'll be able to optimize it by tweaking the size of the chunks, the embedding model, and the number of fragments you retrieve to answer questions.

I can imagine that validating with synthetic data doesn't sit quite right with many of you. So let me show you one more trick that will help you get a more realistic picture of the quality of your solution.

## Evaluating the RAG pattern with user research

In the previous section, we validated the RAG pattern through automated testing using synthetic data. It's a great start to get the right quality. However, it doesn't always match how production will play out. But there's a great tool to solve this.

At one of the projects I worked on, we used guerrilla-style user research sessions to gather (sometimes brutally honest) user feedback on the user experience of our product. We ran short sessions of around 15 minutes where we let users perform a task in our product. During the session, we recorded the face of the person performing the task and the computer screen they were working on. Afterward, we would sit down with the team and write down any problems we spotted. This approach helped us improve our product quickly and efficiently. And it was fun because we had a good laugh about many of the weird problems we produced.

While our testing revolved around user experience, you can use a similar approach to gather focused data for the quality of your RAG system.

First, make sure your application is up and running. You have monitoring that allows you to grab trace information, including the input, context, and generated answer for the RAG pattern implementation you're testing.

Next, ask two or three people to join you for a 15-minute testing session. Let them use the system in a realistic scenario. You can ask them to bring along cases they want to try, or you can write down one or two scenarios you want the users to work through.

Then, sit down with the users and let them work through the scenarios. Make sure the users understand what they're supposed to do, and be prepared to answer any UX-related questions to help them successfully complete the test.

After the test, grab the recorded trace information and run the evaluation steps from the previous section using the question, context, and answer data you collected. It will give you a realistic picture of what the system does. You can use this data after you've made improvements to test if you've made noticeable improvements.

The steps in this section and previous sections should get you toward a helpful RAG implementation. While the basic RAG pattern implementation is great for many scenarios, it's not a one-size-fits-all solution.

With the basics in hand, I recommend checking out other variations of the RAG pattern to see if they're a good fit for your solution.

## Variations on the RAG pattern

I can fill a bookshelf with information about building RAG systems. As I'm writing this book, many variations are being developed, making it impossible to mention all the variations here.

I want to mention a few ideas to inspire you if you want to try them out or if the basic pattern doesn't help as well as you'd hope.

### Using a Graph RAG

During the summer of 2024, I tried a new pattern called the Graph RAG. This approach to RAG involves building a graph out of concepts mentioned in the text and linking text fragments to those concepts.

Once a graph links concepts to pieces of text, you can use a cosine similarity search like a traditional vector database to find an initial match to the user's question and link other important text fragments to the initial match by navigating the group via the related concepts.

It's a powerful idea that can be helpful if you need to link multiple pieces of information about the same concept to answer user questions.

The people at Neo4J did a great job describing the Graph RAG in greater detail on [their website][NEO4J]. I recommend looking since they describe the concept well and have a working solution.

### Optimizing retrieval using reranking

Graph RAG is a radically different approach to implementing the RAG pattern, so it may be hard to get right the first time. Fear not, though. There are other ways to improve your RAG implementation.

You can, for example, choose to retrieve more fragments initially and then use reranking to reorder the items from most-relevant to least-relevant. Afterward, you can limit the final resultset to fewer pieces than initially retrieved. 

Rerankers sort embedding vectors based on their relevance to the question. This sorting mechanism sounds similar to matching documents in a vector database where we use cosine similarity. The trick here is that rerankers use a neural network to sort, often resulting in a more precise ranking.

I don't often use rerankers because they're almost as slow as a typical LLM. On top of that, rerankers usually are language-specific, so it's hard to use them in scenarios where you're dealing with multiple languages.

If speed is not an issue, you can consider implementing a reranker to improve the precision of the retrieval results in your RAG implementation.

### Differentiating matching and retrieval

Another method that can help improve the accuracy of the responses in your RAG implementation is matching small fragments while retrieving bigger ones.

The idea here is this: You can get a more precise match on embedding vectors covering a small text but a better response from a larger text because you can get a more detailed response.

Remember that you have around 1500 numbers to describe the content of a piece of text. If you have to summarize a whole chapter with 1,500 words, it's harder to express what the text means than using the same 1,500 numbers to describe one paragraph.

Retrieving slightly larger text pieces helps prevent made-up answers because the LLM can copy more from the original text.

Whatever you choose, it's essential to ensure you have an evaluation pipeline running to measure what ideas help improve your RAG implementation.

## Summary

In this chapter, we covered a lot about the RAG pattern, one of the first design patterns discussed in the book. We talked about the components involved in the RAG pattern and how to implement an end-to-end RAG pattern. We also covered how to test various parts of the RAG pattern using model-based tests and user research. Finally, we discussed variations on the regular RAG pattern, including how to use graphs and reranking and optimizing retrieval mechanisms to improve your RAG implementation.

In the next chapter, we'll shift our focus towards using LLMs to generate structured output. I promise it will be less involved than implementing a RAG pattern, but it will still be fun and essential if you want to build more complicated workflows as we continue working towards chapters 9 and 10 of the book.

[SAMPLE_SOURCE_1]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-07/csharp/Chapter7.RetrievalAugmentedGeneration/
[SO_BLOG]: https://stackoverflow.blog/2024/12/27/breaking-up-is-hard-to-do-chunking-in-rag-applications/
[GEN_VAL_DATA_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-07/csharp/Chapter7.ValidationDatasetGeneration/
[GH_TEST_SAMPLES]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-07/csharp/Chapter7.TestSampleGeneration/
[GH_FM_SAMPLE]: https://github.com/wmeints/effective-llm-applications/tree/publish/samples/chapter-07/csharp/Chapter7.FaithfulnessMetric/
[RAGAS_LIBRARY]: https://docs.ragas.io/en/stable/concepts/metrics/available_metrics/faithfulness/
[NEO4J]: https://neo4j.com/blog/news/graphrag-ecosystem-tools/
[ES]: https://www.elastic.co/
