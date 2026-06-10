---
title: Autosuggest Service
category: Services
order: 10
description: "Type-ahead address and place suggestions."
apiRef: IAutosuggestService
---

## Setup

Inject `IAutosuggestService` into your component. It is registered automatically by `AddHerePlatformBlazor`.

```csharp
@inject IAutosuggestService AutosuggestService
```

## SuggestAsync

Return rich place and address suggestions as the user types. Results include position coordinates and highlight ranges for matched text.

```csharp
var result = await AutosuggestService.SuggestAsync("Brandenburger", new AutosuggestOptions
{
    Limit = 5,
    Lang = "de",
    In = "countryCode:DEU",
    At = new LatLngLiteral(52.52, 13.405)
});

foreach (var item in result.Items ?? [])
{
    Console.WriteLine($"{item.Title} ({item.ResultType})");
    Console.WriteLine($"  Address: {item.Address?.Label}");
    if (item.Position is { } pos)
        Console.WriteLine($"  Position: {pos.Lat}, {pos.Lng}");
}
```

## AutocompleteAsync

Return lightweight address completions optimized for type-ahead input fields. Faster than `SuggestAsync` but returns fewer details.

```csharp
var result = await AutosuggestService.AutocompleteAsync("Invaliden", new AutosuggestOptions
{
    Limit = 5,
    Lang = "de",
    In = "countryCode:DEU"
});

foreach (var item in result.Items ?? [])
{
    Console.WriteLine($"{item.Title} — {item.Address?.Label}");
}
```

## AutosuggestOptions

Both methods accept an optional `AutosuggestOptions` parameter:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Limit` | `int` | `5` | Maximum number of results. |
| `Lang` | `string?` | `"de"` | Language for results (BCP 47). |
| `In` | `string?` | `"countryCode:DEU"` | Geographic filter (`countryCode:…`, `circle:…`, `bbox:…`). |
| `At` | `LatLngLiteral?` | `51.1657, 10.4515` (center of Germany) | Search context; results near this position rank higher. |

> **Spatial context required:** The HERE Autosuggest API requires exactly one of `at`, `in=circle:…` or `in=bbox:…` per request — `in=countryCode:…` alone is rejected with HTTP 400. `SuggestAsync` therefore throws an `InvalidOperationException` when `At` is `null` and `In` does not contain a `circle:`/`bbox:` expression. Because `at` and `circle`/`bbox` are mutually exclusive, `At` is automatically omitted from the request when `In` provides its own spatial context. The Autocomplete API has no such requirement, so `AutocompleteAsync` accepts a standalone `countryCode:` filter — the default `At` is still sent there as a ranking bias (a supported Autocomplete parameter); set `At = null` to omit it.

## Result Types

`AutosuggestResult` and `AutocompleteResult` both contain a list of `AutosuggestItem`:

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string?` | Display name of the suggestion. |
| `Id` | `string?` | HERE place ID. |
| `ResultType` | `string?` | `"place"`, `"street"`, `"houseNumber"`, `"locality"`, etc. |
| `Address` | `AutosuggestAddress?` | Structured address with Label, City, Street, PostalCode, etc. |
| `Position` | `LatLngLiteral?` | Geographic coordinates (may be null for autocomplete). |
| `Highlights` | `AutosuggestHighlights?` | Matched character ranges for highlighting. |
