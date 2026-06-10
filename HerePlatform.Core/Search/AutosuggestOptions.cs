using HerePlatform.Core.Coordinates;

namespace HerePlatform.Core.Search;

/// <summary>
/// Configuration options for the HERE Autosuggest search.
/// </summary>
/// <remarks>
/// The HERE Autosuggest API requires exactly one spatial context per request:
/// either <c>At</c>, or an <c>in=circle:…</c> / <c>in=bbox:…</c> expression in <c>In</c>.
/// A <c>countryCode:</c> filter in <c>In</c> is only valid in combination with one of those —
/// on its own the API rejects the request with HTTP 400.
/// </remarks>
public class AutosuggestOptions
{
    /// <summary>
    /// Maximum number of results to return. Default: 5.
    /// </summary>
    public int Limit { get; set; } = 5;

    /// <summary>
    /// Language for results (BCP 47 tag, e.g. "de", "en"). Default: "de".
    /// </summary>
    public string? Lang { get; set; } = "de";

    /// <summary>
    /// Geographic filter expression (e.g. "countryCode:DEU", "circle:52.5,13.4;r=10000",
    /// "bbox:13.08,52.33,13.76,52.67"). Default: "countryCode:DEU".
    /// A <c>countryCode:</c> filter is a hard filter only — it does not provide the spatial
    /// context the Autosuggest API requires. Combine it with <c>At</c> (the default),
    /// or use a <c>circle:</c>/<c>bbox:</c> expression instead.
    /// </summary>
    public string? In { get; set; } = "countryCode:DEU";

    /// <summary>
    /// Geographic search context. Results near this position are ranked higher.
    /// Required by the Autosuggest API unless <c>In</c> contains a
    /// <c>circle:</c> or <c>bbox:</c> expression (the API treats <c>at</c> and
    /// <c>circle</c>/<c>bbox</c> as mutually exclusive; requests omit <c>at</c> in that case).
    /// Default: the geographic center of Germany (51.1657, 10.4515), matching the
    /// <c>countryCode:DEU</c> default of <c>In</c>.
    /// </summary>
    public LatLngLiteral? At { get; set; } = new LatLngLiteral(51.1657, 10.4515);

    /// <summary>
    /// True when <c>In</c> itself provides the spatial context required by the
    /// Autosuggest API, i.e. contains a <c>circle:</c> or <c>bbox:</c> expression.
    /// In that case requests must omit <c>at</c> (mutually exclusive per the HERE API).
    /// </summary>
    public bool InProvidesSpatialContext()
    {
        if (In is null)
            return false;

        // Parse clause-wise instead of raw substring matching, so embedded tokens
        // like "notcircle:…" do not count as a spatial context. The semicolon also
        // separates circle's radius part ("circle:52.5,13.4;r=10000"), which is fine —
        // the clause before it already starts with "circle:".
        foreach (var clause in In.Split(';'))
        {
            var token = clause.Trim();
            if (token.StartsWith("circle:", StringComparison.Ordinal) ||
                token.StartsWith("bbox:", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Validates that this combination of <c>At</c> and <c>In</c> forms a
    /// valid Autosuggest request per the HERE API contract.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when neither <c>At</c> nor a <c>circle:</c>/<c>bbox:</c> expression in
    /// <c>In</c> provides a spatial context — the HERE API rejects such requests with
    /// HTTP 400 (a <c>countryCode:</c> filter alone is not sufficient).
    /// </exception>
    public void EnsureValidForAutosuggest()
    {
        if (!At.HasValue && !InProvidesSpatialContext())
        {
            throw new InvalidOperationException(
                "HERE Autosuggest requires a spatial context: set AutosuggestOptions.At to a coordinate, " +
                "or use an 'in=circle:…'/'in=bbox:…' expression. A 'countryCode:…' filter alone is " +
                "rejected by the HERE API with HTTP 400.");
        }
    }
}
