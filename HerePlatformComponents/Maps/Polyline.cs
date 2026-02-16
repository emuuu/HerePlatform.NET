using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps.Coordinates;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.map.Polyline.
/// </summary>
public class Polyline : ListableEntityBase<PolylineOptions>
{
    public static async Task<Polyline> CreateAsync(IJSRuntime jsRuntime, PolylineOptions? opts = null)
    {
        var path = opts?.Path ?? new List<LatLngLiteral>();
        var style = opts?.Style;
        var jsOptions = new { style };

        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.map.Polyline", path, jsOptions);
        var obj = new Polyline(jsObjectRef);
        return obj;
    }

    internal Polyline(JsObjectRef jsObjectRef)
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
}
