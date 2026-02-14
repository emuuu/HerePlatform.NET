using HerePlatformComponents.Maps.Services.Routing;

namespace HerePlatformComponents.Tests.Services.Routing;

[TestFixture]
public class TruckOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var options = new TruckOptions();

        Assert.That(options.Height, Is.Null);
        Assert.That(options.Width, Is.Null);
        Assert.That(options.Length, Is.Null);
        Assert.That(options.GrossWeight, Is.Null);
        Assert.That(options.WeightPerAxle, Is.Null);
        Assert.That(options.AxleCount, Is.Null);
        Assert.That(options.TrailerCount, Is.Null);
        Assert.That(options.TunnelCategory, Is.Null);
        Assert.That(options.HazardousGoods, Is.EqualTo(HazardousGoods.None));
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var options = new TruckOptions
        {
            Height = 4.0,
            Width = 2.5,
            Length = 16.5,
            GrossWeight = 36000,
            WeightPerAxle = 10000,
            AxleCount = 5,
            TrailerCount = 1,
            TunnelCategory = TunnelCategory.C,
            HazardousGoods = HazardousGoods.Flammable | HazardousGoods.Gas
        };

        Assert.That(options.Height, Is.EqualTo(4.0));
        Assert.That(options.Width, Is.EqualTo(2.5));
        Assert.That(options.Length, Is.EqualTo(16.5));
        Assert.That(options.GrossWeight, Is.EqualTo(36000));
        Assert.That(options.WeightPerAxle, Is.EqualTo(10000));
        Assert.That(options.AxleCount, Is.EqualTo(5));
        Assert.That(options.TrailerCount, Is.EqualTo(1));
        Assert.That(options.TunnelCategory, Is.EqualTo(TunnelCategory.C));
        Assert.That(options.HazardousGoods.HasFlag(HazardousGoods.Flammable), Is.True);
        Assert.That(options.HazardousGoods.HasFlag(HazardousGoods.Gas), Is.True);
        Assert.That(options.HazardousGoods.HasFlag(HazardousGoods.Explosive), Is.False);
    }

    [Test]
    public void RoutingRequest_TruckOptions_DefaultNull()
    {
        var request = new RoutingRequest();
        Assert.That(request.Truck, Is.Null);
    }

    [Test]
    public void RoutingRequest_TruckOptions_Settable()
    {
        var request = new RoutingRequest
        {
            TransportMode = TransportMode.Truck,
            Truck = new TruckOptions { Height = 4.0, GrossWeight = 36000 }
        };

        Assert.That(request.Truck, Is.Not.Null);
        Assert.That(request.Truck.Height, Is.EqualTo(4.0));
        Assert.That(request.Truck.GrossWeight, Is.EqualTo(36000));
    }
}
