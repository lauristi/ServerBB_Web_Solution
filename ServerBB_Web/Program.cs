using ServerBB_Web.Components;
using ServerBB_Web.Service;
using ServerBB_Web.Service.Interface;
using Microsoft.AspNetCore.HttpOverrides;
using NLog.Extensions.Logging;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Adicionando o Nlog ao pipeline
builder.Logging.AddNLog();

// Configure o Kestrel para ouvir em todas as interfaces de rede na porta 5020
// Adiciona o middleware UseStaticWebAssets para servir arquivos est�ticos, incluindo CSS, em produ��o
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(IPAddress.Parse("192.168.0.156"), 5023);
    });

    builder.WebHost.UseStaticWebAssets();
    //builder.Services.AddDirectoryBrowser();
}

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// ENDERECO PARA A WEBAPI (altere de acordo com sua implanta��o)
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("http://192.168.0.156:5020")
    });

//Servico de Download
builder.Services.AddScoped<IFileDownloadService, FileDownloadService>();
builder.Services.AddSingleton<IMonthService, MonthService>();


var app = builder.Build();

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