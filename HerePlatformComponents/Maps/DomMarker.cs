using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.map.DomMarker.
/// </summary>
public class DomMarker : ListableEntityBase<DomMarkerOptions>
{
    public static async Task<DomMarker> CreateAsync(IJSRuntime jsRuntime, DomMarkerOptions? opts = null)
    {
        var position = opts?.Position ?? new LatLngLiteral(0, 0);
        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.map.DomMarker", position, opts);
        var obj = new DomMarker(jsObjectRef);
        return obj;
    }

    internal DomMarker(JsObjectRef jsObjectRef)
        : base(jsObjectRef)
    {
    }

    public Task<LatLngLiteral> GetGeometry()
    {
        return _jsObjectRef.InvokeAsync<LatLngLiteral>("getGeometry");
    }

    public Task SetGeometry(LatLngLiteral position)
    {
        return _jsObjectRef.InvokeAsync("setGeometry", position);
    }

    public Task SetIcon(object icon)
    {
        return _jsObjectRef.InvokeAsync("setIcon", icon);
    }
}
