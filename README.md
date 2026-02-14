# BlazorHerePlatform

Blazor components for the [HERE Maps JS API](https://developer.here.com/develop/javascript-api). Provides declarative map, marker, polygon, polyline, circle, clustering, routing, and more as Blazor components.

## Installation

```bash
dotnet add package BlazorHerePlatform
```

## Quick Start

1. Register the service in `Program.cs`:

```csharp
builder.Services.AddBlazorHerePlatform("YOUR_HERE_API_KEY");
```

2. Add the map component:

```razor
<AdvancedHereMap Id="myMap" Height="400px" Options="@(new MapOptions())">
    <MarkerComponent Lat="52.52" Lng="13.405" />
</AdvancedHereMap>
```

## Limitations

- **Single API key per page**: The HERE Maps JS API is loaded once per page with a single
  API key. Multiple keys on the same page are not supported.

## License

MIT
