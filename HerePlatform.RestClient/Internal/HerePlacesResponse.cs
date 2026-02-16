namespace HerePlatform.RestClient.Internal;

/// <summary>
/// Internal DTOs matching HERE Geocoding &amp; Search v7 Places response structure.
/// </summary>
internal sealed class HerePlacesResponse
{
    public List<HerePlaceItem>? Items { get; set; }
}

internal sealed class HerePlaceItem
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public HerePosition? Position { get; set; }
    public HereAddress? Address { get; set; }
    public List<HerePlaceCategory>? Categories { get; set; }
    public List<HereOpeningHours>? OpeningHours { get; set; }
    public List<HerePlaceContactList>? Contacts { get; set; }
    public int? Distance { get; set; }
}

internal sealed class HerePlaceCategory
{
    public string? Name { get; set; }
}

internal sealed class HereOpeningHours
{
    public List<string>? Text { get; set; }
}

internal sealed class HerePlaceContactList
{
    public List<HerePlaceContactValue>? Phone { get; set; }
    public List<HerePlaceContactValue>? Www { get; set; }
    public List<HerePlaceContactValue>? Email { get; set; }
}

internal sealed class HerePlaceContactValue
{
    public string? Value { get; set; }
}
