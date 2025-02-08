import asyncio
from pathlib import Path

from dotenv import load_dotenv
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.kernel import KernelArguments
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
    args = KernelArguments(dish="pizza")
    prompt_template = (Path(__file__).parent / "prompt.txt").read_text()

    response = await kernel.invoke_prompt(
        prompt_template, arguments=args, template_format="semantic-kernel"
    )

    print(response)


asyncio.run(main())
