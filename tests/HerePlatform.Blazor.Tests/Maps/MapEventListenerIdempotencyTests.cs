using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using HerePlatform.Blazor;
using HerePlatform.Blazor.Maps;
using NUnit.Framework;

namespace HerePlatform.Blazor.Tests.Maps;

/// <summary>
/// Reviewer finding #8: MapEventListener.RemoveAsync was not idempotent — a second
/// DisposeAsync call would re-invoke the JS "remove" call. These tests verify the
/// IsRemoved / _isDisposed guards.
/// </summary>
public class MapEventListenerIdempotencyTests : BunitTestBase
{
    [Test]
    public async Task RemoveAsync_CalledTwice_OnlyInvokesJsOnce()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var jsRuntime = (Microsoft.JSInterop.IJSRuntime)Context.Services
            .GetService(typeof(Microsoft.JSInterop.IJSRuntime))!;

        var jsObjectRef = new JsObjectRef(jsRuntime, Guid.NewGuid());
        var listener = new MapEventListener(jsObjectRef);

        await listener.RemoveAsync();
        await listener.RemoveAsync();

        Assert.That(listener.IsRemoved, Is.True);

        // JsObjectRef.InvokeAsync forwards via Helper.MyInvokeAsync, which calls
        // jsRuntime.InvokeVoidAsync(identifier, enumerable). The enumerable goes
        // through as a single Args[0] element. Walk all invocations and count
        // the ones whose flattened arg list contains "remove" as the function name.
        var removeCalls = JSInterop.Invocations
            .Count(inv => FlattenArgs(inv.Arguments).Any(a => a is string s && s == "remove"));

        Assert.That(removeCalls, Is.EqualTo(1),
            "Second RemoveAsync must short-circuit instead of re-invoking 'remove' on JS");
    }

    [Test]
    public async Task DisposeAsync_CalledTwice_DoesNotThrow()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var jsRuntime = (Microsoft.JSInterop.IJSRuntime)Context.Services
            .GetService(typeof(Microsoft.JSInterop.IJSRuntime))!;

        var jsObjectRef = new JsObjectRef(jsRuntime, Guid.NewGuid());
        var listener = new MapEventListener(jsObjectRef);

        await listener.DisposeAsync();
        Assert.DoesNotThrowAsync(async () => await listener.DisposeAsync());
    }

    private static IEnumerable<object?> FlattenArgs(IReadOnlyList<object?> args)
    {
        foreach (var arg in args)
        {
            if (arg is System.Collections.IEnumerable enumerable && arg is not string)
            {
                foreach (var item in enumerable)
                    yield return item;
            }
            else
            {
                yield return arg;
            }
        }
    }
}
