using HerePlatform.Core.Coordinates;
using System.Collections.Generic;

namespace HerePlatform.Core.Geocoding;

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

    /// <summary>
    /// Structured address fields (street, house number, postal code, city, …).
    /// </summary>
    /// <remarks>
    /// <see cref="Address"/> stays the formatted label for backward compatibility;
    /// this carries the structured breakdown alongside it. Null if the API response
    /// did not include an <c>address</c> object.
    /// </remarks>
    public GeocodeAddress? AddressDetails { get; set; }
}
