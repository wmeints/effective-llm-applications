# Retrieval augmented generation

In this chapter you'll learn how to use Retrieval Augmented Generation as a way to answer questions based on internal knowledge not yet encoded in the LLM. You'll learn how to index documents for retrieval, and how to use indexed content in the LLMs response.

We'll cover the following topics:

- What is Retrieval Augmented Generation (RAG)
- Building an end-to-end RAG pipeline with Semantic Kernel
- Testing the RAG pipeline
- Optimizing retrieval for RAG
- Variations on the RAG pattern

Let's get started by discussing what Retrieval Augmented Generation (RAG) is and learn what components are involved in this pattern.

## What is Retrieval Augmented Generation (RAG)

Retrieval Augmented Generation is a pattern where you use a retrieval system to find relevant information from an external source to answer questions or generate text. [#s](#rag-pattern-architecture) shows the structure of the RAG pattern.

![RAG pattern architecture](rag-pattern-architecture.png)

You typically find two important components in the RAG pattern:

1. First, there's the retrieval portion, which is responsible for finding relevant information based on the prompt that was entered.
2. Next, there's the generation portion, which is responsible for generating the final response to the prompt.

The basics of the RAG pattern aren't overly involved but you can find a lot of variants on the internet of it. That's the beauty of the pattern. You can adapt and extend it to your needs.

### Retrieving information

For example, you could use a regular search engine for the retrieval portion of the pattern. Elastic Search is one of the most widely used search engines that does an excellent job finding documents based on a query.

Regular search engines will preprocess the query into search terms and match those terms against the documents in the database. To find a document, one or more search terms must match the text in the document. If you use a synonym or make a spelling mistake, the search match will be less precise.

That's why you will find that most people will use vector search because it provides better search results when compared to regular search engines.

Vector search turns your initial question into an embedding vector and matches it against the embedding vectors of the documents in the database. The documents whose vectors are closes to the query vector are returned by the search engine. Vector search uses cosine similarity to determine how close vectors are to each other. Imagine an arrow for the document and another for the query. The cosine similarity is the angle between the two arrows. [#s](#cosine-similarity) demonstrates this principle.

![Cosine similarity](cosine-similarity.png)

Since we're looking at arrows being close, you can have spelling errors, use synonyms, or even use different words to describe the same thing. As long as the text in the document is similar to the test in the query, the vector search will find it, because the embedding vectors will be similar.

Both text search and vector search assume you'll enter the prompt of the user to find relevant documentation. However, that's not a requirement. You can also introduce a tool to the LLM that can find information in a relational database. For example, you can create a tool that finds product information based on a serial number or product name. As we've learned from [#s](#enhancing-llms-with-tools) LLMs are powerful enough to extract relevant information from the prompt and call the right tool to find the product information.

It goes to show, the retrieval portion of the RAG pattern is very flexible and I can spend a lot of pages discussing the different ways you can implement it. But I won't. I'll stick to the basic pattern in this chapter and help you explore beyond that on your own. We'll focus ourselves on implementing vector search for the retrieval portion of the RAG pattern.

Once you've found relevant information, it's time to generate a response. Depending on the kind of solution you're building, you'll have to implement the generation portion slightly differently.

### Generating responses

In general, the generation portion of the RAG pattern is straightforward. You take the information retrieved earlier and inject it into the prompt to generate a response.

However, based on the kind of solution you're building, you'll need a different approach to get the information into the prompt.

In chat scenarios, you'll want to implement the RAG pattern as a tool, and include the output of the tool in the chat history as a tool response. Using a tool gives the LLM the option to just give a straight answer without considering extra content. This is useful when you connect multiple data sources or when you have a more general purpose chat solution. It's weird to have a chatbot inject information it then can't use to generate a useful response. And if you're using multiple sources, it makes no sense to always mix the information from two sources. Sometimes it can make sense, but that's an advanced topic and depends on what you're building.

In non-chat scenarios, you'll want to create a specialized prompt to help guide the LLM in the right direction. The pattern here is to use few-shot learning to help the LLM generate the right response as we discussed earlier in [#s](#few-shot-learning).

Let's look at how to put the theory into practice with Semantic Kernel by building an end-to-end pipeline.

## Building an end-to-end RAG pipeline with Semantic Kernel

There's a lot of theory surrounding RAG online, and it's easy to get confused by the fancy implementations that are out there. But the basics of the RAG pattern are straighforward. To implement one in Semantic Kernel we'll need to integrate a few components as shown in [#s](#end-to-end-rag-pipeline).

![End-to-end RAG pipeline in Semantic Kernel](end-to-end-rag-pipeline.png)

We'll need to configure the following pieces:

1. First, we need to build a data model for the data we want retrieve.
2. Next, we need to connect a vector store to house the data.
3. Then, we need to build a workflow that uses the vector store.

The scenario in this section will cover the basic RAG pattern with a single prompt. We'll discuss integrating the same components into a chat-based solution after discussing the basic pattern implementation.

### Setting up the project structure

Let's start by setting up the project structure. For the scenarion in this chapter, we'll assume you're building a web application. You can create a new web application 
using the following command:

```bash
dotnet new web -n Chapter7.RetrievalAugmentedGeneration
```

Next we need to add a few packages by running the following commands from the project directory we just created:

```bash
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.SemanticKernel.Connectors.Postgres --prerelease
```

The first package adds the basic components for Semantic Kernel to the web application.
The second package adds vector store support based on a Postgres database. You can include other connectors as well, but for now, we'll stick to the PostgreSQL one as most people are familiar with using a relational database such as PostgresSQL.

After setting up the packages, we'll need to modify the `Program.cs` file in the project to include basic configuration for Semantic Kernel. The file should look like this:

```csharp

```

This code configures Semantic Kernel with a new vector store. It performs the following steps:

1. First, we create a new web application builder.
2. Next, we configure the `Kernel` service with the Azure OpenAI connector.
3. Then, we configure the vector store for the application.
4. Finally, we build the web application and map a basic endpoint.

### Building a data model for retrieval



### Connecting a vector store

### Using the vector store with a prompt

### Using the vector store as a tool

## Testing the RAG pipeline

## Optimizing retrieval for RAG

## Variations on the RAG pattern

- Using graphs for retrieval (graphrag: https://microsoft.github.io/graphrag/)
- Reranking 
