# Contributing to onenote2md

Thank you for your interest in contributing to the onenote2md project! This document will help you get started with understanding, building, and contributing to the project.

## Project Overview

**onenote2md** is a modern .NET tool for converting Microsoft OneNote pages to Markdown format. The project has been modernized to .NET 8 and includes comprehensive testing infrastructure.

### Project Components:
- **Onenote2md.Core**: Core logic for parsing OneNote files and generating Markdown (.NET 8)
- **Onenote2md.Cmd**: Command-line interface for running conversions (.NET 8)
- **Onenote2md.Shared**: Shared data structures and interfaces (.NET 8)
- **Onenote2md.Tester**: WPF GUI application for development and debugging (.NET 8)
- **Onenote2md.Tests**: Comprehensive xUnit test suite with 53+ unit tests (.NET 8)

## Getting Started

### Prerequisites
- Windows OS (required for OneNote COM Interop)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 
- Visual Studio 2022 or VS Code (recommended)
- Microsoft OneNote installed (for COM Interop functionality)

### Building the Project

#### Using Visual Studio Code (Recommended)
1. Clone the repository:
   ```sh
   git clone https://github.com/amir-tepper/onenote2md.git
   ```
2. Open the project in VS Code
3. Use F6 to build (configured task) or run:
   ```sh
   dotnet build "Onenote2md.Solution/onenote2md solution.sln"
   ```

#### Using Command Line
1. Clone and navigate to the project:
   ```sh
   git clone https://github.com/amir-tepper/onenote2md.git
   cd onenote2md
   ```
2. Build the solution:
   ```sh
   dotnet build "Onenote2md.Solution/onenote2md solution.sln" -c Release
   ```

### Running the Applications

#### Command Line Interface
```sh
# Build and run the CLI
dotnet build Onenote2md.Cmd/Onenote2md.Cmd.csproj
dotnet run --project Onenote2md.Cmd/Onenote2md.Cmd.csproj
```

#### WPF GUI Tester
```sh
# Build and run the WPF tester
dotnet build Onenote2md.Tester/Onenote2md.Tester.csproj  
dotnet run --project Onenote2md.Tester/Onenote2md.Tester.csproj
```

#### Running Tests
```sh
# Run all unit tests (53+ tests)
dotnet test Onenote2md.Tests/Onenote2md.Tests.csproj --verbosity normal

# Run specific test class
dotnet test --filter "ClassName~MDGeneratorTests"
```

### Project Structure
- `Onenote2md.Core/`: Core OneNote parsing and Markdown generation logic (.NET 8)
- `Onenote2md.Cmd/`: Command-line interface (.NET 8)  
- `Onenote2md.Shared/`: Shared data models, interfaces, and utilities (.NET 8)
- `Onenote2md.Tester/`: WPF GUI application for testing and debugging (.NET 8)
- `Onenote2md.Tests/`: Comprehensive xUnit test suite (.NET 8)
- `Doc/`: Documentation, samples, and example OneNote files

## Development Workflow

### Architecture Overview
The project follows a clean architecture pattern:
- **Core**: Contains the main OneNote parsing logic and Markdown generation
- **Shared**: Common data structures (MarkdownPage, ObjectType, etc.)
- **CMD**: Console application entry point
- **Tester**: WPF GUI for interactive testing
- **Tests**: Comprehensive test coverage with xUnit

### Key Technologies
- **.NET 8**: Modern .NET with Windows targeting for COM interop
- **Microsoft.Office.Interop.OneNote**: COM interop for OneNote integration
- **WPF**: Modern UI framework replacing legacy Windows Forms
- **xUnit v3**: Latest testing framework with 53+ unit tests
- **SDK-style projects**: Modern project format with simplified .csproj files

### Testing
The project includes comprehensive test coverage:
- **53+ unit tests** covering core functionality
- **Integration tests** for end-to-end workflows  
- **Edge case validation** for error handling
- **Performance tests** for large content processing
- **Unicode support tests** for international content

Test classes include:
- `MDGeneratorTests`: Core markdown generation logic
- `ComplexLogicTests`: Advanced algorithms and workflows
- `XmlParsingTests`: OneNote XML parsing validation
- `IntegrationTests`: End-to-end functionality
- `SubpageExportTests`: Hierarchical page export logic

### Development Environment Setup

#### VS Code (Recommended)
1. Install C# extension
2. Open project folder
3. Use F6 to build (configured in tasks.json)
4. Use integrated terminal for running tests

#### Visual Studio 2022
1. Open `Onenote2md.Solution/onenote2md solution.sln`
2. Ensure .NET 8 SDK is installed
3. Build solution (Ctrl+Shift+B)

## Contributing

### Code Changes
1. Fork the repository and create a feature branch
2. Make your changes following existing code patterns
3. **Add unit tests** for new functionality (required)
4. Ensure all tests pass: `dotnet test`
5. Submit a pull request with clear description

### Adding Tests
When adding new functionality:
- Create corresponding unit tests in `Onenote2md.Tests`
- Test both happy path and error conditions
- Follow existing test naming conventions
- Aim for high code coverage

### Project Modernization Status
✅ **Completed:**
- Migrated from .NET Framework 4.8 to .NET 8
- Converted Windows Forms to WPF
- Created comprehensive test suite (53+ tests)
- Modernized project files to SDK-style
- Set up VS Code development environment
- All builds and tests passing

## Issues
If you find a bug or have a feature request, please open an issue on GitHub with:
- Clear description of the problem/feature
- Steps to reproduce (for bugs)
- Expected vs actual behavior
- System information (.NET version, OneNote version)

## License
This project is licensed under the MIT License.

---

For more details, see the `readme.md` and `CONTRIBUTING.md` files.
