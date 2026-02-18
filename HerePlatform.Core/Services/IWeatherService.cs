using HerePlatform.Core.Weather;

namespace HerePlatform.Core.Services;

/// <summary>
/// Weather observations and forecasts via the HERE Destination Weather API v3.
/// </summary>
[HereApi("Destination Weather API", "v3")]
public interface IWeatherService
{
    /// <summary>
    /// Get weather observations and/or forecasts for a location.
    /// </summary>
    /// <param name="request">Location, products, and optional language.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<WeatherResult> GetWeatherAsync(WeatherRequest request, CancellationToken cancellationToken = default);
}
