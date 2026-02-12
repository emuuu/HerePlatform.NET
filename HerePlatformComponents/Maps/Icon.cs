using HerePlatformComponents.Maps.Coordinates;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.map.Icon (bitmap or SVG icon for markers).
/// </summary>
public class Icon : IJsObjectRef, IAsyncDisposable
{
    private readonly JsObjectRef _jsObjectRef;
    public Guid Guid => _jsObjectRef.Guid;

    public static async Task<Icon> CreateAsync(IJSRuntime jsRuntime, string bitmapOrSvg, object? options = null)
    {
        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.map.Icon", bitmapOrSvg, options);
        return new Icon(jsObjectRef);
    }

    private Icon(JsObjectRef jsObjectRef)
    {
        _jsObjectRef = jsObjectRef;
    }

    /// <summary>
    /// Gets the size of the icon (H.map.Icon.getSize).
    /// </summary>
    public Task<Point> GetSize()
    {
        return _jsObjectRef.InvokeAsync<Point>("getSize");
    }

    /// <summary>
    /// Gets the loading state of the icon (H.map.Icon.getState).
    /// </summary>
    public Task<string> GetState()
    {
        return _jsObjectRef.InvokeAsync<string>("getState");
    }

    /// <summary>
    /// Gets the anchor point of the icon (H.map.Icon.getAnchor).
    /// </summary>
    public Task<Point> GetAnchor()
    {
        return _jsObjectRef.InvokeAsync<Point>("getAnchor");
    }

    public async ValueTask DisposeAsync()
    {
        await _jsObjectRef.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
