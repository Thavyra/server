using System.Security.Claims;
using System.Text.Json;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Client.WebIntegration;
using OpenIddict.Server.AspNetCore;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Providers;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Controllers;

public class ProviderController : Controller
{
    private readonly IRequestClient<ProviderLogin> _providerLogin;
    private readonly IRequestClient<LinkProvider> _linkProvider;

    public ProviderController(
        IRequestClient<ProviderLogin> providerLogin,
        IRequestClient<LinkProvider> linkProvider)
    {
        _providerLogin = providerLogin;
        _linkProvider = linkProvider;
    }
    
    [HttpPost("/login/{provider}"), ValidateAntiForgeryToken]
    public IActionResult Login(string provider, string returnUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
        };

        return provider switch
        {
            Constants.Providers.Discord => Challenge(properties,
                OpenIddictClientWebIntegrationConstants.Providers.Discord),
            Constants.Providers.GitHub => Challenge(properties,
                OpenIddictClientWebIntegrationConstants.Providers.GitHub),
            _ => BadRequest("Invalid provider.")
        };
    }

    [HttpGet("/callback/{provider}")]
    public async Task<IActionResult> CallbackAsync(string provider, CancellationToken cancellationToken)
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

        var redirectUri = new Uri(result.Properties?.RedirectUri ?? "/");

        var login = provider switch
        {
            Constants.Providers.Discord => GetDiscordLogin(result.Principal),
            Constants.Providers.GitHub => GetGitHubLogin(result.Principal),
            _ => new InvalidProviderLoginModel()
        };

        if (login is InvalidProviderLoginModel)
        {
            return BadRequest("Invalid provider.");
        }

        Response response = await _providerLogin.GetResponse<UserRegistered, LoginSucceeded>(new ProviderLogin
        {
            Provider = provider switch
            {
                Constants.Providers.Discord => Constants.LoginTypes.Discord,
                Constants.Providers.GitHub => Constants.LoginTypes.GitHub,
                _ => throw new InvalidOperationException()
            },
            AccountId = login.AccountId,
            Username = login.Username,
            AvatarUrl = login.AvatarUrl
        }, cancellationToken);

        return response switch
        {
            (_, UserRegistered message) => SignInWithProvider(provider, message.UserId, message.Username,
                login.AvatarUrl, redirectUri),
            (_, LoginSucceeded message) => SignInWithProvider(provider, message.UserId, message.Username,
                login.AvatarUrl, redirectUri),
            _ => throw new InvalidOperationException()
        };
    }
    
    private SignInResult SignInWithProvider(string provider, Guid userId, string username, string? avatarUrl, Uri redirectUri)
    {
        var identity = new ClaimsIdentity(authenticationType: provider);

        identity.SetClaim(ClaimTypes.NameIdentifier, userId.ToString());
        identity.SetClaim(ClaimTypes.Name, username);
        identity.SetClaim(OpenIddictConstants.Claims.Picture, avatarUrl);

        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri.OriginalString
        };

        return SignIn(new ClaimsPrincipal(identity), properties, CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("/callback/{provider}/link")]
    public async Task<IActionResult> LinkCallbackAsync(string provider, CancellationToken cancellationToken)
    {
        var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!cookieResult.Succeeded
            || !Guid.TryParse(cookieResult.Principal.GetClaim(ClaimTypes.NameIdentifier), out var userId))
        {
            return Challenge(
                authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + Request.QueryString
                });
        }

        var providerResult =
            await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

        if (!Uri.TryCreate(providerResult.Properties?.RedirectUri, UriKind.Absolute, out var redirectUri))
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictConstants.Parameters.Error] = OpenIddictConstants.Errors.ServerError,
                    [OpenIddictConstants.Parameters.ErrorDescription] = "Redirect URI was invalid."
                }));
        }

        var login = provider switch
        {
            Constants.Providers.Discord => GetDiscordLogin(cookieResult.Principal),
            Constants.Providers.GitHub => GetGitHubLogin(cookieResult.Principal),
            _ => new InvalidProviderLoginModel()
        };

        if (login is InvalidProviderLoginModel)
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictConstants.Parameters.Error] = OpenIddictConstants.Errors.InvalidRequest,
                    [OpenIddictConstants.Parameters.ErrorDescription] = "Provider was invalid."
                }));
        }

        Response response = await _linkProvider.GetResponse<ProviderLinked, AccountAlreadyRegistered>(new LinkProvider
        {
            Provider = provider switch
            {
                Constants.Providers.Discord => Constants.LoginTypes.Discord,
                Constants.Providers.GitHub => Constants.LoginTypes.GitHub,
                _ => throw new InvalidOperationException()
            },
            UserId = userId,
            AccountId = login.AccountId,
            Username = login.Username,
            AvatarUrl = login.AvatarUrl
        }, cancellationToken);

        return response switch
        {
            (_, ProviderLinked) => Redirect(redirectUri.ToString()),
            (_, AccountAlreadyRegistered) => LinkProviderFail(
                error: $"link_{provider}_error",
                errorDescription: "This account is already in use by another user.",
                redirectUri),
            _ => throw new InvalidOperationException()
        };
    }
    
    private static IProviderLoginModel GetDiscordLogin(ClaimsPrincipal? principal)
    {
        if (principal is not { Identity.IsAuthenticated: true } || principal.GetClaim("user") is not { } claim)
        {
            return new InvalidProviderLoginModel();
        }

        var login = JsonSerializer.Deserialize<DiscordLoginModel>(claim);

        if (login is null)
        {
            return new InvalidProviderLoginModel();
        }

        return login;
    }

    private static IProviderLoginModel GetGitHubLogin(ClaimsPrincipal? principal)
    {
        if (principal is not { Identity.IsAuthenticated: true }
            || principal.GetClaim("id") is not { } githubId
            || principal.GetClaim("login") is not { } username
            || principal.GetClaim("avatar_url") is not { } avatarUrl)
        {
            return new InvalidProviderLoginModel();
        }

        return new DefaultProviderLoginModel(AccountId: githubId, username, avatarUrl);
    }

    private RedirectResult LinkProviderFail(string error, string errorDescription, Uri redirectUri)
    {
        var uriBuilder = new UriBuilder(redirectUri);
        
        var query = QueryString.FromUriComponent(redirectUri.Query);
        
        query = query.Add(OpenIddictConstants.Parameters.Error, error);
        query = query.Add(OpenIddictConstants.Parameters.ErrorDescription, errorDescription);

        uriBuilder.Query = query.ToUriComponent();

        return Redirect(uriBuilder.ToString());
    }
}