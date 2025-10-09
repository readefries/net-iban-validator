using System;

namespace IbanValidator
{
    public class IbanValidationException : Exception
    {
        public IbanValidationException(string message) : base(message) { }
        public IbanValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class CountryNotFoundException : IbanValidationException
    {
        public string CountryCode { get; }

        public CountryNotFoundException(string countryCode)
            : base($"Country model not found for country code: {countryCode}")
        {
            CountryCode = countryCode;
        }
    }

    public class CountryDataLoadException : IbanValidationException
    {
        public CountryDataLoadException(string message, Exception innerException)
            : base($"Failed to load country data: {message}", innerException) { }
    }
}