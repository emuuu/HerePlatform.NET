using HerePlatformComponents.Maps.Extension;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.ui.InfoBubble.
/// Unlike Google's InfoWindow, HERE InfoBubble requires a UI instance to open/close.
/// </summary>
public class InfoBubble : EventEntityBase, IJsObjectRef
{
    private new readonly JsObjectRef _jsObjectRef;
    public Guid Guid => _jsObjectRef.Guid;

    public static async Task<InfoBubble> CreateAsync(IJSRuntime jsRuntime, LatLngLiteral position, InfoBubbleOptions? opts = null)
    {
        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.ui.InfoBubble", position, opts);
        var obj = new InfoBubble(jsObjectRef);
        return obj;
    }

    private InfoBubble(JsObjectRef jsObjectRef) : base(jsObjectRef)
    {
        _jsObjectRef = jsObjectRef;
    }

    /// <summary>
    /// Opens the InfoBubble on the given map by adding it to the UI instance.
    /// </summary>
    public Task Open(Map map)
    {
        if (map.UIGuid == null)
            throw new InvalidOperationException("Map was created without UI support. Set EnableUI = true in MapOptions.");

        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.addInfoBubble",
            map.UIGuid.Value.ToString(), _jsObjectRef.Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Closes the InfoBubble by removing it from the UI instance.
    /// </summary>
    public Task Close(Map map)
    {
        if (map.UIGuid == null) return Task.CompletedTask;

        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.removeInfoBubble",
            map.UIGuid.Value.ToString(), _jsObjectRef.Guid.ToString()).AsTask();
    }

    public Task SetContent(string content)
    {
        return _jsObjectRef.InvokeAsync("setContent", content);
    }

    public Task SetPosition(LatLngLiteral position)
    {
        return _jsObjectRef.InvokeAsync("setPosition", position);
    }

    /// <summary>
    /// Gets the state of the InfoBubble (H.ui.InfoBubble.getState): "open" or "closed".
    /// </summary>
    public Task<string> GetState()
    {
        return _jsObjectRef.InvokeAsync<string>("getState");
    }

    /// <summary>
    /// Gets the current position of the InfoBubble (H.ui.InfoBubble.getPosition).
    /// </summary>
    public Task<LatLngLiteral> GetPosition()
    {
        return _jsObjectRef.InvokeAsync<LatLngLiteral>("getPosition");
    }

    public override void Dispose()
    {
        base.Dispose();
        _jsObjectRef.Dispose();
    }
}
