using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Places;

/// <summary>
/// Request parameters for Places API (discover, browse, lookup).
/// </summary>
public class PlacesRequest
{
    /// <summary>
    /// Free-text search query (used by discover).
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Center position for proximity search.
    /// </summary>
    public LatLngLiteral? At { get; set; }

    /// <summary>
    /// Bounding box to limit results (format: "south,west,north,east").
    /// </summary>
    public string? BoundingBox { get; set; }

    /// <summary>
    /// Category filter IDs (e.g. "100-1000-0000" for restaurants).
    /// </summary>
    public List<string>? Categories { get; set; }

    /// <summary>
    /// Maximum number of results.
    /// </summary>
    public int Limit { get; set; } = 20;

    /// <summary>
    /// Language for results (e.g. "en", "de").
    /// </summary>
    public string? Lang { get; set; }

    /// <summary>
    /// HERE Place ID for lookup requests.
    /// </summary>
    public string? Id { get; set; }
}
