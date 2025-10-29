namespace ClubProcessor.Configuration
{
    public static class CompetitionOverrides
    {
        /// <summary>
        /// Represents a collection of claim identifiers and their associated names that should be treated as the first
        /// claim.
        /// </summary>
        /// <remarks>
        /// This dictionary maps integer ClubNumber to the names of individuals. It is
        /// used to determine which club members should be treated as first claim in processing. 
        /// Additional entries can be added as needed as per the TimeTrial committee's decisions.
        /// </remarks>
        public static readonly Dictionary<int, string> TreatAsFirstClaim = new()
        {
            { 1144, "Jamie Kershaw" },
            { 1188, "Ruby Isaac" },
            // Add others here as needed
        };
    }

}
