namespace HerePlatform.Core.Exceptions;

/// <summary>
/// Thrown when a HERE API call fails due to authentication (HTTP 401/403).
/// </summary>
public class HereApiAuthenticationException : Exception
{
    /// <summary>
    /// The service that reported the authentication failure (e.g. "routing", "geocoding").
    /// </summary>
    public string? Service { get; }

    public HereApiAuthenticationException(string message, string? service)
        : base(message)
    {
        Service = service;
    }

    public HereApiAuthenticationException(string message, string? service, Exception innerException)
        : base(message, innerException)
    {
        Service = service;
    }
}
