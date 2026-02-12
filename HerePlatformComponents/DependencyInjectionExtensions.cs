using Microsoft.Extensions.DependencyInjection;

namespace HerePlatformComponents;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBlazorHerePlatform(this IServiceCollection services, string apiKey)
    {
        services.AddScoped<IBlazorHerePlatformKeyService>(_ => new BlazorHerePlatformKeyService(apiKey));
        return services;
    }

    public static IServiceCollection AddBlazorHerePlatform(this IServiceCollection services, Maps.HereApiLoadOptions opts)
    {
        services.AddScoped<IBlazorHerePlatformKeyService>(_ => new BlazorHerePlatformKeyService(opts));
        return services;
    }

    public static IServiceCollection AddBlazorHerePlatform(this IServiceCollection services, IBlazorHerePlatformKeyService keyService)
    {
        services.AddScoped(_ => keyService);
        return services;
    }
}
