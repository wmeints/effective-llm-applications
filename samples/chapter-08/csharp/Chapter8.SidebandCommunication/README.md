# Combining chat with structured output

This sample demonstrates how to use tool calling to get structured output to a frontend
connected to the LLM-based application in parallel to a chat endpoint.

## System requirements

- [.NET 9 or higher](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Access to Azure OpenAI with `gpt-4o` model deployed
- [Visual Studio Code](https://code.visualstudio.com/)

## Setting up the sample

Make sure you have access to an Azure OpenAI resource with the GPT-4o model deployed.
You can learn how to set up Azure OpenAI in [this quickstart tutorial](https://learn.microsoft.com/en-us/azure/ai-services/openai/chatgpt-quickstart?tabs=command-line%2Ckeyless%2Ctypescript-keyless%2Cpython-new&pivots=programming-language-studio).

## Running this sample

To run this sample you'll need to configure a number of user secrets by executing
`dotnet user-secrets set <key> <value>` with a terminal in the project directory.
Please find the secrets in the table below:

| Secret Name                  | Secret Value                                          |
| ---------------------------- | ----------------------------------------------------- |
| LanguageModel:Endpoint       | The URL of the Azure OpenAI instance you want to use  |
| LanguageModel:ApiKey         | The API key for the Azure OpenAI instance want to use |
| LanguageModel:DeploymentName | The name of the GPT-4o deployment you want to use     |

After you've configured the user secrets you can run the sample using the following command:

```bash
dotnet run
```

