using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Extension;

public abstract class EventEntityBase
{
    protected readonly JsObjectRef _jsObjectRef;
    private readonly Dictionary<string, List<MapEventListener>> EventListeners;
    private bool _isDisposed;

    protected void AddEvent(string eventName, MapEventListener listener)
    {
        if (!EventListeners.TryGetValue(eventName, out var collection))
        {
            collection = new List<MapEventListener>();
            EventListeners.Add(eventName, collection);
        }

        collection.Add(listener);
    }

    protected EventEntityBase(JsObjectRef jsObjectRef)
    {
        _jsObjectRef = jsObjectRef;
        EventListeners = new Dictionary<string, List<MapEventListener>>();
    }

    /// <summary>
    /// Adds an event listener. HERE uses addEventListener instead of addListener.
    /// </summary>
    public async Task<MapEventListener> AddListener(string eventName, Action handler)
    {
        var listenerRef = await _jsObjectRef.InvokeWithReturnedObjectRefAsync(
            "addEventListener", eventName, handler);

        var eventListener = new MapEventListener(listenerRef);
        AddEvent(eventName, eventListener);
        return eventListener;
    }

    /// <summary>
    /// Adds a typed event listener.
    /// </summary>
    public async Task<MapEventListener> AddListener<T>(string eventName, Action<T> handler)
    {
        var listenerRef = await _jsObjectRef.InvokeWithReturnedObjectRefAsync(
            "addEventListener", eventName, handler);

        var eventListener = new MapEventListener(listenerRef);
        AddEvent(eventName, eventListener);
        return eventListener;
    }

    /// <summary>
    /// Adds an event listener that fires only once.
    /// HERE does not have addListenerOnce natively - the JS objectManager wraps this.
    /// </summary>
    public async Task<MapEventListener> AddListenerOnce(string eventName, Action handler)
    {
        var listenerRef = await _jsObjectRef.InvokeWithReturnedObjectRefAsync(
            "addEventListenerOnce", eventName, handler);

        var eventListener = new MapEventListener(listenerRef);
        AddEvent(eventName, eventListener);
        return eventListener;
    }

    public async Task<MapEventListener> AddListenerOnce<T>(string eventName, Action<T> handler)
    {
        var listenerRef = await _jsObjectRef.InvokeWithReturnedObjectRefAsync(
            "addEventListenerOnce", eventName, handler);

        var eventListener = new MapEventListener(listenerRef);
        AddEvent(eventName, eventListener);
        return eventListener;
    }

    public async Task ClearListeners(string eventName)
    {
        if (EventListeners.TryGetValue(eventName, out var listeners))
        {
            foreach (var listener in listeners.Where(listener => !listener.IsRemoved))
            {
                await listener.RemoveAsync();
            }

            EventListeners[eventName].Clear();
        }
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        foreach (var eventListener in EventListeners.SelectMany(listener => listener.Value))
        {
            if (eventListener.IsRemoved)
            {
                continue;
            }

            await eventListener.DisposeAsync();
        }

        EventListeners.Clear();
        await _jsObjectRef.DisposeAsync();
    }
}
