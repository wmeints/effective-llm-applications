# Extracting test data from application insights

This sample demonstrates how to process raw application insights trace data into potential test data for your
application. This can be useful for expanding test cases in your application with real-world data.

## Prerequisites

Make sure you have configured the required user-secrets for this sample. 
Use the command `dotnet user-secrets set <key> <value` to set the secrets. The following secrets are required:

| Key                          | Value                                 |
|------------------------------|---------------------------------------|
| LanguageModel:Endpoint       | `<url to your azure OpenAI resource>` |
| LanguageModel:DeploymentName | `<name of the GPT-4o deployment>`     |
| LanguageModel:ApiKey         | `<API key for the GPT-4o deployment>` |

## Running the sample

This sample is built as a console application. You can run it from this directory using the following command:

```bash
dotnet run
```

The output of the program is a CSV file with the extracted test data. Make sure to review your test data
before including it in your tests!