# Contributing to Onenote2md

Thank you for your interest in contributing to the Onenote2md project! This document provides guidelines and instructions to help you get started and contribute effectively.

## Getting Started

1. **Clone the repository:**
   ```sh
   git clone https://github.com/ChristosMylonas/onenote2md.git
   ```
2. **Open the solution:**
   - Open `Onenote2md.Solution` in Visual Studio (recommended for .NET Framework 4.8 projects).

3. **Restore NuGet packages:**
   - Visual Studio should restore packages automatically. If not, right-click the solution and select `Restore NuGet Packages`.

4. **Build the solution:**
   - Build the solution to ensure everything compiles.

## Project Structure
- **Onenote2md.Core**: Core logic for parsing and converting OneNote notebooks to Markdown.
- **Onenote2md.Shared**: Shared types and utilities.
- **Onenote2md.Cmd**: Command-line interface for running conversions.
- **Onenote2md.Tester**: Test project for validating functionality.

## Coding Guidelines
- Use C# 7.3 features and target .NET Framework 4.8.
- Follow existing code style and naming conventions.
- Write clear, concise comments for complex logic.
- Prefer using existing helper methods and utilities.
- Add or update unit tests in `Onenote2md.Tester` when adding new features or fixing bugs.

## Making Changes
1. **Create a new branch:**
   ```sh
   git checkout -b feature/your-feature-name
   ```
2. **Make your changes and commit:**
   - Write descriptive commit messages.
3. **Test your changes:**
   - Run and verify all tests pass.
4. **Push and open a pull request:**
   - Push your branch and open a PR on GitHub.

## Useful Tips
- The main entry point for conversion logic is in `MDGenerator.cs`.
- Use the `NotebookParser` class to interact with OneNote data.
- For Markdown output, see the `IWriter` and `MarkdownPage` types.
- If you add new dependencies, update the relevant `.csproj` files.

## Need Help?
- Review the code comments and existing documentation.
- Open an issue on GitHub if you have questions or need clarification.

---
Happy coding!
