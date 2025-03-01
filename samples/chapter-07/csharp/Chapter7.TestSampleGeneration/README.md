# Test sample generation for RAG validation

This sample demonstrates how to run a set of synthetic questions through a RAG pipeline
to obtain test data for validating the RAG pipeline.

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

## Preparing the data for the sample

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

The application will process all samples from [./Input/validation-data.csv](./Input/validation-data.csv)
to generate an answer based on the content in [Content](./Content).

At the end of the program, the output is stored in `test-samples.json` in the project directory.
