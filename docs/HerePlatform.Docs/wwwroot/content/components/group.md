---
title: GroupComponent
category: Components
order: 10
description: "Group container for organizing map objects."
apiRef: GroupComponent
demo: map-groups
---

## Basic Usage

`GroupComponent` is a container that organizes map objects (markers, polylines, polygons, etc.) into a logical group. Place it inside an `AdvancedHereMap` and nest child components within it.

```csharp
@using HerePlatformComponents.Maps

<AdvancedHereMap Options="_options" Height="500px">
    <GroupComponent @ref="_group">
        <MarkerComponent Position="new LatLngLiteral(52.52, 13.405)" />
        <MarkerComponent Position="new LatLngLiteral(52.53, 13.41)" />
    </GroupComponent>
</AdvancedHereMap>

@code {
    private GroupComponent _group = default!;

    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };
}
```

The group cascades itself as `HereGroup` so child components automatically register with it.

## Nesting

Groups can be nested to create a hierarchy. Each level manages its own visibility and z-index independently.

```csharp
<AdvancedHereMap Options="_options" Height="500px">
    <GroupComponent ZIndex="1">
        <MarkerComponent Position="new LatLngLiteral(52.52, 13.405)" />

        <GroupComponent ZIndex="2">
            <PolylineComponent Path="_path" StrokeColor="#FF0000" />
        </GroupComponent>
    </GroupComponent>
</AdvancedHereMap>
```

Use `GetBoundsAsync()` on a group reference to retrieve the bounding box of all objects it contains:

```csharp
var bounds = await _group.GetBoundsAsync();
```

## Visibility Toggle

Control the visibility of an entire group (and all its children) with the `Visible` parameter. Toggle it dynamically to show or hide sets of map objects at once.

```csharp
<AdvancedHereMap Options="_options" Height="500px">
    <GroupComponent Visible="@_showMarkers">
        <MarkerComponent Position="new LatLngLiteral(48.8566, 2.3522)" />
        <MarkerComponent Position="new LatLngLiteral(48.8584, 2.2945)" />
    </GroupComponent>
</AdvancedHereMap>

<button @onclick="() => _showMarkers = !_showMarkers">
    Toggle Markers
</button>

@code {
    private bool _showMarkers = true;

    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(48.8566, 2.3522),
        Zoom = 13
    };
}
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Visible` | `bool` | `true` | Controls visibility of the group and all children. |
| `ZIndex` | `int?` | `null` | Stacking order for the group. |
| `Data` | `object?` | `null` | Arbitrary data associated with the group. |
| `ChildContent` | `RenderFragment?` | -- | Child map components. |

### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `AddObjectAsync(Guid)` | `Task` | Adds a map object to the group by its GUID. |
| `RemoveObjectAsync(Guid)` | `Task` | Removes a map object from the group by its GUID. |
| `GetBoundsAsync()` | `Task<GeoRect?>` | Returns the bounding box of all objects in the group. |
