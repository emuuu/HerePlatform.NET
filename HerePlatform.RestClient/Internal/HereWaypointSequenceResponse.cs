namespace HerePlatform.RestClient.Internal;

internal sealed class HereWaypointSequenceResponse
{
    public List<HereWpsResult>? Results { get; set; }
}

internal sealed class HereWpsResult
{
    public List<HereWpsWaypoint>? Waypoints { get; set; }
    public int Distance { get; set; }
    public int Time { get; set; }
}

internal sealed class HereWpsWaypoint
{
    public string? Id { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public int Sequence { get; set; }
}
