using FluentAssertions;

namespace IbanValidator.Tests;

[TestClass]
public class ISO7064Tests
{
    [TestMethod]
    public void MOD97_10_ValidInput_ReturnsCorrectResult()
    {
        var input = "182316110001234567232100";

        var result = ISO7064.MOD97_10(input);

        result.Should().Be(78, "182316110001234567232100 mod 97 should be 78, not {result}");
    }

    [TestMethod]
    public void MOD97_10_InvalidCharacters_ReturnsMinusOne()
    {
        var input = "INVALID CHARACTERS";

        var result = ISO7064.MOD97_10(input);

        result.Should().Be(-1, $"{input} should return -1, not {result}");
    }

    [TestMethod]
    public void MOD97_10_EmptyString_ReturnsMinusOne()
    {
        var result = ISO7064.MOD97_10("");

        result.Should().Be(-1, "Empty string should return -1");
    }

    [TestMethod]
    public void MOD97_10_SingleDigit_ReturnsCorrectResult()
    {
        var result = ISO7064.MOD97_10("5");

        result.Should().Be(5, "Single digit should return itself");
    }

    [TestMethod]
    public void MOD97_10_LargeNumber_ReturnsCorrectResult()
    {
        var input = "123456789012345678901234567890";

        var result = ISO7064.MOD97_10(input);

        result.Should().BeGreaterThanOrEqualTo(0).And.BeLessThan(97, "MOD97 result should be between 0-96");
    }

    [TestMethod]
    public void MOD97_10_NumberWithLeadingZeros_ReturnsCorrectResult()
    {
        var input = "0000123";

        var result = ISO7064.MOD97_10(input);

        result.Should().Be(26, "Number with leading zeros should be processed correctly");
    }
}