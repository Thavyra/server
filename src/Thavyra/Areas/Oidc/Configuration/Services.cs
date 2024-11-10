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
                            .SetAuthorizationEndpointUris("accounts/connect")
                            .SetTokenEndpointUris("accounts/token")
                            .SetUserinfoEndpointUris("accounts/@me")
                            .SetIssuer(section["Issuer"] ??
                                       throw new Exception("OIDC issuer not provided."));

                        options
                            .AllowAuthorizationCodeFlow()
                            .AllowImplicitFlow()
                            .AllowClientCredentialsFlow()
                            .AllowRefreshTokenFlow()
                            .AllowNoneFlow();

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
                                    var discordClientId = provider["ClientId"]
                                                          ?? throw new Exception("Discord ClientId not provided.");
                                    
                                    var discordClientSecret = provider["ClientSecret"]
                                                              ?? throw new Exception("Discord ClientSecret not provided.");

                                    var discordScopes = provider.GetValue<string[]>("Scopes") ?? [];

                                    providers.AddDiscord(discord => discord
                                            
                                        // ReSharper disable once AccessToModifiedClosure
                                        .SetClientId(discordClientId)
                                        
                                        // ReSharper disable once AccessToModifiedClosure
                                        .SetClientSecret(discordClientSecret)
                                        
                                        .SetRedirectUri($"accounts/callback/{Constants.Providers.Discord}")
                                        
                                        // ReSharper disable once AccessToModifiedClosure
                                        .AddScopes(discordScopes));

                                    providers.AddDiscord(linkDiscord => linkDiscord
                                            
                                        // ReSharper disable once AccessToModifiedClosure
                                        .SetClientId(discordClientId)
                                        
                                        // ReSharper disable once AccessToModifiedClosure
                                        .SetClientSecret(discordClientSecret)
                                        
                                        .SetRedirectUri($"accounts/callback/{Constants.Providers.Discord}/link")
                                        
                                        // ReSharper disable once AccessToModifiedClosure
                                        .AddScopes(discordScopes)
                                        
                                        .SetProviderName(OidcConstants.AuthenticationSchemes.LinkDiscord));

                                    break;

                                case "GitHub":
                                    var githubClientId = provider["ClientId"]
                                                         ?? throw new Exception("GitHub ClientId not provided.");

                                    var githubClientSecret = provider["ClientSecret"]
                                                             ?? throw new Exception(
                                                                 "GitHub ClientSecret not provided.");
                                    
                                    var githubScopes = provider.GetValue<string[]>("Scopes") ?? [];
                                    
                                    providers.AddGitHub(github => github
                                            
                                        // ReSharper disable once AccessToModifiedClosure
                                        .SetClientId(githubClientId)
                                        
                                        // ReSharper disable once AccessToModifiedClosure
                                        .SetClientSecret(githubClientSecret)
                                        .SetRedirectUri($"accounts/callback/{Constants.Providers.GitHub}")
                                        
                                        // ReSharper disable once AccessToModifiedClosure
                                        .AddScopes(githubScopes));

                                    providers.AddGitHub(linkGitHub => linkGitHub
                                            
                                        // ReSharper disable once AccessToModifiedClosure
                                        .SetClientId(githubClientId)
                                        
                                        // ReSharper disable once AccessToModifiedClosure
                                        .SetClientSecret(githubClientSecret)
                                        .SetRedirectUri($"accounts/callback/{Constants.Providers.GitHub}/link")
                                        
                                        // ReSharper disable once AccessToModifiedClosure
                                        .AddScopes(githubScopes)
                                        .SetProviderName(OidcConstants.AuthenticationSchemes.LinkGitHub));
                                    
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
            options.LoginPath = "/accounts/login";
            options.SlidingExpiration = false;
        });

        return builder;
    }
}