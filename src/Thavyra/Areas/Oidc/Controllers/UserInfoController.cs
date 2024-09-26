using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Thavyra.Oidc.Managers;

namespace Thavyra.Oidc.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
[Route("/connect/userinfo")]
public class UserInfoController : Controller
{
    private readonly IUserManager _userManager;

    public UserInfoController(IUserManager userManager)
    {
        _userManager = userManager;
    }
    
    [HttpGet, HttpPost]
    public async Task<IActionResult> IndexAsync(CancellationToken cancellationToken)
    {
        if (User.GetClaim(OpenIddictConstants.Claims.Subject) is not { } subject)
        {
            throw new InvalidOperationException();
        }
        
        var user = await _userManager.FindByIdAsync(subject, cancellationToken);

        if (user is null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User does not exist."
                }));
        }

        var claims = new Dictionary<string, object>
        {
            [OpenIddictConstants.Claims.Subject] = user.Id.ToString(),
            [OpenIddictConstants.Claims.Username] = user.Username
        };

        return Ok(claims);
    }
}