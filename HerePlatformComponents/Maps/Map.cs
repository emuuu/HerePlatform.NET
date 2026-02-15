using HerePlatformComponents.Maps.Coordinates;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.Map - the HERE Maps core map object.
/// </summary>
public class Map : EventEntityBase, IJsObjectRef, IAsyncDisposable
{
    public Guid Guid => _jsObjectRef.Guid;

    /// <summary>
    /// GUID of the H.mapevents.Behavior instance (for interaction control).
    /// </summary>
    public Guid? BehaviorGuid { get; private set; }

    /// <summary>
    /// GUID of the H.ui.UI instance (for InfoBubble management).
    /// </summary>
    public Guid? UIGuid { get; private set; }

    public static async Task<Map> CreateAsync(
        IJSRuntime jsRuntime,
        ElementReference mapDiv,
        MapOptions? opts = null)
    {
        if (opts?.ApiLoadOptions != null)
        {
            HereApiLoadOptions apiOpts = opts.ApiLoadOptions;
            await jsRuntime.InvokeVoidAsync(JsInteropIdentifiers.InitMap, apiOpts);
        }

        var result = await jsRuntime.InvokeAsync<MapInitResult>(
            JsInteropIdentifiers.CreateHereMap,
            mapDiv, opts);

        if (!Guid.TryParse(result.MapGuid, out var mapGuid))
            throw new InvalidOperationException($"JS returned invalid map GUID: '{result.MapGuid}'");
        var jsObjectRef = new JsObjectRef(jsRuntime, mapGuid);
        var map = new Map(jsObjectRef);

        if (result.BehaviorGuid != null)
        {
            if (!Guid.TryParse(result.BehaviorGuid, out var behaviorGuid))
                throw new InvalidOperationException($"JS returned invalid behavior GUID: '{result.BehaviorGuid}'");
            map.BehaviorGuid = behaviorGuid;
        }
        if (result.UIGuid != null)
        {
            if (!Guid.TryParse(result.UIGuid, out var uiGuid))
                throw new InvalidOperationException($"JS returned invalid UI GUID: '{result.UIGuid}'");
            map.UIGuid = uiGuid;
        }

        JsObjectRefInstances.Add(map);

        return map;
    }

    private Map(JsObjectRef jsObjectRef) : base(jsObjectRef)
    {
    }

    /// <summary>
    /// Sets the map center (H.Map.setCenter).
    /// </summary>
    /// <param name="latLng">The new center position.</param>
    /// <param name="animate">Whether to animate the transition.</param>
    public Task SetCenter(LatLngLiteral latLng, bool animate = false)
    {
        return _jsObjectRef.InvokeAsync("setCenter", latLng, animate);
    }

    public Task<LatLngLiteral> GetCenter()
    {
        return _jsObjectRef.InvokeAsync<LatLngLiteral>("getCenter");
    }

    /// <summary>
    /// Sets the map zoom level (H.Map.setZoom).
    /// </summary>
    /// <param name="zoom">The new zoom level.</param>
    /// <param name="animate">Whether to animate the transition.</param>
    public Task SetZoom(double zoom, bool animate = false)
    {
        return _jsObjectRef.InvokeAsync("setZoom", zoom, animate);
    }

    public Task<double> GetZoom()
    {
        return _jsObjectRef.InvokeAsync<double>("getZoom");
    }

    public Task<GeoRect> GetViewBounds()
    {
        return _jsObjectRef.JSRuntime.InvokeAsync<GeoRect>(
            JsInteropIdentifiers.GetViewBounds, Guid.ToString()).AsTask();
    }

