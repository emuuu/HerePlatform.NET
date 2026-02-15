using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Text.Json;

namespace HerePlatformComponents;

public class JsCallableAction
{
    private readonly Delegate _delegate;
    private readonly Type[] _argumentTypes;
    private readonly IJSRuntime _jsRuntime;

    public JsCallableAction(IJSRuntime jsRuntime, Delegate @delegate, params Type[] argumentTypes)
    {
        _jsRuntime = jsRuntime;
        _delegate = @delegate;
        _argumentTypes = argumentTypes;
    }

    [JSInvokable]
    public void Invoke(string args, string guid)
    {
        if (string.IsNullOrWhiteSpace(args) || !_argumentTypes.Any())
        {
            _delegate.DynamicInvoke();
            return;
        }

        var rootElement = JsonDocument.Parse(args).RootElement;
        JsonElement.ArrayEnumerator jArray = rootElement.ValueKind == JsonValueKind.Array
            ? rootElement.EnumerateArray() : [];

        var arguments = _argumentTypes.Zip(jArray, (type, jToken) => new { jToken, type })
            .Select(x =>
            {
                var obj = Helper.DeSerializeObject(x.jToken, x.type);
                if (obj is IActionArgument actionArg)
                {
                    if (!Guid.TryParse(guid, out var parsedGuid))
                        throw new InvalidOperationException($"JS returned invalid GUID: '{guid}'");
                    actionArg.JsObjectRef = new JsObjectRef(_jsRuntime, parsedGuid);
                }

                return obj;
            })
            .ToArray();

        _delegate.DynamicInvoke(arguments);
    }
}
