using FluentAssertions;

namespace IbanValidator.Tests;

[TestClass]
public class AsyncValidatorTests
{
    [TestMethod]
    public async Task IsValidIbanAsync_ValidGBIban_ReturnsValid()
    {
        var iban = "GB82WEST12345698765432";

        var result = await Validator.IsValidIbanAsync(iban);

        result.Should().Be(IbanCheckStatus.ValidIban, $"{iban} should be a valid IBAN, yet the result is {result}");
    }

    [TestMethod]
    public async Task IsValidIbanAsync_InvalidCountryCode_ReturnsInvalidCountryCode()
    {
        var iban = "KR00BANK0123456789";

        var result = await Validator.IsValidIbanAsync(iban);

        result.Should().Be(IbanCheckStatus.InvalidCountryCode, $"{iban} should result in an invalid country, yet the result is {result}");
    }

    [TestMethod]
    public async Task CreateIBANAsync_WithValidNLData_ReturnsValidIban()
    {
        var result = await Validator.CreateIBANAsync("417164300", "ABNANL2A");
        var expectedResult = "NL91ABNA0417164300";

        result.Should().Be(expectedResult, $"The expected result is '{expectedResult}', not '{result}'");
    }

    [TestMethod]
    public async Task CreateIBANAsync_WithEmptyAccount_ReturnsEmpty()
    {
        var result = await Validator.CreateIBANAsync("", "");

        result.Should().Be("", "Creating an IBAN whilst providing an empty account should return an empty string");
    }

    [TestMethod]
    public async Task IsValidIbanAsync_MultipleCallsUseSingleton()
    {
        var iban1 = "GB82WEST12345698765432";
        var iban2 = "NL20INGB0001234567";

        var task1 = Validator.IsValidIbanAsync(iban1);
        var task2 = Validator.IsValidIbanAsync(iban2);

        var results = await Task.WhenAll(task1, task2);

        results[0].Should().Be(IbanCheckStatus.ValidIban);
        results[1].Should().Be(IbanCheckStatus.ValidIban);
    }
}