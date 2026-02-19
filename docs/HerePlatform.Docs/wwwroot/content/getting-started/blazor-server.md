---
title: Blazor Server Notes
category: Getting Started
order: 4
description: Important considerations when using BlazorHerePlatform in Blazor Server apps.
---

## Server vs. WebAssembly

In Blazor WebAssembly, all code runs in the browser. In Blazor Server, C# runs on the server and DOM updates are sent over a SignalR connection. This has several implications for map components.

**JS interop is asynchronous over the wire.** Every call to the HERE Maps JS API crosses the SignalR connection. High-frequency operations (e.g., updating marker positions in a loop) will have noticeable latency compared to WASM.

**Map rendering happens client-side.** The HERE JS API still runs entirely in the browser. Only the Blazor component logic and event handlers execute on the server.

## Scoped Services

All services registered by `AddBlazorHerePlatform` are **scoped**. In Blazor Server, a scope corresponds to a single circuit (user connection). Each user gets their own:

- `IBlazorHerePlatformKeyService`
- `IRoutingService`, `IGeocodingService`, and other service instances

This means `IsApiInitialized` state is tracked per circuit. When a user disconnects and reconnects, a new scope is created and the HERE JS API is re-initialized.

## Shared Key Service Instance

When you pass a custom `IBlazorHerePlatformKeyService` instance directly:

```csharp
var keyService = new BlazorHerePlatformKeyService("YOUR_API_KEY");
builder.Services.AddBlazorHerePlatform(keyService);
```

That **same object** is shared across all circuits. This means:

- `IsApiInitialized` is shared -- one user marking it `true` affects all users.
- `UpdateApiKey` changes the key for every connected user.

If you need per-user key isolation, register a factory instead:

```csharp
builder.Services.AddScoped<IBlazorHerePlatformKeyService>(sp =>
    new BlazorHerePlatformKeyService("YOUR_API_KEY"));
```

Or implement `IBlazorHerePlatformKeyService` with per-user logic and register it as scoped.

## SignalR Message Size

Large JS interop payloads (GeoJSON data, routing responses, cluster data) can exceed the default SignalR message size limit. Increase it in `Program.cs`:

```csharp
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 512 * 1024; // 512 KB
});
```

Adjust the value based on the largest payload your application handles.

## Prerendering

Blazor Server prerendering (`rendermode="ServerPrerendered"`) runs component code on the server before the SignalR circuit is established. During prerendering, `IJSRuntime` is not available, so the map cannot be initialized.

`HereMap` handles this correctly -- it initializes in `OnAfterRenderAsync(firstRender: true)`, which only runs after the circuit is connected. No special action is required.

However, if you access `_map.InteropObject` outside of `OnAfterInit`, guard against `null`:

```csharp
private async Task MoveToLocation(LatLngLiteral position)
{
    if (_map.InteropObject is null)
        return; // Not yet initialized (prerender phase)

    await _map.InteropObject.SetCenterAsync(position);
}
```

## Circuit Disconnection

When a Blazor Server circuit disconnects (browser tab closed, network drop), the component's `DisposeAsync` is called. `MapComponent` handles disposal gracefully, catching `TaskCanceledException` and `ObjectDisposedException` that occur when the JS runtime is no longer reachable.

No special cleanup code is needed in your components. If you hold references to `InteropObject` in your own services, check for null after reconnection.

## Recommendations

- Prefer **Blazor WebAssembly** or **Auto** render mode for map-heavy applications to reduce SignalR overhead.
- Keep JS interop calls batched where possible -- use single operations like `AddMarkersAsync` over looping `AddMarkerAsync`.
- Set `MaximumReceiveMessageSize` based on your data needs.
- Use `OnAfterInit` to safely interact with the map after it is fully initialized.
