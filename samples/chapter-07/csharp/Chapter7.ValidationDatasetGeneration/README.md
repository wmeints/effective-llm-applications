# Validation question generation for RAG validation

This sample demonstrates how to generate synthetic questions to validate a RAG implementation.

## Check the other samples

Note that this sample is part of a series of samples:

- [Chapter7.ValidationDatasetGeneration](../Chapter7.ValidationDatasetGeneration/) - Generates synthetic questions
- [Chapter7.TestSampleGeneration](./) - Generates output needed to run the validation
- [Chapter7.FaithfulnessMetric](../Chapter7.FaithfulnessMetric/) - Runs the validation of the RAG pipeline

## System requirements

- [.NET 9 or higher](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Access to Azure OpenAI with `gpt-4o` and `text-embedding-3-small` models deployed
- [Visual Studio Code](https://code.visualstudio.com/)

## Setting up the sample

Make sure you have access to an Azure OpenAI resource with GPT-4o and text-embedding-small models deployed.
You can learn how to set up Azure OpenAI in [this quickstart tutorial](https://learn.microsoft.com/en-us/azure/ai-services/openai/chatgpt-quickstart?tabs=command-line%2Ckeyless%2Ctypescript-keyless%2Cpython-new&pivots=programming-language-studio).

This sample relies on the book content itself. You download the markdown
files from [manuscript](../../../../manuscript/)
and save them in the [Content](./Content/) directory of the project.

## Running this sample

To run this sample you'll need to configure a number of user secrets by executing
`dotnet user-secrets set <key> <value>` with a terminal in the project directory.
Please find the secrets in the table below:

| Secret Name                            | Secret Value                                          |
| -------------------------------------- | ----------------------------------------------------- |
| LanguageModel:Endpoint                 | The URL of the Azure OpenAI instance you want to use  |
| LanguageModel:ApiKey                   | The API key for the Azure OpenAI instance want to use |
| LanguageModel:CompletionDeploymentName | The name of the GPT-4o deployment you want to use     |

After you've configured the user secrets you can run the sample using the following command:

```bash
dotnet run
```

The application will process content stored in the [Content](./Content) directory
and generate a `validation-data.csv` file in the same [Content](./Content) directory.
