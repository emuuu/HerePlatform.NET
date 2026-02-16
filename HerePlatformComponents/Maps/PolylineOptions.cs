using HerePlatform.Core.Coordinates;
using System.Collections.Generic;

namespace HerePlatformComponents.Maps;

public class PolylineOptions : ListableEntityOptionsBase
{
    /// <summary>
    /// The path as a sequence of {lat, lng} points.
    /// Will be converted to H.geo.LineString in JS.
    /// </summary>
    public IEnumerable<LatLngLiteral>? Path { get; set; }

    /// <summary>
    /// Visual style for the polyline.
    /// </summary>
    public StyleOptions? Style { get; set; }
}
