using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Utils
{
    public static class AbbreviationUtils
    {
        public static string GenerateUniqueAbbreviation(string baseAbbreviation, IEnumerable<string> existingAbbreviations)
        {
            var newAbbreviation = baseAbbreviation.ToLower();
            int counter = 1;

            // Check for any conflicts with existing abbreviations
            while (existingAbbreviations.Any(a => a.Equals(newAbbreviation, StringComparison.OrdinalIgnoreCase)))
            {
                var parsedAbbreviation = ParseAbbreviation(baseAbbreviation);
                newAbbreviation = $"{parsedAbbreviation.textPart}{parsedAbbreviation.numberPart + counter}";
                counter++;
            }

            return newAbbreviation;
        }

        private static (string textPart, int numberPart) ParseAbbreviation(string baseAbbreviation)
        {
            var match = System.Text.RegularExpressions.Regex.Match(baseAbbreviation, @"^(.*?)(\d+)$");
            if (match.Success)
            {
                return (match.Groups[1].Value, int.Parse(match.Groups[2].Value));
            }
            return (baseAbbreviation, 0); // No number found
        }
    }
}
