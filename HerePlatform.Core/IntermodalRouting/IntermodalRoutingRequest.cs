using HerePlatform.Core.Coordinates;

namespace HerePlatform.Core.IntermodalRouting;

/// <summary>
/// Request parameters for the Intermodal Routing API.
/// </summary>
public class IntermodalRoutingRequest
{
    /// <summary>
    /// Starting point.
    /// </summary>
    public LatLngLiteral Origin { get; set; }

    /// <summary>
    /// End point.
    /// </summary>
    public LatLngLiteral Destination { get; set; }

    /// <summary>
    /// Departure time in ISO 8601 format (e.g. "2024-01-15T08:00:00").
    /// </summary>
    public string? DepartAt { get; set; }

    /// <summary>
    /// Arrival time in ISO 8601 format.
    /// </summary>
    public string? ArriveAt { get; set; }

    /// <summary>
    /// Number of alternative routes to calculate.
    /// </summary>
    public int Alternatives { get; set; }

    /// <summary>
    /// Whether to return polylines for each section (default true).
    /// </summary>
    public bool ReturnPolyline { get; set; } = true;

    /// <summary>
    /// Whether to return turn-by-turn actions.
    /// </summary>
    public bool ReturnActions { get; set; }

    /// <summary>
    /// Whether to return travel summary per section (default true).
    /// </summary>
    public bool ReturnTravelSummary { get; set; } = true;

    /// <summary>
    /// Language for results (e.g. "en", "de").
    /// </summary>
    public string? Lang { get; set; }
}
