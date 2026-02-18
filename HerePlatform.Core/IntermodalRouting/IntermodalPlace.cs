using HerePlatform.Core.Coordinates;

namespace HerePlatform.Core.IntermodalRouting;

/// <summary>
/// A departure or arrival place in an intermodal route section.
/// </summary>
public class IntermodalPlace
{
    /// <summary>
    /// Place name (e.g. station name).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Geographic position.
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Time in ISO 8601 format.
    /// </summary>
    public string? Time { get; set; }
}
