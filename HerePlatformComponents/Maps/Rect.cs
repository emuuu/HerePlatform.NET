using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps.Coordinates;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.map.Rect.
/// </summary>
public class Rect : ListableEntityBase<RectOptions>
{
    public static async Task<Rect> CreateAsync(IJSRuntime jsRuntime, RectOptions? opts = null)
    {
        var bounds = opts?.Bounds ?? new GeoRect(0, 0, 0, 0);
        var style = opts?.Style;
        var jsOptions = new { style };

        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.map.Rect", bounds, jsOptions);
        var obj = new Rect(jsObjectRef);
        return obj;
    }

    internal Rect(JsObjectRef jsObjectRef)
        : base(jsObjectRef)
    {
    }

    public Task SetBounds(GeoRect bounds)
    {
        return _jsObjectRef.InvokeAsync("setBoundingBox", bounds);
    }

    public Task<GeoRect> GetBounds()
    {
        return _jsObjectRef.InvokeAsync<GeoRect>("getBoundingBox");
    }

    public Task SetStyle(StyleOptions style)
    {
        return _jsObjectRef.InvokeAsync("setStyle", style);
    }

    public Task<StyleOptions> GetStyle()
    {
        return _jsObjectRef.InvokeAsync<StyleOptions>("getStyle");
    }

    /// <summary>
    /// Gets the elevation of this rectangle in meters (H.map.Rect.getElevation).
    /// </summary>
    public Task<double> GetElevation()
    {
        return _jsObjectRef.InvokeAsync<double>("getElevation");
    }

    /// <summary>
    /// Gets the extrusion height of this rectangle in meters (H.map.Rect.getExtrusion).
    /// </summary>
    public Task<double> GetExtrusion()
    {
        return _jsObjectRef.InvokeAsync<double>("getExtrusion");
    }
}
