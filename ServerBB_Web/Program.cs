using Blazored.LocalStorage;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using NLog.Extensions.Logging;
using ServerBB_Web.Components;
using ServerBB_Web.Service;
using ServerBB_Web.Service.Interface;
using ServerBB_Web.Service.Refs;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Logging (NLog)
// =========================
builder.Logging.AddNLog();

// =========================
// Configuration
// =========================
var configuration = builder.Configuration;

// URL da API externa (Server_API - porta 5020)
var apiBaseAddress =
    configuration["ConnectionSettings:ApiBaseAddress"]
    ?? throw new InvalidOperationException("ConnectionSettings:ApiBaseAddress não configurado");

// Porta do Frontend (ServerBB_Web)
var bindPort =
    int.Parse(configuration["ConnectionSettings:BindPort"] ?? "5023");

// =========================
// Kestrel
// =========================
// ⚠️ IMPORTANTE:
// - Em Development: NÃO forçamos porta (VS / launchSettings controlam)
// - Em Production: usamos BindPort do appsettings
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(bindPort); // Frontend
    });

    // Garante arquivos estáticos após publish
    builder.WebHost.UseStaticWebAssets();
}

// =========================
// Services
// =========================
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// HttpClient para acessar o backend (Server_API - 5020)
builder.Services.AddScoped(_ =>
    new HttpClient
    {
        BaseAddress = new Uri(apiBaseAddress)
    });

builder.Services.AddSingleton<IMonthService, MonthService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<SpendingService>();
builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

// =========================
// Localization (pt-BR)
// =========================
var supportedCultures = new[] { new CultureInfo("pt-BR") };

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

CultureInfo.DefaultThreadCurrentCulture = supportedCultures[0];
CultureInfo.DefaultThreadCurrentUICulture = supportedCultures[0];

app.UseRequestLocalization(localizationOptions);

// =========================
// Proxy / Nginx
// =========================
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

// =========================
// Static files
// =========================
app.UseStaticFiles();

// =========================
// HTTP pipeline
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();