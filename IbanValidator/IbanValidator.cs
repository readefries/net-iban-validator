using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IbanValidator
{

    public enum IbanCheckStatus
    {
        ValidIban,
        InvalidCountryCode,
        InvalidBankAccount,
        InvalidChecksum,
        InvalidInnerStructure,
        InvalidStartBytes,
        InvalidStructure,
        InvalidLength,
    }

    public abstract class Validator
    {
        private static readonly Regex IbanStructureRegex = new Regex("^([A-Za-z0-9]{4,})*$", RegexOptions.Compiled);
        private static readonly Regex DecimalsAndCharactersRegex = new Regex("^([A-Za-z0-9])*$", RegexOptions.Compiled);
        private static readonly Regex DecimalsAndUppercaseCharactersRegex = new Regex("^([A-Z0-9])*$", RegexOptions.Compiled);
        private static readonly Regex DecimalsAndLowercaseCharactersRegex = new Regex("^([a-z0-9])*$", RegexOptions.Compiled);
        private static readonly Regex CharactersRegex = new Regex("^([A-Za-z])*$", RegexOptions.Compiled);
        private static readonly Regex DecimalsRegex = new Regex("^([0-9])*$", RegexOptions.Compiled);
        private static readonly Regex LowercaseCharactersRegex = new Regex("^([a-z])*$", RegexOptions.Compiled);
        private static readonly Regex UppercaseCharactersRegex = new Regex("^([A-Z])*$", RegexOptions.Compiled);
        private static readonly Regex StartBytesRegex = new Regex("^([A-Z]{2}[0-9]{2})$", RegexOptions.Compiled);

        public static string CreateIBAN(string account, string? bic = null, string? countryCode = null)
        {
            var countryModels = new CountryModels();
            countryModels.LoadModels();

            if (string.IsNullOrEmpty(account))
            {
                return "";
            }

            if (!string.IsNullOrEmpty(bic))
            {
                if (bic.Length != 8 && bic.Length != 11)
                {
                    return "";
                }

                var bankCode = bic[..4];
                var countryCodeFromBic = bic.Substring(4, 2);
                countryCode ??= countryCodeFromBic;

                CountryModel countryModel;
                try
                {
                    countryModel = countryModels.GetModel(countryCode);
                }
                catch
                {
                    throw new ArgumentException($"CountryModel not found for {countryCode}");
                }

                var accountNumber = PreFixZerosToAccount(account, countryModel.Length - 4);

                var ibanWithoutChecksum = $"{countryCode}00{bankCode}{accountNumber}";

                var checksum = CheckSumForIban(ibanWithoutChecksum);

                return $"{countryCode}{checksum}{bankCode}{accountNumber}";
            }

            if (!string.IsNullOrEmpty(countryCode))
            {
                var ibanWithoutChecksum = $"{countryCode}00{account}";
                var checksum = CheckSumForIban(ibanWithoutChecksum);

                return $"{countryCode}{checksum}{account}";
            }

            return "";
        }

        public static IbanCheckStatus IsValidIban(string iban)
        {
            var countryModels = new CountryModels();
            countryModels.LoadModels();

            if (!Regex.IsMatch(iban, IbanStructure))
            {
                return IbanCheckStatus.InvalidStructure;
            }

            if (iban.Length < 4)
            {
                return IbanCheckStatus.InvalidStructure;
            }

            var countryCode = iban[..2];

            CountryModel countryModel;
            try
            {
                countryModel = countryModels.GetModel(countryCode);
            }
            catch
            {
                return IbanCheckStatus.InvalidCountryCode;
            }

            var startBytes = iban[..4];

            if (!StartBytesRegex.IsMatch(startBytes))
            {
                return IbanCheckStatus.InvalidStartBytes;
            }

            var innerStructure = countryModel.InnerStructure;
            var bbanOffset = 0;
            var bban = iban[4..];

            if (string.IsNullOrEmpty(bban))
            {
                return IbanCheckStatus.InvalidBankAccount;
            }

            for (var i = 0; i < innerStructure.Length / 3; i++)
            {
                var startIndex = i * 3;
                var format = innerStructure.Substring(startIndex, 3);

                if (!int.TryParse(innerStructure.Substring(startIndex + 1, 2), out var formatLength))
                {
                    return IbanCheckStatus.InvalidInnerStructure;
                }

                var partEndIndex = Math.Min(bbanOffset + formatLength, bban.Length);
                var innerPart = bban.Substring(bbanOffset, partEndIndex - bbanOffset);

                if (!IsStringConformFormat(innerPart, format))
                {
                    return IbanCheckStatus.InvalidInnerStructure;
                }

                bbanOffset += formatLength;
            }

            if (countryModel.Length != iban.Length)
            {
                return IbanCheckStatus.InvalidLength;
            }

            var checksumString = iban.Substring(2, 2);

            if (!int.TryParse(checksumString, out var expectedChecksum))
            {
                return IbanCheckStatus.InvalidChecksum;
            }

            if (expectedChecksum == CheckSumForIban(iban))
            {
                return IbanCheckStatus.ValidIban;
            }

            return IbanCheckStatus.InvalidChecksum;
        }

        public static bool IsStringConformFormat(string input, string format)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(format))
            {
                return false;
            }

            var formatLength = int.Parse(format.Substring(1, 2));

            if (formatLength != input.Length)
            {
                return false;
            }

            return format[0] switch
            {
                'A' => DecimalsAndCharactersRegex.IsMatch(input),
                'B' => DecimalsAndUppercaseCharactersRegex.IsMatch(input),
                'C' => CharactersRegex.IsMatch(input),
                'F' => DecimalsRegex.IsMatch(input),
                'L' => LowercaseCharactersRegex.IsMatch(input),
                'U' => UppercaseCharactersRegex.IsMatch(input),
                'W' => DecimalsAndLowercaseCharactersRegex.IsMatch(input),
                _ => false,
            };
        }

        private static int CheckSumForIban(string iban)
        {
            var bankCode = iban[4..];
            var countryCode = iban[..2];
            var checkedIban = $"{bankCode}{countryCode}00";

            checkedIban = IntValueForString(checkedIban.ToUpper());

            checkedIban = checkedIban.TrimStart('0');
            var remainder = ISO7064.MOD97_10(checkedIban);

            return 98 - remainder;
        }

        public static string IntValueForString(string input)
        {
            if (!DecimalsAndUppercaseCharactersRegex.IsMatch(input))
            {
                return "";
            }

            var returnValue = new List<int>();

            foreach (var charValue in input)
            {
                var decimalCharacter = 0;

                if (char.IsDigit(charValue))
                {
                    decimalCharacter = charValue - '0';
                }
                else if (char.IsUpper(charValue))
                {
                    decimalCharacter = charValue - 'A' + 10;
                }

                returnValue.Add(decimalCharacter);
            }

            return string.Join("", returnValue);
        }

        private static string PreFixZerosToAccount(string bankNumber, int length)
        {
            var bankNumberWithPrefixes = bankNumber;

            for (var i = bankNumber.Length; i < length; i++)
            {
                bankNumberWithPrefixes = $"0{bankNumber}";
            }

            return bankNumberWithPrefixes;
        }
    }
}
