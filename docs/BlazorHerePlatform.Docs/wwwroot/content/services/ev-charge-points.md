---
title: EV Charge Points Service
category: Services
order: 13
description: "Find electric vehicle charging stations nearby."
apiRef: IEvChargePointsService
---

## Setup

Inject `IEvChargePointsService` into your component. It is registered automatically by `AddBlazorHerePlatform`.

```csharp
@inject IEvChargePointsService EvChargePointsService
```

## SearchStationsAsync

Search for EV charging stations near a location. Filter by connector type and radius.

```csharp
var result = await EvChargePointsService.SearchStationsAsync(new EvChargePointsRequest
{
    Position = new LatLngLiteral(52.5200, 13.4050),
    Radius = 5000,
    ConnectorTypes = [ConnectorType.CcsCombo2, ConnectorType.Type2],
    MaxResults = 10
});

foreach (var station in result.Stations ?? [])
{
    Console.WriteLine($"{station.Address} — {station.TotalNumberOfConnectors} connectors");
    foreach (var conn in station.Connectors ?? [])
    {
        Console.WriteLine($"  {conn.ConnectorTypeName}: {conn.MaxPowerLevel} kW");
    }
}
```

## EvChargePointsRequest Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Position` | `LatLngLiteral` | -- | Search center (required). |
| `Radius` | `double` | `5000` | Search radius in meters. |
| `ConnectorTypes` | `List<ConnectorType>?` | `null` | Filter by connector type. |
| `MaxResults` | `int` | `20` | Maximum number of stations. |

## ConnectorType Enum

| Value | Description |
|-------|-------------|
| `Type2` | Type 2 (Mennekes). |
| `Chademo` | CHAdeMO DC fast charging. |
| `CcsCombo1` | CCS Combo 1 (SAE). |
| `CcsCombo2` | CCS Combo 2 (EU). |
| `TeslaSupercharger` | Tesla Supercharger. |

## EvStation

| Property | Type | Description |
|----------|------|-------------|
| `PoolId` | `string?` | Station pool identifier. |
| `Address` | `string?` | Station address. |
| `Position` | `LatLngLiteral?` | Geographic coordinates. |
| `TotalNumberOfConnectors` | `int` | Total connectors at station. |
| `Connectors` | `List<EvConnector>?` | Available connectors. |

## EvConnector

| Property | Type | Description |
|----------|------|-------------|
| `SupplierName` | `string?` | Charging network name. |
| `ConnectorTypeName` | `string?` | Connector type label. |
| `ConnectorTypeId` | `string?` | Connector type identifier. |
| `MaxPowerLevel` | `double?` | Maximum power in kW. |
| `ChargeCapacity` | `int?` | Number of simultaneous charges. |
| `FixedCable` | `bool?` | Whether the cable is attached. |
