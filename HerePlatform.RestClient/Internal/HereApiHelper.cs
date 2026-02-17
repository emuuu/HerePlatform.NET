using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Routing;

namespace HerePlatform.RestClient.Internal;

internal static class HereApiHelper
{
    public static string FormatCoord(LatLngLiteral coord)
        => string.Create(CultureInfo.InvariantCulture, $"{coord.Lat},{coord.Lng}");

    public static string Invariant(double value)
        => value.ToString(CultureInfo.InvariantCulture);

    public static string Invariant(int value)
        => value.ToString(CultureInfo.InvariantCulture);

    public static void EnsureAuthSuccess(HttpResponseMessage response, string serviceName)
    {
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            throw new HereApiAuthenticationException(
                $"HERE API authentication failed for {serviceName} (HTTP {(int)response.StatusCode}).",
                serviceName);
        }
    }

    public static string BuildQueryString(params (string key, string? value)[] parameters)
    {
        var pairs = parameters
            .Where(p => p.value is not null)
            .Select(p => $"{Uri.EscapeDataString(p.key)}={Uri.EscapeDataString(p.value!)}");

        return string.Join("&", pairs);
    }

    public static string GetEnumMemberValue<T>(T value) where T : struct, Enum
    {
        var member = typeof(T).GetMember(value.ToString()!)[0];
        var attr = member.GetCustomAttributes(typeof(EnumMemberAttribute), false)
            .Cast<EnumMemberAttribute>()
            .FirstOrDefault();
        return attr?.Value ?? value.ToString()!.ToLowerInvariant();
    }

    public static string[] GetAvoidFeatures(RoutingAvoidFeature avoid)
    {
        var features = new List<string>();
        if (avoid.HasFlag(RoutingAvoidFeature.Tolls)) features.Add("tollRoad");
        if (avoid.HasFlag(RoutingAvoidFeature.Highways)) features.Add("controlledAccessHighway");
        if (avoid.HasFlag(RoutingAvoidFeature.Ferries)) features.Add("ferry");
        if (avoid.HasFlag(RoutingAvoidFeature.Tunnels)) features.Add("tunnel");
        return features.ToArray();
    }

    public static string[] GetHazardousGoods(HazardousGoods goods)
    {
        var result = new List<string>();
        if (goods.HasFlag(HazardousGoods.Explosive)) result.Add("explosive");
        if (goods.HasFlag(HazardousGoods.Gas)) result.Add("gas");
        if (goods.HasFlag(HazardousGoods.Flammable)) result.Add("flammable");
        if (goods.HasFlag(HazardousGoods.Combustible)) result.Add("combustible");
        if (goods.HasFlag(HazardousGoods.Organic)) result.Add("organic");
        if (goods.HasFlag(HazardousGoods.Poison)) result.Add("poison");
        if (goods.HasFlag(HazardousGoods.RadioActive)) result.Add("radioActive");
        if (goods.HasFlag(HazardousGoods.Corrosive)) result.Add("corrosive");
        if (goods.HasFlag(HazardousGoods.PoisonousInhalation)) result.Add("poisonousInhalation");
        if (goods.HasFlag(HazardousGoods.HarmfulToWater)) result.Add("harmfulToWater");
        if (goods.HasFlag(HazardousGoods.Other)) result.Add("other");
        return result.ToArray();
    }
}
