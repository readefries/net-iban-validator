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
        private static readonly string IbanStructure = "^([A-Za-z0-9]{4,})*$";
        private static readonly string DecimalsAndCharacters = "^([A-Za-z0-9])*$";
        private static readonly string DecimalsAndUppercaseCharacters = "^([A-Z0-9])*$";
        private static readonly string DecimalsAndLowercaseCharacters = "^([a-z0-9])*$";
        private static readonly string Characters = "^([A-Za-z])*$";
        private static readonly string Decimals = "^([0-9])*$";
        private static readonly string LowercaseCharacters = "^([a-z])*$";
        private static readonly string UppercaseCharacters = "^([A-Z])*$";
        private static readonly string StartBytesRegex = "^([A-Z]{2}[0-9]{2})$";

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

            if (!Regex.IsMatch(startBytes, StartBytesRegex))
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
                'A' => Regex.IsMatch(input, DecimalsAndCharacters),
                'B' => Regex.IsMatch(input, DecimalsAndUppercaseCharacters),
                'C' => Regex.IsMatch(input, Characters),
                'F' => Regex.IsMatch(input, Decimals),
                'L' => Regex.IsMatch(input, LowercaseCharacters),
                'U' => Regex.IsMatch(input, UppercaseCharacters),
                'W' => Regex.IsMatch(input, DecimalsAndLowercaseCharacters),
                _ => false,
            };
        }

        public static int CheckSumForIban(string iban)
        {
            var bankCode = iban[4..];
            var countryCode = iban[..2];
            var checkedIban = $"{bankCode}{countryCode}00";

            checkedIban = IntValueForString(checkedIban.ToUpper());
            var remainder = ISO7064.MOD97_10(checkedIban);

            return 98 - remainder;
        }

        public static string IntValueForString(string input)
        {
            if (!Regex.IsMatch(input, DecimalsAndUppercaseCharacters))
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

        public static string PreFixZerosToAccount(string bankNumber, int length)
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