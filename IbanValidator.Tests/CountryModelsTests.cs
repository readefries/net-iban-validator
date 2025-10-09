using FluentAssertions;

namespace IbanValidator.Tests;

[TestClass]
public class CountryModelsTests
{
    [TestMethod]
    public void CountryModels_Instance_ReturnsSameInstance()
    {
        var instance1 = CountryModels.Instance;
        var instance2 = CountryModels.Instance;

        instance1.Should().BeSameAs(instance2, "CountryModels should be a singleton");
    }

    [TestMethod]
    public void GetModel_ValidCountryCode_ReturnsCountryModel()
    {
        var countryModels = CountryModels.Instance;

        var model = countryModels.GetModel("GB");

        model.CountryCode.Should().Be("GB");
        model.Length.Should().BeGreaterThan(0);
        model.InnerStructure.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public void GetModel_InvalidCountryCode_ThrowsCountryNotFoundException()
    {
        var countryModels = CountryModels.Instance;

        var action = () => countryModels.GetModel("XX");

        action.Should().Throw<CountryNotFoundException>()
            .WithMessage("*XX*");
    }

    [TestMethod]
    public async Task LoadModelsAsync_CanBeCalledMultipleTimes()
    {
        var countryModels = CountryModels.Instance;

        await countryModels.LoadModelsAsync();
        await countryModels.LoadModelsAsync(); // Should not throw or reload

        var model = countryModels.GetModel("DE");
        model.CountryCode.Should().Be("DE");
    }

    [TestMethod]
    public void LoadModels_CanBeCalledMultipleTimes()
    {
        var countryModels = CountryModels.Instance;

        countryModels.LoadModels();
        countryModels.LoadModels(); // Should not throw or reload

        var model = countryModels.GetModel("FR");
        model.CountryCode.Should().Be("FR");
    }

    [TestMethod]
    public void GetModel_CommonCountries_AllExist()
    {
        var countryModels = CountryModels.Instance;
        string[] commonCountries = { "GB", "DE", "FR", "NL", "ES", "IT", "US", "BR" };

        foreach (var country in commonCountries)
        {
            if (country == "US") continue; // US doesn't use IBAN

            var action = () => countryModels.GetModel(country);

            if (country == "BR")
            {
                action.Should().NotThrow($"{country} should be supported");
            }
            else
            {
                action.Should().NotThrow($"{country} should be supported");
            }
        }
    }
}