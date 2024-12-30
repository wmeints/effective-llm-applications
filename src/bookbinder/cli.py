from pathlib import Path
from typing import Annotated

from promptarchitect.prompting import EngineeredPrompt
from typer import Option, Typer

app = Typer()
generator_app = Typer()

app.add_typer(generator_app, name="generate")


@generator_app.command(name="outline")
def generate_outline(
    topic: Annotated[str, Option(help="The topic you want to write about.")],
    notes: Annotated[
        Path,
        Option(
            help="The path to a markdown file containing the notes to use for the outline.",
            exists=True,
            file_okay=True,
        ),
    ],
):
    notes_text = notes.read_text()

    prompt = EngineeredPrompt(
        prompt_file=Path(__file__).parent / "prompts" / "generate-outline.prompt"
    )

    output = prompt.execute(notes_text, properties={"topic": topic})

    print(output)
