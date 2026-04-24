# SignalR Event Handler Core - Unit Tests

## Overview
This project contains comprehensive unit tests for the SignalR Event Handler Core library using NUnit framework with Moq for mocking dependencies.

## Test Framework

- **Testing Framework**: NUnit 4.3.2
- **Mocking Framework**: Moq 4.20.72
- **Assertion Library**: FluentAssertions 7.0.0
- **Code Coverage**: Coverlet 6.0.4
- **Test Adapter**: NUnit3TestAdapter 4.6.0

## Target Coverage

**Target Code Coverage**: 70% minimum

## Project Structure

```
SignalR.Event.Handler.Core.Tests/
├── Clients/
│   └── EventDetailsApi/
│       ├── EventDetailsApiClientTests.cs
│       └── Responses/
│           └── EventDetailsResponseTests.cs
├── Configuration/
│   ├── ApiSettingsTests.cs
│   └── HubConnectionSettingsTests.cs
└── Utilities/
    ├── ConnectionStatusTests.cs
    ├── HttpClientUtilityTests.cs
    ├── RetryPolicyTests.cs
    └── Extensions/
        └── StringExtensionMethodsTests.cs
```

## Test Coverage Summary

### Utilities (High Coverage Expected)

#### RetryPolicyTests.cs
- ✅ Success on first attempt
- ✅ Success after retries
- ✅ All attempts fail (AggregateException)
- ✅ Exponential backoff calculation
- ✅ Max delay respected
- ✅ Zero retries behavior
- ✅ Async execution paths

**Test Count**: 12 tests
**Coverage Target**: 95%+

#### StringExtensionMethodsTests.cs
- ✅ IsNullOrEmptyTrimmed with various inputs
- ✅ CompareIgnoreCase with various scenarios
- ✅ Edge cases (null, empty, whitespace)

**Test Count**: 15 tests
**Coverage Target**: 100%

#### ConnectionStatusTests.cs
- ✅ Enum values and ordinals
- ✅ ToDisplayString for all states
- ✅ Invalid enum value handling

**Test Count**: 7 tests
**Coverage Target**: 100%

#### HttpClientUtilityTests.cs
- ✅ ExecuteHttpGetSync - success, errors, validation
- ✅ ExecuteHttpGetAsync - success, errors, validation
- ✅ ExecuteHttpPostSync - success, errors, validation
- ✅ ExecuteHttpPostAsync - success, errors, validation
- ✅ HTTP error responses
- ✅ JSON deserialization errors

**Test Count**: 14 tests
**Coverage Target**: 90%+

### Clients (Medium-High Coverage)

#### EventDetailsApiClientTests.cs
- ✅ Valid API calls
- ✅ URL construction
- ✅ Parameter validation
- ✅ Null response handling
- ✅ Exception propagation
- ✅ Retry policy integration
- ✅ Special characters in parameters

**Test Count**: 11 tests
**Coverage Target**: 85%+

#### EventDetailsResponseTests.cs
- ✅ Property initialization
- ✅ JSON serialization/deserialization
- ✅ ISerializable implementation
- ✅ JsonProperty attribute behavior
- ✅ Edge cases (null, empty, extra fields)

**Test Count**: 11 tests
**Coverage Target**: 100%

### Configuration (High Coverage)

#### ApiSettingsTests.cs
- ✅ Default values
- ✅ Property setters
- ✅ All properties assignment
- ✅ Edge cases (zero, negative values)

**Test Count**: 9 tests
**Coverage Target**: 100%

#### HubConnectionSettingsTests.cs
- ✅ Default values
- ✅ Property setters
- ✅ Various URL formats
- ✅ Null/empty handling

**Test Count**: 8 tests
**Coverage Target**: 100%

## Running Tests

### Visual Studio
1. Open Test Explorer (Test → Test Explorer)
2. Click "Run All Tests"
3. View results in Test Explorer

### Command Line
```powershell
# Run all tests
dotnet test

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run with verbose output
dotnet test --verbosity detailed

# Run specific test class
dotnet test --filter "FullyQualifiedName~RetryPolicyTests"
```

### Visual Studio Code
```bash
# Install .NET Test Explorer extension
# Use Test Explorer sidebar to run tests
```

## Code Coverage

### Generating Coverage Report

