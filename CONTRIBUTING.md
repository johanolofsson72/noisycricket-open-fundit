# Contributing to OpenFundit

Thank you for your interest in contributing to OpenFundit! This document provides guidelines for contributing to the project.

## Getting Started

1. Fork the repository
2. Clone your fork locally
3. Create a new branch for your feature or bugfix
4. Make your changes
5. Test your changes thoroughly
6. Submit a pull request

## Development Setup

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022, Rider or VS Code
- SQL Server (LocalDB for development)

### Setting Up the Development Environment

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/openfundit.git
   cd openfundit
   ```

2. Copy configuration templates:
   ```bash
   # Copy and configure appsettings.Development.json files
   cp src/AppAdmin/appsettings.json src/AppAdmin/appsettings.Development.json
   cp src/AppClient/appsettings.json src/AppClient/appsettings.Development.json
   cp src/Server/appsettings.json src/Server/appsettings.Development.json
   ```

3. Update connection strings and configuration values in the Development files with your local settings.

4. Build the solution:
   ```bash
   dotnet build
   ```

5. Run database migrations:
   ```bash
   dotnet ef database update --project src/Server
   ```

## Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and concise
- Write unit tests for new functionality

## Submitting Changes

1. Ensure your code follows the project's coding standards
2. Add or update tests as needed
3. Update documentation if required
4. Make sure all tests pass
5. Create a pull request with a clear description of your changes

## Reporting Issues

- Use the GitHub issue tracker
- Provide a clear description of the problem
- Include steps to reproduce the issue
- Mention your environment (OS, .NET version, etc.)

## Questions?

If you have questions about contributing, feel free to open an issue or contact the maintainers.

Thank you for contributing to OpenFundit!
