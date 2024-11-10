using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Net.Http.Headers;
using OpenIddict.Validation.AspNetCore;
using Tailwind;
using Thavyra.Data.Configuration;
using Thavyra.Oidc.Configuration;
using Thavyra.Rest.Configuration;
using Thavyra.Rest.Security;
using Thavyra.Storage.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Secret.json");
builder.Configuration.AddJsonFile("usernames.json");
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMassTransit(x =>
{
    x.AddDataConsumers();
    
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
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
            builder.Services.AddOidcAuthorizationServer(section);
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
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

app.Run();

public partial class Program;
