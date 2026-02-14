using Bunit;
using HerePlatformComponents.Maps;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HerePlatformComponents.Tests;

public abstract class BunitTestBase : IDisposable
{
    protected BunitContext Context { get; private set; } = default!;

    protected BunitJSInterop JSInterop => Context.JSInterop;

    [SetUp]
    public void BaseSetup()
    {
        Context = new BunitContext();
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        Context.Services.AddSingleton<IBlazorHerePlatformKeyService>(
            new BlazorHerePlatformKeyService("test-api-key"));

        // Map.CreateAsync expects a MapInitResult from JS — provide a valid one.
        // Must use InvocationMatcher to accept any arguments (mapDiv, opts).
        Context.JSInterop
            .Setup<MapInitResult>("blazorHerePlatform.objectManager.createHereMap", _ => true)
            .SetResult(new MapInitResult
            {
                MapGuid = Guid.NewGuid().ToString(),
                BehaviorGuid = Guid.NewGuid().ToString(),
                UIGuid = Guid.NewGuid().ToString()
            });
    }

    protected IRenderedComponent<TComponent> Render<TComponent>(
        Action<ComponentParameterCollectionBuilder<TComponent>>? parameterBuilder = null)
        where TComponent : Microsoft.AspNetCore.Components.IComponent
        => Context.Render<TComponent>(parameterBuilder);

    [TearDown]
    public void BaseTearDown()
    {
        Context?.Dispose();
    }

    public void Dispose()
    {
        Context?.Dispose();
        GC.SuppressFinalize(this);
    }
}
