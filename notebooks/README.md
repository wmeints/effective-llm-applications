# Interactive Python Notebooks

This book not only comes with samples in C#. I've included some useful notebooks to help
visualize various concepts. Currently, I have one notebook for the following topics:

- Temperature scaling
- Top-P sampling

## Running the notebooks

You need to have python 3.12 for the notebooks to work. You can install it from the
[official website](https://www.python.org/downloads/). In addition to a working Python
installation you'll want to get the [Notebook
extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode)
for Visual Studio Code.

I recommend using a dedicated virtual environment for the notebooks. You can create one
by executing the following command from the root of the repository in a terminal window:

```bash
python -m venv .venv
```

Then, activate the virtual environment:

```bash
source .venv/bin/activate
```

**Note:** On Windows, you should use `. .venv/scripts/Activate` in Powershell instead.

Finally, install the required packages:

```bash
pip install -r requirements.txt
```

You can now open [the notebook](./visualize-top-p.ipynb) in Visual Studio Code and run
the contents by pressing the "Run All" button on the toolbar.
