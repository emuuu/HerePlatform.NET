using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using HerePlatform.Core.Services;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services;
using HerePlatform.Core.Utilities;

namespace HerePlatformComponents.Tests.Services.Routing;

[TestFixture]
public class RoutingServiceTests : ServiceTestBase
{
    [Test]
    public async Task CalculateRouteAsync_WithPolyline_DecodesPolyline()
    {
        var testCoords = new List<LatLngLiteral>
        {
            new(52.5200, 13.4050),
            new(52.5210, 13.4060),
            new(52.5220, 13.4070)
        };
        var encodedPolyline = FlexiblePolyline.Encode(testCoords);

        MockJsResult("blazorHerePlatform.objectManager.calculateRoute", new RoutingResult
        {
            Routes = new List<Route>
            {
                new()
                {
                    Sections = new List<RouteSection>
                    {
                        new()
                        {
                            Polyline = encodedPolyline,
                            Summary = new RouteSummary { Duration = 1706, Length = 12483, BaseDuration = 1580 },
                            Transport = "car"
                        }
                    }
                }
            }
        });
        var service = new RoutingService(JsRuntime);

        var result = await service.CalculateRouteAsync(new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5200, 13.4050),
            Destination = new LatLngLiteral(52.5220, 13.4070)
        });

        Assert.That(result.Routes, Has.Count.EqualTo(1));
        var section = result.Routes![0].Sections![0];
        Assert.That(section.DecodedPolyline, Is.Not.Null);
        Assert.That(section.DecodedPolyline, Has.Count.EqualTo(3));
        Assert.That(section.DecodedPolyline![0].Lat, Is.EqualTo(52.5200).Within(0.00001));
        Assert.That(section.DecodedPolyline[0].Lng, Is.EqualTo(13.4050).Within(0.00001));
        Assert.That(section.Summary!.Duration, Is.EqualTo(1706));
        Assert.That(section.Summary.Length, Is.EqualTo(12483));
        Assert.That(section.Summary.BaseDuration, Is.EqualTo(1580));
        Assert.That(section.Transport, Is.EqualTo("car"));
    }

    [Test]
    public async Task CalculateRouteAsync_WithTurnByTurn_DeserializesActions()
    {
        var encodedPolyline = FlexiblePolyline.Encode(new List<LatLngLiteral>
        {
            new(52.5200, 13.4050),
            new(52.5220, 13.4070)
        });

        MockJsResult("blazorHerePlatform.objectManager.calculateRoute", new RoutingResult
        {
            Routes = new List<Route>
            {
                new()
                {
                    Sections = new List<RouteSection>
                    {
                        new()
                        {
                            Polyline = encodedPolyline,
                            Summary = new RouteSummary { Duration = 300, Length = 1500 },
                            Transport = "car",
                            TurnByTurnActions = new List<TurnInstruction>
                            {
                                new() { Action = "depart", Instruction = "Head south on Invalidenstr.", Duration = 18, Length = 132 },
                                new() { Action = "turn", Instruction = "Turn right onto Friedrichstr.", Duration = 45, Length = 320 },
                                new() { Action = "arrive", Instruction = "Arrive at destination.", Duration = 0, Length = 0 }
                            }
                        }
                    }
                }
            }
        });
        var service = new RoutingService(JsRuntime);

        var result = await service.CalculateRouteAsync(new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5200, 13.4050),
            Destination = new LatLngLiteral(52.5220, 13.4070),
            ReturnInstructions = true
        });

        var actions = result.Routes![0].Sections![0].TurnByTurnActions;
        Assert.That(actions, Has.Count.EqualTo(3));
        Assert.That(actions![0].Action, Is.EqualTo("depart"));
        Assert.That(actions[0].Instruction, Does.Contain("Invalidenstr."));
        Assert.That(actions[1].Action, Is.EqualTo("turn"));
        Assert.That(actions[2].Action, Is.EqualTo("arrive"));
        Assert.That(actions[2].Duration, Is.EqualTo(0));
    }

    [Test]
    public async Task CalculateRouteAsync_NullResult_ReturnsEmptyResult()
    {
        var service = new RoutingService(JsRuntime);

        var result = await service.CalculateRouteAsync(new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5200, 13.4050),
            Destination = new LatLngLiteral(52.5220, 13.4070)
        });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Routes, Is.Null);
    }
}
