---
title: InfoBubbleComponent
category: Components
order: 9
description: "Info bubble popup for markers and map locations."
apiRef: InfoBubbleComponent
demo: map-infobubble
---

## Basic Usage

`InfoBubbleComponent` renders an `H.ui.InfoBubble` popup at a given geographic position. Place it inside an `AdvancedHereMap` and control its visibility with the `IsOpen` parameter.

```xml
<AdvancedHereMap Options="@options">
    <InfoBubbleComponent Lat="52.52" Lng="13.405" IsOpen="true">
        <p>Hello from Berlin!</p>
    </InfoBubbleComponent>
</AdvancedHereMap>

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };
}
```

Parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Lat` | `double` | `0` | Latitude of the anchor position. |
| `Lng` | `double` | `0` | Longitude of the anchor position. |
| `IsOpen` | `bool` | `false` | Whether the bubble is visible. Two-way bindable via `@bind-IsOpen`. |
| `AutoPan` | `bool` | `true` | Whether the map pans to show the bubble when opened. |

## Custom Content

The `ChildContent` render fragment provides the HTML content for the bubble. The content is captured as static HTML.

```xml
<InfoBubbleComponent Lat="48.8566" Lng="2.3522" IsOpen="@showBubble">
    <div style="min-width: 200px;">
        <h4 style="margin: 0 0 8px 0;">Eiffel Tower</h4>
        <p style="margin: 0;">Iconic iron lattice tower on the Champ de Mars.</p>
        <a href="https://www.toureiffel.paris" target="_blank">Visit website</a>
    </div>
</InfoBubbleComponent>
```

Use `@bind-IsOpen` to toggle the bubble and react when the user closes it by clicking the X button:

```xml
<InfoBubbleComponent Lat="52.52" Lng="13.405" @bind-IsOpen="showBubble">
    <p>This bubble can be closed.</p>
</InfoBubbleComponent>

<button @onclick="() => showBubble = !showBubble">
    Toggle Bubble
</button>

@code {
    private bool showBubble = true;
}
```

When the user clicks the close button on the bubble, `IsOpen` is set to `false` and the `IsOpenChanged` callback fires.

## Positioning

The InfoBubble anchors to the geographic coordinate specified by `Lat` and `Lng`. When `AutoPan` is `true` (the default), the map automatically scrolls to ensure the bubble is visible within the viewport.

Disable auto-panning for cases where you do not want the map to shift:

```xml
<InfoBubbleComponent Lat="40.7128" Lng="-74.0060"
                     IsOpen="true"
                     AutoPan="false">
    <p>New York City</p>
</InfoBubbleComponent>
```

Open an InfoBubble in response to a marker click:

```xml
<AdvancedHereMap Options="@options">
    <MarkerComponent Lat="52.52" Lng="13.405"
                     Clickable="true"
                     OnClick="@HandleMarkerClick" />

    <InfoBubbleComponent Lat="52.52" Lng="13.405" @bind-IsOpen="showInfo">
        <h4>Berlin</h4>
        <p>Capital of Germany</p>
    </InfoBubbleComponent>
</AdvancedHereMap>

@code {
    private bool showInfo = false;
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };

    private void HandleMarkerClick(MapPointerEventArgs e)
    {
        showInfo = true;
    }
}
```

You can also use `MarkerComponent`'s built-in InfoBubble support by providing `ChildContent` directly to the marker, which is simpler for single-marker popups. Use the standalone `InfoBubbleComponent` when you need independent control over visibility, positioning, or want to share a bubble across multiple triggers.
