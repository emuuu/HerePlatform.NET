namespace HerePlatform.Core.IntermodalRouting;

/// <summary>
/// Transport details for a transit section.
/// </summary>
public class IntermodalTransport
{
    /// <summary>
    /// Transport mode (e.g. "bus", "subway", "pedestrian").
    /// </summary>
    public string? Mode { get; set; }

    /// <summary>
    /// Line or service name (e.g. "U6").
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Direction/headsign text.
    /// </summary>
    public string? Headsign { get; set; }

    /// <summary>
    /// Short name or line number.
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// Line color as hex string (e.g. "#0066CC").
    /// </summary>
    public string? Color { get; set; }
}
