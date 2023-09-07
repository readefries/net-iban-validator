# IBAN Validator in .net core

Based on my validator in [Swift](https://github.com/readefries/IBAN-Helper).


## Usage

```csharp
var iban = "GB82WEST12345698765432";

var result = Validator.IsValidIban(iban);
```

Do note, the validator expects the IBAN string to no longer contain spaces as documented by the standard for IBANs.
