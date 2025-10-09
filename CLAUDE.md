# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Build and Test
- **Build**: `dotnet build --configuration Release`
- **Test**: `dotnet test --no-build --configuration Release`
- **Restore dependencies**: `dotnet restore`
- **Run single test**: `dotnet test --filter "MethodName=TestMethodName"`
- **Build with dependencies**: `dotnet restore && dotnet build --configuration Release`

### Project Structure
This is a .NET Standard 2.1 library with MSTest unit tests:
- **Main project**: `IbanValidator/IbanValidator.csproj` - .NET Standard 2.1 library
- **Test project**: `IbanValidator.Tests/IbanValidator.Tests.csproj` - .NET 7.0 test project
- **Solution file**: `IbanValidator.sln`

## Architecture Overview

### Core Components
- **`Validator`**: Static class containing the main IBAN validation logic and IBAN creation functionality
- **`CountryModel`**: Simple struct defining country-specific IBAN rules (length, inner structure)
- **`CountryModels`**: Loads and manages country data from `iban-countries.json`
- **`ISO7064`**: Implements MOD97-10 checksum algorithm for IBAN validation
- **`IbanCheckStatus`**: Enum defining validation result types

### Key Files
- **`IbanValidator.cs`**: Main validation logic with `IsValidIban()` and `CreateIBAN()` methods
- **`iban-countries.json`**: Configuration data for 74 countries with IBAN structure rules
- **`CountryModel.cs`**: Data structure for country-specific IBAN rules
- **`ISO7064.cs`**: Checksum calculation implementation

### Validation Flow
1. Basic structure validation (regex patterns)
2. Country code lookup from JSON data
3. Length validation against country rules
4. Inner structure validation using format codes (A, B, C, F, L, U, W)
5. MOD97-10 checksum verification

### Testing Framework
Uses MSTest with FluentAssertions for readable test assertions. Test coverage includes:
- Valid/invalid IBAN validation
- Character replacement for checksum calculation
- Error condition handling
- Country-specific validation rules

### NuGet Package
- Package ID: `cloudcosmonaut.ibanvalidator`
- Targets .NET Standard 2.1 for broad compatibility
- Auto-generates package on build
- Includes embedded `iban-countries.json` resource