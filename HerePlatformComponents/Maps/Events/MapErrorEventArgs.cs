namespace HerePlatformComponents.Maps.Events;

/// <summary>
/// Event data for map-level errors such as authentication failures.
/// </summary>
public class MapErrorEventArgs
{
    /// <summary>
    /// The error source (e.g. "tile", "service").
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Human-readable error message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// HTTP status code when available (e.g. 401, 403).
    /// </summary>
    public int? StatusCode { get; set; }
}
