# Building effective LLM-based applications with Semantic Kernel

Welcome to my new book about building effective LLM-based applications with Semantic
Kernel. This book is available on Leanpub! This repository is where I build the book.

## Reading the book

The first chapter of the book is complete. Get a copy now and start reading! Read it on
Leanpub: [Building effective LLM-based applications with Semantic
Kernel](https://leanpub.com/effective-llm-applications-with-semantic-kernel)

## Progress

I'm writing this book, and I'm at **15% completion**.

## Working with the examples

This book features useful examples that demonstrate how to implement various patterns
with Semantic Kernel. To use the examples you'll need:

- [.NET 9](https://dot.net/)
- [Visual Studio Code](https://code.visualstudio.com)
- Access to the OpenAI API or Azure OpenAI

You can open the solution in the `examples` directory to browse through them.
Follow the instructions provided with each example to run the example.

## Sponsor me

I am writing this in my spare time to help people gain practical knowledge about
working with large language models. The book is free, but I take donations as a token
of gratitude.

You can donate by purchasing the book online through Leanpub. You can also sponsor me
through the sponsorship button on this repository.

## My workflow

Since you're here anyway, let me share my workflow for this book so you'll get to know
a little more about how I work and what you can expect from me.

My workflow has the following phases:

- Research
- Writing
- Review and initial editing
- Formatting and layout
- Publishing

I publish after each chapter is complete and undergone initial editing. This way, you
get the content as fast as possible. After I've completed the full book, I'll go through
the entire book again to ensure everything is consistent and correct.

### Research

The book contains my experience with building LLM-based applications using Semantic Kernel.
I try to pull from experience as much as possible because it gives the best results for me.

However, before I write, I do my research to verify that my ideas are going to work and build
a sample to demonstrate the principles. Working code is still the best way to demonstrate
things. The book is the narrative to the example.

### Writing

I write an initial set of notes in Markdown and then use Claude.ai to generate an
outline for the chapter. This helps me to bootstrap the chapter. I then refine the
outline as I see fit to ensure that the storyline matches with what I have in mind.

After the initial outline, I'll start by adding detail to each of the sections in the
outline. I still use a note writing style as I want to make sure that each section makes
sense to you as the reader of my work. Often, I need to rework the outline as I write
the notes because I find that the initial outline doesn't work out as I thought.

Once I have the notes for the full chapter, I'll go back to Claude.ai to let it generate
portions of text for me. I have a project in Claude with custom style instructions, the
outline for the book, and the chapters I've already completed.

I'll then go through all the generated text to fine tune it to what I want to say.
Claude is here to speed things up, but I'm the boss. The only value I gain from Claude
in this process is that it helps me prevent writer's block. The notes contain almost the
full text but without looking for fancy words or making sure that everything is perfect.
Claude helps me to get the text out of my head and into a format that I can work with.

### Review and initial editing

Once everything is in the chapter, I'll put aside the text for a day and let it rot,
ripen, or marinate if you will. This helps me to get a fresh perspective on the text.
I'll then go through the text and make sure that everything makes sense and that the
text still conveys what I wanted to tell you.

I perform a final spell check using LanguageTool. I prefer to use [Silviof's Docker
container](https://hub.docker.com/r/silviof/docker-languagetool) for this. In
combination with a VSCode extension called LanguageTool Linter. This helps me to catch
most if not all the spelling and grammar mistakes.

### Formatting and layout

At this point I use the Leanpub preview generator to preview the final look of my book.
I find that I often need to insert additional line breaks and correct figures to make
sure that everything looks good. I also need to make sure that the layout of the code
samples isn't broken.

### Publishing

When I think the chapter is ready, I'll publish a new version in Leanpub.
I'll also update the README.md file to reflect the current state of the book.
