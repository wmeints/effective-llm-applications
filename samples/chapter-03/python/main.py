import asyncio

from dotenv import load_dotenv
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

from settings import ApplicationSettings

load_dotenv(".venv")

app_settings = ApplicationSettings()
kernel = Kernel()

kernel.add_service(
    AzureChatCompletion(
        api_key=app_settings.api_key,
        deployment_name=app_settings.deployment_name,
        endpoint=app_settings.endpoint,
    )
)


async def main():
    response = await kernel.invoke_prompt(
        "Generate 5 product names for a new line of shoes."
    )

    print(response)


asyncio.run(main())
