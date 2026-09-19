// JS-side tests for the autosuggest keyboard handling in
// HerePlatform.Blazor/wwwroot/js/objectManager.js.
//
// Run from the repo root:
//   node --test tests/HerePlatform.Blazor.Tests.Js/autosuggestKeyboard.test.mjs
//
// Unlike autosuggestHelpers.test.mjs these tests do NOT mirror the implementation: the real
// objectManager.js is evaluated in a vm context with a minimal window stub, so attach/detach,
// the delegated listener and disposeAutosuggest are exercised as shipped. Nothing here can
// drift away from the source.

import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { createContext, runInContext } from 'node:vm';
import { dirname, join } from 'node:path';

const repoRoot = join(dirname(new URL(import.meta.url).pathname), '..', '..');
const source = readFileSync(join(repoRoot, 'HerePlatform.Blazor/wwwroot/js/objectManager.js'), 'utf8');

// objectManager.js is an IIFE assigned to window.herePlatform; at load time it only touches
// window and document.querySelectorAll (to derive its static-asset base path).
function loadObjectManager() {
    const document = { querySelectorAll: () => [], createElement: () => createElement() };
    const sandbox = { window: {}, document, console, crypto };
    sandbox.globalThis = sandbox;
    createContext(sandbox);
    runInContext(source, sandbox);
    return sandbox.window.herePlatform.objectManager;
}

// Minimal element stub: attributes, listener registry and event dispatch.
function createElement(attributes = {}) {
    return {
        attributes: { ...attributes },
        listeners: {},
        getAttribute(name) {
            return Object.prototype.hasOwnProperty.call(this.attributes, name) ? this.attributes[name] : null;
        },
        setAttribute(name, value) { this.attributes[name] = value; },
        matches(selector) {
            const attribute = selector.replace(/^\[|\]$/g, '');
            return Object.prototype.hasOwnProperty.call(this.attributes, attribute);
        },
        addEventListener(type, handler) {
            (this.listeners[type] = this.listeners[type] || []).push(handler);
        },
        removeEventListener(type, handler) {
            this.listeners[type] = (this.listeners[type] || []).filter(h => h !== handler);
        },
        listenerCount(type) { return (this.listeners[type] || []).length; },
        dispatchKeyDown(key, target) {
            const ev = {
                key,
                target: target || this,
                defaultPrevented: false,
                preventDefault() { this.defaultPrevented = true; }
            };
            for (const handler of this.listeners.keydown || []) handler(ev);
            return ev;
        }
    };
}

// The wrapper div Blazor renders, with the marked input inside it.
function createComponent(isOpen = false) {
    const root = createElement({ 'data-here-autosuggest-open': isOpen ? 'true' : 'false' });
    const input = createElement({ 'data-here-autosuggest-input': '' });
    root.input = input;
    root.setOpen = open => root.setAttribute('data-here-autosuggest-open', open ? 'true' : 'false');
    return root;
}

const SUPPRESSED_KEYS = ['Enter', 'ArrowUp', 'ArrowDown'];
const PASSTHROUGH_KEYS = [
    'a', 'B', '7', ' ', 'ä', 'Backspace', 'Delete', 'Tab', 'Home', 'End',
    'ArrowLeft', 'ArrowRight', 'Escape', 'Shift', 'Control'
];

test('attach registers exactly one delegated keydown listener on the wrapper', () => {
    const om = loadObjectManager();
    const root = createComponent();

    om.attachAutosuggestKeyboard('guid-1', root);

    assert.equal(root.listenerCount('keydown'), 1);
});

test('open dropdown: Enter, ArrowUp and ArrowDown are suppressed', () => {
    const om = loadObjectManager();
    const root = createComponent(true);
    om.attachAutosuggestKeyboard('guid-1', root);

    for (const key of SUPPRESSED_KEYS) {
        assert.equal(root.dispatchKeyDown(key, root.input).defaultPrevented, true, key);
    }
});

test('open dropdown: typing, deleting and tabbing keep their browser default', () => {
    const om = loadObjectManager();
    const root = createComponent(true);
    om.attachAutosuggestKeyboard('guid-1', root);

    for (const key of PASSTHROUGH_KEYS) {
        assert.equal(root.dispatchKeyDown(key, root.input).defaultPrevented, false, key);
    }
});

