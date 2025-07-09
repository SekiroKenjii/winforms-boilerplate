# Unit Testing Structure for WinForms Boilerplate

This document describes the unit testing structure created for the WinForms Boilerplate project.

## Test Project Structure

The test structure follows the source code organization with a dedicated test project for each main project:

```
test/
├── WinformsBoilerplate.Core.Tests/           # Tests for Core library
├── WinformsBoilerplate.Infrastructure.Tests/ # Tests for Infrastructure layer
└── WinformsBoilerplate.App.Tests/            # Tests for Application layer
```

## Test Framework and Tools

The test projects use the following testing framework and tools:

- **xUnit**: Primary testing framework
- **FluentAssertions**: For readable assertions
- **Moq**: For mocking dependencies
- **AutoFixture**: For test data generation
- **Coverlet**: For code coverage collection

## Project Configuration

Each test project is configured with:

- Target Framework: `.NET 9.0`
- Test SDK packages for running tests
- Code coverage collection
- Project references to the corresponding source projects

## Test Organization

### Core Tests (`WinformsBoilerplate.Core.Tests`)
Tests for the core business logic and utilities:

- `Abstractions/` - Tests for abstract base classes
- `Constants/` - Tests for constant values
- `Enums/` - Tests for enumerations
- `Extensions/` - Tests for extension methods
- `Helpers/` - Tests for helper utilities

### Infrastructure Tests (`WinformsBoilerplate.Infrastructure.Tests`)
Tests for infrastructure services and data access:

- `Services/` - Tests for business services
- `Stores/` - Tests for data stores
- `Configurations/` - Tests for configuration classes

### App Tests (`WinformsBoilerplate.App.Tests`)
Tests for application-specific functionality:

- `Extensions/` - Tests for app-specific extensions
- `Handlers/` - Tests for event handlers
- `Helpers/` - Tests for app helper utilities

## Running Tests

### From Visual Studio
1. Open the solution in Visual Studio
2. Use Test Explorer (Test > Test Explorer)
3. Click "Run All Tests" or run specific test projects

### From Command Line
```powershell
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test test/WinformsBoilerplate.Core.Tests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests in Release mode
dotnet test -c Release
```

### PowerShell Scripts
Use the provided PowerShell scripts for common operations:

```powershell
# Run all tests
.\scripts\run-tests.ps1

# Run tests with coverage report
.\scripts\run-tests-with-coverage.ps1
```

## Test Guidelines

### Naming Conventions
- Test classes: `{ClassUnderTest}Tests`
- Test methods: `{MethodUnderTest}_{Scenario}_{ExpectedResult}`

### Test Structure
Follow the Arrange-Act-Assert (AAA) pattern:

```csharp
[Fact]
public void Method_WhenCondition_ShouldExpectedResult()
{
    // Arrange
    var input = "test";

    // Act
    var result = ClassUnderTest.Method(input);

    // Assert
    result.Should().Be("expected");
}
```

### Best Practices
1. Each test should test one specific behavior
2. Tests should be independent and isolated
3. Use descriptive test names
4. Use FluentAssertions for readable assertions
5. Mock external dependencies
6. Use AutoFixture for test data generation

## Coverage Reports

Code coverage reports are generated using Coverlet and can be viewed in various formats:
- Console output during test runs
- XML reports for CI/CD integration
- HTML reports for detailed analysis

## Continuous Integration

The test structure is designed to work with CI/CD pipelines:
- All tests can be run with a single `dotnet test` command
- Coverage reports are generated in standard formats
- Test results are compatible with most CI systems

## Sample Tests

Example test files are provided to demonstrate:
- Unit testing patterns
- Mocking strategies
- Test data generation
- Base test classes for common functionality

These can be used as templates for writing additional tests as the application grows.
