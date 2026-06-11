// JS-side tests for the autosuggest helpers in
// HerePlatform.Blazor/wwwroot/js/objectManager.js.
//
// Run from the repo root:
//   node --test tests/HerePlatform.Blazor.Tests.Js/autosuggestHelpers.test.mjs
//
// These helpers cannot be imported directly because objectManager.js is an
// IIFE intended for the Blazor static-files pipeline (no module exports).
// The implementations below MUST mirror the source. If you edit
// formatAutosuggestError / buildAutosuggestParams / mapAutosuggestItems in
// objectManager.js, mirror the change here and vice versa.

import { test } from 'node:test';
import assert from 'node:assert/strict';

// ---------- start mirror of objectManager.js helpers ----------

function formatAutosuggestError(error) {
    if (typeof error === 'string') return error;
    if (error) {
        if (error.message) return error.message;
        var status = error.status || error.statusCode;
        var detail = error.title || error.cause || error.error_description ||
            error.error || error.statusText || error.responseText;
        if (status || detail) {
            return 'HERE Autosuggest request failed' +
                (status ? ' (HTTP ' + status + ')' : '') +
                (detail ? ': ' + detail : '');
        }
        try {
            return 'HERE Autosuggest request failed: ' + JSON.stringify(error);
        } catch (e) { /* circular structure — fall through */ }
    }
    return 'HERE Autosuggest request failed: ' + String(error);
}

function buildAutosuggestParams(query, options) {
    var params = {
        q: query,
        limit: options.limit || 5
    };
    if (options.lang) params.lang = options.lang;
    if (options.in) params.in = options.in;
    if (options.show) params.show = options.show;
    if (options.at) params.at = options.at.lat + ',' + options.at.lng;
    return params;
}

function mapAutosuggestItems(items) {
    return (items || []).map(function (item) {
        var mapped = {
            title: item.title || null,
            id: item.id || null,
            resultType: item.resultType || null,
            address: null,
            position: null,
            highlights: null
        };

        if (item.address) {
            mapped.address = {
                label: item.address.label || null,
                countryCode: item.address.countryCode || null,
                countryName: item.address.countryName || null,
                state: item.address.state || null,
                stateCode: item.address.stateCode || null,
                county: item.address.county || null,
                countyCode: item.address.countyCode || null,
                city: item.address.city || null,
                district: item.address.district || null,
                street: item.address.street || null,
                postalCode: item.address.postalCode || null,
                houseNumber: item.address.houseNumber || null
            };
        }

        if (item.position) {
            mapped.position = { lat: item.position.lat, lng: item.position.lng };
        }

        if (item.highlights && item.highlights.title) {
            mapped.highlights = {
                title: item.highlights.title.map(function (r) {
                    return { start: r.start, end: r.end };
                }),
                address: item.highlights.address && item.highlights.address.label
                    ? item.highlights.address.label.map(function (r) {
                        return { start: r.start, end: r.end };
                    })
                    : null
            };
        }

        return mapped;
    });
}

// ---------- end mirror ----------

// ── formatAutosuggestError ──

test('string errors pass through unchanged', () => {
    assert.equal(formatAutosuggestError('boom'), 'boom');
});

test('Error objects use their message', () => {
    assert.equal(formatAutosuggestError(new Error('network down')), 'network down');
});

test('HERE error response bodies keep status and cause', () => {
    // Shape of the Geocoding & Search v7 error body
    const msg = formatAutosuggestError({
        status: 400,
        title: 'Required parameter missing',
        cause: "One of 'at', 'in=circle' or 'in=bbox' is required",
        correlationId: 'abc'
    });
    assert.match(msg, /HTTP 400/);
    assert.match(msg, /Required parameter missing/);
});

test('objects without message or status are stringified, not [object Object]', () => {
    const msg = formatAutosuggestError({ foo: 'bar' });
    assert.doesNotMatch(msg, /\[object Object\]/);
    assert.match(msg, /"foo":"bar"/);
});

test('circular objects fall back to String()', () => {
    const circular = {};
    circular.self = circular;
    const msg = formatAutosuggestError(circular);
    assert.match(msg, /HERE Autosuggest request failed/);
});

test('null and undefined do not throw', () => {
    assert.match(formatAutosuggestError(null), /HERE Autosuggest request failed/);
    assert.match(formatAutosuggestError(undefined), /HERE Autosuggest request failed/);
});

// ── buildAutosuggestParams ──

