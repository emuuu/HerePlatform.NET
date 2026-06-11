namespace HerePlatform.RestClient.Internal;

/// <summary>
/// Internal DTOs matching HERE Geocoding &amp; Search v7 response structure.
/// </summary>
internal sealed class HereGeocodeResponse
{
    public List<HereGeocodeItem>? Items { get; set; }
}

internal sealed class HereGeocodeItem
{
    public string? Title { get; set; }
    public HerePosition? Position { get; set; }
    public HereAddress? Address { get; set; }
    public string? ResultType { get; set; }
}

internal sealed class HerePosition
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

internal sealed class HereAddress
{
    public string? Label { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string? State { get; set; }
    public string? StateCode { get; set; }
    public string? County { get; set; }
    public string? CountyCode { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? HouseNumber { get; set; }
}
