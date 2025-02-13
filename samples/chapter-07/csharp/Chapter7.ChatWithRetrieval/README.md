# Retrieval Augmented Generation

This sample demonstrates how to use Retrieval Augmented Generation with chat completions and functions.

## System requirements

- [.NET 9 or higher](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Access to Azure OpenAI
- [Visual Studio Code](https://code.visualstudio.com/)
- [REST Client extension for VSCode](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)

## Setting up the sample

Before you can run the sample, you need to make sure you have the content that you want to use for the RAG solution.
You can use [the chapters of the book ](../../../../manuscript/) as the content. To use the content from the book,
download the markdown files to the `Content` directory in the project.

Next, make sure you have the following user secrets configured:

| Secret Name                     | Description                                                                        |
| ------------------------------- | ---------------------------------------------------------------------------------- |
| `LanguageModel:ApiKey`          | The API Key for the Azure OpenAI resource                                          |
| `LanguageModel:Endpoint`        | The endpoint for the Azure OpenAI resource                                         |
| `LanguageModel:CompletionModel` | The name of a `GPT-4o` deployment in your Azure OpenAI environment                 |
| `LanguageModel:EmbeddingModel`  | The name of a `text-embedding-3-small` deployment in your Azure OpenAI environment |

You can configure the secrets by using the following command:

```bash
dotnet user-secrets set "<Key>" "<Value>"
```

You can learn how to set up Azure OpenAI in [this quickstart tutorial](https://learn.microsoft.com/en-us/azure/ai-services/openai/chatgpt-quickstart?tabs=command-line%2Ckeyless%2Ctypescript-keyless%2Cpython-new&pivots=programming-language-studio).

You'll also need a Qdrant server running on your local machine. You can do this using the following command:

```bash
docker run -d --name qdrant -p 6333:6333 -p 6334:6334 qdrant/qdrant:latest
```

## Running the sample

Assuming you've setup the sample correctly, run the following command from the project directory to run the project.

```bash
dotnet run
```

When the project is started, open the request in [test.http](./chat.http) in VSCode to test the chat API endpoint. 
You can execute the request from the `Send request` contextual command in the editor.
