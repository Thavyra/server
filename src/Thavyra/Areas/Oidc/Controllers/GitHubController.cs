using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Client.WebIntegration;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Providers;

namespace Thavyra.Oidc.Controllers;

public class GitHubController : Controller
{
    private readonly IRequestClient<ProviderLogin> _providerLogin;
    private readonly IRequestClient<LinkProvider> _linkProvider;

    public GitHubController(
        IRequestClient<ProviderLogin> providerLogin,
        IRequestClient<LinkProvider> linkProvider)
    {
        _providerLogin = providerLogin;
        _linkProvider = linkProvider;
    }

    [FromQuery] public string? ReturnUrl { get; set; }

    [HttpGet("/login/github")]
    public IActionResult GitHubAsync()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : "/"
        };

        return Challenge(properties, OpenIddictClientWebIntegrationConstants.Providers.GitHub);
    }

    [HttpGet("/callback/github")]
    public async Task<IActionResult> GitHubCallbackAsync(CancellationToken cancellationToken)
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

        var redirectUri = new Uri(result.Properties?.RedirectUri ?? "/");
        
        if (result.Principal is not { Identity.IsAuthenticated: true }
            || result.Principal.GetClaim("id") is not { } githubId
            || result.Principal.GetClaim("login") is not { } username
            || result.Principal.GetClaim("avatar_url") is not { } avatarUrl)
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        Response response = await _providerLogin.GetResponse<UserRegistered, LoginSucceeded>(new ProviderLogin
        {
            Provider = Constants.LoginTypes.GitHub,
            AccountId = githubId,
            Username = username,
            AvatarUrl = avatarUrl
        }, cancellationToken);

        return response switch
        {
            (_, UserRegistered message) => SignInWithGitHub(message.UserId, message.Username, avatarUrl, redirectUri),
            (_, LoginSucceeded message) => SignInWithGitHub(message.UserId, message.Username, avatarUrl, redirectUri),
            _ => throw new InvalidOperationException()
        };
    }

    [HttpGet("/callback/github/link")]
    public async Task<IActionResult> LinkGitHubAsync(CancellationToken cancellationToken)
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

        var githubResult = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

        if (!Uri.TryCreate(githubResult.Properties?.RedirectUri, UriKind.Absolute, out var redirectUri))
        {
            throw new InvalidOperationException("Redirect URI is not valid.");
        }
        
        if (githubResult.Principal is not { Identity.IsAuthenticated: true }
            || githubResult.Principal.GetClaim("id") is not { } githubId
            || githubResult.Principal.GetClaim("login") is not { } username
            || githubResult.Principal.GetClaim("avatar_url") is not { } avatarUrl)
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        Response response = await _linkProvider.GetResponse<ProviderLinked, AccountAlreadyRegistered>(new LinkProvider
        {
            UserId = userId,
            Provider = Constants.LoginTypes.GitHub,
            AccountId = githubId,
            Username = username,
            AvatarUrl = avatarUrl
        }, cancellationToken);

        return response switch
        {
            (_, ProviderLinked message) => SignInWithGitHub(
                userId: message.UserId,
                username: cookieResult.Principal.GetClaim(ClaimTypes.Name) ?? message.Username,
                avatarUrl: null,
                redirectUri),
            (_, AccountAlreadyRegistered) => Fail(
                error: "link_github_error",
                errorDescription: "This GitHub account is already in use by another user.",
                redirectUri),
            _ => throw new InvalidOperationException()
        };
    }

    private SignInResult SignInWithGitHub(Guid userId, string username, string? avatarUrl, Uri redirectUri)
    {
        var identity = new ClaimsIdentity(authenticationType: "GitHub");

        identity.SetClaim(ClaimTypes.NameIdentifier, userId.ToString());
        identity.SetClaim(ClaimTypes.Name, username);
        identity.SetClaim(OpenIddictConstants.Claims.Picture, avatarUrl);

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
        
        query = query.Add(OpenIddictConstants.Parameters.Error, error);
        query = query.Add(OpenIddictConstants.Parameters.ErrorDescription, errorDescription);

        uriBuilder.Query = query.ToUriComponent();

        return Redirect(uriBuilder.ToString());
    }
}