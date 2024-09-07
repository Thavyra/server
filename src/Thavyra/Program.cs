using MassTransit;
using Tailwind;
using Thavyra.Data.Configuration;
using Thavyra.Data.Contexts;
using Thavyra.Data.Seeds;
using Thavyra.Oidc.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMassTransit(x =>
{
    
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

var authenticationBuilder = builder.Services.AddAuthentication();

foreach (var section in builder.Configuration.GetChildren())
{
    switch (section.Key)
    {
        case "Oidc":
            builder.Services.AddOidcAuthorizationServer(section);
            authenticationBuilder.AddOidcAuthentication(section);
            break;
        case "Data":
            builder.Services.AddEntityFramework(section);
            break;
    }
}

builder.Services.AddAuthorization();

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

if (app.Environment.IsDevelopment())
{
    _ = app.RunTailwind("tailwind:watch");
}

app.Run();