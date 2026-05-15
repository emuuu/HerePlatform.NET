using Bunit;
using HerePlatform.Blazor.Maps;
using HerePlatform.Blazor.Maps.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using NUnit.Framework;

namespace HerePlatform.Blazor.Tests.Maps;

/// <summary>
/// Verifies that the Map/MapComponent dispose paths swallow JSDisconnectedException,
/// matching the Microsoft guidance for components that issue JSInterop calls in Dispose.
/// </summary>
public class CircuitDisconnectDisposeTests : BunitTestBase
{
    [Test]
    public async Task AdvancedHereMap_DisposeAsync_SwallowsJSDisconnectedException()
    {
        var cut = Render<AdvancedHereMap>(p => p.Add(x => x.Id, "tear-down-map"));

        SimulateCircuitDisconnect(new JSDisconnectedException("circuit gone"));

        Assert.DoesNotThrowAsync(async () => await cut.Instance.DisposeAsync());
    }

    [Test]
    public async Task AdvancedHereMap_DisposeAsync_SwallowsOperationCanceledException()
    {
        var cut = Render<AdvancedHereMap>(p => p.Add(x => x.Id, "cancelled-map"));

        SimulateCircuitDisconnect(new OperationCanceledException());

        Assert.DoesNotThrowAsync(async () => await cut.Instance.DisposeAsync());
    }

    [Test]
    public async Task AdvancedHereMap_WithMarkerChild_DisposeAsync_SwallowsJSDisconnect()
    {
        // Bestückte Map: Marker als ChildContent. Beim DisposeAsync läuft sowohl
        // MapObjectComponentBase.DisposeAsync (Marker) als auch Map.DisposeAsyncCore
        // → EventEntityBase.DisposeAsyncCore → MapEventListener.RemoveAsync —
        // also alle drei zuvor gefixten Pfade.
        RenderFragment childContent = builder =>
        {
            builder.OpenComponent<MarkerComponent>(0);
            builder.AddAttribute(1, nameof(MarkerComponent.Lat), 52.5);
            builder.AddAttribute(2, nameof(MarkerComponent.Lng), 13.4);
            builder.CloseComponent();
        };

        var cut = Render<AdvancedHereMap>(p => p
            .Add(x => x.Id, "loaded-map")
            .Add(x => x.ChildContent, childContent));

        SimulateCircuitDisconnect(new JSDisconnectedException("circuit gone"));

        Assert.DoesNotThrowAsync(async () => await cut.Instance.DisposeAsync());
    }

    [Test]
    public async Task AdvancedHereMap_WithZoomRectangleUi_DisposeAsync_SwallowsJSDisconnect()
    {
        // UI-Aux-Komponente: registriert sich beim Parent. AdvancedHereMap.DisposeAsync
        // ruft MarkDisposed() → die ZoomRectangle.DisposeAsync skippt ihren JS-Call.
        RenderFragment childContent = builder =>
        {
            builder.OpenComponent<ZoomRectangleComponent>(0);
            builder.AddAttribute(1, nameof(ZoomRectangleComponent.Active), true);
            builder.CloseComponent();
        };

        var cut = Render<AdvancedHereMap>(p => p
            .Add(x => x.Id, "ui-map")
            .Add(x => x.ChildContent, childContent));

        SimulateCircuitDisconnect(new JSDisconnectedException("circuit gone"));

        Assert.DoesNotThrowAsync(async () => await cut.Instance.DisposeAsync());
    }

    private void SimulateCircuitDisconnect(System.Exception ex)
    {
        Context.JSInterop.Mode = JSRuntimeMode.Strict;
        Context.JSInterop.SetupVoid().SetException(ex);
        Context.JSInterop.Setup<object>(_ => true).SetException(ex);
        Context.JSInterop.Setup<string>(_ => true).SetException(ex);
    }
}
