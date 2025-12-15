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


// Logging (NLog)
// Configuration

builder.Logging.AddNLog();

var configuration = builder.Configuration;

// URL da API externa (Server_API - porta 5020)
var apiBaseAddress =
    configuration["ConnectionSettings:ApiBaseAddress"]
    ?? throw new InvalidOperationException(
        "ConnectionSettings:ApiBaseAddress não configurado");

// Porta do Frontend (ServerBB_Web)
var bindPort =
    int.Parse(configuration["ConnectionSettings:BindPort"] ?? "5023");


// Kestrel
// ✔️ Development: Visual Studio / launchSettings controlam
// ✔️ Production: Kestrel escuta na porta configurada

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // escuta em todas as interfaces
        options.ListenAnyIP(bindPort); 
    });

    // Necessário após publish
    builder.WebHost.UseStaticWebAssets();
}


// Services
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// HttpClient para acessar o backend (Server_API)
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


// Localization (pt-BR)
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


// Proxy / Nginx
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});


// Static files
app.UseStaticFiles();


// HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// IMPORTANTE
// Se você NÃO usa HTTPS no Kestrel diretamente,
// este middleware pode causar problema em produção atrás do Nginx
// Se der erro, COMENTE esta linha
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
