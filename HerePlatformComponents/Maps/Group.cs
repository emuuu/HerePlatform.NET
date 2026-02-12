using HerePlatformComponents.Maps.Coordinates;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.map.Group - a container for map objects.
/// </summary>
public class Group : ListableEntityBase<GroupOptions>
{
    public static async Task<Group> CreateAsync(IJSRuntime jsRuntime, GroupOptions? opts = null)
    {
        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.map.Group", opts);
        var obj = new Group(jsObjectRef);
        return obj;
    }

    internal Group(JsObjectRef jsObjectRef)
        : base(jsObjectRef)
    {
    }

    public Task AddObject(IJsObjectRef mapObject)
    {
        return _jsObjectRef.InvokeAsync("addObject", mapObject);
    }

    public Task RemoveObject(IJsObjectRef mapObject)
    {
        return _jsObjectRef.InvokeAsync("removeObject", mapObject);
    }

    /// <summary>
    /// Adds multiple objects to this group at once (H.map.Group.addObjects).
    /// </summary>
    public Task AddObjects(IEnumerable<IJsObjectRef> mapObjects)
    {
        var guids = mapObjects.Select(o => o.Guid.ToString()).ToArray();
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.groupAddObjects",
            _jsObjectRef.Guid.ToString(), guids).AsTask();
    }

    /// <summary>
    /// Removes multiple objects from this group at once (H.map.Group.removeObjects).
    /// </summary>
    public Task RemoveObjects(IEnumerable<IJsObjectRef> mapObjects)
    {
        var guids = mapObjects.Select(o => o.Guid.ToString()).ToArray();
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.groupRemoveObjects",
            _jsObjectRef.Guid.ToString(), guids).AsTask();
    }

    public Task RemoveAll()
    {
        return _jsObjectRef.InvokeAsync("removeAll");
    }

    public Task<GeoRect> GetBoundingBox()
    {
        return _jsObjectRef.InvokeAsync<GeoRect>("getBoundingBox");
    }
}
