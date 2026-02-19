using System.Text.Json;
using System.Web;

namespace HerePlatform.Docs.Generator;

public class StaticHtmlGenerator
{
    private readonly string _baseUrl;

    private static readonly Dictionary<string, (string Title, string Description)> DemoPages = new()
    {
        ["map-basic"] = ("Basic Map", "Render a HERE Map with different base layers and locale settings."),
        ["map-controls"] = ("Map Controls", "Programmatic map manipulation with live state feedback."),
        ["map-enhancements"] = ("Map Enhancements", "Advanced map features: animations, behavior control, capture, overlays, and context menus."),
        ["map-markers"] = ("Markers", "Add, click, and drag markers with two-way binding and InfoBubble content."),
        ["map-shapes"] = ("Shapes", "Declarative Polyline, Circle, and Rectangle components with custom styles."),
        ["map-infobubble"] = ("InfoBubble", "Open, update, and close InfoBubbles programmatically with HTML content."),
        ["map-groups"] = ("Groups & DomMarkers", "Group markers for batch operations, plus DomMarker and standalone InfoBubble."),
        ["map-clustering"] = ("Clustering", "Cluster large marker sets for performance with configurable radius and weight."),
        ["map-data"] = ("Data Import", "Load GeoJSON and KML data onto the map from inline text or remote URLs."),
        ["map-heatmap"] = ("Heatmap", "Visualize data intensity with a configurable heatmap overlay layer."),
        ["map-traffic"] = ("Traffic", "View real-time traffic incidents with severity markers and detail popups."),
        ["map-routing"] = ("Routing", "Calculate routes with multiple transport modes, isoline analysis, and turn-by-turn instructions."),
        ["map-geocoding"] = ("Geocoding & Places", "Forward and reverse geocoding, plus Places API for POI discovery."),
        ["map-autosuggest"] = ("Autosuggest", "Address autocomplete with customizable design variants and templates."),
        ["map-matrix-routing"] = ("Matrix Routing", "Calculate travel time and distance matrices between multiple origins and destinations."),
        ["map-ui-controls"] = ("UI Controls", "Built-in map tools for measuring distances, overview navigation, and rectangle zoom."),
        ["map-utilities"] = ("Utilities", "WKT parsing, GeoJSON export, Flexible Polyline codec, and custom tile layers.")
    };

