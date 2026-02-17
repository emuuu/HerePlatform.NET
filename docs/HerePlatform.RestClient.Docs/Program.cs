using HerePlatform.RestClient;
using HerePlatform.RestClient.Docs;
using HerePlatform.RestClient.Docs.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register mutable API key service
builder.Services.AddSingleton<DocsApiKeyService>();

// Register our custom IHttpClientFactory BEFORE AddHereRestServices
// so that TryAddSingleton inside AddHttpClient won't overwrite it
builder.Services.AddSingleton<IHttpClientFactory>(sp =>
    new DocsHttpClientFactory(sp.GetRequiredService<DocsApiKeyService>()));

// Register all 9 HERE REST services (auth handler won't be used — our factory handles auth)
builder.Services.AddHereRestServices(opts => opts.ApiKey = "placeholder");

// HttpClient for loading static assets (JSON docs)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IRestApiDocService, RestApiDocService>();

await builder.Build().RunAsync();
