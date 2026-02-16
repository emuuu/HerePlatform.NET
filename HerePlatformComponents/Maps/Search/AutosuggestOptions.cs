using HerePlatform.Core.Coordinates;

namespace HerePlatformComponents.Maps.Search;

/// <summary>
/// Configuration options for the HERE Autosuggest search.
/// </summary>
public class AutosuggestOptions
{
    /// <summary>
    /// Maximum number of results to return. Default: 5.
    /// </summary>
    public int Limit { get; set; } = 5;

    /// <summary>
    /// Language for results (BCP 47 tag, e.g. "de", "en"). Default: "de".
    /// </summary>
    public string? Lang { get; set; } = "de";

    /// <summary>
    /// Geographic filter expression (e.g. "countryCode:DEU"). Default: "countryCode:DEU".
    /// </summary>
    public string? In { get; set; } = "countryCode:DEU";

    /// <summary>
    /// Geographic bias location. Results near this position are ranked higher.
    /// </summary>
    public LatLngLiteral? At { get; set; }
}
