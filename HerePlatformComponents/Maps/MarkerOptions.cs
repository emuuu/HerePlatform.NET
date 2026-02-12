using System.Text.Json.Serialization;

namespace HerePlatformComponents.Maps;

public class MarkerOptions : ListableEntityOptionsBase
{
    /// <summary>
    /// The geographic position of the marker. Set via CreateAsync, not serialized.
    /// </summary>
    [JsonIgnore]
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Icon for the marker (URL string, SVG string, or Icon object reference).
    /// </summary>
    public object? Icon { get; set; }

    /// <summary>
    /// Whether the marker is draggable.
    /// </summary>
    public bool? Draggable { get; set; }
}
