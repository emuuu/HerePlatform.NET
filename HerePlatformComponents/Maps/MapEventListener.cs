using System;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public class MapEventListener : IJsObjectRef, IDisposable, IAsyncDisposable
{
    private readonly JsObjectRef _jsObjectRef;
    public bool IsRemoved;
    private bool _isDisposed;

    internal MapEventListener(JsObjectRef jsObjectRef)
    {
        _jsObjectRef = jsObjectRef;
    }

    public Guid Guid => _jsObjectRef.Guid;

    public async Task RemoveAsync()
    {
        await _jsObjectRef.InvokeAsync("remove");
        await _jsObjectRef.DisposeAsync();
        IsRemoved = true;
    }

    public async ValueTask DisposeAsync()
    {
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
