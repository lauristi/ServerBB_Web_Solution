using Blazored.LocalStorage;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using NLog.Extensions.Logging;
using ServerBB_Web.Components;
using ServerBB_Web.Service;
using ServerBB_Web.Service.Interface;
using ServerBB_Web.Service.Refs;
using System.Globalization;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Adicionando o Nlog ao pipeline
builder.Logging.AddNLog();

// A porta 5023 é para a aplicação Blazor Server (ServerBB_Web)
// A porta 5020 é para a Web API (Server_API)

var configuration = builder.Configuration;

var apiBaseAddress = configuration["ConnectionSettings:ApiBaseAddress"];
var bindAddress = configuration["ConnectionSettings:BindAddress"];
var bindPort = int.Parse(configuration["ConnectionSettings:BindPort"] ?? "5023");

// Configure o Kestrel para ouvir em todas as interfaces de rede na porta 5020
// Adiciona o middleware UseStaticWebAssets para servir arquivos estáticos, incluindo CSS, em produção

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(IPAddress.Parse(bindAddress), bindPort);
    });

    builder.WebHost.UseStaticWebAssets();
    //builder.Services.AddDirectoryBrowser();
}

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// ENDERECO PARA A WEBAPI (altere de acordo com sua implantação)
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(apiBaseAddress)
    });

//Servico de Download
//builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddSingleton<IMonthService, MonthService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<SpendingService>();

//Local Storage Service
builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

//Determinando o uso de Pt-br para a App
var supportedCultures = new[] { new CultureInfo("pt-BR") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

//Garatir que seja usado em todas as threads
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

app.UseRequestLocalization(localizationOptions);

//Configuracao de Cabecalho encaminhado para funcionar com proxy reverso... Ngnix
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

//REMOVIDO PARA EVITAR ERRO DE AUTENTICACAO NO  Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider No Raspbery PI
//app.UseAuthentication();

//app.UseFileServer();
//var provider = new FileExtensionContentTypeProvider();
//provider.Mappings["{EXTENSION}"] = "{CONTENT TYPE}";
//app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });
//app.UseFileServer(enableDirectoryBrowsing: true);

app.Run();