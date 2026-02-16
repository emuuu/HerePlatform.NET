using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps.Coordinates;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.map.Circle.
/// </summary>
public class Circle : ListableEntityBase<CircleOptions>
{
    public static async Task<Circle> CreateAsync(IJSRuntime jsRuntime, CircleOptions? opts = null)
    {
        var center = opts?.Center ?? new LatLngLiteral(0, 0);
        var radius = opts?.Radius ?? 1000;
        var style = opts?.Style;
        var jsOptions = new { style };

        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.map.Circle", center, radius, jsOptions);
        var obj = new Circle(jsObjectRef);
        return obj;
    }

    internal Circle(JsObjectRef jsObjectRef)
        : base(jsObjectRef)
    {
    }

    public Task SetCenter(LatLngLiteral center)
    {
        return _jsObjectRef.InvokeAsync("setCenter", center);
    }

    public Task<LatLngLiteral> GetCenter()
    {
        return _jsObjectRef.InvokeAsync<LatLngLiteral>("getCenter");
    }

    public Task SetRadius(double radius)
    {
        return _jsObjectRef.InvokeAsync("setRadius", radius);
    }

    public Task<double> GetRadius()
    {
        return _jsObjectRef.InvokeAsync<double>("getRadius");
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
    /// Gets the precision (number of segments) of this circle (H.map.Circle.getPrecision).
    /// </summary>
    public Task<double> GetPrecision()
    {
        return _jsObjectRef.InvokeAsync<double>("getPrecision");
    }

    /// <summary>
    /// Gets the elevation of this circle in meters (H.map.Circle.getElevation).
    /// </summary>
    public Task<double> GetElevation()
    {
        return _jsObjectRef.InvokeAsync<double>("getElevation");
    }

    /// <summary>
    /// Gets the extrusion height of this circle in meters (H.map.Circle.getExtrusion).
    /// </summary>
    public Task<double> GetExtrusion()
    {
        return _jsObjectRef.InvokeAsync<double>("getExtrusion");
    }

    /// <summary>
    /// Gets the bounding box of this circle (H.map.Circle.getBoundingBox).
    /// </summary>
    public Task<GeoRect> GetBoundingBox()
    {
        return _jsObjectRef.InvokeAsync<GeoRect>("getBoundingBox");
    }
}
