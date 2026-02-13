namespace HerePlatformComponents.Maps.Events;

/// <summary>
/// Event arguments for context menu item selection.
/// </summary>
public class ContextMenuEventArgs
{
    /// <summary>
    /// Geographic position where the context menu was opened.
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Label of the selected menu item.
    /// </summary>
    public string? ItemLabel { get; set; }

    /// <summary>
    /// Custom data associated with the selected menu item.
    /// </summary>
    public object? ItemData { get; set; }

    /// <summary>
    /// Viewport X coordinate where the menu was opened.
    /// </summary>
    public double ViewportX { get; set; }

    /// <summary>
    /// Viewport Y coordinate where the menu was opened.
    /// </summary>
    public double ViewportY { get; set; }
}