test('closed dropdown: no key is suppressed', () => {
    const om = loadObjectManager();
    const root = createComponent(false);
    om.attachAutosuggestKeyboard('guid-1', root);

    for (const key of SUPPRESSED_KEYS.concat(PASSTHROUGH_KEYS)) {
        assert.equal(root.dispatchKeyDown(key, root.input).defaultPrevented, false, key);
    }
});

test('the listener follows the rendered open state, it caches nothing', () => {
    const om = loadObjectManager();
    const root = createComponent(false);
    om.attachAutosuggestKeyboard('guid-1', root);

    assert.equal(root.dispatchKeyDown('Enter', root.input).defaultPrevented, false);
    root.setOpen(true);
    assert.equal(root.dispatchKeyDown('Enter', root.input).defaultPrevented, true);
    root.setOpen(false);
    assert.equal(root.dispatchKeyDown('Enter', root.input).defaultPrevented, false);
});

test('keystrokes from elements without the marker are ignored', () => {
    const om = loadObjectManager();
    const root = createComponent(true);
    om.attachAutosuggestKeyboard('guid-1', root);

    const unrelated = createElement();
    assert.equal(root.dispatchKeyDown('Enter', unrelated).defaultPrevented, false);
});

test('re-attaching with the same guid leaves a single listener', () => {
    const om = loadObjectManager();
    const root = createComponent(true);

    om.attachAutosuggestKeyboard('guid-1', root);
    om.attachAutosuggestKeyboard('guid-1', root);
    om.attachAutosuggestKeyboard('guid-1', root);

    assert.equal(root.listenerCount('keydown'), 1);
    assert.equal(root.dispatchKeyDown('Enter', root.input).defaultPrevented, true);
});

test('multiple component instances stay independent', () => {
    const om = loadObjectManager();
    const first = createComponent(true);
    const second = createComponent(false);

    om.attachAutosuggestKeyboard('guid-1', first);
    om.attachAutosuggestKeyboard('guid-2', second);

    assert.equal(first.dispatchKeyDown('Enter', first.input).defaultPrevented, true);
    assert.equal(second.dispatchKeyDown('Enter', second.input).defaultPrevented, false);

    // Disposing one must not touch the other.
    om.disposeAutosuggest('guid-1');
    assert.equal(first.listenerCount('keydown'), 0);
    assert.equal(second.listenerCount('keydown'), 1);
});

test('disposeAutosuggest removes the listener', () => {
    const om = loadObjectManager();
    const root = createComponent(true);
    om.attachAutosuggestKeyboard('guid-1', root);

    om.disposeAutosuggest('guid-1');

    assert.equal(root.listenerCount('keydown'), 0);
    assert.equal(root.dispatchKeyDown('Enter', root.input).defaultPrevented, false);
});

test('disposeAutosuggest is safe without a previous attach and when called twice', () => {
    const om = loadObjectManager();
    const root = createComponent(true);

    om.disposeAutosuggest('never-attached');
    om.attachAutosuggestKeyboard('guid-1', root);
    om.disposeAutosuggest('guid-1');
    om.disposeAutosuggest('guid-1');

    assert.equal(root.listenerCount('keydown'), 0);
});

test('a search request does not clobber the listener bookkeeping', () => {
    const om = loadObjectManager();
    const root = createComponent(true);
    om.attachAutosuggestKeyboard('guid-1', root);

    // Without an initialized HERE platform the request short-circuits, but it must not drop the
    // listener state stored under the same guid.
    const callbackRef = { invokeMethodAsync: () => Promise.resolve() };
    om.autosuggest('guid-1', 'Berlin', { limit: 5 }, callbackRef);

    assert.equal(root.listenerCount('keydown'), 1);
    assert.equal(root.dispatchKeyDown('Enter', root.input).defaultPrevented, true);

    om.disposeAutosuggest('guid-1');
    assert.equal(root.listenerCount('keydown'), 0);
});

test('attach is a no-op for a missing or non-element root', () => {
    const om = loadObjectManager();

    assert.doesNotThrow(() => om.attachAutosuggestKeyboard('guid-1', null));
    assert.doesNotThrow(() => om.attachAutosuggestKeyboard('guid-2', {}));
});
