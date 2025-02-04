import argparse
import re


def parse_arguments():
    parser = argparse.ArgumentParser(description="Process an input file.")
    parser.add_argument("input_file", type=str, help="Path to the input file")
    return parser.parse_args()


def remove_line_endings(text):
    # Split text into paragraphs separated by two or more newlines.
    paragraphs = text.split("\n\n")
    # Remove internal linebreaks and rejoin using double newlines.
    fixed_paragraphs = [" ".join(paragraph.splitlines()) for paragraph in paragraphs]
    return "\n\n".join(fixed_paragraphs)


def process_content(content):
    # Pattern to match fenced code blocks that start and end with ```
    pattern = r"(```[\s\S]*?```)"
    # Split the content into segments
    segments = re.split(pattern, content)
    list_item_pattern = re.compile(r"^\s*(?:[-*+]|\d+\.)\s+")
    processed_segments = []
    for segment in segments:
        if segment.startswith("```"):
            # Keep code blocks unchanged.
            processed_segments.append(segment)
        else:
            # Process non-code parts: remove internal line breaks except between list items.
            lines = segment.splitlines()
            processed_lines = []
            buffer = []
            for line in lines:
                # If this line looks like a list item, flush any buffered text first.
                if list_item_pattern.match(line):
                    if buffer:
                        processed_lines.append(" ".join(buffer))
                        buffer = []
                    processed_lines.append(line)
                else:
                    # On blank lines, flush the buffer to preserve intentional breaks.
                    if not line.strip():
                        if buffer:
                            processed_lines.append(" ".join(buffer))
                            buffer = []
                        processed_lines.append(line)
                    else:
                        buffer.append(line.strip())
            if buffer:
                processed_lines.append(" ".join(buffer))
            processed_segments.append("\n".join(processed_lines))
    # Join all segments back together.
    return "".join(processed_segments)


def main():
    args = parse_arguments()

    with open(args.input_file, "r", encoding="utf-8") as file:
        content = file.read()

    result = process_content(content)

    with open(args.input_file, "w", encoding="utf-8") as file:
        file.write(result)


if __name__ == "__main__":
    main()
