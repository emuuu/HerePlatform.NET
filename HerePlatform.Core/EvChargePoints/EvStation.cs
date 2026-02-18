using System.Collections.Generic;
using HerePlatform.Core.Coordinates;

namespace HerePlatform.Core.EvChargePoints;

/// <summary>
/// An EV charging station with its connectors.
/// </summary>
public class EvStation
{
    /// <summary>
    /// Unique pool/station identifier.
    /// </summary>
    public string? PoolId { get; set; }

    /// <summary>
    /// Formatted address of the station.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Geographic position.
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Total number of connectors at this station.
    /// </summary>
    public int TotalNumberOfConnectors { get; set; }

    /// <summary>
    /// Available connectors at this station.
    /// </summary>
    public List<EvConnector>? Connectors { get; set; }
}
