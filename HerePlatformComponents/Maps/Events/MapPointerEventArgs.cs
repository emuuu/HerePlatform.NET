using HerePlatform.Core.Coordinates;

namespace HerePlatformComponents.Maps.Events;

/// <summary>
/// Event data for pointer/interaction events (tap, dbltap, longpress, contextmenu,
/// pointerdown, pointerup, pointermove, pointerenter, pointerleave).
/// </summary>
public class MapPointerEventArgs
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
    /// Geographic position of the pointer on the map, if available.
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Mouse button that triggered the event (0 = left, 1 = middle, 2 = right).
    /// </summary>
    public int Button { get; set; }

    /// <summary>
    /// Bitmask of currently pressed buttons.
    /// </summary>
    public int Buttons { get; set; }

    /// <summary>
    /// Pointer type: "mouse", "touch", or "pen".
    /// </summary>
    public string? PointerType { get; set; }

    /// <summary>
    /// The HERE event type string (e.g. "tap", "pointerdown").
    /// </summary>
    public string? Type { get; set; }
}
