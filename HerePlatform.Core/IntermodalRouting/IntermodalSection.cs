using System.Collections.Generic;
using HerePlatform.Core.Coordinates;

namespace HerePlatform.Core.IntermodalRouting;

/// <summary>
/// A single section of an intermodal route (e.g. pedestrian, transit, etc.).
/// </summary>
public class IntermodalSection
{
    /// <summary>
    /// Section type (e.g. "pedestrian", "transit").
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Encoded polyline string.
    /// </summary>
    public string? Polyline { get; set; }

    /// <summary>
    /// Decoded polyline geometry.
    /// </summary>
    public List<LatLngLiteral>? Geometry { get; set; }

    /// <summary>
    /// Departure place.
    /// </summary>
    public IntermodalPlace? Departure { get; set; }

    /// <summary>
    /// Arrival place.
    /// </summary>
    public IntermodalPlace? Arrival { get; set; }

    /// <summary>
    /// Travel summary for this section.
    /// </summary>
    public IntermodalSummary? Summary { get; set; }

    /// <summary>
    /// Transport details (for transit sections).
    /// </summary>
    public IntermodalTransport? Transport { get; set; }
}
