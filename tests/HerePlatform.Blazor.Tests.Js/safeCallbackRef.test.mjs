// JS-side tests for the circuit-disconnect protection helpers in
// HerePlatform.Blazor/wwwroot/js/objectManager.js.
//
// Run from the repo root:
//   node --test tests/HerePlatform.Blazor.Tests.Js/safeCallbackRef.test.mjs
//
// These helpers cannot be imported directly because objectManager.js is an
// IIFE intended for the Blazor static-files pipeline (no module exports).
// The implementations below MUST mirror the source. If you edit safeCallbackRef
// / isTeardownError / ensureSafeCallbackRef in objectManager.js, mirror the
// change here and vice versa. Smoke test: a grep of "isTeardownError" should
// return one definition there and one here.

import { test } from 'node:test';
import assert from 'node:assert/strict';

// ---------- start mirror of objectManager.js helpers ----------

function isTeardownError(e) {
    if (!e) return false;
    var raw = (e && e.message) ? e.message : ('' + e);
    var msg = raw.toString().toLowerCase();
    if (msg.indexOf('jsdisconnectedexception') !== -1) return true;
    if (msg.indexOf('no tracked object with id') !== -1) return true;
    if (msg.indexOf('there is no tracked object') !== -1) return true;
    if (msg.indexOf('circuit has disconnected') !== -1) return true;
    if (msg.indexOf('circuit was disconnected') !== -1) return true;
    if (msg.indexOf('the circuit failed to initialize') !== -1) return true;
    return false;
}

function safeCallbackRef(ref) {
    const wrapper = {
        _disposed: false,
        invokeMethodAsync: function () {
            if (wrapper._disposed) return Promise.resolve();
            try {
                var result = ref.invokeMethodAsync.apply(ref, arguments);
                if (result && typeof result.then === 'function') {
                    return result.then(undefined, function (e) {
                        if (isTeardownError(e)) {
                            return;
                        }
                        throw e;
                    });
                }
                return result;
            }
            catch (e) {
                if (isTeardownError(e)) {
                    return Promise.resolve();
                }
                return Promise.reject(e);
            }
        },
        dispose: function () { wrapper._disposed = true; }
    };
    return wrapper;
}

function ensureSafeCallbackRef(ref) {
    if (!ref) return ref;
    if (typeof ref._disposed !== 'undefined') return ref;
    return safeCallbackRef(ref);
}

// ---------- end mirror ----------

function makeRef(behavior) {
    return {
        invokeMethodAsync: behavior,
        callCount: 0
    };
}

test('disposed wrapper returns a resolved Promise (not undefined)', async () => {
    const ref = { invokeMethodAsync: () => Promise.resolve('called') };
    const wrapper = safeCallbackRef(ref);

    wrapper.dispose();
    const result = wrapper.invokeMethodAsync('Foo');

    assert.ok(result && typeof result.then === 'function', 'expected a Promise');
    assert.equal(await result, undefined);
});

test('teardown rejection (JSDisconnectedException) is suppressed', async () => {
    const err = new Error('Microsoft.JSInterop.JSDisconnectedException: circuit gone');
    const ref = { invokeMethodAsync: () => Promise.reject(err) };
    const wrapper = safeCallbackRef(ref);

    await assert.doesNotReject(wrapper.invokeMethodAsync('Foo'));
});

test('teardown rejection ("no tracked object with id") is suppressed', async () => {
    const err = new Error("There is no tracked object with id '42'. Perhaps the DotNetObjectReference instance was already disposed.");
    const ref = { invokeMethodAsync: () => Promise.reject(err) };
    const wrapper = safeCallbackRef(ref);

    await assert.doesNotReject(wrapper.invokeMethodAsync('Foo'));
});

test('real .NET callback rejection propagates', async () => {
    const err = new Error('NullReferenceException: Object reference not set to an instance');
    const ref = { invokeMethodAsync: () => Promise.reject(err) };
    const wrapper = safeCallbackRef(ref);

    await assert.rejects(wrapper.invokeMethodAsync('Foo'), /NullReferenceException/);
});

test('synchronous teardown throw is converted to resolved Promise', async () => {
    const ref = { invokeMethodAsync: () => { throw new Error('JSDisconnectedException'); } };
    const wrapper = safeCallbackRef(ref);

    await assert.doesNotReject(wrapper.invokeMethodAsync('Foo'));
});

test('synchronous non-teardown throw becomes a rejected Promise', async () => {
    const ref = { invokeMethodAsync: () => { throw new Error('something else broke'); } };
    const wrapper = safeCallbackRef(ref);

    await assert.rejects(wrapper.invokeMethodAsync('Foo'), /something else broke/);
});

test('ensureSafeCallbackRef wraps a raw DotNetObjectReference', () => {
    const raw = { invokeMethodAsync: () => Promise.resolve() };
    const wrapped = ensureSafeCallbackRef(raw);

    assert.equal(typeof wrapped._disposed, 'boolean');
    assert.equal(typeof wrapped.dispose, 'function');
    assert.notEqual(wrapped, raw);
});

test('ensureSafeCallbackRef is idempotent on an already-wrapped ref', () => {
    const wrapper = safeCallbackRef({ invokeMethodAsync: () => Promise.resolve() });
    const reWrapped = ensureSafeCallbackRef(wrapper);

    assert.equal(reWrapped, wrapper, 'expected same wrapper, not a double-wrap');
});

test('ensureSafeCallbackRef passes through null/undefined', () => {
    assert.equal(ensureSafeCallbackRef(null), null);
    assert.equal(ensureSafeCallbackRef(undefined), undefined);
});

test('isTeardownError matches unambiguous Blazor teardown signatures', () => {
    assert.equal(isTeardownError(new Error('Microsoft.JSInterop.JSDisconnectedException: circuit gone')), true);
    // case-insensitive
    assert.equal(isTeardownError(new Error('jsdisconnectedexception')), true);
    assert.equal(isTeardownError(new Error("There is no tracked object with id '42'.")), true);
    assert.equal(isTeardownError(new Error('no tracked object with id 1')), true);
    assert.equal(isTeardownError(new Error('The circuit has disconnected and is being disposed.')), true);
    assert.equal(isTeardownError(new Error('The circuit was disconnected from the client.')), true);
    assert.equal(isTeardownError(new Error('The circuit failed to initialize.')), true);
});

test('isTeardownError does NOT swallow ambiguous .NET exception types', () => {
    // Reviewer-Blocking: ObjectDisposedException / TaskCanceledException /
    // OperationCanceledException können aus echten Anwendungs-Bugs stammen.
    // Sie dürfen nicht pauschal anhand des Typnamens suppressed werden.
    assert.equal(isTeardownError(new Error('System.ObjectDisposedException: Cannot access a disposed object.')), false);
    assert.equal(isTeardownError(new Error('TaskCanceledException: A task was canceled.')), false);
    assert.equal(isTeardownError(new Error('OperationCanceledException')), false);
});

test('isTeardownError does NOT match loose "circuit" + "disconnected" combinations', () => {
    // Vorher: msg.indexOf('circuit') && msg.indexOf('disconnected') hätte hier matched.
    assert.equal(isTeardownError(new Error('user disconnected from circuit board diagnostic')), false);
});

test('isTeardownError negative classification', () => {
    assert.equal(isTeardownError(new Error('NullReferenceException')), false);
    assert.equal(isTeardownError(new Error('Some unrelated failure')), false);
    assert.equal(isTeardownError(null), false);
    assert.equal(isTeardownError(undefined), false);
});
