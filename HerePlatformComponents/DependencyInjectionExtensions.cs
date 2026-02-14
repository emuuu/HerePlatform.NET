using HerePlatformComponents.Maps.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HerePlatformComponents;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBlazorHerePlatform(this IServiceCollection services, string apiKey)
    {
        services.AddScoped<IBlazorHerePlatformKeyService>(_ => new BlazorHerePlatformKeyService(apiKey));
        RegisterServices(services);
        return services;
    }

    public static IServiceCollection AddBlazorHerePlatform(this IServiceCollection services, Maps.HereApiLoadOptions opts)
    {
        services.AddScoped<IBlazorHerePlatformKeyService>(_ => new BlazorHerePlatformKeyService(opts));
        RegisterServices(services);
        return services;
    }

    public static IServiceCollection AddBlazorHerePlatform(this IServiceCollection services, IBlazorHerePlatformKeyService keyService)
    {
        services.AddSingleton<IBlazorHerePlatformKeyService>(keyService);
        RegisterServices(services);
        return services;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IRoutingService, RoutingService>();
        services.AddScoped<IGeocodingService, GeocodingService>();
        services.AddScoped<ITrafficService, TrafficService>();
        services.AddScoped<IPublicTransitService, PublicTransitService>();
        services.AddScoped<IWaypointSequenceService, WaypointSequenceService>();
        services.AddScoped<IGeofencingService, GeofencingService>();
        services.AddScoped<IPlacesService, PlacesService>();
        services.AddScoped<IIsolineService, IsolineService>();
        services.AddScoped<IMatrixRoutingService, MatrixRoutingService>();
    }
}
