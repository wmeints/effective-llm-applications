# Getting started with Semantic Kernel in Python

This sample demonstrates how to use the Semantic Kernel template language in Python.

## System requirements

- [Python 3.12](https://www.python.org/downloads/) or later
- [Visual Studio Code](https://code.visualstudio.com/download)
- [Access to Azure OpenAI](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource?pivots=web-portal)

## Setting up the virtual environment

This sample uses a virtual environment to manage dependencies. To set up the virtual environment, run the following commands:

```bash
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
```

For Windows users, the commands are slightly different:

```bash
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
```

## Running the sample

Make sure to [deploy
GPT-4o](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model)
in your Azure OpenAI resource and name the deployment `gpt-4o`. Next, create a new file named `.env` in the root directory of the sample and add the following variables:

| Variable              | Description                                |
| --------------------- | ------------------------------------------ |
| `APP_DEPLOYMENT_NAME` | The name of the deployment                 |
| `APP_API_KEY`         | The API key of your Azure OpenAI resource  |
| `APP_ENDPOINT`        | The endpoint of your Azure OpenAI resource |

You can find a sample `.env` file in the `.env.example` file.

To run the sample, execute the following command:

```bash
python main.py
```
