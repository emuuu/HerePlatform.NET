using System.Text.Json;
using System.Text.Json.Serialization;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Serialization;

namespace HerePlatform.RestClient;

internal static class HereJsonDefaults
{
    private static readonly JsonSerializerOptions _options = Create();

    public static JsonSerializerOptions Options => _options;

    private static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // LatLngLiteral already has [JsonConverter] attribute on the struct,
        // so it will be picked up automatically. We don't need to add it here.

        return options;
    }
}
