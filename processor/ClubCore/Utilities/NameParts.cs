using System.Text.RegularExpressions;

namespace ClubCore.Utilities
{
    public static class NameParts
    {
        private static readonly Regex ClubSuffixPattern =
            new(@"\s*\([^)]*\)\s*$", RegexOptions.Compiled);

        public static (string Surname, string GivenNames) Split(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return ("", "");

            // Remove ALL trailing "(XYZ)" blocks, even multiple
            string cleaned = fullName;
            while (ClubSuffixPattern.IsMatch(cleaned))
                cleaned = ClubSuffixPattern.Replace(cleaned, "").TrimEnd();

            var parts = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return (parts[0], "");

            var surname = parts[^1];
            var givenNames = string.Join(" ", parts.Take(parts.Length - 1));

            return (surname, givenNames);
        }
    }
}
