using Microsoft.JSInterop;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatform.Blazor;

/// <summary>
/// Lightweight DTO used for serializing JsObjectRef GUIDs to JavaScript.
/// </summary>
internal class JsObjectRefDto
{
    public string Guid { get; set; }

    public JsObjectRefDto(Guid guid)
    {
        Guid = guid.ToString();
    }
}

public class JsObjectRef : IJsObjectRef
{
    private readonly Guid _guid;
    private readonly IJSRuntime _jsRuntime;
    internal List<IDisposable>? _trackedDisposables;

    public IJSRuntime JSRuntime => _jsRuntime;
    public Guid Guid => _guid;

    [JsonIgnore]
    public Dictionary<string, object>? Properties { get; set; }

    internal JsObjectRef(IJSRuntime jsRuntime, Guid guid)
    {
        _jsRuntime = jsRuntime;
        _guid = guid;
    }

    public static async Task<JsObjectRef> CreateAsync(
        IJSRuntime jsRuntime,
        string constructorName,
        params object?[] args)
    {
        var guid = Guid.NewGuid();
        var jsFriendlyArgs = new object?[] { guid.ToString(), constructorName }
            .Concat(args).ToArray();

        await jsRuntime.MyInvokeAsync<object>(
            JsInteropIdentifiers.CreateObject,
            jsFriendlyArgs);

        return new JsObjectRef(jsRuntime, guid);
    }

    public static async Task<Dictionary<string, JsObjectRef>> CreateMultipleAsync(
        IJSRuntime jsRuntime,
        string functionName,
        Dictionary<Guid, object> dictArgs)
    {
        var jsObjectRefs = dictArgs.ToDictionary(e => e.Key, e => new JsObjectRef(jsRuntime, e.Key));

        await jsRuntime.MyInvokeAsync<object>(
            JsInteropIdentifiers.CreateMultipleObject,
            new object[] { dictArgs.Select(e => e.Key.ToString()).ToList(), functionName }
                .Concat(dictArgs.Values).ToArray()
        );

        return jsObjectRefs.ToDictionary(e => e.Key.ToString(), e => e.Value);
    }

    internal Task AddMultipleAsync(
        string functionName,
        Dictionary<string, object> dictArgs)
    {
        var guidDict = dictArgs.ToDictionary(e => Guid.NewGuid(), e => e.Value);
        return _jsRuntime.MyInvokeAsync<object>(
            JsInteropIdentifiers.CreateMultipleObject,
            new object[] { guidDict.Select(e => e.Key.ToString()).ToList(), functionName }
                .Concat(guidDict.Values).ToArray()
        );
    }

