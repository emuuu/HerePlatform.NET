using System.Text;

namespace HerePlatform.Docs.Generator;

public class SitemapGenerator
{
    private readonly string _baseUrl;

    private static readonly string[] DemoSlugs =
    [
        "map-basic", "map-controls", "map-enhancements",
        "map-markers", "map-shapes", "map-infobubble", "map-groups",
        "map-clustering", "map-data", "map-heatmap", "map-traffic",
        "map-routing", "map-geocoding", "map-autosuggest",
        "map-matrix-routing", "map-ui-controls", "map-utilities"
    ];

    private static readonly string[] RestApiSlugs =
    [
        "geocoding", "routing", "places", "isoline", "matrix-routing",
        "traffic", "public-transit", "waypoint-sequence", "geofencing",
        "autosuggest", "weather", "route-matching", "ev-charge-points",
        "map-image", "intermodal-routing", "tour-planning"
    ];

    public SitemapGenerator(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task GenerateAsync(string wwwrootPath, List<ContentIndexEntry> entries)
    {
        await GenerateSitemap(wwwrootPath, entries);
        await GenerateRobotsTxt(wwwrootPath);
    }

    private async Task GenerateSitemap(string wwwrootPath, List<ContentIndexEntry> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        sb.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");

        // Root
        AppendUrl(sb, $"{_baseUrl}/", "1.0");

        // Doc pages
        foreach (var entry in entries)
        {
            AppendUrl(sb, $"{_baseUrl}/docs/{entry.Slug}", "0.8");
        }

        // Demo index
        AppendUrl(sb, $"{_baseUrl}/demo", "0.7");

        // Demo pages
        foreach (var slug in DemoSlugs)
        {
            AppendUrl(sb, $"{_baseUrl}/demo/{slug}", "0.5");
        }

        // REST API pages
        AppendUrl(sb, $"{_baseUrl}/rest-api", "0.7");
        AppendUrl(sb, $"{_baseUrl}/rest-api/getting-started", "0.6");
        foreach (var slug in RestApiSlugs)
        {
            AppendUrl(sb, $"{_baseUrl}/rest-api/{slug}", "0.5");
        }

        sb.AppendLine("</urlset>");

        var outputPath = Path.Combine(wwwrootPath, "sitemap.xml");
        await File.WriteAllTextAsync(outputPath, sb.ToString());

        var totalUrls = 1 + entries.Count + 1 + DemoSlugs.Length + 2 + RestApiSlugs.Length;
        Console.WriteLine($"  Generated sitemap.xml with {totalUrls} URLs");
    }

    private static void AppendUrl(StringBuilder sb, string loc, string priority)
    {
        sb.AppendLine("  <url>");
        sb.AppendLine($"    <loc>{loc}</loc>");
        sb.AppendLine($"    <priority>{priority}</priority>");
        sb.AppendLine("  </url>");
    }

    private async Task GenerateRobotsTxt(string wwwrootPath)
    {
        var content = $"""
            User-agent: *
            Allow: /
            Sitemap: {_baseUrl}/sitemap.xml
            """;

        var outputPath = Path.Combine(wwwrootPath, "robots.txt");
        await File.WriteAllTextAsync(outputPath, content);
        Console.WriteLine("  Generated robots.txt");
    }
}
