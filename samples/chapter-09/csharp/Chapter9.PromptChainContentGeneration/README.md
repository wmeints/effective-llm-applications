# Generating content with a chain-of-thought prompt

This sample demonstrates how to use a prompt chain to generate content for an article.
If you're looking to compare techniques, I recommend looking at the 
[Chapter9.ChainOfThoughtContentGeneration](../Chapter9.ChainOfThoughtContentGeneration/)
as well. It uses a single prompt instead of a prompt chain.

## System requirements

- [.NET 9 or higher](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Access to Azure OpenAI](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource?pivots=web-portal)
- [Visual Studio Code](https://code.visualstudio.com/)

## Setting up the sample

### Configuring access to the Google Search API

Before you can run this sample, make sure you have access to a Google custom Search API key.
You can follow the guide [here](https://developers.google.com/custom-search/v1/overview) to configure one.
In addition, you will need to have a Google Custom Search Engine ID. You can get one
[here](https://programmablesearchengine.google.com/controlpanel/all).

After obtaining a key and search engine ID, configure both as a user secret using the command
`dotnet user-secrets set <key> <value>`.
Use the following table as a reference for the key and value to set:

| Key                   | Value                                                                     |
| --------------------- | ------------------------------------------------------------------------- |
| Google:ApiKey         | The API Key you obtained through the Google Developer Console             |
| Google:SearchEngineId | The Search Engine ID you obtained through the search engine control panel |

### Configuring the language model settings

To connect to the LLM, you'll need to configure the following settings as user secrets. Use the command
`dotnet user-secrets set <key> <value>` to set the values.

| Key                          | Value                                                                  |
| ---------------------------- | ---------------------------------------------------------------------- |
| LanguageModel:ApiKey         | The API Key for the Azure OpenAI resource you're using                 |
| LanguageModel:Endpoint       | The URL to the Azure OpenAI resource you're using                      |
| LanguageModel:DeploymentName | The deployment name of a GPT-4o deployment within your OpenAI resource |

## Running the sample

The sample has a preconfigured topic. You can modify the topic by editing the code in `Program.cs`. To run the sample, execute the following command in a terminal from the project directory:

```bash
dotnet run
```
