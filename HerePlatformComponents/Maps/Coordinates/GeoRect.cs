namespace HerePlatformComponents.Maps.Coordinates;

/// <summary>
/// Represents a geographic bounding box (H.geo.Rect).
/// </summary>
public class GeoRect
{
    public double Top { get; set; }
    public double Left { get; set; }
    public double Bottom { get; set; }
    public double Right { get; set; }

    public GeoRect()
    {
    }

    public GeoRect(double top, double left, double bottom, double right)
    {
        Top = top;
        Left = left;
        Bottom = bottom;
        Right = right;
    }
}
