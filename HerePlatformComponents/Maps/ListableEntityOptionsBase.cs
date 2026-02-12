namespace HerePlatformComponents.Maps;

public interface IListableEntityOptionsBase
{
}

public abstract class ListableEntityOptionsBase : IListableEntityOptionsBase
{
    /// <summary>
    /// Whether the entity is visible on the map.
    /// </summary>
    public bool? Visibility { get; set; }

    /// <summary>
    /// Z-index for stacking order.
    /// </summary>
    public int? ZIndex { get; set; }

    /// <summary>
    /// Minimum zoom level at which the entity is visible.
    /// </summary>
    public double? Min { get; set; }

    /// <summary>
    /// Maximum zoom level at which the entity is visible.
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Indicates whether the object can change its appearance at any time (HARP engine optimization hint).
    /// </summary>
    public bool? Volatility { get; set; }

    /// <summary>
    /// Arbitrary data to associate with the map object.
    /// </summary>
    public object? Data { get; set; }
}
