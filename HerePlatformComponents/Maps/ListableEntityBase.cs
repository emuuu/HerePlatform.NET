using HerePlatformComponents.Maps.Extension;
using Microsoft.JSInterop;
using OneOf;
using System;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Base class for map objects that can be added to/removed from a map.
/// Unlike Google Maps (which uses setMap(map)/getMap()), HERE uses map.addObject(obj)/map.removeObject(obj).
/// </summary>
public class ListableEntityBase<TEntityOptions> : EventEntityBase, IJsObjectRef
    where TEntityOptions : IListableEntityOptionsBase
{
    public Guid Guid => _jsObjectRef.Guid;

    internal ListableEntityBase(JsObjectRef jsObjectRef) : base(jsObjectRef)
    {
    }

    /// <summary>
    /// Adds this object to a map. HERE equivalent of Google's setMap().
    /// </summary>
    public virtual Task AddToMap(Map map)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.addObjectToMap",
            map.Guid.ToString(), _jsObjectRef.Guid.ToString()).AsTask();
    }

    /// <summary>
    /// Removes this object from a map.
    /// </summary>
    public virtual Task RemoveFromMap(Map map)
    {
        return _jsObjectRef.JSRuntime.InvokeVoidAsync(
            "blazorHerePlatform.objectManager.removeObjectFromMap",
            map.Guid.ToString(), _jsObjectRef.Guid.ToString()).AsTask();
    }

    public Task InvokeAsync(string functionName, params object[] args)
    {
        return _jsObjectRef.InvokeAsync(functionName, args);
    }

    public Task<T> InvokeAsync<T>(string functionName, params object[] args)
    {
        return _jsObjectRef.InvokeAsync<T>(functionName, args);
    }

    public Task<OneOf<T, U>> InvokeAsync<T, U>(string functionName, params object[] args)
    {
        return _jsObjectRef.InvokeAsync<T, U>(functionName, args);
    }

    public Task<OneOf<T, U, V>> InvokeAsync<T, U, V>(string functionName, params object[] args)
    {
        return _jsObjectRef.InvokeAsync<T, U, V>(functionName, args);
    }

    /// <summary>
    /// Sets arbitrary data on this map object (H.map.Object.setData).
    /// </summary>
    public Task SetData(object data)
    {
        return _jsObjectRef.InvokeAsync("setData", data);
    }

    /// <summary>
    /// Gets the arbitrary data attached to this map object (H.map.Object.getData).
    /// </summary>
    public Task<T> GetData<T>()
    {
        return _jsObjectRef.InvokeAsync<T>("getData");
    }

    /// <summary>
    /// Sets the z-index for this map object (H.map.Object.setZIndex).
    /// </summary>
    public Task SetZIndex(int? zIndex)
    {
        return _jsObjectRef.InvokeAsync("setZIndex", zIndex);
    }

    /// <summary>
    /// Gets the z-index of this map object (H.map.Object.getZIndex).
    /// </summary>
    public Task<int?> GetZIndex()
    {
        return _jsObjectRef.InvokeAsync<int?>("getZIndex");
    }

    /// <summary>
    /// Sets the visibility of this map object (H.map.Object.setVisibility).
    /// </summary>
    public Task SetVisibility(bool visible)
    {
        return _jsObjectRef.InvokeAsync("setVisibility", visible);
    }

    /// <summary>
    /// Gets the visibility of this map object (H.map.Object.getVisibility).
    /// </summary>
    /// <param name="optEffective">If true, returns the effective visibility considering parent objects.</param>
    public Task<bool> GetVisibility(bool? optEffective = null)
    {
        if (optEffective.HasValue)
            return _jsObjectRef.InvokeAsync<bool>("getVisibility", optEffective.Value);
        return _jsObjectRef.InvokeAsync<bool>("getVisibility");
    }

    /// <summary>
    /// Sets volatility of this map object (H.map.Object.setVolatility).
    /// Volatile objects may change appearance frequently, informing the rendering engine.
    /// </summary>
    public Task SetVolatility(bool @volatile)
    {
        return _jsObjectRef.InvokeAsync("setVolatility", @volatile);
    }

    /// <summary>
    /// Gets the volatility flag of this map object (H.map.Object.getVolatility).
    /// </summary>
    /// <param name="optEffective">If true, returns the effective volatility considering parent objects.</param>
    public Task<bool> GetVolatility(bool? optEffective = null)
    {
        if (optEffective.HasValue)
            return _jsObjectRef.InvokeAsync<bool>("getVolatility", optEffective.Value);
        return _jsObjectRef.InvokeAsync<bool>("getVolatility");
    }
}
