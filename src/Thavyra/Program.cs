using FastEndpoints.ClientGen.Kiota;
using Kiota.Builder;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Net.Http.Headers;
using OpenIddict.Validation.AspNetCore;
using Tailwind;
using Thavyra.Data.Configuration;
using Thavyra.Oidc.Configuration;
using Thavyra.Rest.Configuration;
using Thavyra.Rest.Security;
using Thavyra.Storage.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Secret.json", optional: true);
builder.Configuration.AddJsonFile("usernames.json");
builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedHost |
                               ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMassTransit(x =>
{
    x.AddDataConsumers();

    x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
});

var authenticationBuilder = builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "Combined";
        options.DefaultAuthenticateScheme = "Combined";
    })
    .AddPolicyScheme("Combined", "Combined", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var header) &&
                header.FirstOrDefault()?.StartsWith("Bearer ") is true)
            {
                return OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            }

            return CookieAuthenticationDefaults.AuthenticationScheme;
        };
    });

foreach (var section in builder.Configuration.GetChildren())
{
    switch (section.Key)
    {
        case "Data":
            builder.Services.AddEntityFramework(section);
            break;
        case "Storage":
            builder.Services.AddCloudStorage(section);
            break;
        case "Oidc":
            builder.Services.AddOidcAuthorizationServer(section,
                useDevelopmentCertificates: false);
            authenticationBuilder.AddOidcAuthentication(section);
            break;
        case "Rest":
            builder.Services.AddRestApi(section);
            break;
    }
}

builder.Services.AddAuthorizationBuilder()
    .AddOperationPolicies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/accounts/error");
    app.UseForwardedHeaders();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseForwardedHeaders();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseRestApi();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Oidc}/{controller=Login}/{action=Login}/{id?}");

if (app.Environment.IsDevelopment())
{
    _ = app.RunTailwind("tailwind:watch");
}

await app.GenerateApiClientsAndExitAsync(c =>
{
    c.SwaggerDocumentName = "v1";
    c.Language = GenerationLanguage.CSharp;
    c.OutputPath = Path.Combine(app.Environment.WebRootPath, "Clients", "dotnet");
    c.ClientNamespaceName = "Thavyra.Rest.Core";
    c.ClientClassName = "CoreRestClient";
});

app.Run();

public partial class Program;