    public virtual void Dispose()
    {
        _ = DisposeGuarded();

        async Task DisposeGuarded()
        {
            try { await DisposeAsync(); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[HerePlatform] Sync dispose fallback failed: {ex.Message}"); }
        }
    }

    public async ValueTask<object> DisposeAsync()
    {
        if (_trackedDisposables != null)
        {
            foreach (var d in _trackedDisposables)
                d.Dispose();
            _trackedDisposables = null;
        }

        try
        {
            return await _jsRuntime.InvokeAsync<object>(
                JsInteropIdentifiers.DisposeObject,
                _guid.ToString()
            );
        }
        catch (JSDisconnectedException) { return default!; }
        catch (OperationCanceledException) { return default!; }
    }

    public ValueTask<object> DisposeMultipleAsync(List<Guid> guids)
    {
        return _jsRuntime.InvokeAsync<object>(
            JsInteropIdentifiers.DisposeMultipleObjects,
            guids.Select(e => e.ToString()).ToList()
        );
    }

    public async Task InvokeAsync(string functionName, params object?[] args)
    {
        _trackedDisposables ??= new List<IDisposable>();
        await _jsRuntime.MyInvokeAsync(
            JsInteropIdentifiers.Invoke,
            _trackedDisposables,
            new object?[] { _guid.ToString(), functionName }
                .Concat(args).ToArray()
        );
    }

    public Task InvokeMultipleAsync(string functionName, Dictionary<Guid, object> dictArgs)
    {
        return _jsRuntime.MyInvokeAsync(
            JsInteropIdentifiers.InvokeMultiple,
            new object[] { dictArgs.Select(e => e.Key.ToString()).ToList(), functionName }
                .Concat(dictArgs.Values).ToArray()
        );
    }

    public Task AddMultipleListenersAsync(string eventName, Dictionary<Guid, object> dictArgs)
    {
        _trackedDisposables ??= new List<IDisposable>();
        return _jsRuntime.MyAddListenerAsync(
            JsInteropIdentifiers.AddMultipleListeners,
            _trackedDisposables,
            new object[] { dictArgs.Select(e => e.Key.ToString()).ToList(), eventName }
                .Concat(dictArgs.Values).ToArray()
        );
    }

    public Task<T> InvokeAsync<T>(string functionName, params object?[] args)
    {
        _trackedDisposables ??= new List<IDisposable>();
        return _jsRuntime.MyInvokeAsync<T>(
            JsInteropIdentifiers.Invoke,
            _trackedDisposables,
            new object?[] { _guid.ToString(), functionName }
                .Concat(args).ToArray()
        )!;
    }

    public Task<OneOf<T, U>> InvokeAsync<T, U>(string functionName, params object[] args)
    {
        _trackedDisposables ??= new List<IDisposable>();
        return _jsRuntime.MyInvokeAsync<T, U>(
            JsInteropIdentifiers.Invoke,
            _trackedDisposables,
            new object[] { _guid.ToString(), functionName }
                .Concat(args).ToArray()
        );
    }

    public Task<OneOf<T, U, V>> InvokeAsync<T, U, V>(string functionName, params object[] args)
    {
        _trackedDisposables ??= new List<IDisposable>();
        return _jsRuntime.MyInvokeAsync<T, U, V>(
            JsInteropIdentifiers.Invoke,
            _trackedDisposables,
            new object[] { _guid.ToString(), functionName }
                .Concat(args).ToArray()
        );
    }

    public Task<Dictionary<string, T>> InvokeMultipleAsync<T>(string functionName, Dictionary<Guid, object> dictArgs)
    {
        return _jsRuntime.MyInvokeAsync<Dictionary<string, T>>(
            JsInteropIdentifiers.InvokeMultiple,
            new object[] { dictArgs.Select(e => e.Key.ToString()).ToList(), functionName }
                .Concat(dictArgs.Values).ToArray()
        )!;
    }

    public async Task<JsObjectRef> InvokeWithReturnedObjectRefAsync(string functionName, params object[] args)
    {
        var guid = await _jsRuntime.MyInvokeAsync<string>(
            JsInteropIdentifiers.InvokeWithReturnedObjectRef,
            new object[] { _guid.ToString(), functionName }
                .Concat(args).ToArray()
        );

        if (!Guid.TryParse(guid, out var parsed))
            throw new InvalidOperationException($"JS returned invalid GUID: '{guid}'");
        return new JsObjectRef(_jsRuntime, parsed);
    }

    public Task<T> GetValue<T>(string propertyName)
    {
        return _jsRuntime.MyInvokeAsync<T>(
            JsInteropIdentifiers.ReadObjectPropertyValue,
            _guid.ToString(),
            propertyName)!;
    }

    public async Task<JsObjectRef> GetObjectReference(string propertyName)
    {
        var guid = await _jsRuntime.MyInvokeAsync<string>(
            JsInteropIdentifiers.ReadObjectPropertyValueWithReturnedObjectRef,
            _guid.ToString(),
            propertyName);

        if (!Guid.TryParse(guid, out var parsed))
            throw new InvalidOperationException($"JS returned invalid GUID: '{guid}'");
        return new JsObjectRef(_jsRuntime, parsed);
    }

    public override bool Equals(object? obj)
    {
        if (obj is JsObjectRef other)
        {
            return other.Guid == this.Guid;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return _guid.GetHashCode();
    }
}
