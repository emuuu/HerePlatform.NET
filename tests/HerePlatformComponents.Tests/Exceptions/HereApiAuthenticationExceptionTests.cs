using HerePlatform.Core.Exceptions;

namespace HerePlatformComponents.Tests.Exceptions;

[TestFixture]
public class HereApiAuthenticationExceptionTests
{
    [Test]
    public void Constructor_SetsMessageAndService()
    {
        var ex = new HereApiAuthenticationException("Auth failed", "routing");

        Assert.That(ex.Message, Is.EqualTo("Auth failed"));
        Assert.That(ex.Service, Is.EqualTo("routing"));
    }

    [Test]
    public void Constructor_WithInnerException_SetsAll()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new HereApiAuthenticationException("Auth failed", "geocoding", inner);

        Assert.That(ex.Message, Is.EqualTo("Auth failed"));
        Assert.That(ex.Service, Is.EqualTo("geocoding"));
        Assert.That(ex.InnerException, Is.SameAs(inner));
    }

    [Test]
    public void Service_CanBeNull()
    {
        var ex = new HereApiAuthenticationException("Auth failed", null);

        Assert.That(ex.Service, Is.Null);
    }

    [Test]
    public void IsException()
    {
        var ex = new HereApiAuthenticationException("test", "routing");

        Assert.That(ex, Is.InstanceOf<Exception>());
    }
}
