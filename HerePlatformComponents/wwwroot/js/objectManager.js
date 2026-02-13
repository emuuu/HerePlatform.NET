window.blazorHerePlatform = window.blazorHerePlatform || {};

window.blazorHerePlatform.objectManager = function () {

    const mapObjects = {};
    let herePlatform = null;
    let defaultLayers = null;
    let defaultLayerOpts = {};
    let apiKeyValid = false;
    let harpEngineType = null;
    let uiCssUrl = null;
    let hereBaseUrl = null;
    let hereApiKey = null;

    // Derive the library's static-asset base path from this script's URL.
    // e.g. "_content/BlazorHerePlatform/" from "…/js/objectManager.js"
    const libBasePath = (function () {
        const scripts = document.querySelectorAll('script[src*="objectManager.js"]');
        if (scripts.length > 0) {
            return scripts[scripts.length - 1].src.replace(/js\/objectManager\.js.*$/, '');
        }
        return '';
    })();

    function uuidv4() {
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
            (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
        );
    }

    function addMapObject(guid, obj) {
        mapObjects[guid] = obj;
    }

    function getMapObject(guid) {
        return mapObjects[guid];
    }

    function removeMapObject(guid) {
        delete mapObjects[guid];
    }

    function loadScript(url) {
        return new Promise((resolve, reject) => {
            if (document.querySelector(`script[src="${url}"]`)) {
                resolve();
                return;
            }
            const script = document.createElement('script');
            script.src = url;
            script.onload = resolve;
            script.onerror = reject;
            document.head.appendChild(script);
        });
    }

    function loadCSS(url) {
        return new Promise((resolve) => {
            if (document.querySelector(`link[href="${url}"]`)) {
                resolve();
                return;
            }
            const link = document.createElement('link');
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.href = url;
            link.onload = resolve;
            document.head.appendChild(link);
        });
    }

    function tryParseJson(item) {
        if (typeof item !== 'string') return item;
        try {
            const parsed = JSON.parse(item);
            // Check if it's a JsObjectRef (has guid property)
            if (parsed && parsed.guid) {
                const ref = mapObjects[parsed.guid];
                if (ref !== undefined) return ref;
            }
            return parsed;
        } catch {
            return item;
        }
    }

    function resolveArgs(args) {
        return args.map(arg => tryParseJson(arg));
    }

    function isHereApiLoaded() {
        return typeof H !== 'undefined' && H.Map !== undefined;
    }

    // Ensures the HERE Maps UI CSS and our override CSS are present in
    // <head> and in the correct order.  Called after every map creation
    // because Blazor enhanced navigation can remove dynamic <link> tags.
    const overrideCssUrl = libBasePath + 'css/here-platform.css';

    function ensureUiCssOverrides() {
        // 1. Re-add mapsjs-ui.css if Blazor navigation removed the <link>
        if (uiCssUrl && !document.querySelector(`link[href="${uiCssUrl}"]`)) {
            loadCSS(uiCssUrl);
        }

        // 2. Ensure our override CSS is loaded and comes LAST in <head>
        const existing = document.querySelector(`link[href="${overrideCssUrl}"]`);
        if (existing) {
            // Move to end of <head> to guarantee cascade order
            document.head.appendChild(existing);
        } else {
            loadCSS(overrideCssUrl);
        }
    }

    // Custom JSON stringify that handles circular references and special types
    function extendableStringify(obj) {
        const seen = new WeakSet();
        return JSON.stringify(obj, function (key, value) {
            if (typeof value === 'object' && value !== null) {
                if (seen.has(value)) return undefined;
                seen.add(value);
            }
            // Strip DOM elements and functions
            if (value instanceof HTMLElement) return undefined;
            if (typeof value === 'function') return undefined;
            return value;
        });
    }

    // Extract rich event data from HERE pointer events for C# MapPointerEventArgs
    function extractPointerEventData(evt, map) {
        const data = {
            type: evt.type || null,
            viewportX: 0,
            viewportY: 0,
            position: null,
            button: 0,
            buttons: 0,
            pointerType: null
        };
        if (evt.currentPointer) {
            data.viewportX = evt.currentPointer.viewportX;
            data.viewportY = evt.currentPointer.viewportY;
            data.button = evt.currentPointer.button || 0;
            data.buttons = evt.currentPointer.buttons || 0;
            data.pointerType = evt.currentPointer.type || null;
            if (map && typeof map.screenToGeo === 'function') {
                try {
                    const geo = map.screenToGeo(evt.currentPointer.viewportX, evt.currentPointer.viewportY);
                    if (geo) {
                        data.position = { lat: geo.lat, lng: geo.lng };
                    }
                } catch (e) { }
            }
        }
        return data;
    }

    // Extract event data from HERE drag events for C# MapDragEventArgs
    function extractDragEventData(evt, map) {
        const data = {
            type: evt.type || null,
            viewportX: 0,
            viewportY: 0,
            position: null
        };
        if (evt.currentPointer) {
            data.viewportX = evt.currentPointer.viewportX;
            data.viewportY = evt.currentPointer.viewportY;
        }
        // Use target geometry for position (most accurate for dragend)
        if (evt.target && evt.target.getGeometry) {
            try {
                const geo = evt.target.getGeometry();
                if (geo && typeof geo.lat === 'number') {
                    data.position = { lat: geo.lat, lng: geo.lng };
                }
            } catch (e) { }
        }
        // Fallback to screen-to-geo conversion
        if (!data.position && evt.currentPointer && map && typeof map.screenToGeo === 'function') {
            try {
                const geo = map.screenToGeo(evt.currentPointer.viewportX, evt.currentPointer.viewportY);
                if (geo) {
                    data.position = { lat: geo.lat, lng: geo.lng };
                }
            } catch (e) { }
        }
        return data;
    }

    // Extract rich event data for the imperative AddListener API
    function extractEventDataForImperative(evt) {
        const eventData = {};
        if (evt) {
            eventData.type = evt.type || null;
            if (evt.target && evt.target.getGeometry) {
                const geo = evt.target.getGeometry();
                if (geo && typeof geo.lat === 'number') {
                    eventData.lat = geo.lat;
                    eventData.lng = geo.lng;
                }
            }
            if (evt.currentPointer) {
                eventData.viewportX = evt.currentPointer.viewportX;
                eventData.viewportY = evt.currentPointer.viewportY;
                eventData.button = evt.currentPointer.button || 0;
                eventData.buttons = evt.currentPointer.buttons || 0;
                eventData.pointerType = evt.currentPointer.type || null;
            }
        }
        return eventData;
    }

    // Wire all pointer/interaction events on a map object (marker, polygon)
    // and forward them to C# via the unified OnObject* JSInvokable methods.
    function wireObjectEvents(obj, objectType, id, callbackRef, map) {
        const pointerEvents = [
            'tap', 'dbltap', 'longpress', 'contextmenu',
            'pointerdown', 'pointerup', 'pointermove',
            'pointerenter', 'pointerleave'
        ];

        for (const eventName of pointerEvents) {
            obj.addEventListener(eventName, function (evt) {
                const data = extractPointerEventData(evt, map);
                callbackRef.invokeMethodAsync('OnObjectPointerEvent', objectType, id, eventName, data);
            });
        }

        obj.addEventListener('contextmenuclose', function () {
            callbackRef.invokeMethodAsync('OnObjectSimpleEvent', objectType, id, 'contextmenuclose');
        });
        obj.addEventListener('pointercancel', function () {
            callbackRef.invokeMethodAsync('OnObjectSimpleEvent', objectType, id, 'pointercancel');
        });
    }

    return {
        resetPlatform: function () {
            // Dispose all existing map objects
            for (const guid of Object.keys(mapObjects)) {
                const obj = mapObjects[guid];
                if (obj && typeof obj.dispose === 'function') {
                    try { obj.dispose(); } catch (e) { }
                }
            }
            for (const key of Object.keys(mapObjects)) {
                delete mapObjects[key];
            }
            herePlatform = null;
            defaultLayers = null;
            apiKeyValid = false;
            harpEngineType = null;
        },

        initMap: async function (apiOptions) {
            if (herePlatform) {
                // Platform already initialized — load any additional modules that weren't loaded before
                var extraUrl = apiOptions.baseUrl || `https://js.api.here.com/v3/${apiOptions.version || '3.1'}`;
                hereBaseUrl = hereBaseUrl || extraUrl;
                var extraLoads = [];
                if (apiOptions.loadClustering && typeof H !== 'undefined' && !H.clustering) {
                    extraLoads.push(loadScript(extraUrl + '/mapsjs-clustering.js'));
                }
                if (apiOptions.loadData && (typeof H === 'undefined' || !H.data || !H.data.heatmap)) {
                    extraLoads.push(loadScript(extraUrl + '/mapsjs-data.js'));
                }
                if (extraLoads.length > 0) {
                    await Promise.all(extraLoads);
                }
                return;
            }

            const key = apiOptions.apiKey;
            if (!key || key === 'YOUR_API_KEY' || key.trim() === '') {
                console.warn('[BlazorHerePlatform] No valid HERE API key configured. Map will not be initialized. ' +
                    'Set a valid API key via services.AddBlazorHerePlatform("your-key") or in appsettings.json.');
                apiKeyValid = false;
                return;
            }

            const baseUrl = apiOptions.baseUrl || `https://js.api.here.com/v3/${apiOptions.version || '3.1'}`;
            hereBaseUrl = baseUrl;
            hereApiKey = key;

            // Load core module first
            await loadScript(`${baseUrl}/mapsjs-core.js`);
            await loadScript(`${baseUrl}/mapsjs-service.js`);

            const loadPromises = [];

            if (apiOptions.loadMapEvents !== false) {
                loadPromises.push(loadScript(`${baseUrl}/mapsjs-mapevents.js`));
            }
            if (apiOptions.loadUI !== false) {
                loadPromises.push(loadScript(`${baseUrl}/mapsjs-ui.js`));
                uiCssUrl = `${baseUrl}/mapsjs-ui.css`;
                loadPromises.push(loadCSS(uiCssUrl));
            }
            if (apiOptions.loadClustering) {
                loadPromises.push(loadScript(`${baseUrl}/mapsjs-clustering.js`));
            }
            if (apiOptions.loadData) {
                loadPromises.push(loadScript(`${baseUrl}/mapsjs-data.js`));
            }
            if (apiOptions.useHarpEngine) {
                loadPromises.push(loadScript(`${baseUrl}/mapsjs-harp.js`));
            }

            await Promise.all(loadPromises);

            herePlatform = new H.service.Platform({
                apikey: key
            });

            defaultLayerOpts = {};
            if (apiOptions.useHarpEngine && H.Map && H.Map.EngineType && H.Map.EngineType['HARP']) {
                harpEngineType = H.Map.EngineType['HARP'];
                defaultLayerOpts.engineType = harpEngineType;
            }
            if (apiOptions.language) {
                defaultLayerOpts.lg = apiOptions.language;
            }

            try {
                defaultLayers = herePlatform.createDefaultLayers(defaultLayerOpts);
            } catch (e) {
                // HARP engine failed — fall back to non-HARP
                if (defaultLayerOpts.engineType) {
                    console.warn('[BlazorHerePlatform] HARP engine failed, falling back to default engine.');
                    delete defaultLayerOpts.engineType;
                    harpEngineType = null;
                    defaultLayers = herePlatform.createDefaultLayers(defaultLayerOpts);
                } else {
                    throw e;
                }
            }

            apiKeyValid = true;
        },

        isHereMapsReady: function () {
            // Returns true even without a valid key so components can render the placeholder
            return herePlatform !== null || apiKeyValid === false;
        },

        createHereMap: function (container, options) {
            const mapGuid = uuidv4();

            if (!apiKeyValid || !herePlatform) {
                // Show a placeholder instead of crashing with tile errors
                container.style.display = 'flex';
                container.style.alignItems = 'center';
                container.style.justifyContent = 'center';
                container.style.backgroundColor = '#f0f0f0';
                container.style.border = '2px dashed #ccc';
                container.style.color = '#666';
                container.style.fontFamily = 'sans-serif';
                container.style.fontSize = '14px';
                container.style.textAlign = 'center';
                container.style.padding = '20px';
                container.innerHTML = '<div><strong>HERE Map</strong><br/>No valid API key configured.<br/>' +
                    '<small>Set your key via <code>services.AddBlazorHerePlatform("your-key")</code><br/>' +
                    'or in <code>appsettings.json</code> under <code>HerePlatform:ApiKey</code></small></div>';

                // Store a placeholder sentinel so invoke() can return safe defaults
                addMapObject(mapGuid, { _blzPlaceholder: true });
                return { mapGuid: mapGuid, behaviorGuid: null, uiGuid: null };
            }

            // Create FRESH defaultLayers per map instance.  map.dispose()
            // corrupts the layer objects it was constructed with, so sharing
            // a single global defaultLayers across maps causes rendering
            // failures (broken InfoBubble triangles, missing tiles) after
            // the first map is disposed during page navigation.
            const mapLayers = herePlatform.createDefaultLayers(defaultLayerOpts);

            // Resolve layer from options
            let baseLayer = mapLayers.vector.normal.map;
            let initialOverlays = [];
            if (options && options.layerType) {
                const layerPath = options.layerType;
                try {
                    const parts = layerPath.split('.');
                    if (parts[0] === 'hybrid') {
                        const group = parts[1];
                        const component = parts[2];
                        const hybridGroup = mapLayers.hybrid[group];
                        if (hybridGroup) {
                            baseLayer = hybridGroup.raster;
                            if (component === 'vector' || component === 'traffic') {
                                initialOverlays.push(hybridGroup.vector);
                            }
                            if (component === 'traffic') {
                                initialOverlays.push(hybridGroup.traffic);
                            }
                        }
                    } else {
                        let layer = mapLayers;
                        for (const part of parts) {
                            layer = layer[part];
                        }
                        if (layer) baseLayer = layer;
                    }
                } catch (e) {
                    console.warn('Could not resolve layer type:', layerPath, e);
                }
            }

            const mapOptions = {
                zoom: options?.zoom ?? 10,
                center: options?.center ?? { lat: 52.5, lng: 13.4 }
            };

            if (options?.tilt != null) mapOptions.tilt = options.tilt;
            if (options?.heading != null) mapOptions.heading = options.heading;

            const engineOpt = harpEngineType ? { engineType: harpEngineType } : {};
            const map = new H.Map(container, baseLayer, { ...mapOptions, ...engineOpt });

            // Store per-map layers for setBaseLayer lookups
            map['_blzLayers'] = mapLayers;
            map['_blzOverlays'] = [];
            for (const ol of initialOverlays) {
                map.addLayer(ol);
                map['_blzOverlays'].push(ol);
            }

            addMapObject(mapGuid, map);

            const result = { mapGuid: mapGuid, behaviorGuid: null, uiGuid: null };

            // Add map events + behavior for interaction
            if (options?.enableInteraction !== false && typeof H.mapevents !== 'undefined') {
                const mapEvents = new H.mapevents.MapEvents(map);
                const behavior = new H.mapevents.Behavior(mapEvents);
                const behaviorGuid = uuidv4();
                addMapObject(behaviorGuid, behavior);
                result.behaviorGuid = behaviorGuid;
                map['_blzBehavior'] = behavior;
                map['_blzBehaviorGuid'] = behaviorGuid;
            }

            // Add default UI with per-map layers
            if (options?.enableUI !== false && H.ui && H.ui.UI) {
                const locale = options?.uiLocale || undefined;
                const ui = H.ui.UI.createDefault(map, mapLayers, locale);
                const uiGuid = uuidv4();
                addMapObject(uiGuid, ui);
                result.uiGuid = uiGuid;
                map['_blzUiGuid'] = uiGuid;
                map['_blzUI'] = ui;
                ensureUiCssOverrides();
            }

            // Handle resize
            window.addEventListener('resize', () => map.getViewPort().resize());

            return result;
        },

        createObject: function (args) {
            if (!isHereApiLoaded()) {
                console.warn('[BlazorHerePlatform] HERE API not loaded. Cannot create object.');
                return null;
            }

            const resolvedArgs = resolveArgs(args);
            const [guid, constructorName, ...ctorArgs] = resolvedArgs;

            // Special handling for constructors that need LineString conversion
            if (constructorName === 'H.map.Polyline' || constructorName === 'H.map.Polygon') {
                const pathData = ctorArgs[0]; // array of {lat, lng}
                const options = ctorArgs[1] || {};

                const lineString = new H.geo.LineString();
                if (Array.isArray(pathData)) {
                    pathData.forEach(p => lineString.pushPoint(p));
                }

                let obj;
                if (constructorName === 'H.map.Polyline') {
                    obj = new H.map.Polyline(lineString, options);
                } else {
                    obj = new H.map.Polygon(lineString, options);
                }

                addMapObject(guid, obj);
                return guid;
            }

            // Special handling for Circle
            if (constructorName === 'H.map.Circle') {
                const center = ctorArgs[0];
                const radius = ctorArgs[1];
                const options = ctorArgs[2] || {};
                const circle = new H.map.Circle(center, radius, options);
                addMapObject(guid, circle);
                return guid;
            }

            // Special handling for Rect
            if (constructorName === 'H.map.Rect') {
                const bounds = ctorArgs[0]; // {top, left, bottom, right}
                const options = ctorArgs[1] || {};
                const rect = new H.map.Rect(
                    new H.geo.Rect(bounds.top, bounds.left, bounds.bottom, bounds.right),
                    options
                );
                addMapObject(guid, rect);
                return guid;
            }

            // Special handling for InfoBubble
            if (constructorName === 'H.ui.InfoBubble') {
                const position = ctorArgs[0];
                const options = ctorArgs[1] || {};
                const bubble = new H.ui.InfoBubble(position, options);
                addMapObject(guid, bubble);
                return guid;
            }

            // Special handling for Icon
            if (constructorName === 'H.map.Icon') {
                const bitmap = ctorArgs[0];
                const options = ctorArgs[1];
                const icon = options ? new H.map.Icon(bitmap, options) : new H.map.Icon(bitmap);
                addMapObject(guid, icon);
                return guid;
            }

            // Special handling for DomIcon
            if (constructorName === 'H.map.DomIcon') {
                const html = ctorArgs[0];
                const options = ctorArgs[1];
                const domIcon = options ? new H.map.DomIcon(html, options) : new H.map.DomIcon(html);
                addMapObject(guid, domIcon);
                return guid;
            }

            // Generic object creation
            const parts = constructorName.split('.');
            let ctor = window;
            for (const part of parts) {
                ctor = ctor[part];
                if (!ctor) {
                    console.error(`Constructor ${constructorName} not found`);
                    return null;
                }
            }

            const obj = new ctor(...ctorArgs);
            addMapObject(guid, obj);
            return guid;
        },

        invoke: function (args) {
            const resolvedArgs = resolveArgs(args);
            const [guid, methodName, ...methodArgs] = resolvedArgs;
            const obj = mapObjects[guid];

            if (!obj) {
                console.warn(`Object with guid ${guid} not found`);
                return null;
            }

            // Placeholder maps (no API key) — return safe defaults for common queries
            if (obj._blzPlaceholder) {
                const placeholderDefaults = {
                    'getCenter': { lat: 0, lng: 0 },
                    'getZoom': 0
                };
                if (methodName in placeholderDefaults) return placeholderDefaults[methodName];
                return null;
            }

            // Special handling for addEventListener - wrap with GUID tracking
            if (methodName === 'addEventListener') {
                const [eventName, callback] = methodArgs;
                const handlerGuid = uuidv4();

                let handler;
                if (callback && callback.invokeMethodAsync) {
                    // DotNet callback — extract rich event data
                    handler = function (evt) {
                        const eventData = extractEventDataForImperative(evt);
                        const json = extendableStringify([eventData]);
                        callback.invokeMethodAsync('Invoke', json, guid);
                    };
                } else {
                    handler = callback;
                }

                obj.addEventListener(eventName, handler);

                // Store the handler reference for removal
                const listenerInfo = { obj, eventName, handler };
                addMapObject(handlerGuid, listenerInfo);
                return handlerGuid;
            }

            // Special handling for addEventListenerOnce
            if (methodName === 'addEventListenerOnce') {
                const [eventName, callback] = methodArgs;
                const handlerGuid = uuidv4();

                let handler;
                if (callback && callback.invokeMethodAsync) {
                    handler = function (evt) {
                        const eventData = extractEventDataForImperative(evt);
                        const json = extendableStringify([eventData]);
                        callback.invokeMethodAsync('Invoke', json, guid);
                        // Auto-remove after first invocation
                        obj.removeEventListener(eventName, handler);
                    };
                } else {
                    const origCallback = callback;
                    handler = function (evt) {
                        origCallback(evt);
                        obj.removeEventListener(eventName, handler);
                    };
                }

                obj.addEventListener(eventName, handler);

                const listenerInfo = { obj, eventName, handler };
                addMapObject(handlerGuid, listenerInfo);
                return handlerGuid;
            }

            // Special handling for remove (event listener removal)
            if (methodName === 'remove') {
                const listenerInfo = obj;
                if (listenerInfo && listenerInfo.obj && listenerInfo.eventName && listenerInfo.handler) {
                    listenerInfo.obj.removeEventListener(listenerInfo.eventName, listenerInfo.handler);
                }
                removeMapObject(guid);
                return null;
            }

            // General method invocation
            const method = obj[methodName];
            if (typeof method === 'function') {
                const result = method.apply(obj, methodArgs);
                // Don't return 'this' (chaining pattern) - complex objects can't be serialized by JSInterop
                if (result === obj) return undefined;
                // For simple types, return directly
                if (result == null || typeof result === 'number' || typeof result === 'string' || typeof result === 'boolean') {
                    return result;
                }
                // For complex objects, return a plain copy (strips prototypes/methods/circular refs)
                try {
                    return JSON.parse(JSON.stringify(result));
                } catch (e) {
                    return undefined;
                }
            }

            // Try as property getter
            return obj[methodName];
        },

        // Change the base layer of the map.
        // Hybrid layers are composites: raster=base, vector=overlay, traffic=overlay.
        setBaseLayer: function (mapGuid, layerPath) {
            const map = mapObjects[mapGuid];
            if (!map || map._blzPlaceholder) return;

            // Use per-map layers (immune to dispose corruption)
            const layers = map['_blzLayers'] || defaultLayers;
            if (!layers) return;

            try {
                // Remove any previously added overlay layers from hybrid switching
                const prevOverlays = map['_blzOverlays'];
                if (prevOverlays && prevOverlays.length > 0) {
                    for (const ol of prevOverlays) {
                        try { map.removeLayer(ol); } catch (e) { }
                    }
                }
                map['_blzOverlays'] = [];

                const parts = layerPath.split('.');

                // Hybrid layers need composite handling
                if (parts[0] === 'hybrid') {
                    const group = parts[1];
                    const component = parts[2];
                    const hybridGroup = layers.hybrid[group];

                    if (!hybridGroup) {
                        console.warn('[BlazorHerePlatform] Unknown hybrid group:', group);
                        return;
                    }

                    // Always set the raster as the base layer (satellite imagery)
                    map.setBaseLayer(hybridGroup.raster);

                    // Add vector overlay if requested (vector or traffic)
                    if (component === 'vector' || component === 'traffic') {
                        map.addLayer(hybridGroup.vector);
                        map['_blzOverlays'].push(hybridGroup.vector);
                    }

                    // Add traffic overlay if requested
                    if (component === 'traffic') {
                        map.addLayer(hybridGroup.traffic);
                        map['_blzOverlays'].push(hybridGroup.traffic);
                    }
                } else {
                    // Non-hybrid layers: resolve and set directly
                    let layer = layers;
                    for (const part of parts) {
                        layer = layer[part];
                    }
                    if (layer) {
                        map.setBaseLayer(layer);
                    }
                }
            } catch (e) {
                console.warn('[BlazorHerePlatform] Could not set base layer:', layerPath, e);
            }
        },

        // Tilt/heading require the ViewModel API in HERE Maps v3.1
        setMapLookAt: function (mapGuid, data) {
            const map = mapObjects[mapGuid];
            if (!map || map._blzPlaceholder) return;
            map.getViewModel().setLookAtData(data);
        },

        getMapLookAt: function (mapGuid) {
            const map = mapObjects[mapGuid];
            if (!map || map._blzPlaceholder) return { tilt: 0, heading: 0 };
            const data = map.getViewModel().getLookAtData();
            return { tilt: data.tilt || 0, heading: data.heading || 0 };
        },

        invokeWithReturnedObjectRef: async function (args) {
            const result = await blazorHerePlatform.objectManager.invoke(args);
            const uuid = uuidv4();
            addMapObject(uuid, result);
            return uuid;
        },

        addObjectToMap: function (mapGuid, objGuid) {
            const map = mapObjects[mapGuid];
            const obj = mapObjects[objGuid];
            if (map && obj && !map._blzPlaceholder) {
                map.addObject(obj);
            }
        },

        removeObjectFromMap: function (mapGuid, objGuid) {
            const map = mapObjects[mapGuid];
            const obj = mapObjects[objGuid];
            if (map && obj && !map._blzPlaceholder) {
                try {
                    map.removeObject(obj);
                } catch (e) {
                    // Object might not be on the map
                }
            }
        },

        addObjectsToMap: function (mapGuid, objGuids) {
            const map = mapObjects[mapGuid];
            if (!map || map._blzPlaceholder) return;
            const objects = objGuids.map(g => mapObjects[g]).filter(o => o);
            if (objects.length > 0) {
                map.addObjects(objects);
            }
        },

        removeObjectsFromMap: function (mapGuid, objGuids) {
            const map = mapObjects[mapGuid];
            if (!map || map._blzPlaceholder) return;
            const objects = objGuids.map(g => mapObjects[g]).filter(o => o);
            if (objects.length > 0) {
                try {
                    map.removeObjects(objects);
                } catch (e) { }
            }
        },

        addInfoBubble: function (uiGuid, bubbleGuid) {
            const ui = mapObjects[uiGuid];
            const bubble = mapObjects[bubbleGuid];
            if (ui && bubble) {
                ui.addBubble(bubble);
            }
        },

        removeInfoBubble: function (uiGuid, bubbleGuid) {
            const ui = mapObjects[uiGuid];
            const bubble = mapObjects[bubbleGuid];
            if (ui && bubble) {
                ui.removeBubble(bubble);
            }
        },

        readObjectPropertyValue: function (args) {
            const [objectId, property] = args;
            const obj = mapObjects[objectId];
            return obj?.[property];
        },

        writeObjectPropertyValue: function (objectId, property, value) {
            const obj = mapObjects[objectId];
            if (obj) {
                obj[property] = value;
            }
        },

        groupAddObjects: function (groupGuid, objectGuids) {
            const group = mapObjects[groupGuid];
            if (group) {
                const objects = objectGuids.map(g => mapObjects[g]).filter(o => o != null);
                group.addObjects(objects);
            }
        },

        groupRemoveObjects: function (groupGuid, objectGuids) {
            const group = mapObjects[groupGuid];
            if (group) {
                const objects = objectGuids.map(g => mapObjects[g]).filter(o => o != null);
                group.removeObjects(objects);
            }
        },

        readObjectPropertyValueWithReturnedObjectRef: function (args) {
            const [objectId, property] = args;
            const obj = mapObjects[objectId];
            const result = obj?.[property];
            const uuid = uuidv4();
            addMapObject(uuid, result);
            return uuid;
        },

        disposeObject: function (guid) {
            const obj = mapObjects[guid];
            if (obj) {
                if (typeof obj.dispose === 'function') {
                    try { obj.dispose(); } catch (e) { }
                }
                removeMapObject(guid);
            }
        },

        disposeMultipleObjects: function (guids) {
            if (!guids) return;
            for (const guid of guids) {
                blazorHerePlatform.objectManager.disposeObject(guid);
            }
        },

        disposeMap: function (mapGuid) {
            const map = mapObjects[mapGuid];
            if (map && map._blzPlaceholder) {
                removeMapObject(mapGuid);
                return;
            }
            if (map) {
                // Remove hybrid overlay layers
                const overlays = map['_blzOverlays'];
                if (overlays) {
                    for (const ol of overlays) {
                        try { map.removeLayer(ol); } catch (e) { }
                    }
                }

                // Remove all map objects before disposing to reduce
                // HARP tile-loading race conditions during teardown.
                try {
                    const objects = map.getObjects();
                    if (objects && objects.length > 0) {
                        map.removeObjects(objects);
                    }
                } catch (e) { }

                // Dispose the behavior to remove its event listeners and
                // prevent orphaned behaviors from polluting future lookups.
                const behGuid = map['_blzBehaviorGuid'];
                if (behGuid && mapObjects[behGuid]) {
                    try { mapObjects[behGuid].dispose(); } catch (e) { }
                    removeMapObject(behGuid);
                }
                // Clean up auto-InfoBubble if present
                const autoBubble = map['_blzAutoBubble'];
                if (autoBubble) {
                    const uiForBubble = map['_blzUiGuid'] ? mapObjects[map['_blzUiGuid']] : null;
                    if (uiForBubble) {
                        try { uiForBubble.removeBubble(autoBubble); } catch (e) { }
                    }
                    map['_blzAutoBubble'] = null;
                }

                // Do NOT call ui.dispose() — it corrupts shared HERE Maps
                // internal state (similar to flush()), breaking InfoBubble
                // rendering on subsequent maps.  The UI's DOM elements are
                // cleaned up by map.dispose() and Blazor's DOM lifecycle.
                const uiGuid = map['_blzUiGuid'];
                if (uiGuid) {
                    removeMapObject(uiGuid);
                }

                try {
                    map.dispose();
                } catch (e) { }
                removeMapObject(mapGuid);
            }
        },

        // Behavior feature control
        setBehaviorFeatures: function (mapGuid, featureMask, enable) {
            const map = mapObjects[mapGuid];
            if (!map || map._blzPlaceholder) return;
            const behavior = map['_blzBehavior'];
            if (!behavior || !H.mapevents || !H.mapevents.Behavior || !H.mapevents.Behavior.Feature) return;

            const F = H.mapevents.Behavior.Feature;
            const featureMap = [
                [1, F.PANNING],
                [2, F.WHEEL_ZOOM],
                [4, F.PINCH_ZOOM],
                [8, F.DBL_TAP_ZOOM],
                [16, F.TILT],
                [32, F.HEADING],
                [64, F.FRACTIONAL_ZOOM]
            ];

            for (const [bit, feature] of featureMap) {
                if (featureMask & bit) {
                    if (enable) {
                        behavior.enable(feature);
                    } else {
                        behavior.disable(feature);
                    }
                }
            }
        },

        // Map capture as Base64 data URI
        captureMap: function (mapGuid) {
            return new Promise(function (resolve, reject) {
                const map = mapObjects[mapGuid];
                if (!map || map._blzPlaceholder) {
                    reject('Map not found');
                    return;
                }
                map.capture(function (canvas) {
                    if (canvas) {
                        resolve(canvas.toDataURL('image/png'));
                    } else {
                        reject('Capture failed');
                    }
                }, [], 0, 0, map.getViewPort().width, map.getViewPort().height);
            });
        },

        // Viewport padding
        setViewportPadding: function (mapGuid, top, right, bottom, left) {
            const map = mapObjects[mapGuid];
            if (!map || map._blzPlaceholder) return;
            try {
                map.getViewPort().setPadding(top, right, bottom, left);
            } catch (e) {
                console.warn('[BlazorHerePlatform] Failed to set viewport padding:', e);
            }
        },

        // Advanced marker component support
        updateMarkerComponent: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) {
                console.warn('[BlazorHerePlatform] HERE API not loaded. Cannot create marker.');
                return;
            }

            const {
                position,
                draggable,
                clickable,
                zIndex,
                visible,
                iconUrl,
                mapId,
                infoBubbleTemplateId
            } = options;

            const map = mapObjects[mapId];
            const invokeCallback = (method, ...args) => {
                callbackRef?.invokeMethodAsync(method, ...args);
            };

            const existingMarker = mapObjects[id];
            if (existingMarker) {
                existingMarker.setGeometry(position);
                if (typeof existingMarker.setVisibility === 'function') {
                    existingMarker.setVisibility(visible !== false);
                }
                if (typeof existingMarker.setZIndex === 'function') {
                    existingMarker.setZIndex(zIndex || 0);
                }
                return;
            }

            const markerOptions = { data: null };
            if (zIndex != null) markerOptions.zIndex = zIndex;
            if (visible === false) markerOptions.visibility = false;
            if (draggable) markerOptions.volatility = true;

            if (iconUrl) {
                try {
                    markerOptions.icon = new H.map.Icon(iconUrl);
                } catch (e) {
                    console.warn('[BlazorHerePlatform] Failed to create icon from URL:', e);
                }
            } else if (draggable) {
                // Use a distinct orange SVG icon for draggable markers
                try {
                    const svg = '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="32" viewBox="0 0 24 32">' +
                        '<path d="M12 0C5.4 0 0 5.4 0 12c0 9 12 20 12 20s12-11 12-20C24 5.4 18.6 0 12 0z" fill="#E67E22" stroke="#C0392B" stroke-width="1"/>' +
                        '<circle cx="12" cy="11" r="4" fill="white"/>' +
                        '</svg>';
                    markerOptions.icon = new H.map.Icon(svg);
                } catch (e) {
                    console.warn('[BlazorHerePlatform] Failed to create draggable icon:', e);
                }
            }

            const marker = new H.map.Marker(position, markerOptions);

            if (draggable) {
                marker.draggable = true;
            }

            if (map) {
                map.addObject(marker);
            }

            // Wire all pointer/interaction events via the unified event system
            if (clickable) {
                wireObjectEvents(marker, 'marker', id, callbackRef, map);
            }

            // Auto-InfoBubble: on tap, read the hidden <template> innerHTML
            // and open an InfoBubble at the marker position.
            // Content is static HTML — no live Blazor interactivity inside the bubble.
            if (infoBubbleTemplateId && map) {
                marker.addEventListener('tap', function () {
                    const tpl = document.getElementById(infoBubbleTemplateId);
                    if (!tpl) return;
                    const html = tpl.innerHTML;
                    if (!html || !html.trim()) return;

                    const uiGuid = map['_blzUiGuid'];
                    if (!uiGuid) return;
                    const ui = mapObjects[uiGuid];
                    if (!ui) return;

                    // Close previous auto-bubble on this map (if any)
                    const prev = map['_blzAutoBubble'];
                    if (prev) {
                        try { ui.removeBubble(prev); } catch (e) { }
                    }

                    const bubble = new H.ui.InfoBubble(marker.getGeometry(), { content: html });
                    ui.addBubble(bubble);
                    map['_blzAutoBubble'] = bubble;
                });
            }

            if (draggable && map) {
                // Official HERE drag pattern (developer guide §6).
                // Drag events fire on the MAP, not on individual objects.
                // Use direct behavior reference from the map to avoid
                // picking up orphaned behaviors from disposed maps.
                map.addEventListener('dragstart', function (evt) {
                    const target = evt.target;
                    const pointer = evt.currentPointer;
                    if (target instanceof H.map.Marker) {
                        const behavior = map['_blzBehavior'];
                        if (behavior) {
                            behavior.disable(H.mapevents.Behavior.Feature.PANNING);
                        }
                        var targetPosition = map.geoToScreen(target.getGeometry());
                        target['offset'] = new H.math.Point(
                            pointer.viewportX - targetPosition.x,
                            pointer.viewportY - targetPosition.y
                        );
                        // Forward drag event to C#
                        const data = extractDragEventData(evt, map);
                        callbackRef.invokeMethodAsync('OnObjectDragEvent', 'marker', id, 'dragstart', data);
                    }
                }, false);

                map.addEventListener('drag', function (evt) {
                    const target = evt.target;
                    const pointer = evt.currentPointer;
                    if (target instanceof H.map.Marker) {
                        target.setGeometry(map.screenToGeo(
                            pointer.viewportX - target['offset'].x,
                            pointer.viewportY - target['offset'].y
                        ));
                        // Forward drag event to C#
                        const data = extractDragEventData(evt, map);
                        callbackRef.invokeMethodAsync('OnObjectDragEvent', 'marker', id, 'drag', data);
                    }
                }, false);

                map.addEventListener('dragend', function (evt) {
                    const target = evt.target;
                    if (target instanceof H.map.Marker) {
                        const behavior = map['_blzBehavior'];
                        if (behavior) {
                            behavior.enable(H.mapevents.Behavior.Feature.PANNING);
                        }
                        // Forward drag event to C#
                        const data = extractDragEventData(evt, map);
                        callbackRef.invokeMethodAsync('OnObjectDragEvent', 'marker', id, 'dragend', data);
                    }
                }, false);
            }

            addMapObject(id, marker);
        },

        disposeMarkerComponent: function (id) {
            const marker = mapObjects[id];
            if (!marker) return;

            // Try to remove from any map it's on
            const element = marker.getRootGroup && marker.getRootGroup();
            if (element) {
                try {
                    const parent = marker.getParentGroup && marker.getParentGroup();
                    if (parent) parent.removeObject(marker);
                } catch (e) { }
            }

            // Try direct disposal from any map
            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                if (obj instanceof H.Map) {
                    try { obj.removeObject(marker); } catch (e) { }
                }
            }

            blazorHerePlatform.objectManager.disposeObject(id);
        },

        // Polygon component support
        updatePolygonComponent: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) {
                console.warn('[BlazorHerePlatform] HERE API not loaded. Cannot create polygon.');
                return;
            }

            const {
                path,
                strokeColor,
                fillColor,
                lineWidth,
                clickable,
                visible,
                holes,
                extrusion,
                elevation,
                mapId
            } = options;

            const map = mapObjects[mapId];

            // Build geometry: exterior ring, optional holes
            function buildGeometry() {
                const exterior = new H.geo.LineString();
                if (path && path.length > 0) {
                    path.forEach(p => exterior.pushPoint(p));
                }
                if (holes && holes.length > 0) {
                    const interiors = holes.map(function (hole) {
                        const ls = new H.geo.LineString();
                        hole.forEach(p => ls.pushPoint(p));
                        return ls;
                    });
                    return new H.geo.Polygon(exterior, interiors);
                }
                return new H.geo.Polygon(exterior);
            }

            const existingPolygon = mapObjects[id];
            if (existingPolygon) {
                // Update geometry
                if (path && path.length > 0) {
                    existingPolygon.setGeometry(buildGeometry());
                }
                // Update style
                const style = {};
                if (strokeColor) style.strokeColor = strokeColor;
                if (fillColor) style.fillColor = fillColor;
                if (lineWidth != null) style.lineWidth = lineWidth;
                if (Object.keys(style).length > 0) {
                    existingPolygon.setStyle(style);
                }
                if (typeof existingPolygon.setVisibility === 'function') {
                    existingPolygon.setVisibility(visible !== false);
                }
                return;
            }

            // Create new polygon
            const polyOpts = {};
            const style = {};
            if (strokeColor) style.strokeColor = strokeColor;
            if (fillColor) style.fillColor = fillColor;
            if (lineWidth != null) style.lineWidth = lineWidth;
            polyOpts.style = style;

            if (extrusion != null) polyOpts.extrusion = extrusion;
            if (elevation != null) polyOpts.elevation = elevation;

            const polygon = new H.map.Polygon(buildGeometry(), polyOpts);

            if (visible === false) {
                polygon.setVisibility(false);
            }

            if (map) {
                map.addObject(polygon);
            }

            // Wire all pointer/interaction events via the unified event system
            if (clickable) {
                wireObjectEvents(polygon, 'polygon', id, callbackRef, map);
            }

            addMapObject(id, polygon);
        },

        disposePolygonComponent: function (id) {
            const polygon = mapObjects[id];
            if (!polygon) return;

            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                if (obj instanceof H.Map) {
                    try { obj.removeObject(polygon); } catch (e) { }
                }
            }

            blazorHerePlatform.objectManager.disposeObject(id);
        },

        // Polyline component support
        updatePolylineComponent: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) {
                console.warn('[BlazorHerePlatform] HERE API not loaded. Cannot create polyline.');
                return;
            }

            const {
                path,
                strokeColor,
                lineWidth,
                lineCap,
                lineDash,
                clickable,
                visible,
                mapId
            } = options;

            const map = mapObjects[mapId];

            const existingPolyline = mapObjects[id];
            if (existingPolyline) {
                // Update geometry
                if (path && path.length > 0) {
                    const lineString = new H.geo.LineString();
                    path.forEach(p => lineString.pushPoint(p));
                    existingPolyline.setGeometry(lineString);
                }
                // Update style
                const style = {};
                if (strokeColor) style.strokeColor = strokeColor;
                if (lineWidth != null) style.lineWidth = lineWidth;
                if (lineCap) style.lineCap = lineCap;
                if (lineDash) style.lineDash = lineDash;
                if (Object.keys(style).length > 0) {
                    existingPolyline.setStyle(style);
                }
                if (typeof existingPolyline.setVisibility === 'function') {
                    existingPolyline.setVisibility(visible !== false);
                }
                return;
            }

            // Create new polyline
            const lineString = new H.geo.LineString();
            if (path && path.length > 0) {
                path.forEach(p => lineString.pushPoint(p));
            }

            const style = {};
            if (strokeColor) style.strokeColor = strokeColor;
            if (lineWidth != null) style.lineWidth = lineWidth;
            if (lineCap) style.lineCap = lineCap;
            if (lineDash) style.lineDash = lineDash;

            const polyline = new H.map.Polyline(lineString, { style });

            if (visible === false) {
                polyline.setVisibility(false);
            }

            if (map) {
                map.addObject(polyline);
            }

            if (clickable) {
                wireObjectEvents(polyline, 'polyline', id, callbackRef, map);
            }

            addMapObject(id, polyline);
        },

        disposePolylineComponent: function (id) {
            const polyline = mapObjects[id];
            if (!polyline) return;

            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                if (obj instanceof H.Map) {
                    try { obj.removeObject(polyline); } catch (e) { }
                }
            }

            blazorHerePlatform.objectManager.disposeObject(id);
        },

        // Circle component support
        updateCircleComponent: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) {
                console.warn('[BlazorHerePlatform] HERE API not loaded. Cannot create circle.');
                return;
            }

            const {
                centerLat,
                centerLng,
                radius,
                strokeColor,
                fillColor,
                lineWidth,
                precision,
                clickable,
                visible,
                mapId
            } = options;

            const map = mapObjects[mapId];
            const center = { lat: centerLat, lng: centerLng };

            const existingCircle = mapObjects[id];
            if (existingCircle) {
                existingCircle.setCenter(center);
                existingCircle.setRadius(radius);
                // Update style
                const style = {};
                if (strokeColor) style.strokeColor = strokeColor;
                if (fillColor) style.fillColor = fillColor;
                if (lineWidth != null) style.lineWidth = lineWidth;
                if (Object.keys(style).length > 0) {
                    existingCircle.setStyle(style);
                }
                if (typeof existingCircle.setVisibility === 'function') {
                    existingCircle.setVisibility(visible !== false);
                }
                return;
            }

            // Create new circle
            const style = {};
            if (strokeColor) style.strokeColor = strokeColor;
            if (fillColor) style.fillColor = fillColor;
            if (lineWidth != null) style.lineWidth = lineWidth;

            const circleOpts = { style };
            if (precision != null) circleOpts.precision = precision;

            const circle = new H.map.Circle(center, radius, circleOpts);

            if (visible === false) {
                circle.setVisibility(false);
            }

            if (map) {
                map.addObject(circle);
            }

            if (clickable) {
                wireObjectEvents(circle, 'circle', id, callbackRef, map);
            }

            addMapObject(id, circle);
        },

        disposeCircleComponent: function (id) {
            const circle = mapObjects[id];
            if (!circle) return;

            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                if (obj instanceof H.Map) {
                    try { obj.removeObject(circle); } catch (e) { }
                }
            }

            blazorHerePlatform.objectManager.disposeObject(id);
        },

        // Rect component support
        updateRectComponent: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) {
                console.warn('[BlazorHerePlatform] HERE API not loaded. Cannot create rect.');
                return;
            }

            const {
                top,
                left,
                bottom,
                right,
                strokeColor,
                fillColor,
                lineWidth,
                clickable,
                visible,
                mapId
            } = options;

            const map = mapObjects[mapId];
            const bounds = new H.geo.Rect(top, left, bottom, right);

            const existingRect = mapObjects[id];
            if (existingRect) {
                // H.map.Rect inherits setGeometry from GeoShape; pass a Rect geometry
                existingRect.setGeometry(bounds);
                // Update style
                const style = {};
                if (strokeColor) style.strokeColor = strokeColor;
                if (fillColor) style.fillColor = fillColor;
                if (lineWidth != null) style.lineWidth = lineWidth;
                if (Object.keys(style).length > 0) {
                    existingRect.setStyle(style);
                }
                if (typeof existingRect.setVisibility === 'function') {
                    existingRect.setVisibility(visible !== false);
                }
                return;
            }

            // Create new rect
            const style = {};
            if (strokeColor) style.strokeColor = strokeColor;
            if (fillColor) style.fillColor = fillColor;
            if (lineWidth != null) style.lineWidth = lineWidth;

            const rect = new H.map.Rect(bounds, { style });

            if (visible === false) {
                rect.setVisibility(false);
            }

            if (map) {
                map.addObject(rect);
            }

            if (clickable) {
                wireObjectEvents(rect, 'rect', id, callbackRef, map);
            }

            addMapObject(id, rect);
        },

        disposeRectComponent: function (id) {
            const rect = mapObjects[id];
            if (!rect) return;

            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                if (obj instanceof H.Map) {
                    try { obj.removeObject(rect); } catch (e) { }
                }
            }

            blazorHerePlatform.objectManager.disposeObject(id);
        },

        // Group component support
        updateGroupComponent: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) {
                console.warn('[BlazorHerePlatform] HERE API not loaded. Cannot create group.');
                return;
            }

            const { visible, zIndex, mapId } = options;
            const map = mapObjects[mapId];

            const existingGroup = mapObjects[id];
            if (existingGroup) {
                if (typeof existingGroup.setVisibility === 'function') {
                    existingGroup.setVisibility(visible !== false);
                }
                if (zIndex != null && typeof existingGroup.setZIndex === 'function') {
                    existingGroup.setZIndex(zIndex);
                }
                return;
            }

            const group = new H.map.Group();

            if (visible === false) {
                group.setVisibility(false);
            }
            if (zIndex != null) {
                group.setZIndex(zIndex);
            }

            if (map) {
                map.addObject(group);
            }

            addMapObject(id, group);
        },

        disposeGroupComponent: function (id) {
            const group = mapObjects[id];
            if (!group) return;

            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                if (obj instanceof H.Map) {
                    try { obj.removeObject(group); } catch (e) { }
                }
            }

            blazorHerePlatform.objectManager.disposeObject(id);
        },

        // DomMarker component support
        updateDomMarkerComponent: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) {
                console.warn('[BlazorHerePlatform] HERE API not loaded. Cannot create dom marker.');
                return;
            }

            const {
                position,
                clickable,
                draggable,
                zIndex,
                visible,
                mapId,
                groupId,
                templateId
            } = options;

            const map = mapObjects[mapId];

            const existingMarker = mapObjects[id];
            if (existingMarker) {
                existingMarker.setGeometry(position);
                if (typeof existingMarker.setVisibility === 'function') {
                    existingMarker.setVisibility(visible !== false);
                }
                if (typeof existingMarker.setZIndex === 'function') {
                    existingMarker.setZIndex(zIndex || 0);
                }
                return;
            }

            // Read template HTML for DomIcon
            let domIcon = null;
            if (templateId) {
                const tpl = document.getElementById(templateId);
                if (tpl) {
                    const html = tpl.innerHTML.trim();
                    if (html) {
                        domIcon = new H.map.DomIcon(html);
                    }
                }
            }

            const markerOptions = {};
            if (domIcon) markerOptions.icon = domIcon;
            if (zIndex != null) markerOptions.zIndex = zIndex;
            if (visible === false) markerOptions.visibility = false;
            if (draggable) markerOptions.volatility = true;

            const marker = new H.map.DomMarker(position, markerOptions);

            if (draggable) {
                marker.draggable = true;
            }

            // Add to group or map
            const group = groupId ? mapObjects[groupId] : null;
            if (group) {
                group.addObject(marker);
            } else if (map) {
                map.addObject(marker);
            }

            if (clickable) {
                wireObjectEvents(marker, 'dommarker', id, callbackRef, map);
            }

            addMapObject(id, marker);
        },

        disposeDomMarkerComponent: function (id) {
            const marker = mapObjects[id];
            if (!marker) return;

            // Try to remove from parent group
            try {
                const parent = marker.getParentGroup && marker.getParentGroup();
                if (parent) parent.removeObject(marker);
            } catch (e) { }

            // Fallback: try all maps
            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                if (obj instanceof H.Map) {
                    try { obj.removeObject(marker); } catch (e) { }
                }
            }

            blazorHerePlatform.objectManager.disposeObject(id);
        },

        // InfoBubble component support
        updateInfoBubbleComponent: function (id, options, callbackRef) {
            if (!isHereApiLoaded() || !H.ui || !H.ui.InfoBubble) {
                console.warn('[BlazorHerePlatform] HERE API/UI not loaded. Cannot create InfoBubble.');
                return;
            }

            const { lat, lng, isOpen, mapId, templateId } = options;
            const map = mapObjects[mapId];
            if (!map) return;

            const uiGuid = map['_blzUiGuid'];
            if (!uiGuid) return;
            const ui = mapObjects[uiGuid];
            if (!ui) return;

            const position = { lat, lng };

            // Read template HTML
            let html = '';
            if (templateId) {
                const tpl = document.getElementById(templateId);
                if (tpl) html = tpl.innerHTML.trim();
            }

            const existingBubble = mapObjects[id];
            if (existingBubble) {
                existingBubble.setPosition(position);
                if (html) existingBubble.setContent(html);

                if (isOpen && existingBubble.getState() === 'closed') {
                    existingBubble.open();
                } else if (!isOpen && existingBubble.getState() === 'open') {
                    existingBubble.close();
                }
                return;
            }

            if (!isOpen) return; // Don't create if not open

            const bubble = new H.ui.InfoBubble(position, { content: html || '' });

            // Listen for close via X button
            bubble.addEventListener('statechange', function (evt) {
                if (bubble.getState() === 'closed') {
                    callbackRef.invokeMethodAsync('OnInfoBubbleClosed', id);
                }
            });

            ui.addBubble(bubble);
            addMapObject(id, bubble);
        },

        disposeInfoBubbleComponent: function (id) {
            const bubble = mapObjects[id];
            if (!bubble) return;

            // Try to remove from UI
            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                // Check if obj is a UI instance by duck-typing
                if (obj && typeof obj.removeBubble === 'function') {
                    try { obj.removeBubble(bubble); } catch (e) { }
                }
            }

            removeMapObject(id);
        },

        // Setup map-level event forwarding to C# via AdvancedHereMap's JSInvokable methods.
        // Called by AdvancedHereMap after the map is initialized.
        setupMapEvents: function (mapGuid, callbackRef) {
            const map = mapObjects[mapGuid];
            if (!map || !callbackRef || map._blzPlaceholder) return;

            // Pointer events on the map
            const pointerEvents = [
                'tap', 'dbltap', 'longpress', 'contextmenu',
                'pointerdown', 'pointerup', 'pointermove',
                'pointerenter', 'pointerleave'
            ];

            for (const eventName of pointerEvents) {
                map.addEventListener(eventName, function (evt) {
                    const data = extractPointerEventData(evt, map);
                    callbackRef.invokeMethodAsync('OnMapPointerEvent', eventName, data);
                });
            }

            // Parameterless map events
            map.addEventListener('contextmenuclose', function () {
                callbackRef.invokeMethodAsync('OnMapSimpleEvent', 'contextmenuclose');
            });
            map.addEventListener('pointercancel', function () {
                callbackRef.invokeMethodAsync('OnMapSimpleEvent', 'pointercancel');
            });

            // Drag events on the map
            ['dragstart', 'drag', 'dragend'].forEach(function (eventName) {
                map.addEventListener(eventName, function (evt) {
                    const data = extractDragEventData(evt, map);
                    callbackRef.invokeMethodAsync('OnMapDragEvent', eventName, data);
                });
            });

            // Wheel event
            map.addEventListener('wheel', function (evt) {
                callbackRef.invokeMethodAsync('OnMapWheelEvent', {
                    delta: evt.originalEvent ? evt.originalEvent.deltaY : 0,
                    viewportX: evt.currentPointer ? evt.currentPointer.viewportX : 0,
                    viewportY: evt.currentPointer ? evt.currentPointer.viewportY : 0
                });
            });

            // Map view change events
            ['mapviewchange', 'mapviewchangestart', 'mapviewchangeend'].forEach(function (eventName) {
                map.addEventListener(eventName, function () {
                    try {
                        const lookAt = map.getViewModel().getLookAtData();
                        const center = map.getCenter();
                        callbackRef.invokeMethodAsync('OnMapViewChangeEvent', eventName, {
                            center: center ? { lat: center.lat, lng: center.lng } : null,
                            zoom: map.getZoom() || 0,
                            tilt: lookAt.tilt || 0,
                            heading: lookAt.heading || 0,
                            type: eventName
                        });
                    } catch (e) { }
                });
            });

            // Base layer change
            map.addEventListener('baselayerchange', function () {
                callbackRef.invokeMethodAsync('OnMapBaseLayerChangeEvent', {
                    type: 'baselayerchange'
                });
            });

            // Engine state change
            map.addEventListener('enginestatechange', function (evt) {
                callbackRef.invokeMethodAsync('OnMapEngineStateChangeEvent', {
                    state: evt.state || 0,
                    type: 'enginestatechange'
                });
            });
        },

        // Batch listener support
        addMultipleListeners: async function (args) {
            const guids = JSON.parse(args[0]);
            const eventName = args[1];
            const args2 = args.slice(2).map(arg => tryParseJson(arg));

            await Promise.all(guids.map((guid, index) => {
                const obj = mapObjects[guid];
                const additionalArgs = Array.isArray(args2[index]) ? args2[index] : [args2[index]];
                const args3 = [guid, 'addEventListener', eventName, ...additionalArgs];
                return blazorHerePlatform.objectManager.invoke(args3);
            }));

            return true;
        },

        createMultipleObject: function (args) {
            // Not commonly needed for HERE but kept for compatibility
            const guids = JSON.parse(args[0]);
            const constructorName = args[1];
            const rest = args.slice(2);

            for (let i = 0; i < guids.length; i++) {
                const objArgs = [guids[i], constructorName, rest[i]];
                blazorHerePlatform.objectManager.createObject(objArgs);
            }
        },

        invokeMultiple: async function (args) {
            const guids = JSON.parse(args[0]);
            const methodName = args[1];
            const args2 = args.slice(2).map(arg => tryParseJson(arg));

            const results = {};
            const promises = guids.map((guid, index) => {
                const args3 = [guid, methodName, args2[index]];
                const result = blazorHerePlatform.objectManager.invoke(args3);
                return Promise.resolve(result).then(resolvedResult => {
                    results[guid] = resolvedResult;
                });
            });

            await Promise.all(promises);
            return results;
        },

        // Routing service
        calculateRoute: function (request) {
            return new Promise(function (resolve, reject) {
                if (!herePlatform) {
                    reject('Platform not initialized');
                    return;
                }

                var router = herePlatform.getRoutingService(null, 8);
                var params = {
                    routingMode: request.routingMode || 'fast',
                    transportMode: request.transportMode || 'car',
                    origin: request.origin.lat + ',' + request.origin.lng,
                    destination: request.destination.lat + ',' + request.destination.lng,
                    return: 'polyline,summary'
                };

                if (request.via && request.via.length > 0) {
                    request.via.forEach(function (v, i) {
                        params['via'] = v.lat + ',' + v.lng;
                    });
                }

                if (request.alternatives > 0) {
                    params.alternatives = request.alternatives;
                }

                router.calculateRoute(params, function (result) {
                    var routes = [];
                    if (result.routes) {
                        result.routes.forEach(function (route) {
                            var sections = [];
                            if (route.sections) {
                                route.sections.forEach(function (section) {
                                    sections.push({
                                        polyline: section.polyline || null,
                                        summary: section.summary ? {
                                            duration: section.summary.duration || 0,
                                            length: section.summary.length || 0,
                                            baseDuration: section.summary.baseDuration || null
                                        } : null,
                                        transport: section.transport ? section.transport.mode : null
                                    });
                                });
                            }
                            routes.push({ sections: sections });
                        });
                    }
                    resolve({ routes: routes });
                }, function (error) {
                    console.warn('[BlazorHerePlatform] Routing error:', error);
                    resolve({ routes: [] });
                });
            });
        },

        // Geocoding service
        geocode: function (query, options) {
            return new Promise(function (resolve, reject) {
                if (!herePlatform) {
                    reject('Platform not initialized');
                    return;
                }

                var service = herePlatform.getSearchService();
                var params = { q: query, limit: options.limit || 5 };
                if (options.lang) params.lang = options.lang;

                service.geocode(params, function (result) {
                    var items = (result.items || []).map(function (item) {
                        return {
                            title: item.title || null,
                            position: item.position ? { lat: item.position.lat, lng: item.position.lng } : null,
                            address: item.address ? item.address.label : null,
                            resultType: item.resultType || null
                        };
                    });
                    resolve({ items: items });
                }, function (error) {
                    console.warn('[BlazorHerePlatform] Geocode error:', error);
                    resolve({ items: [] });
                });
            });
        },

        reverseGeocode: function (position, options) {
            return new Promise(function (resolve, reject) {
                if (!herePlatform) {
                    reject('Platform not initialized');
                    return;
                }

                var service = herePlatform.getSearchService();
                var params = {
                    at: position.lat + ',' + position.lng,
                    limit: options.limit || 5
                };
                if (options.lang) params.lang = options.lang;

                service.reverseGeocode(params, function (result) {
                    var items = (result.items || []).map(function (item) {
                        return {
                            title: item.title || null,
                            position: item.position ? { lat: item.position.lat, lng: item.position.lng } : null,
                            address: item.address ? item.address.label : null,
                            resultType: item.resultType || null
                        };
                    });
                    resolve({ items: items });
                }, function (error) {
                    console.warn('[BlazorHerePlatform] Reverse geocode error:', error);
                    resolve({ items: [] });
                });
            });
        },

        // Marker cluster component support
        updateMarkerClusterComponent: async function (id, options, callbackRef) {
            if (!isHereApiLoaded()) return;
            if (!H.clustering) {
                if (hereBaseUrl) {
                    await loadScript(hereBaseUrl + '/mapsjs-clustering.js');
                }
                if (!H.clustering) {
                    console.warn('[BlazorHerePlatform] HERE clustering module not available after loading mapsjs-clustering.js.');
                    return;
                }
            }

            const { dataPoints, eps, minWeight, clusterSvgTemplate, noiseSvgTemplate, mapId } = options;
            const map = mapObjects[mapId];
            if (!map) return;

            // Remove existing cluster layer
            const existing = mapObjects[id];
            if (existing) {
                try { map.removeLayer(existing.layer); } catch (e) { }
                removeMapObject(id);
            }

            if (!dataPoints || dataPoints.length === 0) return;

            // Create H.clustering.DataPoint array
            const points = dataPoints.map(function (dp) {
                return new H.clustering.DataPoint(dp.lat, dp.lng, dp.weight || 1, dp.data);
            });

            // Default SVG templates
            const defaultClusterSvg = '<svg xmlns="http://www.w3.org/2000/svg" width="40" height="40"><circle cx="20" cy="20" r="18" fill="{color}" stroke="white" stroke-width="2"/><text x="20" y="25" text-anchor="middle" fill="white" font-size="12">{count}</text></svg>';
            const defaultNoiseSvg = '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"><circle cx="10" cy="10" r="8" fill="#3498DB" stroke="white" stroke-width="2"/></svg>';

            const clusterTpl = clusterSvgTemplate || defaultClusterSvg;
            const noiseTpl = noiseSvgTemplate || defaultNoiseSvg;

            function getClusterColor(weight) {
                if (weight >= 100) return '#E74C3C';
                if (weight >= 50) return '#E67E22';
                if (weight >= 10) return '#F1C40F';
                return '#2ECC71';
            }

            var theme = {
                getClusterPresentation: function (cluster) {
                    var weight = cluster.getWeight();
                    var count = cluster.isCluster() ? weight : 1;
                    var svg = clusterTpl.replace('{count}', count).replace('{color}', getClusterColor(weight));
                    var icon = new H.map.Icon(svg, { size: { w: 40, h: 40 }, anchor: { x: 20, y: 20 } });
                    var marker = new H.map.Marker(cluster.getPosition(), { icon: icon, min: cluster.getMinZoom(), max: cluster.getMaxZoom() });
                    marker.setData(cluster);
                    return marker;
                },
                getNoisePresentation: function (noisePoint) {
                    var icon = new H.map.Icon(noiseTpl, { size: { w: 20, h: 20 }, anchor: { x: 10, y: 10 } });
                    var marker = new H.map.Marker(noisePoint.getPosition(), { icon: icon, min: noisePoint.getMinZoom() });
                    marker.setData(noisePoint);
                    return marker;
                }
            };

            var provider = new H.clustering.Provider(points, {
                clusteringOptions: {
                    eps: eps || 32,
                    minWeight: minWeight || 2
                },
                theme: theme
            });

            var layer = new H.map.layer.ObjectLayer(provider);
            map.addLayer(layer);

            // Wire tap events on the map — ObjectLayer doesn't propagate tap events
            var tapHandler = function (evt) {
                var target = evt.target;
                if (!target) return;

                // Only handle objects that belong to our cluster layer
                var belongsToCluster = false;
                if (target.getProvider && target.getProvider() === provider) {
                    belongsToCluster = true;
                } else if (target.getData && target.getData() && target.getData()._blzClusterId === id) {
                    belongsToCluster = true;
                }
                if (!belongsToCluster) return;

                var pos = null;
                if (target.getGeometry) {
                    var geom = target.getGeometry();
                    if (geom) pos = { lat: geom.lat, lng: geom.lng };
                }

                var isCluster = false;
                var weight = 1;
                try {
                    var data = target.getData ? target.getData() : null;
                    if (data && typeof data.getWeight === 'function') {
                        isCluster = true;
                        weight = data.getWeight();
                    }
                } catch (e) { }

                var eventArgs = {
                    position: pos,
                    weight: weight,
                    pointCount: weight,
                    isCluster: isCluster
                };

                if (isCluster) {
                    // Zoom into cluster on click
                    if (pos) {
                        var currentZoom = map.getZoom();
                        map.getViewModel().setLookAtData({
                            position: { lat: pos.lat, lng: pos.lng },
                            zoom: Math.min(currentZoom + 3, 20)
                        }, true);
                    }
                    callbackRef.invokeMethodAsync('OnClusterTapped', id, eventArgs);
                } else {
                    callbackRef.invokeMethodAsync('OnNoiseTapped', id, eventArgs);
                }
            };

            map.addEventListener('tap', tapHandler);
            addMapObject(id, { provider: provider, layer: layer, tapHandler: tapHandler, mapId: options.mapId });
        },

        disposeMarkerClusterComponent: function (id) {
            const existing = mapObjects[id];
            if (!existing) return;

            // Remove tap handler from the map
            if (existing.mapId && existing.tapHandler) {
                var map = mapObjects[existing.mapId];
                if (map && map.removeEventListener) {
                    try { map.removeEventListener('tap', existing.tapHandler); } catch (e) { }
                }
            }

            // Remove layer from the map
            if (existing.mapId && existing.layer) {
                var map = mapObjects[existing.mapId];
                if (map) {
                    try { map.removeLayer(existing.layer); } catch (e) { }
                }
            }

            removeMapObject(id);
        },

        // GeoJSON reader component support
        updateGeoJsonReaderComponent: async function (id, options, callbackRef) {
            if (!isHereApiLoaded()) return;
            if (!H.data || !H.data.geojson) {
                if (hereBaseUrl) {
                    await loadScript(hereBaseUrl + '/mapsjs-data.js');
                }
                if (!H.data || !H.data.geojson) {
                    console.warn('[BlazorHerePlatform] HERE data module not available after loading mapsjs-data.js.');
                    return;
                }
            }

            const { url, geoJsonString, defaultStyle, visible, mapId } = options;
            const map = mapObjects[mapId];
            if (!map) return;

            // Remove existing
            const existing = mapObjects[id];
            if (existing) {
                try { map.removeLayer(existing.layer); } catch (e) { }
                removeMapObject(id);
            }

            var reader = new H.data.geojson.Reader(url || undefined, {
                disableLegacyMode: true,
                style: defaultStyle ? function (mapObject) {
                    if (typeof mapObject.setStyle === 'function') {
                        var style = {};
                        if (defaultStyle.strokeColor) style.strokeColor = defaultStyle.strokeColor;
                        if (defaultStyle.fillColor) style.fillColor = defaultStyle.fillColor;
                        if (defaultStyle.lineWidth != null) style.lineWidth = defaultStyle.lineWidth;
                        mapObject.setStyle(style);
                    }
                } : undefined
            });

            var layer = reader.getLayer();
            if (visible !== false) {
                map.addLayer(layer);
            }

            if (url) {
                reader.parse();
            } else if (geoJsonString) {
                reader.parseData(geoJsonString);
            }

            reader.addEventListener('statechange', function () {
                if (reader.getState() === H.data.AbstractReader.State.READY) {
                    var objectCount = 0;
                    try {
                        objectCount = reader.getParsedObjects().length;
                    } catch (e) { }
                    callbackRef.invokeMethodAsync('OnGeoJsonLoaded', id, { objectCount: objectCount });
                }
            });

            addMapObject(id, { reader: reader, layer: layer });
        },

        disposeGeoJsonReaderComponent: function (id) {
            const existing = mapObjects[id];
            if (!existing) return;

            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                if (obj instanceof H.Map && existing.layer) {
                    try { obj.removeLayer(existing.layer); } catch (e) { }
                }
            }

            removeMapObject(id);
        },

        // KML reader component support
        updateKmlReaderComponent: async function (id, options, callbackRef) {
            if (!isHereApiLoaded()) return;
            if (!H.data || !H.data.kml) {
                if (hereBaseUrl) {
                    await loadScript(hereBaseUrl + '/mapsjs-data.js');
                }
                if (!H.data || !H.data.kml) {
                    console.warn('[BlazorHerePlatform] HERE data module not available after loading mapsjs-data.js.');
                    return;
                }
            }

            const { url, defaultStyle, visible, mapId } = options;
            const map = mapObjects[mapId];
            if (!map || !url) return;

            // Remove existing
            const existing = mapObjects[id];
            if (existing) {
                try { map.removeLayer(existing.layer); } catch (e) { }
                removeMapObject(id);
            }

            var reader = new H.data.kml.Reader(url);
            var layer = reader.getLayer();

            if (visible !== false) {
                map.addLayer(layer);
            }

            reader.parse();

            reader.addEventListener('statechange', function () {
                if (reader.getState() === H.data.AbstractReader.State.READY) {
                    var objectCount = 0;
                    try {
                        objectCount = reader.getParsedObjects().length;
                    } catch (e) { }
                    callbackRef.invokeMethodAsync('OnKmlLoaded', id, { objectCount: objectCount });
                }
            });

            addMapObject(id, { reader: reader, layer: layer });
        },

        disposeKmlReaderComponent: function (id) {
            const existing = mapObjects[id];
            if (!existing) return;

            for (const key of Object.keys(mapObjects)) {
                const obj = mapObjects[key];
                if (obj instanceof H.Map && existing.layer) {
                    try { obj.removeLayer(existing.layer); } catch (e) { }
                }
            }

            removeMapObject(id);
        },

        // Autosuggest support — calls H.service.SearchService.autosuggest()
        // and returns results to C# via the callbackRef.
        //
        // NOTE: Do NOT call request.cancel() on the HERE ICancelable — it
        // triggers an internal abort → reject chain inside mapsjs-core.js
        // that surfaces as an uncaught promise rejection.  Instead we use a
        // generation counter so stale callbacks are silently ignored.
        autosuggest: function (guid, query, options, callbackRef) {
            if (!herePlatform) {
                callbackRef.invokeMethodAsync('OnAutosuggestResults', []);
                return;
            }

            // Bump generation — any in-flight request with an older gen is stale
            var state = mapObjects[guid];
            if (!state || typeof state.gen !== 'number') {
                state = { gen: 0 };
                addMapObject(guid, state);
            }
            var gen = ++state.gen;

            var service = herePlatform.getSearchService();

            var params = {
                q: query,
                limit: options.limit || 5
            };
            if (options.lang) params.lang = options.lang;
            if (options.in) params.in = options.in;
            if (options.at) params.at = options.at.lat + ',' + options.at.lng;

            service.autosuggest(params, function (result) {
                // Ignore if a newer request has been issued
                if (mapObjects[guid] !== state || state.gen !== gen) return;

                var items = (result.items || []).map(function (item) {
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

                callbackRef.invokeMethodAsync('OnAutosuggestResults', items);
            }, function (error) {
                if (mapObjects[guid] !== state || state.gen !== gen) return;
                console.warn('[BlazorHerePlatform] Autosuggest error:', error);
                callbackRef.invokeMethodAsync('OnAutosuggestResults', []);
            });
        },

        disposeAutosuggest: function (guid) {
            removeMapObject(guid);
        },

        // Heatmap component support
        updateHeatmapComponent: async function (id, options, callbackRef) {
            if (!isHereApiLoaded()) return;

            // Ensure mapsjs-data.js is loaded (self-healing: loads it on demand if initMap didn't)
            if (typeof H === 'undefined' || !H.data || !H.data.heatmap) {
                if (hereBaseUrl) {
                    await loadScript(hereBaseUrl + '/mapsjs-data.js');
                }
                if (!H.data || !H.data.heatmap) {
                    console.warn('[BlazorHerePlatform] HERE data/heatmap module not available after loading mapsjs-data.js.');
                    return;
                }
            }

            const { dataPoints, opacity, colors, visible, mapId } = options;
            const map = mapObjects[mapId];
            if (!map) return;

            // Remove existing heatmap layer
            var existing = mapObjects[id];
            if (existing && existing.layer) {
                map.removeLayer(existing.layer);
            }

            if (!dataPoints || dataPoints.length === 0 || !visible) {
                removeMapObject(id);
                return;
            }

            // Build color stops
            var colorStops = {
                '0': 'rgba(0, 0, 255, 0)',
                '0.3': 'rgba(0, 0, 255, 0.5)',
                '0.5': 'rgba(255, 255, 0, 0.7)',
                '0.8': 'rgba(255, 165, 0, 0.9)',
                '1': 'rgba(255, 0, 0, 1)'
            };
            if (colors) {
                colorStops = {};
                Object.keys(colors).forEach(function (key) {
                    colorStops[String(key)] = colors[key];
                });
            }

            // Build data points array
            var points = dataPoints.map(function (dp) {
                return { lat: dp.lat, lng: dp.lng, value: dp.value || 1 };
            });

            // Create heatmap provider — must pass engineType matching the map's engine
            var providerOpts = {
                colors: new H.data.heatmap.Colors(colorStops, true),
                opacity: opacity || 0.6,
                type: 'value',
                assumeValues: true
            };

            // Match the map's engine type (HARP requires explicit engineType)
            if (harpEngineType) {
                providerOpts.engineType = harpEngineType;
            }

            var provider = new H.data.heatmap.Provider(providerOpts);
            provider.addData(points);

            var layer = new H.map.layer.TileLayer(provider);
            map.addLayer(layer);

            mapObjects[id] = { provider: provider, layer: layer };
        },

        disposeHeatmapComponent: function (id) {
            var existing = mapObjects[id];
            if (existing && existing.layer) {
                // Find and remove from all maps
                Object.keys(mapObjects).forEach(function (key) {
                    var obj = mapObjects[key];
                    if (obj && typeof obj.removeLayer === 'function') {
                        try { obj.removeLayer(existing.layer); } catch (e) { }
                    }
                });
            }
            removeMapObject(id);
        },

        // Traffic service (v7 REST API)
        getTrafficIncidents: async function (north, south, east, west) {
            if (!hereApiKey) return { incidents: [] };

            try {
                var url = 'https://data.traffic.hereapi.com/v7/incidents' +
                    '?in=bbox:' + west + ',' + south + ',' + east + ',' + north +
                    '&locationReferencing=shape' +
                    '&apiKey=' + encodeURIComponent(hereApiKey);

                var resp = await fetch(url);
                if (!resp.ok) {
                    console.warn('[BlazorHerePlatform] Traffic API v7 incidents error:', resp.status);
                    return { incidents: [] };
                }
                var data = await resp.json();
                var incidents = [];
                if (data && data.results) {
                    data.results.forEach(function (item) {
                        var details = item.incidentDetails || {};
                        var loc = item.location || {};
                        var pos = null;
                        // Extract first coordinate from shape
                        if (loc.shape && loc.shape.links && loc.shape.links.length > 0) {
                            var pts = loc.shape.links[0].points;
                            if (pts && pts.length > 0) {
                                pos = { lat: pts[0].lat, lng: pts[0].lng };
                            }
                        }
                        incidents.push({
                            type: details.type || null,
                            severity: details.criticality === 'critical' ? 3 :
                                      details.criticality === 'major' ? 2 :
                                      details.criticality === 'minor' ? 1 : 0,
                            description: details.description ? details.description.value : null,
                            position: pos,
                            roadName: loc.description || null,
                            startTime: details.startTime || null,
                            endTime: details.endTime || null
                        });
                    });
                }
                return { incidents: incidents };
            } catch (error) {
                console.warn('[BlazorHerePlatform] Traffic incidents error:', error);
                return { incidents: [] };
            }
        },

        getTrafficFlow: async function (north, south, east, west) {
            if (!hereApiKey) return { items: [] };

            try {
                var url = 'https://data.traffic.hereapi.com/v7/flow' +
                    '?in=bbox:' + west + ',' + south + ',' + east + ',' + north +
                    '&locationReferencing=shape' +
                    '&apiKey=' + encodeURIComponent(hereApiKey);

                var resp = await fetch(url);
                if (!resp.ok) return { items: [] };
                var data = await resp.json();
                return { items: data.results || [] };
            } catch (error) {
                console.warn('[BlazorHerePlatform] Traffic flow error:', error);
                return { items: [] };
            }
        },

        // Public Transit service
        getTransitDepartures: function (position) {
            return new Promise(function (resolve, reject) {
                if (!herePlatform) {
                    reject('Platform not initialized');
                    return;
                }

                var service = herePlatform.getPublicTransitService();
                var params = {
                    at: position.lat + ',' + position.lng
                };

                service.getDepartures(params, function (result) {
                    var departures = [];
                    if (result && result.boards) {
                        result.boards.forEach(function (board) {
                            if (board.departures) {
                                board.departures.forEach(function (dep) {
                                    departures.push({
                                        lineName: dep.transport ? dep.transport.name : null,
                                        direction: dep.transport ? dep.transport.headsign : null,
                                        departureTime: dep.time || null,
                                        transportType: dep.transport ? dep.transport.mode : null,
                                        stationName: board.place ? board.place.name : null
                                    });
                                });
                            }
                        });
                    }
                    resolve({ departures: departures });
                }, function (error) {
                    console.warn('[BlazorHerePlatform] Transit departures error:', error);
                    resolve({ departures: [] });
                });
            });
        },

        searchTransitStations: function (position, radiusMeters) {
            return new Promise(function (resolve, reject) {
                if (!herePlatform) {
                    reject('Platform not initialized');
                    return;
                }

                var service = herePlatform.getPublicTransitService();
                var params = {
                    at: position.lat + ',' + position.lng,
                    radius: radiusMeters || 500
                };

                service.getStations(params, function (result) {
                    var stations = [];
                    if (result && result.stations) {
                        result.stations.forEach(function (station) {
                            stations.push({
                                name: station.place ? station.place.name : null,
                                position: station.place && station.place.location ? {
                                    lat: station.place.location.lat,
                                    lng: station.place.location.lng
                                } : null,
                                distance: station.distance || 0,
                                transportTypes: station.transports
                                    ? station.transports.map(function (t) { return t.mode; })
                                    : null
                            });
                        });
                    }
                    resolve({ stations: stations });
                }, function (error) {
                    console.warn('[BlazorHerePlatform] Transit stations error:', error);
                    resolve({ stations: [] });
                });
            });
        },

        // UI Controls
        updateDistanceMeasurement: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) return;

            var map = mapObjects[options.mapId];
            if (!map) return;

            // Remove existing
            var existing = mapObjects[id];
            if (existing && existing.control) {
                try {
                    var ui = map['_blzUI'];
                    if (ui) ui.removeControl(existing.controlName);
                } catch (e) { }
            }

            if (!options.active) {
                removeMapObject(id);
                return;
            }

            // H.ui.UI.modules.DistanceMeasurement is part of the UI module
            var ui = map['_blzUI'];
            if (!ui) return;

            try {
                var control = new H.ui.DistanceMeasurement({
                    alignment: options.alignment || 'top-right',
                    lineStyle: {
                        strokeColor: 'rgba(95, 129, 230, 0.8)',
                        lineWidth: 4
                    }
                });
                var controlName = 'distancemeasurement_' + id;
                ui.addControl(controlName, control);
                mapObjects[id] = { control: control, controlName: controlName, mapId: options.mapId };
            } catch (e) {
                console.warn('[BlazorHerePlatform] DistanceMeasurement not available:', e.message);
            }
        },

        disposeDistanceMeasurement: function (id) {
            var existing = mapObjects[id];
            if (existing && existing.control) {
                try {
                    var map = existing.mapId ? mapObjects[existing.mapId] : null;
                    var ui = map && map['_blzUI'];
                    if (ui) ui.removeControl(existing.controlName);
                } catch (e) { }
            }
            removeMapObject(id);
        },

        updateOverviewMap: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) return;

            var map = mapObjects[options.mapId];
            if (!map) return;

            var existing = mapObjects[id];
            if (existing && existing.control) {
                try {
                    var ui = map['_blzUI'];
                    if (ui) ui.removeControl(existing.controlName);
                } catch (e) { }
            }

            var ui = map['_blzUI'];
            if (!ui) return;

            try {
                // H.ui.Overview requires a Layer (not a Map) as first argument
                var baseLayer = map.getBaseLayer();
                if (!baseLayer) {
                    var layers = map['_blzLayers'];
                    baseLayer = layers && layers.vector ? layers.vector.normal.map : null;
                }
                if (!baseLayer) {
                    console.warn('[BlazorHerePlatform] Overview: no base layer available');
                    return;
                }
                var control = new H.ui.Overview(baseLayer, {
                    alignment: options.alignment || 'bottom-right',
                    zoomDelta: options.zoomDelta || 4
                });
                var controlName = 'overview_' + id;
                ui.addControl(controlName, control);
                mapObjects[id] = { control: control, controlName: controlName, mapId: options.mapId };
            } catch (e) {
                console.warn('[BlazorHerePlatform] Overview map not available:', e.message);
            }
        },

        disposeOverviewMap: function (id) {
            var existing = mapObjects[id];
            if (existing && existing.control) {
                try {
                    var map = existing.mapId ? mapObjects[existing.mapId] : null;
                    var ui = map && map['_blzUI'];
                    if (ui) ui.removeControl(existing.controlName);
                } catch (e) { }
            }
            removeMapObject(id);
        },

        updateZoomRectangle: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) return;

            var map = mapObjects[options.mapId];
            if (!map) return;

            var existing = mapObjects[id];
            if (existing && existing.control) {
                try {
                    var ui = map['_blzUI'];
                    if (ui) ui.removeControl(existing.controlName);
                } catch (e) { }
            }

            if (!options.active) {
                removeMapObject(id);
                return;
            }

            var ui = map['_blzUI'];
            if (!ui) return;

            try {
                var control = new H.ui.ZoomRectangle({
                    alignment: options.alignment || 'top-right'
                });
                var controlName = 'zoomrect_' + id;
                ui.addControl(controlName, control);
                mapObjects[id] = { control: control, controlName: controlName, mapId: options.mapId };
            } catch (e) {
                console.warn('[BlazorHerePlatform] ZoomRectangle not available:', e.message);
            }
        },

        disposeZoomRectangle: function (id) {
            var existing = mapObjects[id];
            if (existing && existing.control) {
                try {
                    var map = existing.mapId ? mapObjects[existing.mapId] : null;
                    var ui = map && map['_blzUI'];
                    if (ui) ui.removeControl(existing.controlName);
                } catch (e) { }
            }
            removeMapObject(id);
        },

        // Custom tile layer
        updateCustomTileLayer: function (id, options, callbackRef) {
            if (!isHereApiLoaded()) return;

            var map = mapObjects[options.mapId];
            if (!map) return;

            // Remove existing
            var existing = mapObjects[id];
            if (existing && existing.layer) {
                map.removeLayer(existing.layer);
            }

            if (!options.urlPattern || !options.visible) {
                removeMapObject(id);
                return;
            }

            var provider = new H.map.provider.ImageTileProvider({
                getURL: function (column, row, zoom) {
                    return options.urlPattern
                        .replace('{x}', column)
                        .replace('{y}', row)
                        .replace('{z}', zoom);
                },
                min: options.min || 0,
                max: options.max || 20,
                tileSize: options.tileSize || 256,
                opacity: options.opacity || 1.0
            });

            var layer = new H.map.layer.TileLayer(provider);
            map.addLayer(layer);

            mapObjects[id] = { provider: provider, layer: layer };
        },

        disposeCustomTileLayer: function (id) {
            var existing = mapObjects[id];
            if (existing && existing.layer) {
                Object.keys(mapObjects).forEach(function (key) {
                    var obj = mapObjects[key];
                    if (obj && typeof obj.removeLayer === 'function') {
                        try { obj.removeLayer(existing.layer); } catch (e) { }
                    }
                });
            }
            removeMapObject(id);
        },

        // GeoJSON export from map objects
        exportMapGeoJson: function (mapGuid) {
            var map = mapObjects[mapGuid];
            if (!map || map._blzPlaceholder) return '{"type":"FeatureCollection","features":[]}';

            var features = [];
            var objects = map.getObjects();

            objects.forEach(function (obj) {
                try {
                    if (obj instanceof H.map.Marker) {
                        var pos = obj.getGeometry();
                        features.push({
                            type: 'Feature',
                            geometry: { type: 'Point', coordinates: [pos.lng, pos.lat] },
                            properties: {}
                        });
                    } else if (obj instanceof H.map.Polyline) {
                        var lineString = obj.getGeometry();
                        var coords = [];
                        lineString.eachLatLngAlt(function (lat, lng) {
                            coords.push([lng, lat]);
                        });
                        features.push({
                            type: 'Feature',
                            geometry: { type: 'LineString', coordinates: coords },
                            properties: {}
                        });
                    } else if (obj instanceof H.map.Polygon) {
                        var exterior = obj.getGeometry().getExterior();
                        var coords = [];
                        exterior.eachLatLngAlt(function (lat, lng) {
                            coords.push([lng, lat]);
                        });
                        features.push({
                            type: 'Feature',
                            geometry: { type: 'Polygon', coordinates: [coords] },
                            properties: {}
                        });
                    } else if (obj instanceof H.map.Circle) {
                        var center = obj.getCenter();
                        features.push({
                            type: 'Feature',
                            geometry: { type: 'Point', coordinates: [center.lng, center.lat] },
                            properties: { radius: obj.getRadius(), shape: 'circle' }
                        });
                    }
                } catch (e) { }
            });

            return JSON.stringify({ type: 'FeatureCollection', features: features });
        },

        // Waypoint sequence optimization
        optimizeWaypointSequence: function (request) {
            return new Promise(function (resolve, reject) {
                if (!herePlatform) {
                    reject('Platform not initialized');
                    return;
                }

                var router = herePlatform.getRoutingService(null, 8);
                var destinations = [];
                if (request.waypoints) {
                    request.waypoints.forEach(function (wp) {
                        destinations.push(wp.lat + ',' + wp.lng);
                    });
                }

                var params = {
                    origin: request.start.lat + ',' + request.start.lng,
                    destination: request.end.lat + ',' + request.end.lng,
                    via: destinations.join('!'),
                    transportMode: request.transportMode || 'car',
                    return: 'summary'
                };

                router.calculateRoute(params, function (result) {
                    var optimizedWaypoints = [];
                    var optimizedIndices = [];
                    var totalDistance = 0;
                    var totalDuration = 0;

                    if (result.routes && result.routes.length > 0) {
                        var route = result.routes[0];
                        if (route.sections) {
                            route.sections.forEach(function (section) {
                                if (section.summary) {
                                    totalDistance += section.summary.length || 0;
                                    totalDuration += section.summary.duration || 0;
                                }
                            });
                        }
                        // Return waypoints in original order (HERE v8 doesn't reorder via waypoints)
                        if (request.waypoints) {
                            request.waypoints.forEach(function (wp, idx) {
                                optimizedIndices.push(idx);
                                optimizedWaypoints.push({ lat: wp.lat, lng: wp.lng });
                            });
                        }
                    }

                    resolve({
                        optimizedIndices: optimizedIndices,
                        optimizedWaypoints: optimizedWaypoints,
                        totalDistance: totalDistance,
                        totalDuration: totalDuration
                    });
                }, function (error) {
                    console.warn('[BlazorHerePlatform] Waypoint sequence error:', error);
                    resolve({
                        optimizedIndices: [],
                        optimizedWaypoints: [],
                        totalDistance: 0,
                        totalDuration: 0
                    });
                });
            });
        }
    };
}();
