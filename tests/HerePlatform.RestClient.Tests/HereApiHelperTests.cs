using System.Globalization;
using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.RestClient.Internal;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class HereApiHelperTests
{
    [Test]
    public void EnsureAuthSuccess_401_ThrowsAuthException()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        var ex = Assert.Throws<HereApiAuthenticationException>(
            () => HereApiHelper.EnsureAuthSuccess(response, "geocoding"));

        Assert.That(ex!.Service, Is.EqualTo("geocoding"));
        Assert.That(ex.Message, Does.Contain("401"));
    }

    [Test]
    public void EnsureAuthSuccess_403_ThrowsAuthException()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
        var ex = Assert.Throws<HereApiAuthenticationException>(
            () => HereApiHelper.EnsureAuthSuccess(response, "routing"));

        Assert.That(ex!.Service, Is.EqualTo("routing"));
        Assert.That(ex.Message, Does.Contain("403"));
    }

    [Test]
    public void EnsureAuthSuccess_200_DoesNotThrow()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        Assert.DoesNotThrow(() => HereApiHelper.EnsureAuthSuccess(response, "geocoding"));
    }

    [Test]
    public void BuildQueryString_FiltersNullValues()
    {
        var qs = HereApiHelper.BuildQueryString(
            ("q", "Berlin"),
            ("lang", null),
            ("limit", "5"));

        Assert.That(qs, Is.EqualTo("q=Berlin&limit=5"));
    }

    [Test]
    public void BuildQueryString_EncodesSpecialCharacters()
    {
        var qs = HereApiHelper.BuildQueryString(
            ("q", "Berlin Mitte & Co"));

        Assert.That(qs, Does.Contain("Berlin%20Mitte%20%26%20Co"));
    }

    [Test]
    public void BuildQueryString_Empty_ReturnsEmpty()
    {
        var qs = HereApiHelper.BuildQueryString();
        Assert.That(qs, Is.Empty);
    }

    [Test]
    public void FormatCoord_UsesInvariantCulture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var coord = new LatLngLiteral(52.5, 13.4);
            var result = HereApiHelper.FormatCoord(coord);
            Assert.That(result, Is.EqualTo("52.5,13.4"));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Test]
    public void Invariant_Double_UsesInvariantCulture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var result = HereApiHelper.Invariant(52.5);
            Assert.That(result, Is.EqualTo("52.5"));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Test]
    public void GetEnumMemberValue_ReturnsEnumMemberAttribute()
    {
        var result = HereApiHelper.GetEnumMemberValue(Core.Routing.TransportMode.Car);
        Assert.That(result, Is.EqualTo("car"));
    }
}
