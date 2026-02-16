using HerePlatform.Core.Coordinates;
using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Geocoding;

/// <summary>
/// Result of a geocoding request.
/// </summary>
public class GeocodeResult
{
    /// <summary>
    /// Geocoding result items.
    /// </summary>
    public List<GeocodeItem>? Items { get; set; }
}

/// <summary>
/// A single geocoding result item.
/// </summary>
public class GeocodeItem
{
    /// <summary>
    /// Title/display name of the result.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Geographic position.
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Formatted address string.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Result type (e.g. "houseNumber", "street", "locality").
    /// </summary>
    public string? ResultType { get; set; }
}
