using HerePlatform.Docs;
using HerePlatform.Docs.Services;
using HerePlatform.RestClient;
using HerePlatformComponents;
using HerePlatformComponents.Maps;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazorHerePlatform(new HereApiLoadOptions("YOUR_API_KEY") { LoadClustering = true, LoadData = true });
builder.Services.AddScoped<IDocContentService, DocContentService>();
builder.Services.AddScoped<IApiDocService, ApiDocService>();

// Mutable API key service (Singleton for WASM)
builder.Services.AddSingleton<DocsApiKeyService>();

// Custom IHttpClientFactory BEFORE AddHereRestServices (TryAddSingleton!)
builder.Services.AddSingleton<IHttpClientFactory>(sp =>
    new DocsHttpClientFactory(sp.GetRequiredService<DocsApiKeyService>()));

// All HERE REST Services
builder.Services.AddHereRestServices(opts => opts.ApiKey = "placeholder");

// REST API Doc Service
builder.Services.AddScoped<IRestApiDocService, RestApiDocService>();

await builder.Build().RunAsync();
