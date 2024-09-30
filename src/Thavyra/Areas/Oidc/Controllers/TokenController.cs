using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Thavyra.Contracts.Application;

namespace Thavyra.Oidc.Controllers;

[Route("/oauth/token")]
public class TokenController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IRequestClient<Application_GetByClientId> _getClient;

    public TokenController(IAuthenticationService authenticationService,
        IRequestClient<Application_GetByClientId> getClient)
    {
        _authenticationService = authenticationService;
        _getClient = getClient;
    }

    public async Task<IActionResult> IndexAsync(CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request?.IsClientCredentialsGrantType() is true)
        {
            return await ClientCredentialsAsync(request, cancellationToken);
        }

        var authenticationResult = await _authenticationService.AuthenticateAsync(HttpContext,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (!authenticationResult.Succeeded)
        {
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return SignIn(authenticationResult.Principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> ClientCredentialsAsync(OpenIddictRequest request,
        CancellationToken cancellationToken)
    {
        var application = await _getClient.GetResponse<Application>(new Application_GetByClientId
        {   
            ClientId = request.ClientId ?? throw new InvalidOperationException()
        }, cancellationToken);

        var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType,
            OpenIddictConstants.Claims.Name, OpenIddictConstants.Claims.Role);

        identity.SetClaim(OpenIddictConstants.Claims.Subject, application.Message.Id.ToString());
        identity.SetClaim(OpenIddictConstants.Claims.ClientId, application.Message.ClientId);
        identity.SetClaim(Constants.Claims.ApplicationId, application.Message.Id.ToString());

        identity.SetScopes(request.GetScopes());
        
        identity.SetDestinations(static claim => claim.Type switch
        {
            _ => [OpenIddictConstants.Destinations.AccessToken]
        });

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}