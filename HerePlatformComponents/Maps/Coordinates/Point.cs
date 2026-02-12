namespace HerePlatformComponents.Maps.Coordinates;

/// <summary>
/// Represents a screen point {x, y}.
/// </summary>
public class Point
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point()
    {
    }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}
