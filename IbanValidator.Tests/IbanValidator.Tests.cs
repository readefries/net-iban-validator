using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IbanValidator.Tests;

[TestClass]
public class Tests
{
    [TestMethod]
    public void TestReplacingCharactersWithDigits()
    {
        var sut = "GB82WEST12345698765432";

        var result = Validator.IntValueForString(sut);
        
        result.Should().Be( "1611823214282912345698765432", $"The result should be 1611823214282912345698765432, not {result}");
    }

    [TestMethod]
    public void TestInvalidIntValueForString()
    {
        var sut = ")(*&(*&%&^$";

        var result = Validator.IntValueForString(sut);

        result.Should().Be(result, "", $"The result should be an empty var, not {result}");
    }

    [TestMethod]
    public void TestIBANWithoutBankAccountNumber()
    {
        var sut = "NL26";

        var result = Validator.IsValidIban(sut);

        result.Should().Be(IbanCheckStatus.InvalidBankAccount, "The BBAN part of the IBAN should have at least one digit");
    }

    [TestMethod]
    public void TestIBANWithInvalidStructure()
    {
        var sut = "ES";

        var result = Validator.IsValidIban(sut);

        result.Should().Be(IbanCheckStatus.InvalidStructure, "The IBAN prefix is missing, or the IBAN contains invalid characters");

        sut = "";

        result = Validator.IsValidIban(sut);

        result.Should().Be(IbanCheckStatus.InvalidStructure, "The IBAN prefix is missing, or the IBAN contains invalid characters");
    }

    [TestMethod]
    public void TestCreateNLIban()
    {
        var account = "";
        var bic = "";

        var result = Validator.CreateIBAN(account, bic);

        result.Should().Be(result, "", "Creating an IBAN whilst providing an empty account should return an empty var");

        account = "417164300";

        result = Validator.CreateIBAN(account, bic);

        result.Should().Be(result, "", "Creating an IBAN whilst providing a bic with a length different than 8 or 11");

        bic = "ABNANL2A";

        result = Validator.CreateIBAN(account, bic);

        var expectedResult = "NL91ABNA0417164300";

        result.Should().Be(expectedResult, $"The expected result is '{expectedResult}', not '{result}'");
    }

    [TestMethod]
    public void TestInvalidStartBytes()
    {
        var sut = "NLKR";

        var result = Validator.IsValidIban(sut);

        result.Should().Be(IbanCheckStatus.InvalidStartBytes, $"{result} should be InvalidStartBytes");
    }

    [TestMethod]
    public void TestValidDecimalsAndCharactersFormat()
    {
        var sut = "0124556789ABCde";
        var format = "A15";

        var result = Validator.IsStringConformFormat(sut, format);

        result.Should().Be(result, $"{sut} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void TestValidDecimalAndUppercaseCharacters()
    {
        var sut = "0123456789ABCDEFGHIJ";
        var format = "B20";

        var result = Validator.IsStringConformFormat(sut, format);

        result.Should().Be(result, $"{sut} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void TestValidCharacters()
    {
        var sut = "ABCdef";
        var format = "C06";

        var result = Validator.IsStringConformFormat(sut, format);

        result.Should().Be(result, $"{sut} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void TestValidDecimals()
    {
        var sut = "1234567890";
        var format = "F10";

        var result = Validator.IsStringConformFormat(sut, format);

        result.Should().Be(result, $"{sut} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void TestValidLowercaseCharacters()
    {
        var sut = "abcdefgh";
        var format = "L08";

        var result = Validator.IsStringConformFormat(sut, format);

        result.Should().Be(result, $"{sut} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void TestValidUppercaseCharacters()
    {
        var sut = "ABCDEFGH";
        var format = "U08";

        var result = Validator.IsStringConformFormat(sut, format);

        result.Should().Be(result, $"{sut} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void TestValidDecimalsAndLowercaseCharacters()
    {
        var sut = "0123abcd";
        var format = "W08";

        var result = Validator.IsStringConformFormat(sut, format);

        result.Should().Be(result, $"{sut} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void TestInvalidFormat()
    {
        var sut = "0123456789";
        var format = "X10";
    
        var result = Validator.IsStringConformFormat(sut, format);
    
        result.Should().Be(result);
    }

    [TestMethod]
    public void TestValidityOfGBIban()
    {
        var sut = "GB82WEST12345698765432";

        var result = Validator.IsValidIban(sut);

        result.Should().Be(IbanCheckStatus.ValidIban, $"{sut} should be a valid IBAN, yet the result is {result}");
    }

    [TestMethod]
    public void TestValidityOfNLIban()
    {
        var sut = "NL20INGB0001234567";

        var result = Validator.IsValidIban(sut);

        result.Should().Be(IbanCheckStatus.ValidIban, $"{sut} should be a valid IBAN, yet the result is {result}");
    }

    [TestMethod]
    public void TestValidityOfHUIban()
    {
        var sut = "HU42117730161111101800000000";

        var result = Validator.IsValidIban(sut);

        result.Should().Be(IbanCheckStatus.ValidIban, $"{sut} should be a valid IBAN, yet the result is {result}");
    }

    [TestMethod]
    public void TestInvalidCountryCode()
    {
        var sut = "KR00BANK0123456789";

        var result = Validator.IsValidIban(sut);

        result.Should().Be(IbanCheckStatus.InvalidCountryCode, $"{sut} should result in an invalid country, yet the result is {result}");
    }

    [TestMethod]
    public void TestISO7064_Mod_97_10()
    {
        var sut = "182316110001234567232100";

        var result = ISO7064.MOD97_10(sut);
        result.Should().Be(78, "182316110001234567232100 mod 97 should be 78, not {result}");
    }

    [TestMethod]
    public void TestISO7064_Mod_97_10_WithInvalidCharacters()
    {
        var sut = "INVALID CHARACTERS";

        var result = ISO7064.MOD97_10(sut);
        result.Should().Be(-1, $"{sut} should return int.MinValue, not {result}");
    }

    [TestMethod]
    public void TestThatIbanWithOneCharacterMissingWillNotCrash()
    {
        var sut = "NL20INGB000123456";

        var result = Validator.IsValidIban(sut);

        result.Should().Be(IbanCheckStatus.InvalidInnerStructure, $"{sut} should be an invalid structure, yet the result is {result}");
    }
}