    public StaticHtmlGenerator(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task GenerateAsync(string wwwrootPath, List<ContentIndexEntry> entries)
    {
        var count = 0;

        // Generate doc pages
        foreach (var entry in entries)
        {
            var dir = Path.Combine(wwwrootPath, "docs", entry.Slug);
            Directory.CreateDirectory(dir);
            var html = BuildDocPage(entry);
            await File.WriteAllTextAsync(Path.Combine(dir, "index.html"), html);
            count++;
        }

        Console.WriteLine($"  Generated {count} static doc pages");

        // Generate demo pages
        var demoCount = 0;
        foreach (var (slug, (title, description)) in DemoPages)
        {
            var dir = Path.Combine(wwwrootPath, "demo", slug);
            Directory.CreateDirectory(dir);
            var html = BuildDemoPage(slug, title, description);
            await File.WriteAllTextAsync(Path.Combine(dir, "index.html"), html);
            demoCount++;
        }

        Console.WriteLine($"  Generated {demoCount} static demo pages");

        // Inject meta tags into index.html and 404.html
        await InjectMetaTags(wwwrootPath);
    }

    private string BuildDocPage(ContentIndexEntry entry)
    {
        var title = HttpUtility.HtmlEncode(entry.Title);
        var description = HttpUtility.HtmlEncode(entry.Description);
        var url = $"{_baseUrl}/docs/{entry.Slug}";
        var jsonLd = BuildJsonLd(entry);

        return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>{title} - HerePlatform.NET Docs</title>
                <base href="/" />
                <meta name="description" content="{description}" />
                <link rel="canonical" href="{url}" />
                <meta property="og:type" content="article" />
                <meta property="og:title" content="{title} - HerePlatform.NET" />
                <meta property="og:description" content="{description}" />
                <meta property="og:url" content="{url}" />
                <meta property="og:site_name" content="HerePlatform.NET Docs" />
                <meta property="og:image" content="{_baseUrl}/img/here-platform.svg" />
                <meta name="twitter:card" content="summary" />
                <meta name="twitter:title" content="{title} - HerePlatform.NET" />
                <meta name="twitter:description" content="{description}" />
                <script type="application/ld+json">{jsonLd}</script>
                <link rel="icon" type="image/svg+xml" href="img/here-platform.svg" />
                <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"
                      integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous" />
                <link href="lib/prismjs/prism.css" rel="stylesheet" />
                <link href="css/app.css" rel="stylesheet" />
                <link href="HerePlatform.Docs.styles.css" rel="stylesheet" />
            </head>
            <body>
                <div id="app">
                    <article style="max-width:800px;margin:2rem auto;padding:0 1rem;">
                        <h1>{title}</h1>
                        <p class="lead text-muted">{description}</p>
                        {entry.HtmlContent}
                    </article>
                </div>
                <div id="blazor-error-ui">
                    An unhandled error has occurred.
                    <a href="" class="reload">Reload</a>
                    <a class="dismiss">X</a>
                </div>
                <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"
                        integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
                <script src="lib/prismjs/prism.js"></script>
                <script src="js/docs.js"></script>
                <script src="_content/BlazorHerePlatform/js/objectManager.js"></script>
                <script src="_framework/blazor.webassembly.js"></script>
            </body>
            </html>
            """;
    }

    private string BuildDemoPage(string slug, string title, string description)
    {
        var encodedTitle = HttpUtility.HtmlEncode(title);
        var encodedDesc = HttpUtility.HtmlEncode(description);
        var url = $"{_baseUrl}/demo/{slug}";

        return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>{encodedTitle} Demo - HerePlatform.NET</title>
                <base href="/" />
                <meta name="description" content="{encodedDesc}" />
                <link rel="canonical" href="{url}" />
                <meta property="og:type" content="website" />
                <meta property="og:title" content="{encodedTitle} Demo - HerePlatform.NET" />
                <meta property="og:description" content="{encodedDesc}" />
                <meta property="og:url" content="{url}" />
                <meta property="og:site_name" content="HerePlatform.NET Docs" />
                <meta property="og:image" content="{_baseUrl}/img/here-platform.svg" />
                <meta name="twitter:card" content="summary" />
                <meta name="twitter:title" content="{encodedTitle} Demo - HerePlatform.NET" />
                <meta name="twitter:description" content="{encodedDesc}" />
                <link rel="icon" type="image/svg+xml" href="img/here-platform.svg" />
                <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"
                      integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous" />
                <link href="lib/prismjs/prism.css" rel="stylesheet" />
                <link href="css/app.css" rel="stylesheet" />
                <link href="HerePlatform.Docs.styles.css" rel="stylesheet" />
            </head>
            <body>
                <div id="app">
                    <div class="loading-screen d-flex align-items-center justify-content-center vh-100">
                        <div class="text-center">
                            <div class="spinner-border mb-3" role="status" style="color: #48a9a6;">
                                <span class="visually-hidden">Loading...</span>
                            </div>
                            <p class="text-muted">Loading {encodedTitle} Demo...</p>
                        </div>
                    </div>
                    <noscript>
                        <p style="text-align:center;margin-top:2rem;">This interactive demo requires JavaScript to run.</p>
                    </noscript>
                </div>
                <div id="blazor-error-ui">
                    An unhandled error has occurred.
                    <a href="" class="reload">Reload</a>
                    <a class="dismiss">X</a>
                </div>
                <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"
                        integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
                <script src="lib/prismjs/prism.js"></script>
                <script src="js/docs.js"></script>
                <script src="_content/BlazorHerePlatform/js/objectManager.js"></script>
                <script src="_framework/blazor.webassembly.js"></script>
            </body>
            </html>
            """;
    }

    private string BuildJsonLd(ContentIndexEntry entry)
    {
        var url = $"{_baseUrl}/docs/{entry.Slug}";
        var slugParts = entry.Slug.Split('/');
        var breadcrumbs = new List<object>
        {
            new { @type = "ListItem", position = 1, name = "Docs", item = $"{_baseUrl}/" }
        };

        if (slugParts.Length > 1)
        {
            breadcrumbs.Add(new { @type = "ListItem", position = 2, name = entry.Category, item = $"{_baseUrl}/docs/{slugParts[0]}" });
            breadcrumbs.Add(new { @type = "ListItem", position = 3, name = entry.Title, item = url });
        }
        else
        {
            breadcrumbs.Add(new { @type = "ListItem", position = 2, name = entry.Title, item = url });
        }

        var graph = new object[]
        {
            new
            {
                @context = "https://schema.org",
                @type = "TechArticle",
                headline = entry.Title,
                description = entry.Description,
                url,
                image = $"{_baseUrl}/img/here-platform.svg",
                publisher = new { @type = "Organization", name = "HerePlatform.NET" }
            },
            new
            {
                @context = "https://schema.org",
                @type = "BreadcrumbList",
                itemListElement = breadcrumbs
            }
        };

        return JsonSerializer.Serialize(graph, new JsonSerializerOptions { WriteIndented = false });
    }

    private async Task InjectMetaTags(string wwwrootPath)
    {
        const string metaTags = """

                <meta name="description" content="Blazor components and services for the HERE Maps JavaScript API. Build interactive maps with declarative Razor syntax." />
                <meta property="og:type" content="website" />
                <meta property="og:title" content="HerePlatform.NET Docs" />
                <meta property="og:description" content="Blazor components and services for the HERE Maps JavaScript API. Build interactive maps with declarative Razor syntax." />
                <meta property="og:site_name" content="HerePlatform.NET Docs" />
                <meta property="og:image" content="{0}/img/here-platform.svg" />
                <meta name="twitter:card" content="summary" />
                <meta name="twitter:title" content="HerePlatform.NET Docs" />
                <meta name="twitter:description" content="Blazor components and services for the HERE Maps JavaScript API." />
            """;

        var formattedTags = metaTags.Replace("{0}", _baseUrl);

        foreach (var fileName in new[] { "index.html", "404.html" })
        {
            var filePath = Path.Combine(wwwrootPath, fileName);
            if (!File.Exists(filePath)) continue;

            var content = await File.ReadAllTextAsync(filePath);

            // Skip if already injected
            if (content.Contains("og:title")) continue;

            // Inject after viewport meta tag
            const string marker = """<meta name="viewport" content="width=device-width, initial-scale=1.0" />""";
            content = content.Replace(marker, marker + formattedTags);

            await File.WriteAllTextAsync(filePath, content);
            Console.WriteLine($"  Injected meta tags into {fileName}");
        }
    }
}
