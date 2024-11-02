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

[Area("Oidc")]
public class DiscordController : Controller
{
    private readonly IRequestClient<ProviderLogin> _providerLogin;
    private readonly IRequestClient<LinkProvider> _linkProvider;

    public DiscordController(
        IRequestClient<ProviderLogin> providerLogin,
        IRequestClient<LinkProvider> linkProvider)
    {
        _providerLogin = providerLogin;
        _linkProvider = linkProvider;
    }

    [FromQuery] public string? ReturnUrl { get; set; }

    [HttpGet("/login/discord")]
    public IActionResult DiscordAsync()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : "/"
        };

        return Challenge(properties, OpenIddictClientWebIntegrationConstants.Providers.Discord);
    }

    [HttpGet("/callback/discord")]
    public async Task<IActionResult> DiscordCallbackAsync(CancellationToken cancellationToken)
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        
        var redirectUri = new Uri(result.Properties?.RedirectUri ?? "/");

        var login = GetDiscordLogin(result.Principal);

        Response response = await _providerLogin.GetResponse<UserRegistered, LoginSucceeded>(new ProviderLogin
        {
            Provider = Constants.LoginTypes.Discord,
            AccountId = login.Id,
            Username = login.Username,
            AvatarUrl = login.AvatarUrl
        }, cancellationToken);
        
        return response switch
        {
            (_, UserRegistered message) => SignInWithDiscord(message.UserId, message.Username, redirectUri),
            (_, LoginSucceeded message) => SignInWithDiscord(message.UserId, message.Username, redirectUri),
            _ => throw new InvalidOperationException()
        };
    }

    [HttpGet("/callback/discord/link")]
    public async Task<IActionResult> LinkDiscordAsync(CancellationToken cancellationToken)
    {
        var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!cookieResult.Succeeded 
            || !Guid.TryParse(cookieResult.Principal.GetClaim(ClaimTypes.NameIdentifier), out var userId))
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Request.PathBase + Request.Path + Request.QueryString
            }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        var discordResult = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

        if (!Uri.TryCreate(discordResult.Properties?.RedirectUri, UriKind.Absolute, out var redirectUri))
        {
            throw new InvalidOperationException("Redirect URI is not valid.");
        }

        var login = GetDiscordLogin(discordResult.Principal);

        Response response = await _linkProvider.GetResponse<ProviderLinked, AccountAlreadyRegistered>(new LinkProvider
        {
            UserId = userId,
            Provider = Constants.LoginTypes.Discord,
            AccountId = login.Id,
            Username = login.Username,
            AvatarUrl = login.AvatarUrl
        }, cancellationToken);

        return response switch
        {
            (_, ProviderLinked message) => SignInWithDiscord(
                message.UserId, 
                cookieResult.Principal.GetClaim(ClaimTypes.Name) ?? message.Username, 
                redirectUri),
            (_, AccountAlreadyRegistered) => Fail(
                error: "link_discord_error",
                errorDescription: "This Discord account is already in use by another user.", 
                redirectUri),
            _ => throw new InvalidOperationException()
        };
    }

    private static DiscordLoginModel GetDiscordLogin(ClaimsPrincipal? principal)
    {
        if (principal is not { Identity.IsAuthenticated: true } || principal.GetClaim("user") is not { } claim)
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        var login = JsonSerializer.Deserialize<DiscordLoginModel>(claim);

        if (login is null)
        {
            throw new InvalidOperationException("Could not parse Discord login data.");
        }

        return login;
    }

    private SignInResult SignInWithDiscord(Guid userId, string username, Uri redirectUri)
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            ],
            authenticationType: "Discord");

        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri.OriginalString
        };

        return SignIn(new ClaimsPrincipal(identity), properties, CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private RedirectResult Fail(string error, string errorDescription, Uri redirectUri)
    {
        var uriBuilder = new UriBuilder(redirectUri);
        
        var query = QueryString.FromUriComponent(redirectUri.Query);
        
        query.Add(OpenIddictConstants.Parameters.Error, error);
        query.Add(OpenIddictConstants.Parameters.ErrorDescription, errorDescription);

        uriBuilder.Query = query.ToUriComponent();

        return Redirect(uriBuilder.ToString());
    }
}