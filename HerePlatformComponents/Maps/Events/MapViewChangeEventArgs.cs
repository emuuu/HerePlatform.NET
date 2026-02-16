using HerePlatform.Core.Coordinates;

namespace HerePlatformComponents.Maps.Events;

/// <summary>
/// Event data for map view change events (mapviewchange, mapviewchangestart, mapviewchangeend).
/// </summary>
public class MapViewChangeEventArgs
{
    /// <summary>
    /// Current center of the map.
    /// </summary>
    public LatLngLiteral? Center { get; set; }

    /// <summary>
    /// Current zoom level.
    /// </summary>
    public double Zoom { get; set; }

    /// <summary>
    /// Current tilt angle in degrees.
    /// </summary>
    public double Tilt { get; set; }

    /// <summary>
    /// Current heading (rotation) in degrees.
    /// </summary>
    public double Heading { get; set; }

    /// <summary>
    /// The HERE event type string.
    /// </summary>
    public string? Type { get; set; }
}
