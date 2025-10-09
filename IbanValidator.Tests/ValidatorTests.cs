using FluentAssertions;

namespace IbanValidator.Tests;

[TestClass]
public class ValidatorTests
{
    [TestMethod]
    public void IsValidIban_ValidGBIban_ReturnsValid()
    {
        var iban = "GB82WEST12345698765432";

        var result = Validator.IsValidIban(iban);

        result.Should().Be(IbanCheckStatus.ValidIban, $"{iban} should be a valid IBAN, yet the result is {result}");
    }

    [TestMethod]
    public void IsValidIban_ValidNLIban_ReturnsValid()
    {
        var iban = "NL20INGB0001234567";

        var result = Validator.IsValidIban(iban);

        result.Should().Be(IbanCheckStatus.ValidIban, $"{iban} should be a valid IBAN, yet the result is {result}");
    }

    [TestMethod]
    public void IsValidIban_ValidHUIban_ReturnsValid()
    {
        var iban = "HU42117730161111101800000000";

        var result = Validator.IsValidIban(iban);

        result.Should().Be(IbanCheckStatus.ValidIban, $"{iban} should be a valid IBAN, yet the result is {result}");
    }

    [TestMethod]
    public void IsValidIban_ValidBrazilianIban_ReturnsValid()
    {
        var iban = "BR0200000000010670000117668C1";

        var result = Validator.IsValidIban(iban);

        result.Should().Be(IbanCheckStatus.ValidIban, $"{iban} should be a valid IBAN, yet the result is {result}");
    }

    [TestMethod]
    public void IsValidIban_InvalidCountryCode_ReturnsInvalidCountryCode()
    {
        var iban = "KR00BANK0123456789";

        var result = Validator.IsValidIban(iban);

        result.Should().Be(IbanCheckStatus.InvalidCountryCode, $"{iban} should result in an invalid country, yet the result is {result}");
    }

    [TestMethod]
    public void IsValidIban_InvalidStructure_ReturnsInvalidStructure()
    {
        var result = Validator.IsValidIban("ES");
        result.Should().Be(IbanCheckStatus.InvalidStructure, "The IBAN prefix is missing, or the IBAN contains invalid characters");

        result = Validator.IsValidIban("");
        result.Should().Be(IbanCheckStatus.InvalidStructure, "Empty IBAN should be invalid structure");
    }

    [TestMethod]
    public void IsValidIban_InvalidStartBytes_ReturnsInvalidStartBytes()
    {
        var iban = "NLKR";

        var result = Validator.IsValidIban(iban);

        result.Should().Be(IbanCheckStatus.InvalidStartBytes, $"{result} should be InvalidStartBytes");
    }

    [TestMethod]
    public void IsValidIban_WithoutBankAccount_ReturnsInvalidBankAccount()
    {
        var iban = "NL26";

        var result = Validator.IsValidIban(iban);

        result.Should().Be(IbanCheckStatus.InvalidBankAccount, "The BBAN part of the IBAN should have at least one digit");
    }

    [TestMethod]
    public void IsValidIban_WithMissingCharacter_ReturnsInvalidInnerStructure()
    {
        var iban = "NL20INGB000123456";

        var result = Validator.IsValidIban(iban);

        result.Should().Be(IbanCheckStatus.InvalidInnerStructure, $"{iban} should be an invalid structure, yet the result is {result}");
    }

    [TestMethod]
    public void CreateIBAN_WithEmptyAccount_ReturnsEmpty()
    {
        var result = Validator.CreateIBAN("", "");

        result.Should().Be("", "Creating an IBAN whilst providing an empty account should return an empty string");
    }

    [TestMethod]
    public void CreateIBAN_WithInvalidBicLength_ReturnsEmpty()
    {
        var result = Validator.CreateIBAN("417164300", "");

        result.Should().Be("", "Creating an IBAN whilst providing a bic with a length different than 8 or 11 should return empty string");
    }

    [TestMethod]
    public void CreateIBAN_WithValidNLData_ReturnsValidIban()
    {
        var result = Validator.CreateIBAN("417164300", "ABNANL2A");
        var expectedResult = "NL91ABNA0417164300";

        result.Should().Be(expectedResult, $"The expected result is '{expectedResult}', not '{result}'");
    }

    [TestMethod]
    public void IntValueForString_ValidInput_ReturnsCorrectConversion()
    {
        var input = "GB82WEST12345698765432";

        var result = Validator.IntValueForString(input);

        result.Should().Be("1611823214282912345698765432", $"The result should be 1611823214282912345698765432, not {result}");
    }

    [TestMethod]
    public void IntValueForString_InvalidInput_ReturnsEmpty()
    {
        var input = ")(*&(*&%&^$";

        var result = Validator.IntValueForString(input);

        result.Should().Be("", $"The result should be an empty string, not {result}");
    }
}