    public Task SetViewBounds(GeoRect bounds)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.SetViewBounds,
            Guid.ToString(), bounds).AsTask();
    }

    /// <summary>
    /// Adds a map object (marker, polyline, etc.) to this map.
    /// </summary>
    public Task AddObject(IJsObjectRef mapObject)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.AddObjectToMap,
            _jsObjectRef.Guid.ToString(), mapObject.Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Removes a map object from this map.
    /// </summary>
    public Task RemoveObject(IJsObjectRef mapObject)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.RemoveObjectFromMap,
            _jsObjectRef.Guid.ToString(), mapObject.Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Adds multiple map objects at once.
    /// </summary>
    public Task AddObjects(IEnumerable<IJsObjectRef> mapObjects)
    {
        var guids = new List<string>();
        foreach (var obj in mapObjects)
            guids.Add(obj.Guid.ToString());

        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.AddObjectsToMap,
            _jsObjectRef.Guid.ToString(), guids.ToArray()).AsTask();
    }

    /// <summary>
    /// Removes multiple map objects at once.
    /// </summary>
    public Task RemoveObjects(IEnumerable<IJsObjectRef> mapObjects)
    {
        var guids = new List<string>();
        foreach (var obj in mapObjects)
            guids.Add(obj.Guid.ToString());

        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.RemoveObjectsFromMap,
            _jsObjectRef.Guid.ToString(), guids.ToArray()).AsTask();
    }

    /// <summary>
    /// Removes all objects from this map.
    /// </summary>
    public async Task RemoveAllObjects()
    {
        var objects = await _jsObjectRef.InvokeAsync<object>("getObjects");
        await _jsObjectRef.InvokeAsync("removeObjects", objects);
    }

    /// <summary>
    /// Changes the base layer of the map (e.g. vector, satellite, terrain).
    /// </summary>
    public Task SetBaseLayer(MapLayerType layerType)
    {
        var layerPath = Helper.GetEnumMemberValue(layerType);
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.SetBaseLayer,
            Guid.ToString(), layerPath).AsTask();
    }

    public Task SetTilt(double tilt)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.SetMapLookAt,
            Guid.ToString(), new { tilt }).AsTask();
    }

    public async Task<double> GetTilt()
    {
        var data = await _jsObjectRef.JSRuntime.InvokeAsync<LookAtData>(
            JsInteropIdentifiers.GetMapLookAt,
            Guid.ToString());
        return data.Tilt;
    }

    public Task SetHeading(double heading)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.SetMapLookAt,
            Guid.ToString(), new { heading }).AsTask();
    }

    public async Task<double> GetHeading()
    {
        var data = await _jsObjectRef.JSRuntime.InvokeAsync<LookAtData>(
            JsInteropIdentifiers.GetMapLookAt,
            Guid.ToString());
        return data.Heading;
    }

    /// <summary>
    /// Converts a geographic point to screen coordinates (H.Map.geoToScreen).
    /// </summary>
    public Task<Point> GeoToScreen(LatLngLiteral geoPoint)
    {
        return _jsObjectRef.InvokeAsync<Point>("geoToScreen", geoPoint);
    }

    /// <summary>
    /// Converts screen coordinates to a geographic point (H.Map.screenToGeo).
    /// </summary>
    public Task<GeoPoint> ScreenToGeo(double x, double y)
    {
        return _jsObjectRef.InvokeAsync<GeoPoint>("screenToGeo", x, y);
    }

    /// <summary>
    /// Zooms at a specific screen position (H.Map.zoomAt).
    /// </summary>
    public Task ZoomAt(double zoom, double x, double y)
    {
        return _jsObjectRef.InvokeAsync("zoomAt", zoom, x, y);
    }

    /// <summary>
    /// Gets the base layer of the map (H.Map.getBaseLayer).
    /// </summary>
    public Task<JsObjectRef> GetBaseLayer()
    {
        return _jsObjectRef.InvokeWithReturnedObjectRefAsync("getBaseLayer");
    }

    public Task Resize()
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.ResizeMap, Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Enables specified behavior features on the map.
    /// </summary>
    public Task EnableBehavior(BehaviorFeature features)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.SetBehaviorFeatures,
            Guid.ToString(), (int)features, true).AsTask();
    }

    /// <summary>
    /// Disables specified behavior features on the map.
    /// </summary>
    public Task DisableBehavior(BehaviorFeature features)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.SetBehaviorFeatures,
            Guid.ToString(), (int)features, false).AsTask();
    }

    /// <summary>
    /// Captures the current map view as a Base64 data URI (PNG).
    /// </summary>
    public Task<string> CaptureAsync()
    {
        return _jsObjectRef.JSRuntime.InvokeAsync<string>(
            JsInteropIdentifiers.CaptureMap,
            Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Sets the viewport padding (H.map.ViewPort.setPadding).
    /// </summary>
    public Task SetViewportPadding(int top, int right, int bottom, int left)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.SetViewportPadding,
            Guid.ToString(), top, right, bottom, left).AsTask();
    }

    /// <summary>
    /// Adjusts the viewport to fit the given points.
    /// </summary>
    /// <param name="points">Points to fit in the viewport.</param>
    /// <param name="animate">Whether to animate the transition.</param>
    public Task ZoomToBoundsAsync(List<LatLngLiteral> points, bool animate = true)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.ZoomToBounds,
            Guid.ToString(), points, animate, null).AsTask();
    }

    /// <summary>
    /// Exports all map objects as a GeoJSON FeatureCollection string.
    /// </summary>
    public Task<string> ToGeoJsonAsync()
    {
        return _jsObjectRef.JSRuntime.InvokeAsync<string>(
            JsInteropIdentifiers.ExportMapGeoJson,
            Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Sets the minimum zoom level at runtime.
    /// </summary>
    public Task SetMinZoom(double minZoom)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.SetMinZoom,
            Guid.ToString(), minZoom).AsTask();
    }

    /// <summary>
    /// Sets the maximum zoom level at runtime.
    /// </summary>
    public Task SetMaxZoom(double maxZoom)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.SetMaxZoom,
            Guid.ToString(), maxZoom).AsTask();
    }

    /// <summary>
    /// Gets the current minimum zoom level.
    /// </summary>
    public Task<double> GetMinZoom()
    {
        return _jsObjectRef.JSRuntime.InvokeAsync<double>(
            JsInteropIdentifiers.GetMinZoom,
            Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Gets the current maximum zoom level.
    /// </summary>
    public Task<double> GetMaxZoom()
    {
        return _jsObjectRef.JSRuntime.InvokeAsync<double>(
            JsInteropIdentifiers.GetMaxZoom,
            Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Adds an overlay layer by path (e.g. "vector.traffic.map").
    /// </summary>
    public Task AddLayerAsync(string layerPath)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.AddOverlayLayer,
            Guid.ToString(), layerPath).AsTask();
    }

    /// <summary>
    /// Removes an overlay layer by path.
    /// </summary>
    public Task RemoveLayerAsync(string layerPath)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            JsInteropIdentifiers.RemoveOverlayLayer,
            Guid.ToString(), layerPath).AsTask();
    }

    private record LookAtData(double Tilt = 0, double Heading = 0);

    protected override async ValueTask DisposeAsyncCore()
    {
        try
        {
            await _jsObjectRef.JSRuntime.InvokeAsync<object>(
                JsInteropIdentifiers.DisposeMap, Guid.ToString());
        }
        catch (JSDisconnectedException) { /* Expected during circuit disconnect */ }
        catch (InvalidOperationException) { /* Expected: JS runtime may be unavailable */ }

        JsObjectRefInstances.Remove(_jsObjectRef.Guid.ToString());
        await base.DisposeAsyncCore();
    }
}
