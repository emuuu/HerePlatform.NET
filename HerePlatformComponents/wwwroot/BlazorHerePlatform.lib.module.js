// Blazor JavaScript Initializer — auto-loads objectManager.js before the app starts.
// See https://learn.microsoft.com/aspnet/core/blazor/fundamentals/startup#javascript-initializers

// .NET 6/7 (legacy Blazor WASM / Server)
export function beforeStart() { return loadObjectManager(); }
export function afterStarted() { }

// .NET 8+ (Blazor Web App)
export function beforeWebStart() { return loadObjectManager(); }
export function beforeServerStart() { return loadObjectManager(); }
export function afterWebStarted() { }
export function afterServerStarted() { }

function loadObjectManager() {
    if (window.blazorHerePlatform?.objectManager) return Promise.resolve();

    return new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.src = '_content/BlazorHerePlatform/js/objectManager.js';
        script.onload = resolve;
        script.onerror = () => reject(new Error('Failed to load BlazorHerePlatform objectManager.js'));
        document.head.appendChild(script);
    });
}
