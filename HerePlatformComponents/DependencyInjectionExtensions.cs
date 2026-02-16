using System;
using HerePlatform.Core.Services;
using HerePlatformComponents.Maps.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HerePlatformComponents;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers BlazorHerePlatform services. Only one API key per page is supported.
    /// </summary>
    public static IServiceCollection AddBlazorHerePlatform(this IServiceCollection services, string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        services.AddScoped<IBlazorHerePlatformKeyService>(_ => new BlazorHerePlatformKeyService(apiKey));
        RegisterServices(services);
        return services;
    }

    public static IServiceCollection AddBlazorHerePlatform(this IServiceCollection services, Maps.HereApiLoadOptions opts)
    {
        ArgumentNullException.ThrowIfNull(opts);
        services.AddScoped<IBlazorHerePlatformKeyService>(_ => new BlazorHerePlatformKeyService(opts));
        RegisterServices(services);
        return services;
    }

    /// <summary>
    /// Registers BlazorHerePlatform services with a custom key service instance.
    /// <para>
    /// <strong>Warning (Blazor Server):</strong> The provided instance is shared across
    /// all scopes (circuits). Mutable state like <see cref="IBlazorHerePlatformKeyService.IsApiInitialized"/>
    /// will leak between users. Prefer the <c>AddBlazorHerePlatform(string apiKey)</c> or
    /// <c>AddBlazorHerePlatform(HereApiLoadOptions)</c> overloads for per-circuit isolation.
    /// </para>
    /// </summary>
    public static IServiceCollection AddBlazorHerePlatform(this IServiceCollection services, IBlazorHerePlatformKeyService keyService)
    {
        ArgumentNullException.ThrowIfNull(keyService);
        services.AddScoped<IBlazorHerePlatformKeyService>(_ => keyService);
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
