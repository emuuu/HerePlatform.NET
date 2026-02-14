using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HerePlatformComponents;

internal static partial class Helper
{
    internal static async Task MyInvokeAsync(
        this IJSRuntime jsRuntime,
        string identifier,
        params object?[] args)
    {
        var jsFriendlyArgs = MakeArgJsFriendly(jsRuntime, args);
        await jsRuntime.InvokeVoidAsync(identifier, jsFriendlyArgs);
    }

    internal static async Task<TRes?> MyInvokeAsync<TRes>(
        this IJSRuntime jsRuntime,
        string identifier,
        params object?[] args)
    {
        var jsFriendlyArgs = MakeArgJsFriendly(jsRuntime, args);

        if (typeof(IJsObjectRef).IsAssignableFrom(typeof(TRes)))
        {
            var guid = await jsRuntime.InvokeAsync<string?>(identifier, jsFriendlyArgs);
            if (guid == null) return default;
            try
            {
                return (TRes)JsObjectRefInstances.GetInstance(guid);
            }
            catch (KeyNotFoundException)
            {
                return default;
            }
        }

        if (typeof(IOneOf).IsAssignableFrom(typeof(TRes)))
        {
            var resultObject = await jsRuntime.InvokeAsync<string>(identifier, jsFriendlyArgs);
            object? result = null;

            if (resultObject is string someText)
            {
                try
                {
                    var jo = JsonDocument.Parse(someText);
                    var typeToken = jo.RootElement.GetProperty("dotnetTypeName").GetString();
                    if (typeToken != null)
                    {
                        var oneOfTypeArgs = typeof(TRes).GetGenericArguments();
                        var resolvedType = ResolveOneOfType(typeToken, oneOfTypeArgs);
                        if (resolvedType is not null)
                            result = DeSerializeObject(someText, resolvedType);
                    }
                    else
                    {
                        result = someText;
                    }
                }
                catch (JsonException)
                {
                    result = someText;
                }
                catch (KeyNotFoundException)
                {
                    result = someText;
                }
            }

            return (TRes?)result;
        }
        else
        {
            return await jsRuntime.InvokeAsync<TRes>(identifier, jsFriendlyArgs);
        }
    }

    internal static async Task<object> MyAddListenerAsync(
        this IJSRuntime jsRuntime,
        string identifier,
        params object[] args)
    {
        var jsFriendlyArgs = MakeArgJsFriendly(jsRuntime, args);

        return await jsRuntime.InvokeAsync<object>(identifier, jsFriendlyArgs);
    }

    internal static async Task<OneOf<T, U>> MyInvokeAsync<T, U>(
        this IJSRuntime jsRuntime,
        string identifier,
        params object[] args)
    {
        var resultObject = await jsRuntime.MyInvokeAsync<object>(identifier, args);
        object? result = null;

        if (resultObject is JsonElement jsonElement)
        {
            string? json;
            if (jsonElement.ValueKind == JsonValueKind.Number)
            {
                json = jsonElement.GetRawText();
            }
            else if (jsonElement.ValueKind == JsonValueKind.String)
            {
                json = jsonElement.GetString();
                if (typeof(T) == typeof(string))
                {
                    result = json ?? "";
                    return (T)result;
                }

                if (typeof(U) == typeof(string))
                {
                    result = json ?? "";
                    return (U)result;
                }
            }
            else
            {
                json = jsonElement.GetString();
            }

            var propArray = Helper.DeSerializeObject<Dictionary<string, object>>(json);
            if (propArray?.TryGetValue("dotnetTypeName", out var typeName) ?? false)
            {
                var typeNameString = typeName.ToString()!;
                var type = ResolveOneOfType(typeNameString, typeof(T), typeof(U));
                if (type is not null)
                    result = Helper.DeSerializeObject(json, type);
            }
        }

        switch (result)
        {
            case T t:
                return t;
            case U u:
                return u;
            default:
                return default;
        }
    }

    internal static async Task<OneOf<T, U, V>> MyInvokeAsync<T, U, V>(
        this IJSRuntime jsRuntime,
        string identifier,
        params object[] args)
    {
        var resultObject = await jsRuntime.MyInvokeAsync<object>(identifier, args);
        object? result = null;

        if (resultObject is JsonElement jsonElement)
        {
            var json = jsonElement.GetString();
            var propArray = Helper.DeSerializeObject<Dictionary<string, object>>(json);
            if (propArray?.TryGetValue("dotnetTypeName", out var typeName) ?? false)
            {
                var typeNameString = typeName.ToString()!;
                var type = ResolveOneOfType(typeNameString, typeof(T), typeof(U), typeof(V));
                if (type is not null)
                    result = Helper.DeSerializeObject(json, type);
            }
        }

        switch (result)
        {
            case T t:
                return t;
            case U u:
                return u;
            case V v:
                return v;
            default:
                return default;
        }
    }

    // Note: DotNetObjectReference instances created here for Action/Func parameters
    // are not tracked for disposal. Their lifetime is tied to the JS event listener
    // or callback they wrap. For v0.1 this is acceptable — the leak is proportional
    // to the number of imperative AddListener calls. A future version should track
    // these references on the parent JsObjectRef for cleanup in DisposeAsync.
    private static IEnumerable<object?> MakeArgJsFriendly(IJSRuntime jsRuntime, IEnumerable<object?> args)
    {
        var jsFriendlyArgs = args
            .Select(arg =>
            {
                if (arg == null)
                {
                    return arg;
                }

                if (arg is IOneOf oneof)
                {
                    arg = oneof.Value;
                }

                var argType = arg.GetType();

                switch (arg)
                {
                    case Enum: return GetEnumValue(arg);
                    case ElementReference _:
                    case string _:
                    case int _:
                    case long _:
                    case double _:
                    case float _:
                    case decimal _:
                    case DateTime _:
                    case bool _:
                        return arg;
                    case Action action:
                        return DotNetObjectReference.Create(new JsCallableAction(jsRuntime, action));
                    default:
                        {
                            if (argType.IsGenericType)
                            {
                                var typeDefinition = argType.GetGenericTypeDefinition();
                                if (typeDefinition == typeof(Action<>))
                                {
                                    var genericArguments = argType.GetGenericArguments();
                                    return DotNetObjectReference.Create(new JsCallableAction(jsRuntime, (Delegate)arg, genericArguments));
                                }

                                if (typeDefinition == typeof(Func<,>))
                                {
                                    var genericArguments = argType.GetGenericArguments();
                                    return DotNetObjectReference.Create(new JsCallableFunc((Delegate)arg, genericArguments));
                                }
                            }

                            switch (arg)
                            {
                                case JsCallableAction _:
                                case JsCallableFunc _:
                                    return DotNetObjectReference.Create(arg);
                                case IJsObjectRef jsObjectRef:
                                    {
                                        var guid = jsObjectRef.Guid;
                                        return SerializeObject(new JsObjectRefDto(guid));
                                    }
                                default:
                                    return SerializeObject(arg);
                            }
                        }
                }
            });

        return jsFriendlyArgs;
    }
}
