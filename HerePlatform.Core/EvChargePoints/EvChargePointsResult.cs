using System.Collections.Generic;

namespace HerePlatform.Core.EvChargePoints;

/// <summary>
/// Result of an EV Charge Points API search.
/// </summary>
public class EvChargePointsResult
{
    /// <summary>
    /// Charging stations found.
    /// </summary>
    public List<EvStation>? Stations { get; set; }
}
