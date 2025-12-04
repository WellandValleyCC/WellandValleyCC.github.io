using ClubCore.Models.Enums;

namespace ClubCore.Models.Extensions
{
    public static class LeagueExtensions
    {
        public static League ParseFromCsv(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return League.Undefined;

            switch (value.Trim().ToLowerInvariant())
            {
                case "prem":
                case "premier":
                    return League.Premier;
                case "1":
                    return League.League1;
                case "2":
                    return League.League2;
                case "3":
                    return League.League3;
                case "4":
                    return League.League4;
                default:
                    return League.Undefined;
            }
        }

        public static string ToCsvValue(this League league)
        {
            return league switch
            {
                League.Premier => "Prem",
                League.League1 => "1",
                League.League2 => "2",
                League.League3 => "3",
                League.League4 => "4",
                _ => string.Empty
            };
        }

        public static string GetDisplayName(this League league)
        {
            return league switch
            {
                League.Premier => "Premier",
                League.League1 => "League 1",
                League.League2 => "League 2",
                League.League3 => "League 3",
                League.League4 => "League 4",
                _ => string.Empty
            };
        }
    }
}
