using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IbanValidator
{
    public class CountryModels
    {
        private static readonly Lazy<CountryModels> _instance = new Lazy<CountryModels>(() => new CountryModels());
        private Dictionary<string, CountryModel> _models = new Dictionary<string, CountryModel>();
        private bool _isLoaded = false;
        private readonly object _lockObject = new object();

        public static CountryModels Instance => _instance.Value;

        private CountryModels() { }

        public void LoadModels()
        {
            if (_isLoaded) return;

            lock (_lockObject)
            {
                if (_isLoaded) return;

                try
                {
                    var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "iban-countries.json");

                    if (!File.Exists(jsonPath))
                    {
                        throw new FileNotFoundException("Unable to read iban-countries.json");
                    }

                    var jsonString = File.ReadAllText(jsonPath, Encoding.UTF8);

                    _models = JsonSerializer.Deserialize<Dictionary<string, CountryModel>>(jsonString) ?? new Dictionary<string, CountryModel>();
                    _isLoaded = true;
                }
                catch (Exception ex) when (!(ex is FileNotFoundException))
                {
                    throw new CountryDataLoadException("Unable to parse JSON", ex);
                }
            }
        }

        public async Task LoadModelsAsync()
        {
            if (_isLoaded) return;

            lock (_lockObject)
            {
                if (_isLoaded) return;
            }

            try
            {
                var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "iban-countries.json");

                if (!File.Exists(jsonPath))
                {
                    throw new FileNotFoundException("Unable to read iban-countries.json");
                }

                var jsonString = await File.ReadAllTextAsync(jsonPath, Encoding.UTF8);

                lock (_lockObject)
                {
                    if (!_isLoaded)
                    {
                        _models = JsonSerializer.Deserialize<Dictionary<string, CountryModel>>(jsonString) ?? new Dictionary<string, CountryModel>();
                        _isLoaded = true;
                    }
                }
            }
            catch (Exception ex) when (!(ex is FileNotFoundException))
            {
                throw new CountryDataLoadException("Unable to parse JSON", ex);
            }
        }

        public CountryModel GetModel(string countryCode)
        {
            if (!_isLoaded)
            {
                LoadModels();
            }

            if (_models.TryGetValue(countryCode, out var model))
            {
                return model;
            }

            throw new CountryNotFoundException(countryCode);
        }
    }
}