import asyncio
from pathlib import Path

from dotenv import load_dotenv
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.functions import KernelFunctionFromPrompt
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

prompt_template = kernel.add_function(
    "sample_plugin",
    KernelFunctionFromPrompt(
        "generate_recipe",
        prompt=(Path(__file__).parent / "prompt.txt").read_text(),
        template_format="handlebars",
    ),
)


async def main():
    args = KernelArguments(dish="pizza", ingredients=["mozzarella", "spinach"])

    result = await kernel.invoke(prompt_template, arguments=args)

    print(str(result))


asyncio.run(main())
