using System.Reflection;

namespace HerePlatform.Blazor.Tests;

[TestFixture]
public class TypeForwardsTests
{
    private static readonly Assembly ComponentsAssembly =
        typeof(HerePlatform.Blazor.DependencyInjectionExtensions).Assembly;

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
        // 3 Coordinates + 1 Serialization + 13 Routing + 4 Geocoding + 7 Search
        // + 3 MatrixRouting + 4 Isoline + 4 Traffic + 4 Transit + 4 Places
        // + 2 Geofencing + 4 RouteMatching + 5 Weather + 2 WaypointSequence
        // + 5 EvChargePoints + 3 MapImage + 7 IntermodalRouting + 15 TourPlanning
        // + 3 Utilities + 2 Exceptions + 1 Attributes + 16 Services = 112
        Assert.That(ForwardedTypes, Has.Length.EqualTo(112),
            "TypeForwardedTo count changed — update this test if types were added/removed");
    }
}
