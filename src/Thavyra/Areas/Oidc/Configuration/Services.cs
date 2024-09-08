using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Abstractions;
using Thavyra.Oidc.Managers;
using Thavyra.Oidc.Models.Internal;
using Thavyra.Oidc.Stores;

namespace Thavyra.Oidc.Configuration;

public static class Services
{
    public static void AddOidcAuthorizationServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(Services).Assembly);
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
            options.AccessDeniedPath = "/login";

            options.ExpireTimeSpan = DateTimeOffset.UtcNow.AddMonths(6) - DateTimeOffset.UtcNow;
            options.SlidingExpiration = true;
        });

        return builder;
    }
}