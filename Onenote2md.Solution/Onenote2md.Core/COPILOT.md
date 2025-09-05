# GitHub Copilot Instructions for Onenote2md.Core

This file provides context and guidance for GitHub Copilot and developers to better understand the code and workflow in the Onenote2md.Core project.

## Project Purpose
Onenote2md.Core contains the main logic for converting Microsoft OneNote notebooks into Markdown (.md) format. It handles parsing, transformation, and output generation.

## Key Components
- **MDGenerator**: The main class responsible for traversing OneNote XML, interpreting its structure, and generating Markdown content. Entry point for most conversion logic.
- **NotebookParser**: Used to interact with OneNote data and extract notebook, section, and page information.
- **IWriter / MDWriter**: Abstractions for writing Markdown output to files or other destinations.
- **MarkdownPage**: Represents a single Markdown page, including content, title, and filename.
- **QuickStyleDef / TagDef**: Helpers for handling OneNote styles and tags during conversion.

## Workflow Overview
1. **Initialization**: `MDGenerator` is constructed with a `NotebookParser` instance.
2. **Conversion**: Use methods like `GenerateNotebookMD`, `GenerateSectionMD`, or `GeneratePageMD` to start conversion. These methods traverse the OneNote hierarchy and generate Markdown files.
3. **Parsing**: The code uses LINQ to XML (`XDocument`, `XElement`) to parse OneNote's XML structure.
4. **Markdown Generation**: The `GenerateChildObjectMD` method recursively processes XML nodes, converting them to Markdown syntax based on their type (text, table, image, etc).
5. **Output**: Markdown content is written using the `IWriter` interface, allowing for flexible output destinations.

## Coding Patterns
- Use helper methods for XML attribute and element extraction.
- Prefer `StringBuilder` for content assembly.
- Use context objects (e.g., `MarkdownGeneratorContext`) to pass state during recursive generation.
- Follow the existing naming and code style conventions.

## Tips for Copilot
- When adding new conversion logic, extend `GenerateChildObjectMD` with new cases for additional OneNote node types.
- Use the provided helpers for style, tag, and image handling.
- Keep logic modular and avoid duplicating code found in helpers.
- Add comments for any complex or non-obvious logic.

## References
- See `MDGenerator.cs` for the main conversion logic.
- Review `NotebookParser.cs` for OneNote data access patterns.
- Check `IWriter` and `MDWriter` for output handling.

---
This file is intended to help Copilot and contributors provide relevant, context-aware code suggestions and maintain consistency in the core logic.
