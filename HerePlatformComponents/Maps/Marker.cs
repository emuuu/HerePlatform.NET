using HerePlatform.Core.Coordinates;
using Microsoft.JSInterop;
using OneOf;
using System;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.map.Marker.
/// </summary>
public class Marker : ListableEntityBase<MarkerOptions>
{
    public static async Task<Marker> CreateAsync(IJSRuntime jsRuntime, MarkerOptions? opts = null)
    {
        var position = opts?.Position ?? new LatLngLiteral(0, 0);
        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.map.Marker", position, opts);
        var obj = new Marker(jsObjectRef);
        return obj;
    }

    internal Marker(JsObjectRef jsObjectRef)
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

    public Task SetIcon(OneOf<Icon, string> icon)
    {
        return _jsObjectRef.InvokeAsync("setIcon", icon);
    }

    /// <summary>
    /// Gets the icon associated with this marker (H.map.Marker.getIcon).
    /// </summary>
    public Task<JsObjectRef> GetIcon()
    {
        return _jsObjectRef.InvokeWithReturnedObjectRefAsync("getIcon");
    }

    /// <summary>
    /// Gets the draggable state of the marker.
    /// </summary>
    public Task<bool> IsDraggable()
    {
        return _jsObjectRef.GetValue<bool>("draggable");
    }

    /// <summary>
    /// Sets the draggable state of the marker.
    /// </summary>
    public Task SetDraggable(bool draggable)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.WriteObjectPropertyValue,
            _jsObjectRef.Guid.ToString(), "draggable", draggable).AsTask();
    }
}
