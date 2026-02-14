using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services;
using HerePlatformComponents.Maps.Services.MatrixRouting;

namespace HerePlatformComponents.Tests.Services.MatrixRouting;

[TestFixture]
public class MatrixRoutingServiceTests : ServiceTestBase
{
    [Test]
    public async Task CalculateMatrixAsync_2x2Matrix_ReturnsAllEntries()
    {
        MockJsResult("blazorHerePlatform.objectManager.calculateMatrix", new MatrixRoutingResult
        {
            NumOrigins = 2,
            NumDestinations = 2,
            Matrix = new List<MatrixEntry>
            {
                new() { OriginIndex = 0, DestinationIndex = 0, Duration = 0, Length = 0 },
                new() { OriginIndex = 0, DestinationIndex = 1, Duration = 1200, Length = 15000 },
                new() { OriginIndex = 1, DestinationIndex = 0, Duration = 1350, Length = 16200 },
                new() { OriginIndex = 1, DestinationIndex = 1, Duration = 0, Length = 0 }
            }
        });
        var service = new MatrixRoutingService(JsRuntime);

        var result = await service.CalculateMatrixAsync(new MatrixRoutingRequest
        {
            Origins = new List<LatLngLiteral>
            {
                new(52.5200, 13.4050),
                new(52.5310, 13.3847)
            },
            Destinations = new List<LatLngLiteral>
            {
                new(52.5200, 13.4050),
                new(52.5310, 13.3847)
            }
        });

        Assert.That(result.NumOrigins, Is.EqualTo(2));
        Assert.That(result.NumDestinations, Is.EqualTo(2));
        Assert.That(result.Matrix, Has.Count.EqualTo(4));
        Assert.That(result.Matrix[0].Duration, Is.EqualTo(0));
        Assert.That(result.Matrix[1].OriginIndex, Is.EqualTo(0));
        Assert.That(result.Matrix[1].DestinationIndex, Is.EqualTo(1));
        Assert.That(result.Matrix[1].Duration, Is.EqualTo(1200));
        Assert.That(result.Matrix[1].Length, Is.EqualTo(15000));
        Assert.That(result.Matrix[2].Duration, Is.EqualTo(1350));
        Assert.That(result.Matrix[2].Length, Is.EqualTo(16200));
    }

    [Test]
    public async Task CalculateMatrixAsync_NullResult_ReturnsEmptyResult()
    {
        var service = new MatrixRoutingService(JsRuntime);

        var result = await service.CalculateMatrixAsync(new MatrixRoutingRequest
        {
            Origins = new List<LatLngLiteral> { new(52.5200, 13.4050) },
            Destinations = new List<LatLngLiteral> { new(52.5310, 13.3847) }
        });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Matrix, Is.Empty);
    }
}
