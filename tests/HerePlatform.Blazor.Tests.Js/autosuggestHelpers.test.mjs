// JS-side tests for the autosuggest helpers in
// HerePlatform.Blazor/wwwroot/js/objectManager.js.
//
// Run from the repo root:
//   node --test tests/HerePlatform.Blazor.Tests.Js/autosuggestHelpers.test.mjs
//
// These helpers cannot be imported directly because objectManager.js is an
// IIFE intended for the Blazor static-files pipeline (no module exports).
// The implementations below MUST mirror the source. If you edit
// formatAutosuggestError / buildAutosuggestParams in objectManager.js, mirror
// the change here and vice versa.

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
    if (options.at) params.at = options.at.lat + ',' + options.at.lng;
    return params;
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
