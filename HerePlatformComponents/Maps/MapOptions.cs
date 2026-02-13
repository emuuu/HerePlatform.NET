using HerePlatformComponents.Maps.Coordinates;
using System.Text.Json.Serialization;

namespace HerePlatformComponents.Maps;

public class MapOptions
{
    public LatLngLiteral? Center { get; set; }
    public double Zoom { get; set; } = 10;
    public MapLayerType LayerType { get; set; } = MapLayerType.VectorNormalMap;
    public bool EnableInteraction { get; set; } = true;
    public bool EnableUI { get; set; } = true;
    public double? MinZoom { get; set; }
    public double? MaxZoom { get; set; }
    public double? Tilt { get; set; }
    public double? Heading { get; set; }

    /// <summary>
    /// Padding for the map viewport (H.map.ViewPort.Padding).
    /// </summary>
    public Padding? Padding { get; set; }

    /// <summary>
    /// Whether automatic UI color adaptation is enabled (default true).
    /// </summary>
    public bool? AutoColor { get; set; }

    /// <summary>
    /// Locale for UI controls (e.g. "de-DE", "es-ES", "fr-FR").
    /// Supported: en-US, de-DE, es-ES, fi-FI, fr-FR, it-IT, nl-NL, pl-PL, pt-BR, pt-PT, ru-RU, tr-TR, zh-CN.
    /// </summary>
    public string? UiLocale { get; set; }

    /// <summary>
    /// Pixel ratio for high-DPI displays (e.g. 2.0 for Retina).
    /// </summary>
    public double? PixelRatio { get; set; }

    /// <summary>
    /// Whether the map center stays fixed during resize.
    /// </summary>
    public bool? FixedCenter { get; set; }

    [JsonIgnore]
    public HereApiLoadOptions? ApiLoadOptions { get; set; }
}
