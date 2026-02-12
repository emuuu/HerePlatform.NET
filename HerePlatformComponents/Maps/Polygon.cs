using HerePlatformComponents.Maps.Coordinates;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.map.Polygon.
/// </summary>
public class Polygon : ListableEntityBase<PolygonOptions>
{
    public static async Task<Polygon> CreateAsync(IJSRuntime jsRuntime, PolygonOptions? opts = null)
    {
        var path = opts?.Path ?? new List<LatLngLiteral>();
        var style = opts?.Style;
        var jsOptions = new { style };

        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.map.Polygon", path, jsOptions);
        var obj = new Polygon(jsObjectRef);
        return obj;
    }

    internal Polygon(JsObjectRef jsObjectRef)
        : base(jsObjectRef)
    {
    }

    public Task SetStyle(StyleOptions style)
    {
        return _jsObjectRef.InvokeAsync("setStyle", style);
    }

    public Task<StyleOptions> GetStyle()
    {
        return _jsObjectRef.InvokeAsync<StyleOptions>("getStyle");
    }

    public Task<GeoRect> GetBoundingBox()
    {
        return _jsObjectRef.InvokeAsync<GeoRect>("getBoundingBox");
    }

    /// <summary>
    /// Gets the elevation of this polygon in meters (H.map.Polygon.getElevation).
    /// </summary>
    public Task<double> GetElevation()
    {
        return _jsObjectRef.InvokeAsync<double>("getElevation");
    }

    /// <summary>
    /// Gets the extrusion height of this polygon in meters (H.map.Polygon.getExtrusion).
    /// </summary>
    public Task<double> GetExtrusion()
    {
        return _jsObjectRef.InvokeAsync<double>("getExtrusion");
    }
}
