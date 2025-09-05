# Contributing to onenote2md

Thank you for your interest in contributing to the onenote2md project! This document will help you get started with understanding, building, and contributing to the project.

## Project Overview

**onenote2md** is a tool for converting Microsoft OneNote pages to Markdown format. It consists of several components:
- **Onenote2md.Core**: Core logic for parsing OneNote files and generating Markdown.
- **Onenote2md.Cmd**: Command-line interface for running conversions.
- **Onenote2md.Shared**: Shared data structures and interfaces.
- **Onenote2md.Tester**: GUI tester for development and debugging.

## Getting Started

### Prerequisites
- Windows OS (required for OneNote Interop)
- [.NET Framework 4.7.2+](https://dotnet.microsoft.com/en-us/download/dotnet-framework)
- Visual Studio 2019 or later (recommended)
- Microsoft OneNote installed (for Interop)

### Building the Project
1. Clone the repository:
   ```sh
   git clone https://github.com/ChristosMylonas/onenote2md.git
   ```
2. Open `Onenote2md.Solution/onenote2md solution.sln` in Visual Studio.
3. Restore NuGet packages if prompted.
4. Build the solution (Ctrl+Shift+B).

### Running the CLI
- Build the solution.
- Run `Onenote2md.Cmd.exe` from the `Onenote2md.Cmd/bin/Debug/` directory.
- Example usage:
  ```sh
  Onenote2md.Cmd.exe <input-one-file> <output-md-file>
  ```

### Project Structure
- `Onenote2md.Core/`: Main logic for parsing and conversion.
- `Onenote2md.Cmd/`: Command-line interface.
- `Onenote2md.Shared/`: Shared models and helpers.
- `Onenote2md.Tester/`: Windows Forms tester app.
- `Doc/`: Documentation and sample files.

## Contributing
- Fork the repository and create a feature branch.
- Make your changes and add tests if needed.
- Submit a pull request with a clear description of your changes.

## Issues
If you find a bug or have a feature request, please open an issue on GitHub.

## License
This project is licensed under the MIT License.

---

For more details, see the `readme.md` and `CONTRIBUTING.md` files.
