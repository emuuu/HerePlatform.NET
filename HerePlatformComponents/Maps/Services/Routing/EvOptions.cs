namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// Electric vehicle routing parameters.
/// </summary>
public class EvOptions
{
    /// <summary>
    /// Initial battery charge in kWh.
    /// </summary>
    public double? InitialCharge { get; set; }

    /// <summary>
    /// Maximum battery capacity in kWh (max 1200).
    /// </summary>
    public double? MaxCharge { get; set; }

    /// <summary>
    /// Maximum charge after a charging station stop in kWh.
    /// </summary>
    public double? MaxChargeAfterChargingStation { get; set; }

    /// <summary>
    /// Minimum charge required at a charging station in kWh.
    /// </summary>
    public double? MinChargeAtChargingStation { get; set; }

    /// <summary>
    /// Minimum charge required at destination in kWh.
    /// </summary>
    public double? MinChargeAtDestination { get; set; }

    /// <summary>
    /// Charging curve as comma-separated pairs (charge_level_kWh,charging_speed_kW,...).
    /// </summary>
    public string? ChargingCurve { get; set; }

    /// <summary>
    /// Free-flow speed consumption table: speed(km/h),consumption(kWh/km) pairs.
    /// </summary>
    public string? FreeFlowSpeedTable { get; set; }

    /// <summary>
    /// Auxiliary power consumption in kW.
    /// </summary>
    public double? AuxiliaryConsumption { get; set; }
}
