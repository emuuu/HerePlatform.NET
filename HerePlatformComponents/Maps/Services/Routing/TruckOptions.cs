namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// Truck-specific routing parameters.
/// </summary>
public class TruckOptions
{
    /// <summary>
    /// Vehicle height in meters.
    /// </summary>
    public double? Height { get; set; }

    /// <summary>
    /// Vehicle width in meters.
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// Vehicle length in meters.
    /// </summary>
    public double? Length { get; set; }

    /// <summary>
    /// Gross weight in kilograms.
    /// </summary>
    public int? GrossWeight { get; set; }

    /// <summary>
    /// Weight per axle in kilograms.
    /// </summary>
    public int? WeightPerAxle { get; set; }

    /// <summary>
    /// Number of axles.
    /// </summary>
    public int? AxleCount { get; set; }

    /// <summary>
    /// Number of trailers.
    /// </summary>
    public int? TrailerCount { get; set; }

    /// <summary>
    /// Tunnel category restriction.
    /// </summary>
    public TunnelCategory? TunnelCategory { get; set; }

    /// <summary>
    /// Hazardous goods flags.
    /// </summary>
    public HazardousGoods HazardousGoods { get; set; } = HazardousGoods.None;
}
