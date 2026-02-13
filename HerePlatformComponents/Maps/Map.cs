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

    private bool _isDisposed;

    public static async Task<Map> CreateAsync(
        IJSRuntime jsRuntime,
        ElementReference mapDiv,
        MapOptions? opts = null)
    {
        if (opts?.ApiLoadOptions != null)
        {
            HereApiLoadOptions apiOpts = opts.ApiLoadOptions;
            await jsRuntime.InvokeVoidAsync("blazorHerePlatform.objectManager.initMap", apiOpts);
        }

        var result = await jsRuntime.InvokeAsync<MapInitResult>(
            "blazorHerePlatform.objectManager.createHereMap",
            mapDiv, opts);

        var mapGuid = new Guid(result.MapGuid);
        var jsObjectRef = new JsObjectRef(jsRuntime, mapGuid);
        var map = new Map(jsObjectRef);

        if (result.BehaviorGuid != null)
            map.BehaviorGuid = new Guid(result.BehaviorGuid);
        if (result.UIGuid != null)
            map.UIGuid = new Guid(result.UIGuid);

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
        return _jsObjectRef.InvokeAsync<GeoRect>("getViewModel().getLookAtData().bounds.getBoundingBox");
    }

    public Task SetViewBounds(GeoRect bounds)
    {
        return _jsObjectRef.JSRuntime.MyInvokeAsync(
            "blazorHerePlatform.objectManager.invoke",
            _jsObjectRef.Guid.ToString(), "getViewModel().getLookAtData", bounds);
    }

    /// <summary>
    /// Adds a map object (marker, polyline, etc.) to this map.
    /// </summary>
    public Task AddObject(IJsObjectRef mapObject)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.addObjectToMap",
            _jsObjectRef.Guid.ToString(), mapObject.Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Removes a map object from this map.
    /// </summary>
    public Task RemoveObject(IJsObjectRef mapObject)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.removeObjectFromMap",
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
            "blazorHerePlatform.objectManager.addObjectsToMap",
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
            "blazorHerePlatform.objectManager.removeObjectsFromMap",
            _jsObjectRef.Guid.ToString(), guids.ToArray()).AsTask();
    }

    /// <summary>
    /// Removes all objects from this map.
    /// </summary>
    public Task RemoveAllObjects()
    {
        return _jsObjectRef.InvokeAsync("removeObjects", _jsObjectRef.InvokeAsync<object>("getObjects"));
    }

    /// <summary>
    /// Changes the base layer of the map (e.g. vector, satellite, terrain).
    /// </summary>
    public Task SetBaseLayer(MapLayerType layerType)
    {
        var layerPath = Helper.GetEnumMemberValue(layerType);
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.setBaseLayer",
            Guid.ToString(), layerPath).AsTask();
    }

    public Task SetTilt(double tilt)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.setMapLookAt",
            Guid.ToString(), new { tilt }).AsTask();
    }

    public async Task<double> GetTilt()
    {
        var data = await _jsObjectRef.JSRuntime.InvokeAsync<LookAtData>(
            "blazorHerePlatform.objectManager.getMapLookAt",
            Guid.ToString());
        return data.Tilt;
    }

    public Task SetHeading(double heading)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.setMapLookAt",
            Guid.ToString(), new { heading }).AsTask();
    }

    public async Task<double> GetHeading()
    {
        var data = await _jsObjectRef.JSRuntime.InvokeAsync<LookAtData>(
            "blazorHerePlatform.objectManager.getMapLookAt",
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
            "blazorHerePlatform.objectManager.invoke",
            new object[] { Guid.ToString(), "getViewPort().resize" }).AsTask();
    }

    /// <summary>
    /// Enables specified behavior features on the map.
    /// </summary>
    public Task EnableBehavior(BehaviorFeature features)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.setBehaviorFeatures",
            Guid.ToString(), (int)features, true).AsTask();
    }

    /// <summary>
    /// Disables specified behavior features on the map.
    /// </summary>
    public Task DisableBehavior(BehaviorFeature features)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.setBehaviorFeatures",
            Guid.ToString(), (int)features, false).AsTask();
    }

    /// <summary>
    /// Captures the current map view as a Base64 data URI (PNG).
    /// </summary>
    public Task<string> CaptureAsync()
    {
        return _jsObjectRef.JSRuntime.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.captureMap",
            Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Sets the viewport padding (H.map.ViewPort.setPadding).
    /// </summary>
    public Task SetViewportPadding(int top, int right, int bottom, int left)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.setViewportPadding",
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
            "blazorHerePlatform.objectManager.zoomToBounds",
            Guid.ToString(), points, animate, null).AsTask();
    }

    /// <summary>
    /// Exports all map objects as a GeoJSON FeatureCollection string.
    /// </summary>
    public Task<string> ToGeoJsonAsync()
    {
        return _jsObjectRef.JSRuntime.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.exportMapGeoJson",
            Guid.ToString()).AsTask();
    }

    private record LookAtData(double Tilt = 0, double Heading = 0);

    public override async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await _jsObjectRef.JSRuntime.InvokeAsync<object>(
            "blazorHerePlatform.objectManager.disposeMap", Guid.ToString());
        await base.DisposeAsyncCore();
        JsObjectRefInstances.Remove(_jsObjectRef.Guid.ToString());
    }

    protected override void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            base.Dispose(disposing);

            if (disposing)
            {
            }

            _isDisposed = true;
        }
    }

    public override void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
