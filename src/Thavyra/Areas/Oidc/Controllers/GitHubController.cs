using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Client.WebIntegration;
using Thavyra.Oidc.Managers;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Controllers;

public class GitHubController : Controller
{
    private readonly IUserManager _userManager;

    public GitHubController(IUserManager userManager)
    {
        _userManager = userManager;
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

        if (result.Principal is not { Identity.IsAuthenticated: true } 
            || result.Principal.GetClaim("id") is not { } githubId
            || result.Principal.GetClaim("login") is not { } username)
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        var login = new GitHubLoginModel
        {
            Id = githubId,
            Username = username
        };

        var user = await _userManager.RegisterWithGitHubAsync(login, cancellationToken);

        var identity =
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())], "GitHub");

        var properties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(6),
            IsPersistent = true,
            RedirectUri = result.Properties?.RedirectUri ?? "/"
        };
        
        return SignIn(new ClaimsPrincipal(identity), properties, CookieAuthenticationDefaults.AuthenticationScheme);
    }
}