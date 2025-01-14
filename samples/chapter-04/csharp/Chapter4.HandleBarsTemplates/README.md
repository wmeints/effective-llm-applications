# Using Handlebars templates with Semantic Kernel

This sample demonstrates how to use Handlebars templates with Semantic Kernel.

## System Requirements

- [.NET Core SDK 9.0](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/download)
- [Access to Azure OpenAI](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource?pivots=web-portal)

## Configuring the sample

For this sample I switched to using an API key instead of a managed identity I discussed in the book
to make things simpler for people. You'll need to  add a `settings.json` file with the following content:

```json
{
  "LanguageModel": {
    "Endpoint": "<azure openai endpoint>",
    "DeploymentName": "<name of the deployment>",
    "ApiKey": "<api key>"
  }
}
```

Make sure to fill in the placeholders with the configuration of your OpenAI resource.

| Variable name  | Description                                                          |
|----------------|----------------------------------------------------------------------|
| Endpoint       | The URL for the Azure OpenAI resource                                |
| DeploymentName | The name of the deployment of a GPT-4o model in your OpenAI resource |
| ApiKey         | The API Key to use for the resource                                  |

**Note:** I still recommend using a managed identity for a better security posture! Read section "Setting up a project with
Semantic Kernel" from chapter 3 to understand how to use a more secure method to connect to Azure OpenAI.

 ## Running the sample

I recommend running the sample by executing `dotnet run` from the directory where the sample is stored.
You can run the sample from VSCode or other IDEs. Just make sure that you set the root directory of the sample
as the working directory in your debugger.