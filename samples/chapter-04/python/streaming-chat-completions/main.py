import asyncio

from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import (
    AzureChatCompletion,
    AzureChatPromptExecutionSettings,
)
from semantic_kernel.contents.chat_history import ChatHistory
from settings import ApplicationSettings

app_settings = ApplicationSettings()
kernel = Kernel()

chat_completion = AzureChatCompletion(
    api_key=app_settings.api_key,
    deployment_name=app_settings.deployment_name,
    endpoint=app_settings.endpoint,
)

kernel.add_service(chat_completion)


async def main():
    history = ChatHistory(
        system_message="You're a digital chef help me cook. Your name is Flora."
    )

    history.add_user_message("Hi, I'd like a nice recipe for a french style apple pie")

    execution_settings = AzureChatPromptExecutionSettings(
        max_token=2500, temperature=0.6, top_p=0.98
    )

    response = chat_completion.get_streaming_chat_message_content(
        history, execution_settings
    )

    async for chunk in response:
        if chunk is not None:
            print(chunk.content, end="")


asyncio.run(main())
