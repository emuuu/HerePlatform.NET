using System.Text.Json;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Search;
using HerePlatform.Core.Services;
using HerePlatform.RestClient.Internal;

namespace HerePlatform.RestClient.Services;

internal sealed class RestAutosuggestService : IAutosuggestService
{
    private const string AutosuggestBaseUrl = "https://autosuggest.search.hereapi.com/v1/autosuggest";
    private const string AutocompleteBaseUrl = "https://autocomplete.search.hereapi.com/v1/autocomplete";

    private readonly IHttpClientFactory _httpClientFactory;

    public RestAutosuggestService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AutosuggestResult> SuggestAsync(string query, AutosuggestOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var opts = options ?? new AutosuggestOptions();
        opts.EnsureValidForAutosuggest();

        // Treat empty/whitespace like null so "show=" is never sent — mirrors the
        // JS path, where a falsy options.show is omitted from the params.
        var show = string.IsNullOrWhiteSpace(opts.Show) ? null : opts.Show;

        var hereResponse = await ExecuteRequestAsync(AutosuggestBaseUrl, query, opts, show, "autosuggest", cancellationToken)
            .ConfigureAwait(false);

        return new AutosuggestResult { Items = MapItems(hereResponse) };
    }

    public async Task<AutocompleteResult> AutocompleteAsync(string query, AutosuggestOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // /autocomplete does not support show=details (only streetInfo/hasRelatedMPA)
        // and returns structured addresses natively — never forward Show here.
        var hereResponse = await ExecuteRequestAsync(AutocompleteBaseUrl, query, options ?? new AutosuggestOptions(), show: null, "autocomplete", cancellationToken)
            .ConfigureAwait(false);

        return new AutocompleteResult { Items = MapItems(hereResponse) };
    }

    private async Task<HereAutosuggestResponse?> ExecuteRequestAsync(
        string baseUrl, string query, AutosuggestOptions opts, string? show, string serviceName, CancellationToken cancellationToken)
    {
        // 'at' and 'in=circle/bbox' are mutually exclusive per the HERE API —
        // when In carries its own spatial context, at must be omitted.
        var sendAt = opts.At.HasValue && !opts.InProvidesSpatialContext();

        var qs = HereApiHelper.BuildQueryString(
            ("q", query),
            ("at", sendAt ? HereApiHelper.FormatCoord(opts.At!.Value) : null),
            ("in", opts.In),
            ("lang", opts.Lang),
            ("limit", opts.Limit.ToString()),
            ("show", show));

        var url = $"{baseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient(HereApiHelper.ClientName);
        using var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

        await HereApiHelper.EnsureSuccessOrThrowAsync(response, serviceName, cancellationToken).ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<HereAutosuggestResponse>(json, HereJsonDefaults.Options);
    }

    private static List<AutosuggestItem> MapItems(HereAutosuggestResponse? hereResponse)
    {
        if (hereResponse?.Items is null or { Count: 0 })
            return [];

        return hereResponse.Items.Select(item => new AutosuggestItem
        {
            Title = item.Title,
            Id = item.Id,
            ResultType = item.ResultType,
            Address = MapAddress(item.Address),
            Position = item.Position is not null
                ? new LatLngLiteral(item.Position.Lat, item.Position.Lng)
                : null,
            Highlights = MapHighlights(item.Highlights)
        }).ToList();
    }

    private static AutosuggestAddress? MapAddress(HereAddress? address)
    {
        if (address is null)
            return null;

        return new AutosuggestAddress
        {
            Label = address.Label,
            CountryCode = address.CountryCode,
            CountryName = address.CountryName,
            State = address.State,
            StateCode = address.StateCode,
            County = address.County,
            CountyCode = address.CountyCode,
            City = address.City,
            District = address.District,
            Street = address.Street,
            PostalCode = address.PostalCode,
            HouseNumber = address.HouseNumber
        };
    }

    private static AutosuggestHighlights? MapHighlights(HereHighlights? highlights)
    {
        if (highlights is null)
            return null;

        return new AutosuggestHighlights
        {
            Title = highlights.Title?.Select(r => new AutosuggestHighlightRange
            {
                Start = r.Start,
                End = r.End
            }).ToArray(),
            Address = highlights.Address?.Select(r => new AutosuggestHighlightRange
            {
                Start = r.Start,
                End = r.End
            }).ToArray()
        };
    }
}
