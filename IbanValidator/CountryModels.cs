using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace IbanValidator;

public class CountryModels
{
    private Dictionary<string, CountryModel> _models = new();

    public void LoadModels()
    {
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "iban-countries.json");

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException("Unable to read iban-countries.json");
            }

            var jsonString = File.ReadAllText(jsonPath, Encoding.UTF8);

            _models = JsonConvert.DeserializeObject<Dictionary<string, CountryModel>>(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to parse JSON", ex);
        }
    }

    public bool ModelExists(string countryCode)
    {
        return _models.ContainsKey(countryCode);
    }

    public CountryModel GetModel(string countryCode)
    {
        if (_models.TryGetValue(countryCode, out var model))
        {
            return model;
        }
        else
        {
            throw new ArgumentException($"CountryModel not found for {countryCode}");
        }
    }
}

public static class Bundle
{
    private static readonly Lazy<Assembly> executingAssembly = new Lazy<Assembly>(() => Assembly.GetExecutingAssembly());

    public static Assembly Assets => executingAssembly.Value;
}