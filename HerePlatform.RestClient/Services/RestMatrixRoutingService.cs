using System.Text;
using System.Text.Json;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.MatrixRouting;
using HerePlatform.Core.Routing;
using HerePlatform.Core.Services;
using HerePlatform.RestClient.Internal;

namespace HerePlatform.RestClient.Services;

internal sealed class RestMatrixRoutingService : IMatrixRoutingService
{
    private const string BaseUrl = "https://matrix.router.hereapi.com/v8/matrix";

    private readonly IHttpClientFactory _httpClientFactory;

    public RestMatrixRoutingService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<MatrixRoutingResult> CalculateMatrixAsync(MatrixRoutingRequest request)
    {
        var body = new
        {
            origins = request.Origins.Select(o => new { lat = o.Lat, lng = o.Lng }).ToArray(),
            destinations = request.Destinations.Select(d => new { lat = d.Lat, lng = d.Lng }).ToArray(),
            regionDefinition = new
            {
                type = "world"
            },
            profile = HereApiHelper.GetEnumMemberValue(request.TransportMode)
        };

        var jsonBody = JsonSerializer.Serialize(body, HereJsonDefaults.Options);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient("HereApi");
        using var response = await client.PostAsync(BaseUrl, content).ConfigureAwait(false);

        HereApiHelper.EnsureAuthSuccess(response, "matrix");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var hereResponse = JsonSerializer.Deserialize<HereMatrixResponse>(json, HereJsonDefaults.Options);

        return MapToResult(hereResponse, request.Origins.Count, request.Destinations.Count);
    }

    private static MatrixRoutingResult MapToResult(HereMatrixResponse? hereResponse, int numOrigins, int numDestinations)
    {
        if (hereResponse?.Matrix is null)
            return new MatrixRoutingResult
            {
                NumOrigins = numOrigins,
                NumDestinations = numDestinations,
                Matrix = []
            };

        var matrix = hereResponse.Matrix;

        return new MatrixRoutingResult
        {
            NumOrigins = matrix.NumOrigins,
            NumDestinations = matrix.NumDestinations,
            Matrix = matrix.Entries?.Select(e => new MatrixEntry
            {
                OriginIndex = e.OriginIndex,
                DestinationIndex = e.DestinationIndex,
                Duration = e.TravelTime ?? 0,
                Length = e.Distance ?? 0
            }).ToList() ?? []
        };
    }
}
