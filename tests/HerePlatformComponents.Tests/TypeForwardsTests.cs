using System.Reflection;

namespace HerePlatformComponents.Tests;

[TestFixture]
public class TypeForwardsTests
{
    private static readonly Assembly ComponentsAssembly =
        typeof(HerePlatformComponents.DependencyInjectionExtensions).Assembly;

    private static readonly Assembly CoreAssembly =
        typeof(HerePlatform.Core.Coordinates.LatLngLiteral).Assembly;

    private static readonly Type[] ForwardedTypes = ComponentsAssembly.GetForwardedTypes();

    [Test]
    public void AllForwardedTypes_PointToCoreAssembly()
    {
        Assert.That(ForwardedTypes, Is.Not.Empty, "No forwarded types found");

        foreach (var type in ForwardedTypes)
        {
            Assert.That(type.Assembly, Is.EqualTo(CoreAssembly),
                $"{type.FullName} should be in HerePlatform.Core but is in {type.Assembly.GetName().Name}");
        }
    }

    [Test]
    public void AllForwardedTypes_ArePublic()
    {
        foreach (var type in ForwardedTypes)
        {
            Assert.That(type.IsPublic || type.IsNestedPublic, Is.True,
                $"{type.FullName} is forwarded but not public");
        }
    }

    [Test]
    public void AllPublicCoreTypes_HaveForward()
    {
        var forwardedSet = new HashSet<Type>(ForwardedTypes);

        var publicCoreTypes = CoreAssembly.GetExportedTypes()
            .Where(t => !t.IsNested)
            .ToList();

        var missing = publicCoreTypes
            .Where(t => !forwardedSet.Contains(t))
            .Select(t => t.FullName)
            .ToList();

        Assert.That(missing, Is.Empty,
            $"Public Core types missing TypeForwardedTo: {string.Join(", ", missing)}");
    }

    [Test]
    public void ForwardedTypes_CanBeLoadedByName_ThroughComponentsAssembly()
    {
        foreach (var type in ForwardedTypes)
        {
            var typeName = type.FullName!;
            var resolved = ComponentsAssembly.GetType(typeName);

            Assert.That(resolved, Is.Not.Null,
                $"Type '{typeName}' could not be resolved through {ComponentsAssembly.GetName().Name}");
            Assert.That(resolved, Is.EqualTo(type),
                $"Type '{typeName}' resolved to a different type than expected");
        }
    }

    [Test]
    public void ExpectedForwardCount_Matches()
    {
        // 3 Coordinates + 1 Serialization + 13 Routing + 3 Geocoding + 7 Search
        // + 3 MatrixRouting + 4 Isoline + 4 Traffic + 4 Transit + 4 Places
        // + 2 Geofencing + 4 RouteMatching + 5 Weather + 2 WaypointSequence + 3 Utilities + 1 Exceptions + 1 Attributes + 12 Services = 76
        Assert.That(ForwardedTypes, Has.Length.EqualTo(76),
            "TypeForwardedTo count changed — update this test if types were added/removed");
    }
}
