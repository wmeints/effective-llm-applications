# Model-based testing

This sample demonstrates how to configure telemetry for Semantic Kernel.

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

You should see log events on the terminal and information about metering data being collected.