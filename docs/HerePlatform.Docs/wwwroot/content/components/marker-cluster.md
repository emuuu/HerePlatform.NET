---
title: MarkerClusterComponent
category: Components
order: 12
description: "Marker clustering for large datasets."
apiRef: MarkerClusterComponent
demo: map-clustering
---

## Basic Usage

`MarkerClusterComponent` groups nearby markers into clusters when the map is zoomed out. Place it inside an `AdvancedHereMap` and provide a list of `ClusterDataPoint` objects.

Clustering requires the clustering module. Enable it in your API load options:

```csharp
builder.Services.AddBlazorHerePlatform("YOUR_KEY", opts =>
{
    opts.LoadClustering = true;
});
```

```csharp
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.Clustering

<AdvancedHereMap Options="_options" Height="500px">
    <MarkerClusterComponent DataPoints="_points" />
</AdvancedHereMap>

@code {
    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 10
    };

    private List<ClusterDataPoint> _points = new()
    {
        new(52.52, 13.405),
        new(52.53, 13.41),
        new(52.51, 13.39),
        new(52.525, 13.40, weight: 3)
    };
}
```

## Cluster Options

Fine-tune clustering behavior with `Eps` (cluster radius in pixels) and `MinWeight` (minimum weight to form a cluster).

```csharp
<MarkerClusterComponent DataPoints="_points"
                        Eps="64"
                        MinWeight="3"
                        MinZoom="3"
                        MaxZoom="16" />
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `DataPoints` | `List<ClusterDataPoint>?` | `null` | Data points to cluster. |
| `Eps` | `double` | `32` | Cluster radius in pixels (DBSCAN epsilon). |
| `MinWeight` | `int` | `2` | Minimum aggregated weight to form a cluster. |
| `MinZoom` | `double?` | `null` | Minimum zoom level for clustering. |
| `MaxZoom` | `double?` | `null` | Maximum zoom level for clustering. |

Each `ClusterDataPoint` has `Lat`, `Lng`, `Weight` (default `1`), and an optional `Data` property.

## Custom Themes

Customize cluster and noise (single-point) marker appearance with SVG templates. Use `{count}` and `{color}` as placeholders.

```csharp
<MarkerClusterComponent DataPoints="_points"
    ClusterSvgTemplate="@_clusterSvg"
    NoiseSvgTemplate="@_noiseSvg" />

@code {
    private string _clusterSvg = """
        <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40">
            <circle cx="20" cy="20" r="18" fill="{color}" stroke="#fff" stroke-width="2"/>
            <text x="20" y="25" text-anchor="middle" fill="#fff" font-size="12">{count}</text>
        </svg>
        """;

    private string _noiseSvg = """
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20">
            <circle cx="10" cy="10" r="8" fill="#1a73e8" stroke="#fff" stroke-width="2"/>
        </svg>
        """;
}
```

## Events (OnClusterTap)

Handle taps on clusters or individual noise points with `OnClusterTap` and `OnNoiseTap`.

```csharp
<MarkerClusterComponent DataPoints="_points"
                        OnClusterTap="HandleClusterTap"
                        OnNoiseTap="HandleNoiseTap" />

@code {
    private void HandleClusterTap(ClusterTapEventArgs args)
    {
        Console.WriteLine($"Cluster tapped: {args.PointCount} points, weight {args.Weight}");
    }

    private void HandleNoiseTap(ClusterTapEventArgs args)
    {
        Console.WriteLine($"Single point tapped at {args.Position}");
    }
}
```

### ClusterTapEventArgs Properties

| Property | Type | Description |
|----------|------|-------------|
| `Position` | `LatLngLiteral?` | Geographic position of the cluster or point. |
| `Weight` | `int` | Total weight of the cluster (1 for noise). |
| `PointCount` | `int` | Number of data points in the cluster. |
| `IsCluster` | `bool` | `true` for clusters, `false` for noise points. |
