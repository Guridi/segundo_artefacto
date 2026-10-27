using CatalogoApartamentosWeb.Components;
using CatalogoApartamentosWeb.Interfaces;
using CatalogoApartamentosWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var baseUrl = builder.Configuration["CatalogoApi:BaseUrl"]
    ?? throw new InvalidOperationException("No se configuró CatalogoApi:BaseUrl.");

// Cliente tipado: el HttpClient se inyecta en CatalogoApiService junto con ILogger e IConfiguration.
builder.Services.AddHttpClient<ICatalogoApiService, CatalogoApiService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(
        builder.Configuration.GetValue("CatalogoApi:TimeoutSegundos", 10));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
