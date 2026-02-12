namespace HerePlatformComponents.Maps;

/// <summary>
/// Style options for HERE map shapes (Polyline, Polygon, Circle, Rect).
/// Maps to H.map.SpatialStyle.
/// </summary>
public class StyleOptions
{
    /// <summary>
    /// Stroke color in CSS format (e.g. "rgba(0, 0, 255, 0.5)" or "#0000FF").
    /// </summary>
    public string? StrokeColor { get; set; }

    /// <summary>
    /// Fill color in CSS format.
    /// </summary>
    public string? FillColor { get; set; }

    /// <summary>
    /// Width of the line in pixels.
    /// </summary>
    public double? LineWidth { get; set; }

    /// <summary>
    /// Line cap style: "round", "square", "butt".
    /// </summary>
    public string? LineCap { get; set; }

    /// <summary>
    /// Array of dash pattern [dash, gap, dash, gap, ...].
    /// </summary>
    public double[]? LineDash { get; set; }

    /// <summary>
    /// URL or reference to a dash image (HARP engine feature).
    /// </summary>
    public string? LineDashImage { get; set; }

    /// <summary>
    /// Dash scale mode for the HARP engine: "CONTINUOUS" or "DISCRETE".
    /// </summary>
    public string? LineDashScaleMode { get; set; }
}
