using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Services;
using HerePlatform.Core.WaypointSequence;
using HerePlatform.Blazor.Maps;
using HerePlatform.Blazor.Maps.Services;
using Microsoft.JSInterop;

namespace HerePlatform.Blazor.Tests.Services.WaypointSequence;

[TestFixture]
public class WaypointSequenceServiceTests : ServiceTestBase
{
    [Test]
    public async Task OptimizeSequenceAsync_WithWaypoints_ReturnsOptimizedOrder()
    {
        MockJsResult("herePlatform.objectManager.optimizeWaypointSequence", new WaypointSequenceResult
        {
            OptimizedIndices = new List<int> { 2, 0, 1 },
            OptimizedWaypoints = new List<LatLngLiteral>
            {
                new(52.5310, 13.3847),
                new(52.5200, 13.4050),
                new(52.5219, 13.4132)
            },
            TotalDistance = 15000,
            TotalDuration = 1200
        });
        var service = new WaypointSequenceService(JsRuntime);

        var result = await service.OptimizeSequenceAsync(new WaypointSequenceRequest
        {
            Start = new LatLngLiteral(52.5251, 13.3694),
            End = new LatLngLiteral(52.4907, 13.3880),
            Waypoints = new List<LatLngLiteral>
            {
                new(52.5200, 13.4050),
                new(52.5219, 13.4132),
                new(52.5310, 13.3847)
            }
        });

        Assert.That(result.OptimizedIndices, Is.EqualTo(new List<int> { 2, 0, 1 }));
        Assert.That(result.OptimizedWaypoints, Has.Count.EqualTo(3));
        Assert.That(result.TotalDistance, Is.EqualTo(15000));
        Assert.That(result.TotalDuration, Is.EqualTo(1200));
    }

    [Test]
    public void OptimizeSequenceAsync_AuthError_ThrowsHereApiAuthenticationException()
    {
        MockJsException<WaypointSequenceResult>(
            "herePlatform.objectManager.optimizeWaypointSequence",
            new JSException("Error: HERE_AUTH_ERROR:waypoint-sequence:HTTP 401"));
        var service = new WaypointSequenceService(JsRuntime);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(async () =>
            await service.OptimizeSequenceAsync(new WaypointSequenceRequest
            {
                Start = new LatLngLiteral(52.5251, 13.3694),
                End = new LatLngLiteral(52.4907, 13.3880),
                Waypoints = new List<LatLngLiteral> { new(52.5200, 13.4050) }
            }));

        Assert.That(ex!.Service, Is.EqualTo("waypoint-sequence"));
    }
}
