namespace HerePlatform.Core.Search;

/// <summary>
/// Structured address details from a HERE Autosuggest result.
/// </summary>
/// <remarks>
/// The Autosuggest API populates the structured fields (everything except
/// <see cref="Label"/>) only when the request includes <c>show=details</c>
/// (<see cref="AutosuggestOptions.Show"/>, on by default). Without it the
/// response carries only <see cref="Label"/> and all other fields stay null.
/// </remarks>
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
    /// State or region code (e.g. "NW" for Nordrhein-Westfalen).
    /// </summary>
    public string? StateCode { get; set; }

    /// <summary>
    /// County name.
    /// </summary>
    public string? County { get; set; }

    /// <summary>
    /// County code (e.g. "OB").
    /// </summary>
    public string? CountyCode { get; set; }

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
