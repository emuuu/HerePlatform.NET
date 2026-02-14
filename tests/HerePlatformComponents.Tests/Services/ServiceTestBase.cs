using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace HerePlatformComponents.Tests.Services;

public abstract class ServiceTestBase : BunitTestBase
{
    protected IJSRuntime JsRuntime => Context.Services.GetRequiredService<IJSRuntime>();

    protected void MockJsResult<T>(string identifier, T result)
    {
        Context.JSInterop.Setup<T>(identifier, _ => true).SetResult(result);
    }
}