```powershell
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory:./TestResults

# Install ReportGenerator (one-time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:Html

# Open report
Start-Process ./CoverageReport/index.html
```

### Coverage Thresholds

| Component | Target | Critical |
|-----------|--------|----------|
| Utilities | 90% | 80% |
| Clients | 85% | 75% |
| Configuration | 100% | 90% |
| **Overall** | **70%** | **60%** |

## Test Patterns

### Arrange-Act-Assert (AAA)
All tests follow the AAA pattern:

```csharp
[Test]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var dependency = new Mock<IDependency>();
    var sut = new SystemUnderTest(dependency.Object);

    // Act
    var result = sut.Method();

    // Assert
    result.Should().Be(expectedValue);
}
```

### Naming Convention
```
{MethodName}_{Scenario}_{ExpectedBehavior}
```

Examples:
- `Execute_SuccessOnFirstAttempt_ReturnsResult`
- `GetEventDetailsByUserName_NullUserName_ThrowsArgumentException`
- `ToDisplayString_Disconnected_ReturnsDisconnected`

### Mocking Strategy

#### Use Moq for Interfaces
```csharp
var mockHttpClient = new Mock<IHttpClientUtility>();
mockHttpClient
    .Setup(m => m.ExecuteHttpGetSync<EventDetailsResponse>(It.IsAny<string>()))
    .Returns(new EventDetailsResponse());
```

#### Use HttpMessageHandler Mocking for HttpClient
```csharp
var mockHandler = new Mock<HttpMessageHandler>();
mockHandler.Protected()
    .Setup<HttpResponseMessage>(
        "SendAsync",
        ItExpr.IsAny<HttpRequestMessage>(),
        ItExpr.IsAny<CancellationToken>()
    )
    .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
```

## Assertions

### FluentAssertions Syntax
```csharp
// Value assertions
result.Should().Be(expectedValue);
result.Should().NotBeNull();
result.Should().BeGreaterThan(0);

// String assertions
str.Should().StartWith("prefix");
str.Should().Contain("substring");
str.Should().BeNullOrWhiteSpace();

// Collection assertions
collection.Should().HaveCount(5);
collection.Should().Contain(item);
collection.Should().BeEmpty();

// Exception assertions
Action act = () => method();
act.Should().Throw<ArgumentException>()
    .WithParameterName("paramName")
    .WithMessage("*expected message*");

// Async exception assertions
Func<Task> act = async () => await method();
await act.Should().ThrowAsync<InvalidOperationException>();
```

## Continuous Integration

### GitHub Actions (Recommended)
```yaml
name: Run Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '10.0.x'
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      - name: Code Coverage Report
        uses: codecov/codecov-action@v2
```

## Best Practices

### ✅ DO
- Write tests for all public APIs
- Use descriptive test names
- Follow AAA pattern
- Mock external dependencies
- Test edge cases and error paths
- Keep tests isolated and independent
- Use FluentAssertions for readability

### ❌ DON'T
- Test private methods directly
- Create dependencies between tests
- Use Thread.Sleep for timing (use Task.Delay in async tests)
- Test implementation details
- Ignore failing tests
- Skip test documentation

## Troubleshooting

### Tests Not Discovered
1. Verify NUnit3TestAdapter is installed
2. Clean and rebuild solution
3. Restart Visual Studio/VS Code
4. Check Test Explorer settings

### Mocking Issues
1. Ensure interface is properly set up
2. Verify It.IsAny<T>() matches parameter type
3. Check Setup() returns appropriate value
4. Use .Verifiable() and .Verify() for assertions

### Coverage Not Generated
1. Ensure coverlet.collector is installed
2. Check --collect:"XPlat Code Coverage" flag
3. Verify TestResults directory exists
4. Review test output for errors

## Future Enhancements

- [ ] Add integration tests for SignalR connections
- [ ] Add UI automation tests (WinUI Test Framework)
- [ ] Add performance benchmarking tests
- [ ] Add mutation testing (Stryker.NET)
- [ ] Add architecture tests (NetArchTest)
- [ ] Add contract tests for API clients

## Contributing

When adding new tests:
1. Follow existing naming conventions
2. Add test to appropriate folder
3. Update this README with test count
4. Ensure coverage stays above 70%
5. Run all tests before committing

## References

- [NUnit Documentation](https://docs.nunit.org/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
- [.NET Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
