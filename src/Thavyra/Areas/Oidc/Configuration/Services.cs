using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Abstractions;
using Thavyra.Oidc.Managers;
using Thavyra.Oidc.Models.Internal;
using Thavyra.Oidc.Models.View;
using Thavyra.Oidc.Stores;

namespace Thavyra.Oidc.Configuration;

public static class Services
{
    public static void AddOidcAuthorizationServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IValidator<LoginViewModel>, LoginViewModel.Validator>();
        services.AddScoped<IValidator<RegisterViewModel>, RegisterViewModel.Validator>();
        services.AddFluentValidationClientsideAdapters();
        
        services.AddTransient<IOpenIddictApplicationStore<ApplicationModel>, ApplicationStore>();
        services.AddTransient<IOpenIddictAuthorizationStore<AuthorizationModel>, AuthorizationStore>();
        services.AddTransient<IOpenIddictScopeStore<ScopeModel>, ScopeStore>();
        services.AddTransient<IOpenIddictTokenStore<TokenModel>, TokenStore>();

        services.AddTransient<IUserManager, UserManager>();

        var builder = services.AddOpenIddict()
            .AddCore(options =>
            {
                options.AddApplicationStore<ApplicationStore>()
                    .SetDefaultApplicationEntity<ApplicationModel>()
                    .ReplaceApplicationManager<ApplicationManager>();

                options.AddAuthorizationStore<AuthorizationStore>()
                    .SetDefaultAuthorizationEntity<AuthorizationModel>();

                options.AddScopeStore<ScopeStore>()
                    .SetDefaultScopeEntity<ScopeModel>();

                options.AddTokenStore<TokenStore>()
                    .SetDefaultTokenEntity<TokenModel>();
            });
        
        foreach (var section in configuration.GetChildren())
        {
            switch (section.Key)
            {
                case "Server":
                    builder.AddServer(options =>
                    {
                        options
                            .SetAuthorizationEndpointUris("oauth/authorize")
                            .SetTokenEndpointUris("oauth/token")
                            .SetLogoutEndpointUris("connect/logout")
                            .SetUserinfoEndpointUris("connect/userinfo")
                            .SetIssuer(section["Issuer"] ??
                                       throw new Exception("OIDC issuer not provided."));

                        options
                            .AllowAuthorizationCodeFlow()
                            .AllowImplicitFlow()
                            .AllowClientCredentialsFlow()
                            .AllowRefreshTokenFlow();

                        options
                            .AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();
                        
                        options.UseAspNetCore()
                            .EnableAuthorizationEndpointPassthrough()
                            .EnableTokenEndpointPassthrough()
                            .EnableUserinfoEndpointPassthrough()
                            .DisableTransportSecurityRequirement();
                    })
                    .AddValidation(options =>
                    {
                        options.UseLocalServer();
                        options.UseAspNetCore();
                    });
                    
                    break;
                
                case "Client":
                    builder.AddClient(options =>
                    {
                        options.AllowAuthorizationCodeFlow();

                        options.AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();

                        options.UseAspNetCore()
                            .DisableTransportSecurityRequirement()
                            .EnableRedirectionEndpointPassthrough();

                        var providers = options.UseWebProviders();

                        foreach (var provider in section.GetChildren())
                            switch (provider.Key)
                            {
                                case "Discord":
                                    providers.AddDiscord(discord => discord
                                        .SetClientId(provider["ClientId"]
                                                     ?? throw new Exception("Discord ClientId not provided."))
                                        .SetClientSecret(provider["ClientSecret"]
                                                         ?? throw new Exception("Discord ClientSecret not provided."))
                                        .SetRedirectUri("callback/discord")
                                        .AddScopes(provider.GetValue<string[]>("Scopes") ?? []));

                                    break;

                                case "GitHub":
                                    providers.AddGitHub(github => github
                                        .SetClientId(provider["ClientId"]
                                                     ?? throw new Exception("GitHub ClientId not provided."))
                                        .SetClientSecret(provider["ClientSecret"]
                                                         ?? throw new Exception("GitHub ClientSecret not provided."))
                                        .SetRedirectUri("callback/github")
                                        .AddScopes(provider.GetValue<string[]>("Scopes") ?? []));

                                    break;
                            }
                    });
                    
                    break;
            }
        }
    }

    public static AuthenticationBuilder AddOidcAuthentication(this AuthenticationBuilder builder,
        IConfiguration configuration)
    {
        builder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/login";
            options.SlidingExpiration = false;
        });

        return builder;
    }
}