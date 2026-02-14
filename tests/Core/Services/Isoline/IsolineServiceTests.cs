using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services;
using HerePlatformComponents.Maps.Services.Isoline;
using HerePlatformComponents.Maps.Utilities;

namespace HerePlatformComponents.Tests.Services.Isoline;

[TestFixture]
public class IsolineServiceIntegrationTests : ServiceTestBase
{
    [Test]
    public async Task CalculateIsolineAsync_WithPolygons_ReturnsIsolines()
    {
        MockJsResult("blazorHerePlatform.objectManager.calculateIsoline", new IsolineResult
        {
            Isolines = new List<IsolinePolygon>
            {
                new()
                {
                    Range = 300,
                    Polygon = new List<LatLngLiteral>
                    {
                        new(52.530, 13.390),
                        new(52.535, 13.405),
                        new(52.530, 13.420),
                        new(52.510, 13.420),
                        new(52.505, 13.405),
                        new(52.510, 13.390)
                    }
                },
                new()
                {
                    Range = 600,
                    Polygon = new List<LatLngLiteral>
                    {
                        new(52.540, 13.370),
                        new(52.550, 13.405),
                        new(52.540, 13.440),
                        new(52.500, 13.440),
                        new(52.490, 13.405),
                        new(52.500, 13.370)
                    }
                }
            }
        });
        var service = new IsolineService(JsRuntime);

        var result = await service.CalculateIsolineAsync(new IsolineRequest
        {
            Center = new LatLngLiteral(52.52, 13.405),
            Ranges = new List<int> { 300, 600 }
        });

        Assert.That(result.Isolines, Has.Count.EqualTo(2));
        Assert.That(result.Isolines![0].Range, Is.EqualTo(300));
        Assert.That(result.Isolines[0].Polygon, Has.Count.EqualTo(6));
        Assert.That(result.Isolines[0].Polygon![0].Lat, Is.EqualTo(52.530));
        Assert.That(result.Isolines[1].Range, Is.EqualTo(600));
        Assert.That(result.Isolines[1].Polygon, Has.Count.EqualTo(6));
    }

    [Test]
    public async Task CalculateIsolineAsync_FallbackPolylineDecoding_DecodesEncodedPolyline()
    {
        var testCoords = new List<LatLngLiteral>
        {
            new(52.530, 13.390),
            new(52.535, 13.405),
            new(52.530, 13.420),
            new(52.510, 13.390)
        };
        var encodedPolyline = FlexiblePolyline.Encode(testCoords);

        MockJsResult("blazorHerePlatform.objectManager.calculateIsoline", new IsolineResult
        {
            Isolines = new List<IsolinePolygon>
            {
                new()
                {
                    Range = 300,
                    Polygon = null,
                    EncodedPolyline = encodedPolyline
                }
            }
        });
        var service = new IsolineService(JsRuntime);

        var result = await service.CalculateIsolineAsync(new IsolineRequest
        {
            Center = new LatLngLiteral(52.52, 13.405),
            Ranges = new List<int> { 300 }
        });

        Assert.That(result.Isolines, Has.Count.EqualTo(1));
        Assert.That(result.Isolines![0].Polygon, Is.Not.Null);
        Assert.That(result.Isolines[0].Polygon, Has.Count.EqualTo(4));
        Assert.That(result.Isolines[0].Polygon![0].Lat, Is.EqualTo(52.530).Within(0.00001));
        Assert.That(result.Isolines[0].Polygon[0].Lng, Is.EqualTo(13.390).Within(0.00001));
    }
}
