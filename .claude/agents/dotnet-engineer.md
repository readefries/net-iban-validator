# .NET Project Guidelines

This works as a basic starter guide for Claude Code in .NET 9.0.

It's also available as a blog post at https://www.teleos.ltd.

## Development Environment

### Prerequisites
- .NET 9 SDK
- Docker and Docker Compose
- Visual Studio 2022, Visual Studio Code, or JetBrains Rider

### Development Best Practices
- Make small, incremental changes rather than large-scale changes
- Prefer changing code with less coverage over code with more coverage to avoid breaking existing functionality
- When implementing new features, write tests before or alongside the implementation (TDD approach)
- Implement functionalities one at a time, with proper testing
- Fix one issue at a time, rather than addressing multiple issues simultaneously
- When dealing with external services, use in-memory implementations during development
- Document implementation decisions and architecture trade-offs
- Never comment out functional code just to make tests pass
- Either fix underlying issues or write proper feature flags/toggles
- Don't commit temporary workarounds without clear documentation and tracking

## Code Style Guidelines

### Naming Conventions
- Namespaces: Match directory structure (e.g., `Hypeticker.Core.Markets`)
- Classes/Methods/Properties: PascalCase
- Parameters/Local variables: camelCase
- Private fields: _camelCase with underscore prefix
- Interface names: Prefix with 'I' (e.g., `ITrader`)
- Enum names: PascalCase singular nouns

### Formatting
- Braces: Allman style (on new lines)
- Indentation: 4 spaces (not tabs)
- Max line length: ~120 characters
- One statement per line
- One declaration per line

### Type Usage
- Use decimal data type over lower bit data types for financial values
- Prefer strong typing over primitive obsession
- Use IEnumerable<T> for read-only collections
- Use ICollection<T> or IList<T> for modifiable collections
- Use Dictionary<TKey, TValue> for key-value collections

### General Conventions
- Maintain consistency with existing data structures
- Keep methods focused and small (preferably under 30 lines)
- Avoid magic numbers; use named constants
- Use expression-bodied members for simple methods

### Documentation
- XML comments for all public members
- Use `<param>`, `<returns>`, and `<exception>` tags appropriately
- Document non-obvious behavior
- Include examples for complex APIs

## Architecture Patterns

### General Patterns
- State pattern with separate state objects for persistence
- Interface-based design with focused interfaces
- Factory pattern for object creation
- Repository pattern for data access
- Command pattern for order processing
- Command/Query Responsibility Segregation

### Dependency Injection
- Use constructor injection for required dependencies
- Register services in appropriate ServiceCollection extensions
- Follow ASP.NET Core DI guidelines

### Error Handling
- Use exceptions for exceptional conditions
- Return result objects for expected failure cases
- Null handling: Defensive programming with null checks and null-conditional operators
- Validate method inputs with guard clauses

### Testing
- MSTest with descriptive test method names (e.g., "OrderBook_WhenOrderPlaced_IncreasesBookSize")
- Multiple assertions per test for related conditions
- Use mock objects for external dependencies
- Integration tests for cross-component functionality
- Run specific focused tests during development: `dotnet test --filter "FullyQualifiedName~=OrderBookTest"`
- Ensure each component can be tested in isolation
- For GraphQL, test resolvers independently from query execution

## GraphQL API Guidelines

### Schema Design
- Follow HotChocolate conventions
- Use proper naming for types and fields
- Implement appropriate projections and filtering
- Optimize resolvers for performance
- Create proper Data Transfer Objects (DTOs) for GraphQL types
- Never expose internal domain model implementation details directly
- Avoid using reflection to access private/protected members

### Query Design
- Keep queries focused on specific data needs
- Use fragments for type-specific fields
- Implement pagination for large collections
- Ensure proper namespace handling to avoid type conflicts 
- Use explicit type imports/aliases when dealing with name conflicts

## Code Agent Efficiency Guidelines

### Token Usage Optimization

#### Efficient Tool Usage
- Use targeted tool calls to check only specific files needed
  ```
  # GOOD: Targeted search
  GrepTool pattern="IUserService" include="*.cs"
  
  # BAD: Broad search that wastes tokens
  GrepTool pattern="User"
  ```

- Use BatchTool for multiple operations instead of sequential calls
  ```
  # GOOD: Batched operations
  BatchTool with multiple View operations for related files
  
  # BAD: Sequential View operations for each file
  ```

- For complex searches across multiple files, use dispatch_agent:
  ```
  dispatch_agent prompt="Find all implementations of IRepository interface"
  ```

#### File Handling
- Read only necessary sections of large files:
  ```
  # GOOD: Read specific section
  View file_path="/path/to/large/file.cs" offset=100 limit=50
  
  # BAD: Read entire large file
  View file_path="/path/to/large/file.cs"
  ```

- Choose the right search tool:
  - GlobTool: When searching by filename patterns
  - GrepTool: When searching by content patterns
  - LS: When listing directory contents

#### Response Optimization
- Focus on one issue at a time before proceeding
- Make smaller, focused code edits instead of large rewrites
- Skip detailed output analysis in responses
- Use direct solutions rather than iterative trial-and-error
- Summarize results briefly instead of analyzing all errors
