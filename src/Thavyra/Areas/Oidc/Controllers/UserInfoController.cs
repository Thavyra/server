using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Thavyra.Contracts;
using Thavyra.Contracts.User;

namespace Thavyra.Oidc.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
[Route("/accounts/@me")]
public class UserInfoController : Controller
{
    private readonly IRequestClient<User_GetById> _getUser;

    public UserInfoController(IRequestClient<User_GetById> getUser)
    {
        _getUser = getUser;
    }

    [HttpGet, HttpPost]
    public async Task<IActionResult> IndexAsync(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.GetClaim(OpenIddictConstants.Claims.Subject), out var userId))
        {
            throw new InvalidOperationException();
        }

        Response response = await _getUser.GetResponse<User, NotFound>(new User_GetById
        {
            Id = userId
        }, cancellationToken);

        return response switch
        {
            (_, User user) => Claims(user),

            (_, Contracts.NotFound) => Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User does not exist."
                })),

            _ => throw new InvalidOperationException()
        };
    }

    private IActionResult Claims(User user)
    {
        var host = new UriBuilder(
            HttpContext.Request.Scheme,
            HttpContext.Request.Host.Host,
            HttpContext.Request.Host.Port ?? 80
        );
        
        var claims = new Dictionary<string, object?>
        {
            [OpenIddictConstants.Claims.Subject] = user.Id.ToString(),
            [OpenIddictConstants.Claims.Issuer] = User.GetClaim(OpenIddictConstants.Claims.Issuer),
            [OpenIddictConstants.Claims.Audience] = User.GetClaim(OpenIddictConstants.Claims.ClientId),

            [OpenIddictConstants.Claims.Name] = user.Username,
            [OpenIddictConstants.Claims.Email] = $"{user.Username.ToLower()}@users.{host}"
        };

        if (User.GetScopes().Any(x => x.StartsWith(Constants.Scopes.Account.All)))
        {
            claims[OpenIddictConstants.Claims.Nickname] = user.Username;
            claims[OpenIddictConstants.Claims.PreferredUsername] = user.Username;
            claims[OpenIddictConstants.Claims.Profile] = host + $"@{user.Username}";
            claims[OpenIddictConstants.Claims.Picture] = host + $"api/users/{user.Id}/avatar.png";
        }

        return Ok(claims);
    }
}