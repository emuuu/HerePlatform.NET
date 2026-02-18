using System.Collections.Generic;
using HerePlatform.Core.Coordinates;

namespace HerePlatform.Core.EvChargePoints;

/// <summary>
/// Request parameters for EV Charge Points API station search.
/// </summary>
public class EvChargePointsRequest
{
    /// <summary>
    /// Center position for proximity search.
    /// </summary>
    public LatLngLiteral Position { get; set; }

    /// <summary>
    /// Search radius in meters (default 5000).
    /// </summary>
    public double Radius { get; set; } = 5000;

    /// <summary>
    /// Optional filter for specific connector types.
    /// </summary>
    public List<ConnectorType>? ConnectorTypes { get; set; }

    /// <summary>
    /// Maximum number of results (default 20).
    /// </summary>
    public int MaxResults { get; set; } = 20;
}
