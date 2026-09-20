// JS-side tests for the geocode/reverseGeocode address-mapping helper in
// HerePlatform.Blazor/wwwroot/js/objectManager.js.
//
// Run from the repo root:
//   node --test tests/HerePlatform.Blazor.Tests.Js/geocodeHelpers.test.mjs
//
// objectManager.js is an IIFE intended for the Blazor static-files pipeline
// (no module exports), so the helper cannot be imported directly. The
// implementation below MUST mirror the source. If you edit mapGeocodeAddress
// in objectManager.js, mirror the change here and vice versa.

import { test } from 'node:test';
import assert from 'node:assert/strict';

// ---------- start mirror of objectManager.js helper ----------

function mapGeocodeAddress(item) {
    if (!item.address) return null;
    return {
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

// ---------- end mirror ----------

test('geocode item with a full address maps all structured fields', () => {
    // Response shape as returned by /geocode and /revgeocode by default —
    // unlike Autosuggest, no show=details equivalent is required.
    const address = mapGeocodeAddress({
        title: 'Falkensteinstraße 28, 46047 Oberhausen',
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
    });

    assert.deepEqual(address, {
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
});

test('geocode item without an address maps to null', () => {
    const address = mapGeocodeAddress({ title: 'Restaurants', resultType: 'categoryQuery' });
    assert.equal(address, null);
});

test('label-only address maps label and leaves structured fields null', () => {
    const address = mapGeocodeAddress({
        address: { label: 'Berlin, Deutschland' }
    });

    assert.equal(address.label, 'Berlin, Deutschland');
    assert.equal(address.countryCode, null);
    assert.equal(address.state, null);
    assert.equal(address.stateCode, null);
    assert.equal(address.county, null);
    assert.equal(address.countyCode, null);
    assert.equal(address.city, null);
    assert.equal(address.district, null);
    assert.equal(address.street, null);
    assert.equal(address.postalCode, null);
    assert.equal(address.houseNumber, null);
});
