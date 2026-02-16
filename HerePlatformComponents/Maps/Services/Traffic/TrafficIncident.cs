using HerePlatform.Core.Coordinates;
using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Traffic;

/// <summary>
/// A traffic incident from the HERE Traffic API.
/// </summary>
public class TrafficIncident
{
    /// <summary>
    /// Incident type (e.g., "accident", "construction", "congestion").
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Severity level (0-4, 0=unknown, 4=blocking).
    /// </summary>
    public int Severity { get; set; }

    /// <summary>
    /// Description of the incident.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Location of the incident.
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Road name where the incident occurs.
    /// </summary>
    public string? RoadName { get; set; }

    /// <summary>
    /// Start time of the incident (ISO 8601).
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// End time of the incident (ISO 8601).
    /// </summary>
    public string? EndTime { get; set; }
}

/// <summary>
/// Result containing traffic incidents.
/// </summary>
public class TrafficIncidentsResult
{
    /// <summary>
    /// List of traffic incidents.
    /// </summary>
    public List<TrafficIncident>? Incidents { get; set; }
}
