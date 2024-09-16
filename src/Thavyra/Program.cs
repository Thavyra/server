using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;
using Tailwind;
using Thavyra.Data.Configuration;
using Thavyra.Data.Contexts;
using Thavyra.Data.Seeds;
using Thavyra.Oidc.Configuration;
using Thavyra.Rest.Configuration;
using Thavyra.Rest.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Secret.json");
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

var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

foreach (var section in builder.Configuration.GetChildren())
{
    switch (section.Key)
    {
        case "Data":
            builder.Services.AddEntityFramework(section);
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ThavyraDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Shared}/{controller=Home}/{action=Index}/{id?}");

app.UseRestApi();

if (app.Environment.IsDevelopment())
{
    _ = app.RunTailwind("tailwind:watch");
}

app.Run();