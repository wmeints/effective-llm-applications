# Model-based testing

This sample demonstrates how to run basic unit-tests to validate a prompt.

## Prerequisites

Make sure you have configured the required user-secrets for this sample. 
Use the command `dotnet user-secrets set <key> <value` to set the secrets. The following secrets are required:

| Key                          | Value                               |
|------------------------------|-------------------------------------|
| LanguageModel:Endpoint       | <url to your azure OpenAI resource> |
| LanguageModel:DeploymentName | <name of the GPT-4o deployment>     |
| LanguageModel:ApiKey         | <API key for the GPT-4o deployment> |

## Running the sample

This sample uses xunit to model unit-tests. You can run this sample using the following command:

```bash
dotnet test
```

The output is a green test result.