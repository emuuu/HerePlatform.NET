using HerePlatform.Core.Coordinates;

namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// A single turn-by-turn instruction in a route section.
/// </summary>
public class TurnInstruction
{
    /// <summary>
    /// Action type (e.g. "depart", "arrive", "turnLeft", "turnRight", "continue").
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// Human-readable instruction text.
    /// </summary>
    public string? Instruction { get; set; }

    /// <summary>
    /// Duration for this maneuver in seconds.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Length for this maneuver in meters.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Index offset into the polyline where this maneuver starts.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Position of this maneuver.
    /// </summary>
    public LatLngLiteral? Position { get; set; }
}
