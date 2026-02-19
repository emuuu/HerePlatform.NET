---
title: ContextMenuComponent
category: Components
order: 11
description: "Right-click context menu for maps."
apiRef: ContextMenuComponent
---

## Basic Usage

`ContextMenuComponent` adds a right-click context menu to an `AdvancedHereMap`. Define menu entries with `ContextMenuItem` children and handle selections via `OnItemClick`.

```csharp
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.Events

<AdvancedHereMap Options="_options" Height="500px">
    <ContextMenuComponent OnItemClick="HandleMenuClick">
        <ContextMenuItem Label="Add Marker Here" />
        <ContextMenuItem Label="Zoom In" />
        <ContextMenuItem Label="Center Map" />
    </ContextMenuComponent>
</AdvancedHereMap>

@code {
    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };

    private async Task HandleMenuClick(ContextMenuEventArgs args)
    {
        Console.WriteLine($"Clicked: {args.ItemLabel} at {args.Position}");
    }
}
```

## Menu Items (ContextMenuItem)

Each `ContextMenuItem` defines a single entry in the context menu. Items support a label, a disabled state, and custom data.

```csharp
<ContextMenuComponent OnItemClick="HandleMenuClick">
    <ContextMenuItem Label="Add Marker" Data="@("add")" />
    <ContextMenuItem Label="Remove All" Data="@("clear")" />
    <ContextMenuItem Label="Disabled Action" Disabled="true" />
</ContextMenuComponent>
```

### ContextMenuItem Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Label` | `string?` | `null` | Display text for the menu item. |
| `Disabled` | `bool` | `false` | When `true`, the item appears grayed out and is not clickable. |
| `Data` | `object?` | `null` | Custom data attached to this item, available in the event args. |

## Events

The `OnItemClick` callback fires when the user selects a menu item. The `ContextMenuEventArgs` contain the geographic position where the menu was opened and the selected item details.

```csharp
private async Task HandleMenuClick(ContextMenuEventArgs args)
{
    switch (args.ItemData?.ToString())
    {
        case "add":
            // Use args.Position to place a marker at the right-click location
            break;
        case "clear":
            // Clear all markers
            break;
    }
}
```

### ContextMenuEventArgs Properties

| Property | Type | Description |
|----------|------|-------------|
| `Position` | `LatLngLiteral?` | Geographic coordinates where the context menu was opened. |
| `ItemLabel` | `string?` | Label text of the selected menu item. |
| `ItemData` | `object?` | Custom data from the selected `ContextMenuItem`. |
| `ViewportX` | `double` | Viewport X pixel coordinate of the right-click. |
| `ViewportY` | `double` | Viewport Y pixel coordinate of the right-click. |

Only one `ContextMenuComponent` should be placed per map. Adding a second will replace the first handler.
