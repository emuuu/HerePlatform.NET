---
title: Places Service
category: Services
order: 5
description: "Search for places and points of interest."
apiRef: IPlacesService
---

## Search

`IPlacesService` provides three search methods corresponding to the HERE Places API:

- **DiscoverAsync** -- free-text search for places.
- **BrowseAsync** -- browse places by category near a position.
- **LookupAsync** -- look up a single place by HERE Place ID.

```csharp
@inject IPlacesService PlacesService
```

```csharp
// Free-text search
var result = await PlacesService.DiscoverAsync(new PlacesRequest
{
    Query = "coffee",
    At = new LatLngLiteral(52.52, 13.405),
    Limit = 10
});

foreach (var item in result.Items ?? [])
{
    Console.WriteLine($"{item.Title} ({item.Distance}m)");
    Console.WriteLine($"Address: {item.Address}");
    Console.WriteLine($"Categories: {string.Join(", ", item.Categories ?? [])}");
}

// Browse by category
var restaurants = await PlacesService.BrowseAsync(new PlacesRequest
{
    At = new LatLngLiteral(52.52, 13.405),
    Categories = ["100-1000-0000"],
    Limit = 20
});

// Lookup by Place ID
var detail = await PlacesService.LookupAsync(new PlacesRequest
{
    Id = "here:pds:place:276u33db-abcdef"
});
```

## PlacesRequest

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Query` | `string?` | `null` | Free-text search query (used by `DiscoverAsync`). |
| `At` | `LatLngLiteral?` | `null` | Center position for proximity search. |
| `BoundingBox` | `string?` | `null` | Bounding box filter: `"south,west,north,east"`. |
| `Categories` | `List<string>?` | `null` | Category filter IDs. |
| `Limit` | `int` | `20` | Maximum number of results. |
| `Lang` | `string?` | `null` | Language code (e.g. `"en"`, `"de"`). |
| `Id` | `string?` | `null` | HERE Place ID (used by `LookupAsync`). |

## PlacesResult

Each `PlaceItem` in the result contains:

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string?` | Display name of the place. |
| `Position` | `LatLngLiteral?` | Geographic coordinates. |
| `Address` | `string?` | Formatted address. |
| `Categories` | `List<string>?` | Category names. |
| `OpeningHours` | `string?` | Opening hours text. |
| `Contacts` | `List<PlaceContact>?` | Contact info (phone, website, email). |
| `Distance` | `int?` | Distance from search center in meters. |
| `PlaceId` | `string?` | HERE Place ID for subsequent `LookupAsync` calls. |

```csharp
var place = result.Items?.FirstOrDefault();
if (place?.Contacts is { } contacts)
{
    foreach (var contact in contacts)
    {
        Console.WriteLine($"{contact.Type}: {contact.Value}");
    }
}
```
