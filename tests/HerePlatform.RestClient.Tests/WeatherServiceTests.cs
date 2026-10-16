using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Weather;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class WeatherServiceTests
{
    private static RestWeatherService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestWeatherService(factory);
    }

    [Test]
    public async Task GetWeatherAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{}""");
        var service = CreateService(handler);

        await service.GetWeatherAsync(new WeatherRequest
        {
            Location = new LatLngLiteral(52.5, 13.4),
            Products = [WeatherProduct.Observation],
            Lang = "de"
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://weather.hereapi.com/v3/report?"));
        Assert.That(url, Does.Contain("products=observation"));
        Assert.That(url, Does.Contain("location=52.5%2C13.4"));
        Assert.That(url, Does.Contain("lang=de"));
    }

    [Test]
    public async Task GetWeatherAsync_MultipleProducts_JoinsWithComma()
    {
        var handler = MockHttpHandler.WithJson("""{}""");
        var service = CreateService(handler);

        await service.GetWeatherAsync(new WeatherRequest
        {
            Location = new LatLngLiteral(52.5, 13.4),
            Products = [WeatherProduct.Observation, WeatherProduct.Forecast7Days]
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("products=observation%2Cforecast7days"));
    }

    [Test]
    public async Task GetWeatherAsync_DefaultsToObservation_WhenNoProducts()
    {
        var handler = MockHttpHandler.WithJson("""{}""");
        var service = CreateService(handler);

        await service.GetWeatherAsync(new WeatherRequest
        {
            Location = new LatLngLiteral(52.5, 13.4)
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("products=observation"));
    }

    [Test]
    public async Task GetWeatherAsync_MapsObservationCorrectly()
    {
        var json = """
        {
            "observations": [
                {
                    "temperature": 15.2,
                    "humidity": 65,
                    "windSpeed": 12.5,
                    "windDescShort": "NW",
                    "description": "Partly cloudy",
                    "iconName": "partly_cloudy",
                    "utcTime": "2025-01-15T12:00:00Z",
                    "dewPoint": 8.1,
                    "comfort": 13.5,
                    "barometerPressure": 1013.25,
                    "visibility": 10.0,
                    "uvIndex": 3,
                    "precipitationProbability": 20.0
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GetWeatherAsync(new WeatherRequest
        {
            Location = new LatLngLiteral(52.5, 13.4),
            Products = [WeatherProduct.Observation]
        });

        Assert.That(result.Observations, Has.Count.EqualTo(1));
        var obs = result.Observations![0];
        Assert.That(obs.Temperature, Is.EqualTo(15.2));
        Assert.That(obs.Humidity, Is.EqualTo(65.0));
        Assert.That(obs.WindSpeed, Is.EqualTo(12.5));
        Assert.That(obs.WindDirection, Is.EqualTo("NW"));
        Assert.That(obs.Description, Is.EqualTo("Partly cloudy"));
        Assert.That(obs.Icon, Is.EqualTo("partly_cloudy"));
        Assert.That(obs.Timestamp, Is.EqualTo("2025-01-15T12:00:00Z"));
        Assert.That(obs.DewPoint, Is.EqualTo(8.1));
        Assert.That(obs.Comfort, Is.EqualTo(13.5));
        Assert.That(obs.BarometerPressure, Is.EqualTo(1013.25));
        Assert.That(obs.Visibility, Is.EqualTo(10.0));
        Assert.That(obs.UvIndex, Is.EqualTo(3));
        Assert.That(obs.PrecipitationProbability, Is.EqualTo(20.0));
    }

    [Test]
    public async Task GetWeatherAsync_MapsForecastCorrectly()
    {
        var json = """
        {
            "dailyForecasts": [
                {
                    "forecasts": [
                        {
                            "utcTime": "2025-01-15",
                            "highTemperature": 18.0,
                            "lowTemperature": 5.5,
                            "description": "Sunny",
                            "iconName": "sunny",
                            "humidity": 45,
                            "windSpeed": 8.0,
                            "precipitationProbability": 10.0,
                            "comfort": 16.5
                        }
                    ]
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GetWeatherAsync(new WeatherRequest
        {
            Location = new LatLngLiteral(52.5, 13.4),
            Products = [WeatherProduct.Forecast7Days]
        });

        Assert.That(result.Forecasts, Has.Count.EqualTo(1));
        var fc = result.Forecasts![0];
        Assert.That(fc.Date, Is.EqualTo("2025-01-15"));
        Assert.That(fc.TemperatureHigh, Is.EqualTo(18.0));
        Assert.That(fc.TemperatureLow, Is.EqualTo(5.5));
        Assert.That(fc.Description, Is.EqualTo("Sunny"));
        Assert.That(fc.Icon, Is.EqualTo("sunny"));
        Assert.That(fc.Humidity, Is.EqualTo(45.0));
        Assert.That(fc.WindSpeed, Is.EqualTo(8.0));
        Assert.That(fc.PrecipitationProbability, Is.EqualTo(10.0));
        Assert.That(fc.Comfort, Is.EqualTo(16.5));
    }

    [Test]
    public async Task GetWeatherAsync_CombinedObservationAndForecast()
    {
        var json = """
        {
            "observations": [
                {
                    "temperature": 12.0,
                    "description": "Cloudy"
                }
            ],
            "dailyForecasts": [
                {
                    "forecasts": [
                        {
                            "utcTime": "2025-01-16",
                            "highTemperature": 15.0,
                            "lowTemperature": 3.0
                        }
                    ]
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GetWeatherAsync(new WeatherRequest
        {
            Location = new LatLngLiteral(52.5, 13.4),
            Products = [WeatherProduct.Observation, WeatherProduct.Forecast7Days]
        });

        Assert.That(result.Observations, Has.Count.EqualTo(1));
        Assert.That(result.Observations![0].Temperature, Is.EqualTo(12.0));
        Assert.That(result.Forecasts, Has.Count.EqualTo(1));
        Assert.That(result.Forecasts![0].TemperatureHigh, Is.EqualTo(15.0));
    }

    [Test]
    public async Task GetWeatherAsync_NullNumericFields_MapsToNull()
    {
        var json = """
        {
            "observations": [
                {
                    "description": "Foggy",
                    "iconName": "fog"
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GetWeatherAsync(new WeatherRequest
        {
            Location = new LatLngLiteral(52.5, 13.4)
        });

        Assert.That(result.Observations, Has.Count.EqualTo(1));
        var obs = result.Observations![0];
        Assert.That(obs.Temperature, Is.Null);
        Assert.That(obs.Humidity, Is.Null);
        Assert.That(obs.WindSpeed, Is.Null);
        Assert.That(obs.Description, Is.EqualTo("Foggy"));
        Assert.That(obs.Icon, Is.EqualTo("fog"));
    }

    [Test]
    public async Task GetWeatherAsync_EmptyResponse_ReturnsEmptyLists()
    {
        var handler = MockHttpHandler.WithJson("""{}""");
        var service = CreateService(handler);

        var result = await service.GetWeatherAsync(new WeatherRequest
        {
            Location = new LatLngLiteral(52.5, 13.4)
        });

        Assert.That(result.Observations, Is.Not.Null);
        Assert.That(result.Observations, Is.Empty);
        Assert.That(result.Forecasts, Is.Not.Null);
        Assert.That(result.Forecasts, Is.Empty);
    }

    [Test]
    public async Task GetWeatherAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = await Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.GetWeatherAsync(new WeatherRequest
            {
                Location = new LatLngLiteral(52.5, 13.4)
            }));
        Assert.That(ex!.Service, Is.EqualTo("weather"));
    }
}
