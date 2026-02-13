using HerePlatformComponents.Maps.Services.Routing;

namespace HerePlatformComponents.Tests.Services.Routing;

[TestFixture]
public class EvOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var options = new EvOptions();

        Assert.That(options.InitialCharge, Is.Null);
        Assert.That(options.MaxCharge, Is.Null);
        Assert.That(options.MaxChargeAfterChargingStation, Is.Null);
        Assert.That(options.MinChargeAtChargingStation, Is.Null);
        Assert.That(options.MinChargeAtDestination, Is.Null);
        Assert.That(options.ChargingCurve, Is.Null);
        Assert.That(options.FreeFlowSpeedTable, Is.Null);
        Assert.That(options.AuxiliaryConsumption, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var options = new EvOptions
        {
            InitialCharge = 48000,
            MaxCharge = 64000,
            MaxChargeAfterChargingStation = 57600,
            MinChargeAtChargingStation = 6400,
            MinChargeAtDestination = 6400,
            ChargingCurve = "0,100000,32000,80000,48000,40000,64000,10000",
            FreeFlowSpeedTable = "0,0.239,27,0.239,45,0.259,60,0.196,75,0.207,90,0.238,100,0.26,110,0.296,120,0.337,130,0.351",
            AuxiliaryConsumption = 1500
        };

        Assert.That(options.InitialCharge, Is.EqualTo(48000));
        Assert.That(options.MaxCharge, Is.EqualTo(64000));
        Assert.That(options.MaxChargeAfterChargingStation, Is.EqualTo(57600));
        Assert.That(options.MinChargeAtChargingStation, Is.EqualTo(6400));
        Assert.That(options.MinChargeAtDestination, Is.EqualTo(6400));
        Assert.That(options.ChargingCurve, Is.Not.Null);
        Assert.That(options.FreeFlowSpeedTable, Is.Not.Null);
        Assert.That(options.AuxiliaryConsumption, Is.EqualTo(1500));
    }

    [Test]
    public void RoutingRequest_EvOptions_DefaultNull()
    {
        var request = new RoutingRequest();
        Assert.That(request.Ev, Is.Null);
    }

    [Test]
    public void RoutingRequest_EvOptions_Settable()
    {
        var request = new RoutingRequest
        {
            Ev = new EvOptions
            {
                InitialCharge = 48000,
                MaxCharge = 64000,
                FreeFlowSpeedTable = "0,0.239,27,0.239"
            }
        };

        Assert.That(request.Ev, Is.Not.Null);
        Assert.That(request.Ev.InitialCharge, Is.EqualTo(48000));
    }
}
