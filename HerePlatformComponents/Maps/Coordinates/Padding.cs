namespace HerePlatformComponents.Maps.Coordinates;

/// <summary>
/// Map padding {top, right, bottom, left}.
/// </summary>
public class Padding
{
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
    public int Left { get; set; }

    public Padding()
    {
    }

    public Padding(int top, int right, int bottom, int left)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }
}
