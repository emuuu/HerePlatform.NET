using HerePlatform.Demo.ServerApp.Components;
using HerePlatformComponents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var hereApiKey = builder.Configuration["HerePlatform:ApiKey"] ?? "YOUR_API_KEY";
builder.Services.AddBlazorHerePlatform(new HerePlatformComponents.Maps.HereApiLoadOptions(hereApiKey)
{
    Language = "de"
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
