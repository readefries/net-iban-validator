# IBAN Validator in .net core

Based on my validator in [Swift](https://github.com/readefries/IBAN-Helper).


## Usage

```csharp
var iban = "GB82WEST12345698765432";

var result = Validator.IsValidIban(iban);
```

Do note, the validator expects the IBAN string to no longer contain spaces as documented by the standard for IBANs.

## Releases

Releases are managed from Conventional Commits without updating version files or opening release pull requests. Use `fix:` for a patch release, `feat:` for a minor release, and a `!` or `BREAKING CHANGE:` footer for a major release. Each release creates a GitHub tag and release with generated notes, then publishes the NuGet package using that release's version.

To initialize release automation, create the `v1.0.9` GitHub release on the last commit containing package version `1.0.9`. Later releases are created automatically when qualifying commits reach `main`.
