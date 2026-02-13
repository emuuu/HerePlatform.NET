using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Transit;

/// <summary>
/// A departure from a public transit station.
/// </summary>
public class TransitDeparture
{
    /// <summary>
    /// Line name or number.
    /// </summary>
    public string? LineName { get; set; }

    /// <summary>
    /// Direction/destination of the line.
    /// </summary>
    public string? Direction { get; set; }

    /// <summary>
    /// Departure time (ISO 8601).
    /// </summary>
    public string? DepartureTime { get; set; }

    /// <summary>
    /// Transport type (e.g., "bus", "subway", "train", "tram").
    /// </summary>
    public string? TransportType { get; set; }

    /// <summary>
    /// Station name.
    /// </summary>
    public string? StationName { get; set; }
}

/// <summary>
/// Result containing transit departures.
/// </summary>
public class TransitDeparturesResult
{
    /// <summary>
    /// List of departures.
    /// </summary>
    public List<TransitDeparture>? Departures { get; set; }
}

/// <summary>
/// A transit station.
/// </summary>
public class TransitStation
{
    /// <summary>
    /// Station name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Station position.
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Distance from search center in meters.
    /// </summary>
    public double Distance { get; set; }

    /// <summary>
    /// Transport types available at this station.
    /// </summary>
    public List<string>? TransportTypes { get; set; }
}

/// <summary>
/// Result containing transit stations.
/// </summary>
public class TransitStationsResult
{
    /// <summary>
    /// List of stations.
    /// </summary>
    public List<TransitStation>? Stations { get; set; }
}
