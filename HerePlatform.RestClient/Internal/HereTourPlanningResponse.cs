namespace HerePlatform.RestClient.Internal;

/// <summary>
/// Internal DTOs matching HERE Tour Planning API v3 response structure.
/// </summary>
internal sealed class HereTourPlanningResponse
{
    public List<HereTour>? Tours { get; set; }
    public List<string>? UnassignedJobs { get; set; }
    public HereTourStatistic? Statistic { get; set; }
}

internal sealed class HereTour
{
    public string? VehicleId { get; set; }
    public List<HereTourStop>? Stops { get; set; }
    public HereTourStatistic? Statistic { get; set; }
}

internal sealed class HereTourStop
{
    public HerePosition? Location { get; set; }
    public List<HereTourActivity>? Activities { get; set; }
}

internal sealed class HereTourActivity
{
    public string? Type { get; set; }
    public string? JobId { get; set; }
}

internal sealed class HereTourStatistic
{
    public double? Cost { get; set; }
    public double? Distance { get; set; }
    public double? Duration { get; set; }
}
