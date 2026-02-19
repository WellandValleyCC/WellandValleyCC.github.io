namespace ClubCore.Utilities
{
    /// <summary>
    /// Represents a fragment of HTML that should be emitted without encoding.
    /// </summary>
    public sealed record RawHtml(string Value);
}
