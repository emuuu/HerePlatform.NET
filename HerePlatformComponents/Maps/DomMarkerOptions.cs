using HerePlatform.Core.Coordinates;
using System.Text.Json.Serialization;

namespace HerePlatformComponents.Maps;

public class DomMarkerOptions : ListableEntityOptionsBase
{
    [JsonIgnore]
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// DomIcon for the marker.
    /// </summary>
    public object? Icon { get; set; }

    public bool? Draggable { get; set; }
}
