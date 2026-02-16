using HerePlatform.Core.Coordinates;

namespace HerePlatformComponents.Maps.Events;

/// <summary>
/// Event data for drag events (dragstart, drag, dragend).
/// </summary>
public class MapDragEventArgs
{
    /// <summary>
    /// X coordinate in the map viewport (pixels).
    /// </summary>
    public double ViewportX { get; set; }

    /// <summary>
    /// Y coordinate in the map viewport (pixels).
    /// </summary>
    public double ViewportY { get; set; }

    /// <summary>
    /// Geographic position of the drag point on the map.
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// The HERE event type string (e.g. "dragstart", "drag", "dragend").
    /// </summary>
    public string? Type { get; set; }
}
