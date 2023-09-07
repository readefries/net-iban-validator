using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace IbanValidator
{
    public class CountryModels
    {
        private Dictionary<string, CountryModel> _models = new Dictionary<string, CountryModel>();

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

                _models = JsonSerializer.Deserialize<Dictionary<string, CountryModel>>(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to parse JSON", ex);
            }
        }

        public CountryModel GetModel(string countryCode)
        {
            if (_models.TryGetValue(countryCode, out var model))
            {
                return model;
            }

            throw new ArgumentException($"CountryModel not found for {countryCode}");
        }
    }
}