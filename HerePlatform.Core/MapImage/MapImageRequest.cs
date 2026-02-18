using HerePlatform.Core.Coordinates;

namespace HerePlatform.Core.MapImage;

/// <summary>
/// Request parameters for the Map Image API.
/// </summary>
public class MapImageRequest
{
    /// <summary>
    /// Center position of the map image.
    /// </summary>
    public LatLngLiteral Center { get; set; }

    /// <summary>
    /// Zoom level (default 12).
    /// </summary>
    public int Zoom { get; set; } = 12;

    /// <summary>
    /// Image width in pixels (default 512).
    /// </summary>
    public int Width { get; set; } = 512;

    /// <summary>
    /// Image height in pixels (default 512).
    /// </summary>
    public int Height { get; set; } = 512;

    /// <summary>
    /// Image output format (default PNG).
    /// </summary>
    public MapImageFormat Format { get; set; } = MapImageFormat.Png;

    /// <summary>
    /// Map style (default explore.day).
    /// </summary>
    public MapImageStyle Style { get; set; } = MapImageStyle.Default;

    /// <summary>
    /// Pixels per inch scaling factor (default 72). Use 250 or 320 for high-DPI.
    /// </summary>
    public int Ppi { get; set; } = 72;
}
