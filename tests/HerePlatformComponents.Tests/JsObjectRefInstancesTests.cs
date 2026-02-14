using HerePlatformComponents;

namespace HerePlatformComponents.Tests;

[TestFixture]
public class JsObjectRefInstancesTests
{
    [Test]
    public void GetInstance_MissingGuid_ThrowsKeyNotFoundException()
    {
        var ex = Assert.Throws<KeyNotFoundException>(
            () => JsObjectRefInstances.GetInstance("nonexistent-guid"));

        Assert.That(ex!.Message, Does.Contain("nonexistent-guid"));
    }
}
