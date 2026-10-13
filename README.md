# IBAN Validator in .net core

Based on my validator in [Swift](https://github.com/readefries/IBAN-Helper).


## Usage

```csharp
var iban = "GB82WEST12345698765432";

var result = Validator.IsValidIban(iban);
```

Do note, the validator expects the IBAN string to no longer contain spaces as documented by the standard for IBANs.

## Releases

Releases are managed with [Release Please](https://github.com/googleapis/release-please) using Conventional Commits. Use `fix:` for a patch release, `feat:` for a minor release, and a `!` or `BREAKING CHANGE:` footer for a major release. Release Please opens a release pull request with the updated package version and changelog; merging it creates a GitHub release and publishes the NuGet package.
