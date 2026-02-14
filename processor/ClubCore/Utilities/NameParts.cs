namespace ClubCore.Utilities
{
    public static class NameParts
    {
        public static (string Surname, string GivenNames) Split(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return ("", "");

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return (parts[0], "");

            var surname = parts[^1];
            var givenNames = string.Join(" ", parts.Take(parts.Length - 1));

            return (surname, givenNames);
        }
    }
}
