# Contributing to OpenFundit

Thank you for your interest in contributing to OpenFundit! This document provides guidelines for contributing to the project.

## How to Contribute

### Option 1: For New Contributors (Recommended)
1. **Fork** this repository to your GitHub account
2. **Clone** your fork locally: `git clone https://github.com/YOUR-USERNAME/noisycricket-open-fundit.git`
3. **Create a branch**: `git checkout -b feature/your-feature-name`
4. **Make your changes** and commit them
5. **Push** to your fork: `git push origin feature/your-feature-name`
6. **Create a Pull Request** from your fork to this repository

### Option 2: For Regular Contributors
If you're a regular contributor, Johan can add you as a collaborator for direct access.

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

## Questions and Contact

### For Contributors
- **GitHub Issues**: Use for bug reports and feature requests
- **GitHub Discussions**: Use for questions and general discussion (when enabled)
- **Email**: Contact Johan at the email listed in the repository

### Becoming a Regular Contributor
If you make several good contributions and want direct access to the repository, Johan can add you as a collaborator. Just mention this in a pull request or issue!

## Code of Conduct

- Be respectful and inclusive
- Focus on constructive feedback
- Help newcomers learn and grow

Thank you for contributing to OpenFundit!
