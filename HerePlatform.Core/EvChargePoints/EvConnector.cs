namespace HerePlatform.Core.EvChargePoints;

/// <summary>
/// A single EV charging connector at a station.
/// </summary>
public class EvConnector
{
    /// <summary>
    /// Name of the charging network supplier.
    /// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// Display name of the connector type (e.g. "Type 2", "CCS").
    /// </summary>
    public string? ConnectorTypeName { get; set; }

    /// <summary>
    /// Connector type identifier.
    /// </summary>
    public string? ConnectorTypeId { get; set; }

    /// <summary>
    /// Maximum power output in kW.
    /// </summary>
    public double? MaxPowerLevel { get; set; }

    /// <summary>
    /// Number of vehicles that can charge simultaneously.
    /// </summary>
    public int? ChargeCapacity { get; set; }

    /// <summary>
    /// Whether the connector has a fixed (tethered) cable.
    /// </summary>
    public bool? FixedCable { get; set; }
}