test('at is included when present', () => {
    const params = buildAutosuggestParams('Berlin', {
        limit: 5, lang: 'de', in: 'countryCode:DEU', at: { lat: 51.1657, lng: 10.4515 }
    });
    assert.deepEqual(params, {
        q: 'Berlin', limit: 5, lang: 'de', in: 'countryCode:DEU', at: '51.1657,10.4515'
    });
});

test('at:null from C# (In has circle/bbox) is omitted from the HERE params', () => {
    const params = buildAutosuggestParams('Berlin', {
        limit: 5, lang: 'de', in: 'circle:52.5,13.4;r=10000', at: null
    });
    assert.equal('at' in params, false);
    assert.equal(params.in, 'circle:52.5,13.4;r=10000');
});

test('missing limit falls back to 5', () => {
    const params = buildAutosuggestParams('Berlin', {});
    assert.equal(params.limit, 5);
});

test('show is included when present', () => {
    // Without show=details the HERE Autosuggest API returns only address.label
    // and none of the structured address fields.
    const params = buildAutosuggestParams('Berlin', {
        limit: 5, lang: 'de', in: 'countryCode:DEU',
        at: { lat: 51.1657, lng: 10.4515 }, show: 'details'
    });
    assert.deepEqual(params, {
        q: 'Berlin', limit: 5, lang: 'de', in: 'countryCode:DEU',
        show: 'details', at: '51.1657,10.4515'
    });
});

test('show:null from C# is omitted from the HERE params', () => {
    const params = buildAutosuggestParams('Berlin', {
        limit: 5, lang: 'de', in: 'countryCode:DEU',
        at: { lat: 51.1657, lng: 10.4515 }, show: null
    });
    assert.equal('show' in params, false);
});

// ── mapAutosuggestItems ──

test('details response maps all structured address fields', () => {
    // Response shape as returned with show=details (verified against the live API).
    const items = mapAutosuggestItems([{
        title: 'Falkensteinstraße 28, 46047 Oberhausen',
        id: 'here:af:streetsection:abc',
        resultType: 'houseNumber',
        address: {
            label: 'Falkensteinstraße 28, 46047 Oberhausen, Deutschland',
            countryCode: 'DEU',
            countryName: 'Deutschland',
            stateCode: 'NW',
            state: 'Nordrhein-Westfalen',
            countyCode: 'OB',
            county: 'Oberhausen',
            city: 'Oberhausen',
            district: 'Alstaden',
            street: 'Falkensteinstraße',
            postalCode: '46047',
            houseNumber: '28'
        },
        position: { lat: 51.4696, lng: 6.8344 }
    }]);

    assert.equal(items.length, 1);
    assert.deepEqual(items[0].address, {
        label: 'Falkensteinstraße 28, 46047 Oberhausen, Deutschland',
        countryCode: 'DEU',
        countryName: 'Deutschland',
        state: 'Nordrhein-Westfalen',
        stateCode: 'NW',
        county: 'Oberhausen',
        countyCode: 'OB',
        city: 'Oberhausen',
        district: 'Alstaden',
        street: 'Falkensteinstraße',
        postalCode: '46047',
        houseNumber: '28'
    });
    assert.deepEqual(items[0].position, { lat: 51.4696, lng: 6.8344 });
});

test('label-only response (no show=details) maps label and leaves structured fields null', () => {
    const items = mapAutosuggestItems([{
        title: 'Falkensteinstraße 28, 46047 Oberhausen',
        resultType: 'houseNumber',
        address: { label: 'Falkensteinstraße 28, 46047 Oberhausen, Deutschland' },
        position: { lat: 51.4696, lng: 6.8344 }
    }]);

    const addr = items[0].address;
    assert.equal(addr.label, 'Falkensteinstraße 28, 46047 Oberhausen, Deutschland');
    assert.equal(addr.countryCode, null);
    assert.equal(addr.state, null);
    assert.equal(addr.stateCode, null);
    assert.equal(addr.county, null);
    assert.equal(addr.countyCode, null);
    assert.equal(addr.city, null);
    assert.equal(addr.district, null);
    assert.equal(addr.street, null);
    assert.equal(addr.postalCode, null);
    assert.equal(addr.houseNumber, null);
});

test('items without address or position map to nulls', () => {
    const items = mapAutosuggestItems([{ title: 'Restaurants', resultType: 'categoryQuery' }]);
    assert.equal(items[0].address, null);
    assert.equal(items[0].position, null);
    assert.equal(items[0].highlights, null);
});

test('undefined items map to an empty array', () => {
    assert.deepEqual(mapAutosuggestItems(undefined), []);
});
