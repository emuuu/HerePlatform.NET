---
title: Weather Service
category: Services
order: 11
description: "Current conditions and weather forecasts."
apiRef: IWeatherService
---

## Setup

Inject `IWeatherService` into your component. It is registered automatically by `AddBlazorHerePlatform`.

```csharp
@inject IWeatherService WeatherService
```

## GetWeatherAsync

Retrieve current weather observations and multi-day forecasts for a location.

```csharp
var result = await WeatherService.GetWeatherAsync(new WeatherRequest
{
    Location = new LatLngLiteral(52.5200, 13.4050),
    Products = [WeatherProduct.Observation, WeatherProduct.Forecast7Days],
    Lang = "de"
});

foreach (var obs in result.Observations ?? [])
{
    Console.WriteLine($"{obs.Description}: {obs.Temperature}°C");
    Console.WriteLine($"Wind: {obs.WindSpeed} km/h {obs.WindDirection}");
    Console.WriteLine($"Humidity: {obs.Humidity}%");
}

foreach (var fc in result.Forecasts ?? [])
{
    Console.WriteLine($"{fc.Date}: {fc.TemperatureLow}°–{fc.TemperatureHigh}°C — {fc.Description}");
}
```

## WeatherRequest Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Location` | `LatLngLiteral` | -- | Position to query (required). |
| `Products` | `List<WeatherProduct>?` | `null` | Weather products to include. |
| `Lang` | `string?` | `null` | Language (BCP 47). |

## WeatherProduct Enum

| Value | Description |
|-------|-------------|
| `Observation` | Current conditions. |
| `Forecast7Days` | Detailed 7-day forecast. |
| `Forecast7DaysSimple` | Simplified 7-day forecast. |
| `ForecastHourly` | Hourly forecast. |
| `ForecastAstronomy` | Sunrise/sunset and moon phases. |

## WeatherObservation

| Property | Type | Description |
|----------|------|-------------|
| `Temperature` | `double?` | Temperature in °C. |
| `Humidity` | `double?` | Relative humidity (%). |
| `WindSpeed` | `double?` | Wind speed in km/h. |
| `WindDirection` | `string?` | Cardinal direction (e.g. `"NW"`). |
| `Description` | `string?` | Human-readable description. |
| `Icon` | `string?` | Icon identifier. |
| `Timestamp` | `string?` | Observation time (ISO 8601). |
| `DewPoint` | `double?` | Dew point in °C. |
| `BarometerPressure` | `double?` | Atmospheric pressure in hPa. |
| `Visibility` | `double?` | Visibility in km. |
| `UvIndex` | `int?` | UV index. |

## WeatherForecast

| Property | Type | Description |
|----------|------|-------------|
| `Date` | `string?` | Forecast date (YYYY-MM-DD). |
| `TemperatureHigh` | `double?` | Daily high in °C. |
| `TemperatureLow` | `double?` | Daily low in °C. |
| `Description` | `string?` | Weather description. |
| `Humidity` | `double?` | Expected humidity (%). |
| `WindSpeed` | `double?` | Expected wind speed in km/h. |
| `PrecipitationProbability` | `double?` | Precipitation probability (0–1). |
