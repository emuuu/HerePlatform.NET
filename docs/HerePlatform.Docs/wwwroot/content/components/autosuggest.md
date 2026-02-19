---
title: HereAutosuggest
category: Components
order: 19
description: "Address and place autosuggest search component."
apiRef: HereAutosuggest
demo: map-autosuggest
---

## Basic Usage

`HereAutosuggest` is a standalone search input with autocomplete powered by the HERE Autosuggest API. It does not require an `AdvancedHereMap` parent -- it can be used anywhere on the page.

```csharp
@using HerePlatformComponents.Maps.Search
@using HerePlatform.Core.Search

<HereAutosuggest Placeholder="Search for an address..."
                 OnItemSelected="HandleSelection" />

@code {
    private void HandleSelection(AutosuggestItem item)
    {
        Console.WriteLine($"Selected: {item.Title}");
        Console.WriteLine($"Position: {item.Position?.Lat}, {item.Position?.Lng}");
        Console.WriteLine($"Address: {item.Address?.Label}");
    }
}
```

The component initializes the HERE platform automatically if it has not been loaded yet.

## Customization (AutosuggestOptions, AutosuggestDesign)

Configure search behavior with `AutosuggestOptions` and visual style with `AutosuggestDesign`.

```csharp
<HereAutosuggest Placeholder="Search..."
                 Options="_options"
                 Design="AutosuggestDesign.Rounded"
                 MinChars="2"
                 DebounceMs="200"
                 OnItemSelected="HandleSelection" />

@code {
    private AutosuggestOptions _options = new()
    {
        Limit = 8,
        Lang = "en",
        In = "countryCode:USA",
        At = new LatLngLiteral(40.7128, -74.006)
    };
}
```

### AutosuggestDesign Variants

| Variant | Description |
|---------|-------------|
| `Default` | Standard Bootstrap input with title and address in dropdown items. |
| `Compact` | Smaller font, reduced padding, title-only items. |
| `Filled` | Gray background, borderless until focused. |
| `Outlined` | Thicker border with accent color on focus. |
| `Rounded` | Pill-shaped input with rounded dropdown corners. |

### Component Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Placeholder` | `string?` | `null` | Input placeholder text. |
| `CssClass` | `string?` | `null` | Additional CSS class on the wrapper div. |
| `Options` | `AutosuggestOptions?` | `null` | Search configuration (limit, language, country, bias). |
| `Design` | `AutosuggestDesign` | `Default` | Visual design variant. |
| `Disabled` | `bool` | `false` | Disables the input. |
| `Value` | `string?` | `null` | Current input text (two-way bindable via `@bind-Value`). |
| `MinChars` | `int` | `3` | Minimum characters before triggering search. |
| `DebounceMs` | `int` | `300` | Debounce delay in milliseconds. |

## Templates

Override the rendering of the input, individual items, or the entire dropdown list.

**SuggestionItemTemplate** -- replaces the content inside each `<li>`:

```csharp
<HereAutosuggest OnItemSelected="HandleSelection">
    <SuggestionItemTemplate>
        <strong>@context.Title</strong>
        <br />
        <small>@context.Address?.City, @context.Address?.CountryCode</small>
    </SuggestionItemTemplate>
</HereAutosuggest>
```

**SuggestionListTemplate** -- replaces the entire dropdown. Use `context.SelectItem` to wire up selection:

```csharp
<HereAutosuggest OnItemSelected="HandleSelection">
    <SuggestionListTemplate>
        <div class="custom-dropdown">
            @foreach (var item in context.Items)
            {
                <div @onmousedown="() => context.SelectItem(item)"
                     @onmousedown:preventDefault>
                    @item.Title
                </div>
            }
        </div>
    </SuggestionListTemplate>
</HereAutosuggest>
```

**InputTemplate** -- replaces the input element. Splat `context.InputAttributes` onto your custom input:

```csharp
<HereAutosuggest OnItemSelected="HandleSelection">
    <InputTemplate>
        <input class="my-input" @attributes="context.InputAttributes" />
    </InputTemplate>
</HereAutosuggest>
```

## Events

| Event | Type | Description |
|-------|------|-------------|
| `OnItemSelected` | `EventCallback<AutosuggestItem>` | Fires when a suggestion is selected. |
| `OnCleared` | `EventCallback` | Fires when the input is cleared. |
| `ValueChanged` | `EventCallback<string>` | Two-way binding callback for `Value`. |
