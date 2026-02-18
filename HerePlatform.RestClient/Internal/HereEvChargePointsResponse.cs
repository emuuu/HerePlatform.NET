namespace HerePlatform.RestClient.Internal;

/// <summary>
/// Internal DTOs matching HERE EV Charge Points API v3 response structure.
/// </summary>
internal sealed class HereEvChargePointsResponse
{
    public List<HereEvStation>? EvStations { get; set; }
}

internal sealed class HereEvStation
{
    public string? PoolId { get; set; }
    public HereEvAddress? Address { get; set; }
    public HerePosition? Position { get; set; }
    public int TotalNumberOfConnectors { get; set; }
    public List<HereEvConnector>? Connectors { get; set; }
}

internal sealed class HereEvAddress
{
    public string? Label { get; set; }
}

internal sealed class HereEvConnector
{
    public string? SupplierName { get; set; }
    public HereEvConnectorType? ConnectorType { get; set; }
    public double? MaxPowerLevel { get; set; }
    public int? ChargeCapacity { get; set; }
    public bool? FixedCable { get; set; }
}

internal sealed class HereEvConnectorType
{
    public string? Name { get; set; }
    public string? Id { get; set; }
}
