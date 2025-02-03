# Integrating OpenAPI based plugins

This sample demonstrates how to use OpenAPI based plugins.

## Prerequisites

Make sure you have configured the required user-secrets for this sample.
Use the command `dotnet user-secrets set <key> <value` to set the secrets. The following secrets are required:

| Key                          | Value                                 |
|------------------------------|---------------------------------------|
| LanguageModel:Endpoint       | `<url to your azure OpenAI resource>` |
| LanguageModel:DeploymentName | `<name of the GPT-4o deployment>`     |
| LanguageModel:ApiKey         | `<API key for the GPT-4o deployment>` |

## Running the sample

This sample is built as a console application. It needs a running version of `Chapter6.TimeApi`.
You can start the time API service by navigating to its directory and running:

```bash
dotnet run
```

Next, open up a second terminal and navigate to the directory of this sample, and execute:

```bash
dotnet run
```