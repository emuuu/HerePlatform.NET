using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.EvChargePoints;
using HerePlatform.Core.Exceptions;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class EvChargePointsServiceTests
{
    private static RestEvChargePointsService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestEvChargePointsService(factory);
    }

    [Test]
    public async Task SearchStationsAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"evStations":[]}""");
        var service = CreateService(handler);

        await service.SearchStationsAsync(new EvChargePointsRequest
        {
            Position = new LatLngLiteral(52.5, 13.4),
            Radius = 3000,
            MaxResults = 10
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://ev-v2.cc.api.here.com/ev/stations.json?"));
        Assert.That(url, Does.Contain("prox=52.5%2C13.4%2C3000"));
        Assert.That(url, Does.Contain("maxresults=10"));
    }

    [Test]
    public async Task SearchStationsAsync_IncludesConnectorTypeFilter()
    {
        var handler = MockHttpHandler.WithJson("""{"evStations":[]}""");
        var service = CreateService(handler);

        await service.SearchStationsAsync(new EvChargePointsRequest
        {
            Position = new LatLngLiteral(52.5, 13.4),
            ConnectorTypes = [ConnectorType.Type2, ConnectorType.CcsCombo2]
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("connectortype=2%2C33"));
    }

    [Test]
    public async Task SearchStationsAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "evStations": [
                {
                    "poolId": "pool-123",
                    "address": {"label": "Friedrichstr. 1, 10117 Berlin"},
                    "position": {"lat": 52.5, "lng": 13.4},
                    "totalNumberOfConnectors": 4,
                    "connectors": [
                        {
                            "supplierName": "Ionity",
                            "connectorType": {"name": "CCS", "id": "33"},
                            "maxPowerLevel": 350.0,
                            "chargeCapacity": 2,
                            "fixedCable": true
                        }
                    ]
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SearchStationsAsync(new EvChargePointsRequest
        {
            Position = new LatLngLiteral(52.5, 13.4)
        });

        Assert.That(result.Stations, Has.Count.EqualTo(1));
        var station = result.Stations![0];
        Assert.That(station.PoolId, Is.EqualTo("pool-123"));
        Assert.That(station.Address, Is.EqualTo("Friedrichstr. 1, 10117 Berlin"));
        Assert.That(station.Position!.Value.Lat, Is.EqualTo(52.5));
        Assert.That(station.TotalNumberOfConnectors, Is.EqualTo(4));
        Assert.That(station.Connectors, Has.Count.EqualTo(1));
        var connector = station.Connectors![0];
        Assert.That(connector.SupplierName, Is.EqualTo("Ionity"));
        Assert.That(connector.ConnectorTypeName, Is.EqualTo("CCS"));
        Assert.That(connector.ConnectorTypeId, Is.EqualTo("33"));
        Assert.That(connector.MaxPowerLevel, Is.EqualTo(350.0));
        Assert.That(connector.ChargeCapacity, Is.EqualTo(2));
        Assert.That(connector.FixedCable, Is.True);
    }

    [Test]
    public async Task SearchStationsAsync_EmptyResponse_ReturnsEmptyStations()
    {
        var handler = MockHttpHandler.WithJson("""{"evStations":[]}""");
        var service = CreateService(handler);

        var result = await service.SearchStationsAsync(new EvChargePointsRequest
        {
            Position = new LatLngLiteral(52.5, 13.4)
        });

        Assert.That(result.Stations, Is.Not.Null);
        Assert.That(result.Stations, Is.Empty);
    }

    [Test]
    public void SearchStationsAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.SearchStationsAsync(new EvChargePointsRequest
            {
                Position = new LatLngLiteral(52.5, 13.4)
            }));
        Assert.That(ex!.Service, Is.EqualTo("evChargePoints"));
    }
}
