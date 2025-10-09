using System;
using System.Text.RegularExpressions;

namespace IbanValidator
{
    public abstract class ISO7064
    {
        private static readonly Regex Mod9710ValidCharsRegex = new Regex("^([0-9]+)$", RegexOptions.Compiled);

        public static int MOD97_10(string input)
        {
            if (!Mod9710ValidCharsRegex.IsMatch(input))
            {
                return -1; // Equivalent to NSNotFound in Swift
            }

            var remainingInput = input;

            while (true)
            {
                var chunkSize = Math.Min(9, remainingInput.Length);

                var chunk = remainingInput.Substring(0, chunkSize);

                if (int.TryParse(chunk, out var chunkInt) && chunkInt < 97 || remainingInput.Length < 3)
                {
                    break;
                }

                var remainder = chunkInt % 97;

                var nextChunk = remainingInput.Substring(chunkSize);

                remainingInput = $"{remainder}{nextChunk}";
            }

            return int.Parse(remainingInput);
        }
    }
}
