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

There are a lot of solutions for implementing RAG systems online, and it's easy to get confused by the fancy implementations that are out there. But the basics of the RAG pattern are straighforward. To implement one in Semantic Kernel we'll need to integrate a few components as shown in [#s](#end-to-end-rag-pipeline).

{#end-to-end-rag-pipeline}
![End-to-end RAG pipeline in Semantic Kernel](end-to-end-rag-pipeline.png)

We'll need to configure the following pieces:

1. First, we need to build a data model for the data we want retrieve.
2. Next, we need to connect a vector store to house the data.
3. Then, we need to build a workflow that uses the vector store.

We'll use a basic workflow to explore the RAG pattern. Many of the components in the workflow are the same for chat-based scenarios. We'll discuss how to use the RAG pattern with a chatbot after the initial setup.

Let's take a look at the project structure for RAG first.

### Setting up the project structure

Let's start by setting up the project structure. For the scenarion in this chapter, we'll assume you're building a web application. You can create a new web application using the following command:

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
