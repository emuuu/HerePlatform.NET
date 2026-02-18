namespace HerePlatform.RestClient.Internal;

/// <summary>
/// Internal DTOs matching HERE Intermodal Routing API v8 response structure.
/// </summary>
internal sealed class HereIntermodalRoutingResponse
{
    public List<HereIntermodalRoute>? Routes { get; set; }
}

internal sealed class HereIntermodalRoute
{
    public List<HereIntermodalSection>? Sections { get; set; }
}

internal sealed class HereIntermodalSection
{
    public string? Type { get; set; }
    public string? Polyline { get; set; }
    public HereIntermodalPlaceDto? Departure { get; set; }
    public HereIntermodalPlaceDto? Arrival { get; set; }
    public HereIntermodalTravelSummary? TravelSummary { get; set; }
    public HereIntermodalTransport? Transport { get; set; }
}

internal sealed class HereIntermodalPlaceDto
{
    public string? Name { get; set; }
    public HerePosition? Location { get; set; }
    public string? Time { get; set; }
}

internal sealed class HereIntermodalTravelSummary
{
    public int Duration { get; set; }
    public int Length { get; set; }
}

internal sealed class HereIntermodalTransport
{
    public string? Mode { get; set; }
    public string? Name { get; set; }
    public string? Headsign { get; set; }
    public string? ShortName { get; set; }
    public string? Color { get; set; }
}
