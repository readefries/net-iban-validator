using FluentAssertions;

namespace IbanValidator.Tests;

[TestClass]
public class FormatValidationTests
{
    [TestMethod]
    public void IsStringConformFormat_ValidDecimalsAndCharacters_ReturnsTrue()
    {
        var input = "0124556789ABCde";
        var format = "A15";

        var result = Validator.IsStringConformFormat(input, format);

        result.Should().BeTrue($"{input} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void IsStringConformFormat_ValidDecimalAndUppercaseCharacters_ReturnsTrue()
    {
        var input = "0123456789ABCDEFGHIJ";
        var format = "B20";

        var result = Validator.IsStringConformFormat(input, format);

        result.Should().BeTrue($"{input} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void IsStringConformFormat_ValidCharacters_ReturnsTrue()
    {
        var input = "ABCdef";
        var format = "C06";

        var result = Validator.IsStringConformFormat(input, format);

        result.Should().BeTrue($"{input} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void IsStringConformFormat_ValidDecimals_ReturnsTrue()
    {
        var input = "1234567890";
        var format = "F10";

        var result = Validator.IsStringConformFormat(input, format);

        result.Should().BeTrue($"{input} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void IsStringConformFormat_ValidLowercaseCharacters_ReturnsTrue()
    {
        var input = "abcdefgh";
        var format = "L08";

        var result = Validator.IsStringConformFormat(input, format);

        result.Should().BeTrue($"{input} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void IsStringConformFormat_ValidUppercaseCharacters_ReturnsTrue()
    {
        var input = "ABCDEFGH";
        var format = "U08";

        var result = Validator.IsStringConformFormat(input, format);

        result.Should().BeTrue($"{input} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void IsStringConformFormat_ValidDecimalsAndLowercaseCharacters_ReturnsTrue()
    {
        var input = "0123abcd";
        var format = "W08";

        var result = Validator.IsStringConformFormat(input, format);

        result.Should().BeTrue($"{input} should be validated with format {format} successfully");
    }

    [TestMethod]
    public void IsStringConformFormat_InvalidFormat_ReturnsFalse()
    {
        var input = "0123456789";
        var format = "X10";

        var result = Validator.IsStringConformFormat(input, format);

        result.Should().BeFalse("Invalid format X10 should return false");
    }

    [TestMethod]
    public void IsStringConformFormat_WrongLength_ReturnsFalse()
    {
        var input = "123";
        var format = "F05";

        var result = Validator.IsStringConformFormat(input, format);

        result.Should().BeFalse("Input length doesn't match format specification");
    }

    [TestMethod]
    public void IsStringConformFormat_EmptyInput_ReturnsFalse()
    {
        var result = Validator.IsStringConformFormat("", "F05");

        result.Should().BeFalse("Empty input should return false");
    }

    [TestMethod]
    public void IsStringConformFormat_EmptyFormat_ReturnsFalse()
    {
        var result = Validator.IsStringConformFormat("12345", "");

        result.Should().BeFalse("Empty format should return false");
    }
}