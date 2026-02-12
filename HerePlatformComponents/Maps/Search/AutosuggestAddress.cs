namespace HerePlatformComponents.Maps.Search;

/// <summary>
/// Structured address details from a HERE Autosuggest result.
/// </summary>
public class AutosuggestAddress
{
    /// <summary>
    /// Full formatted address label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// ISO 3166-1 alpha-3 country code (e.g. "DEU").
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Country name.
    /// </summary>
    public string? CountryName { get; set; }

    /// <summary>
    /// State or region name.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// City name.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// District or neighborhood name.
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// Street name.
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// Postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// House number.
    /// </summary>
    public string? HouseNumber { get; set; }
}
