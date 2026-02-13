namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// Electric vehicle routing parameters.
/// </summary>
public class EvOptions
{
    /// <summary>
    /// Initial battery charge in watt-hours.
    /// </summary>
    public int? InitialCharge { get; set; }

    /// <summary>
    /// Maximum battery capacity in watt-hours.
    /// </summary>
    public int? MaxCharge { get; set; }

    /// <summary>
    /// Maximum charge after a charging station stop in watt-hours.
    /// </summary>
    public int? MaxChargeAfterChargingStation { get; set; }

    /// <summary>
    /// Minimum charge required at a charging station in watt-hours.
    /// </summary>
    public int? MinChargeAtChargingStation { get; set; }

    /// <summary>
    /// Minimum charge required at destination in watt-hours.
    /// </summary>
    public int? MinChargeAtDestination { get; set; }

    /// <summary>
    /// Charging curve as comma-separated pairs (charge_level,charging_speed,...).
    /// </summary>
    public string? ChargingCurve { get; set; }

    /// <summary>
    /// Free-flow speed consumption table (speed,consumption,...).
    /// </summary>
    public string? FreeFlowSpeedTable { get; set; }

    /// <summary>
    /// Auxiliary power consumption in watts.
    /// </summary>
    public int? AuxiliaryConsumption { get; set; }
}
