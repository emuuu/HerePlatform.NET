using HerePlatform.Demo.ServerApp.Components;
using HerePlatformComponents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Increase SignalR message size for large routing/data responses
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 512 * 1024; // 512 KB
});

var hereApiKey = builder.Configuration["HerePlatform:ApiKey"] ?? "YOUR_API_KEY";
builder.Services.AddBlazorHerePlatform(new HerePlatformComponents.Maps.HereApiLoadOptions(hereApiKey)
{
    Language = "de",
    LoadClustering = true,
    LoadData = true
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(HerePlatform.Demo.Components._Imports).Assembly);

app.Run();
