using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Client.WebIntegration;
using Thavyra.Oidc.Managers;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Controllers;

public class DiscordController : Controller
{
    private readonly IUserManager _userManager;

    public DiscordController(IUserManager userManager)
    {
        _userManager = userManager;
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
        
        if (result.Principal is not { Identity.IsAuthenticated: true } || result.Principal.GetClaim("user") is not { } claim)
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        var login = JsonSerializer.Deserialize<DiscordLoginModel>(claim);

        if (login is null)
        {
            throw new InvalidOperationException("Could not parse Discord login data.");
        }
        
        var user = await _userManager.RegisterWithDiscordAsync(login, cancellationToken);
        
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())], 
            authenticationType: "Discord");

        var properties = new AuthenticationProperties
        {
            RedirectUri = result.Properties?.RedirectUri ?? "/"
        };
        
        return SignIn(new ClaimsPrincipal(identity), properties, CookieAuthenticationDefaults.AuthenticationScheme);
    }
    
}