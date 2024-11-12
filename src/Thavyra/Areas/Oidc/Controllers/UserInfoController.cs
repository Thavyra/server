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
            (_, User user) => Ok(new Dictionary<string, object?>
                {
                    [OpenIddictConstants.Claims.Subject] = user.Id.ToString(),
                    [OpenIddictConstants.Claims.Username] = user.Username ?? User.GetClaim(OpenIddictConstants.Claims.Username)
                }),
            
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
}