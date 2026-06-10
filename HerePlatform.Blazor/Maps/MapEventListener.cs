using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HerePlatform.Blazor.Maps;

public class MapEventListener : IJsObjectRef, IDisposable, IAsyncDisposable
{
    private readonly JsObjectRef _jsObjectRef;
    public bool IsRemoved { get; internal set; }
    private bool _isDisposed;

    internal MapEventListener(JsObjectRef jsObjectRef)
    {
        _jsObjectRef = jsObjectRef;
    }

    public Guid Guid => _jsObjectRef.Guid;

    public async Task RemoveAsync()
    {
        if (IsRemoved) return;

        try { await _jsObjectRef.InvokeAsync("remove"); }
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }

        try { await _jsObjectRef.DisposeAsync(); }
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }

        IsRemoved = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;

        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore() => await RemoveAsync();

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
            }

            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
