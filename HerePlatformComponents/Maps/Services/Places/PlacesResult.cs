using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Places;

/// <summary>
/// Result of a Places API request.
/// </summary>
public class PlacesResult
{
    /// <summary>
    /// Place result items.
    /// </summary>
    public List<PlaceItem>? Items { get; set; }
}

/// <summary>
/// A single place result item.
/// </summary>
public class PlaceItem
{
    /// <summary>
    /// Display name of the place.
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
    /// Category names.
    /// </summary>
    public List<string>? Categories { get; set; }

    /// <summary>
    /// Opening hours text.
    /// </summary>
    public string? OpeningHours { get; set; }

    /// <summary>
    /// Contact information (phone, website, etc.).
    /// </summary>
    public List<PlaceContact>? Contacts { get; set; }

    /// <summary>
    /// Distance from the search center in meters.
    /// </summary>
    public int? Distance { get; set; }

    /// <summary>
    /// HERE Place ID for subsequent lookup requests.
    /// </summary>
    public string? PlaceId { get; set; }
}

/// <summary>
/// Contact information for a place.
/// </summary>
public class PlaceContact
{
    /// <summary>
    /// Contact type (e.g. "phone", "website", "email").
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Contact value.
    /// </summary>
    public string? Value { get; set; }
}
