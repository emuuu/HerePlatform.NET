using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Wrapper for H.map.DomIcon (DOM-based icon for DomMarker).
/// </summary>
public class DomIcon : IJsObjectRef
{
    private readonly JsObjectRef _jsObjectRef;
    public Guid Guid => _jsObjectRef.Guid;

    public static async Task<DomIcon> CreateAsync(IJSRuntime jsRuntime, string html, object? options = null)
    {
        var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "H.map.DomIcon", html, options);
        return new DomIcon(jsObjectRef);
    }

    private DomIcon(JsObjectRef jsObjectRef)
    {
        _jsObjectRef = jsObjectRef;
    }
}
