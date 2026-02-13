namespace HerePlatformComponents.Maps.Services.Geocoding;

/// <summary>
/// Options for geocoding requests.
/// </summary>
public class GeocodeOptions
{
    /// <summary>
    /// Language for results (e.g. "en", "de").
    /// </summary>
    public string? Lang { get; set; }

    /// <summary>
    /// Maximum number of results.
    /// </summary>
    public int Limit { get; set; } = 5;

    /// <summary>
    /// Optional bounding box to narrow results (format: "south,west,north,east").
    /// </summary>
    public string? BoundingBox { get; set; }
}
