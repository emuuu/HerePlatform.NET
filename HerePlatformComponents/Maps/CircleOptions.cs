namespace HerePlatformComponents.Maps;

public class CircleOptions : ListableEntityOptionsBase
{
    /// <summary>
    /// Center of the circle.
    /// </summary>
    public LatLngLiteral? Center { get; set; }

    /// <summary>
    /// Radius in meters.
    /// </summary>
    public double Radius { get; set; }

    /// <summary>
    /// Visual style for the circle.
    /// </summary>
    public StyleOptions? Style { get; set; }

    /// <summary>
    /// Number of segments for the circle approximation (default 60).
    /// </summary>
    public double? Precision { get; set; }

    /// <summary>
    /// Extrusion height in meters (3D rendering).
    /// </summary>
    public double? Extrusion { get; set; }

    /// <summary>
    /// Elevation in meters (3D rendering).
    /// </summary>
    public double? Elevation { get; set; }
}
