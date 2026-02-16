---
title: Installation
category: Getting Started
order: 1
description: Install the BlazorHerePlatform NuGet package and add it to your Blazor Server or WebAssembly project.
---

## NuGet Package

Install the `BlazorHerePlatform` package from NuGet:

```xml
<PackageReference Include="BlazorHerePlatform" Version="1.0.0" />
```

Or via the .NET CLI:

```
dotnet add package BlazorHerePlatform
```

## Supported Frameworks

BlazorHerePlatform multi-targets three .NET versions:

| Framework | Status |
|-----------|--------|
| .NET 8.0  | Supported |
| .NET 9.0  | Supported |
| .NET 10.0 | Supported (LTS) |

Your project only needs to target one of these. The correct dependency set is selected automatically based on your `<TargetFramework>`.

## Blazor Server Projects

Add the package reference, then register services in `Program.cs`:

```csharp
using HerePlatformComponents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorHerePlatform("YOUR_API_KEY");

var app = builder.Build();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

For large data responses (routing, GeoJSON), increase the SignalR message size limit:

```csharp
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 512 * 1024; // 512 KB
});
```

## Blazor WebAssembly Projects

Add the package reference and register services in the WASM `Program.cs`:

```csharp
using HerePlatformComponents;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorHerePlatform("YOUR_API_KEY");

await builder.Build().RunAsync();
```

## Namespace Imports

For convenience, add these to your `_Imports.razor`:

```csharp
@using HerePlatformComponents
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.Coordinates
```

## JavaScript

All required JavaScript is loaded automatically via a [Blazor JavaScript initializer](https://learn.microsoft.com/aspnet/core/blazor/fundamentals/startup#javascript-initializers). No manual `<script>` tag is needed.

## What Gets Registered

Calling `AddBlazorHerePlatform` registers the following scoped services automatically:

- `IBlazorHerePlatformKeyService` -- manages the HERE API key and load state
- `IRoutingService`, `IGeocodingService`, `ITrafficService`
- `IPublicTransitService`, `IWaypointSequenceService`, `IGeofencingService`
- `IPlacesService`, `IIsolineService`, `IMatrixRoutingService`

No additional service registration is required. All JS interop is handled internally by the components.
