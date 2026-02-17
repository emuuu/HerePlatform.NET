using HerePlatform.Core.Services;
using HerePlatform.RestClient.Auth;
using HerePlatform.RestClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HerePlatform.RestClient;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers HERE REST API service implementations for all core service interfaces.
    /// </summary>
    public static IServiceCollection AddHereRestServices(
        this IServiceCollection services, Action<HereRestClientOptions> configure)
    {
        services.Configure(configure);

        // Validate options eagerly on first resolution
        services.AddSingleton<IValidateOptions<HereRestClientOptions>, HereRestClientOptionsValidator>();

        // Register the auth handler as transient (HttpClientFactory manages handler lifetime)
        services.AddTransient<HereAuthHandler>();

        // Plain HttpClient for OAuth token endpoint (no auth handler)
        services.AddHttpClient("HereOAuthToken");

        // Named HttpClient "HereApi" with auth handler + timeout
        services.AddHttpClient("HereApi")
            .AddHttpMessageHandler<HereAuthHandler>()
            .ConfigureHttpClient((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<HereRestClientOptions>>().Value;
                client.Timeout = opts.Timeout;
            });

        // Register all service implementations
        services.AddSingleton<IGeocodingService, RestGeocodingService>();
        services.AddSingleton<IRoutingService, RestRoutingService>();
        services.AddSingleton<IPlacesService, RestPlacesService>();
        services.AddSingleton<IIsolineService, RestIsolineService>();
        services.AddSingleton<IMatrixRoutingService, RestMatrixRoutingService>();
        services.AddSingleton<ITrafficService, RestTrafficService>();
        services.AddSingleton<IPublicTransitService, RestPublicTransitService>();
        services.AddSingleton<IWaypointSequenceService, RestWaypointSequenceService>();
        services.AddSingleton<IGeofencingService, RestGeofencingService>();

        return services;
    }

    private sealed class HereRestClientOptionsValidator : IValidateOptions<HereRestClientOptions>
    {
        public ValidateOptionsResult Validate(string? name, HereRestClientOptions options)
        {
            try
            {
                options.Validate();
                return ValidateOptionsResult.Success;
            }
            catch (InvalidOperationException ex)
            {
                return ValidateOptionsResult.Fail(ex.Message);
            }
        }
    }
}
