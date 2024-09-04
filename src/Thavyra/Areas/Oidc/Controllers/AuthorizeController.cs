using System.Collections.Immutable;
using System.Security.Claims;
using MassTransit.Internals;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Thavyra.Oidc.Managers;
using Thavyra.Oidc.Models.Internal;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Thavyra.Oidc.Controllers;

[Route("/oauth/authorize")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AuthorizeController : Controller
{
    private readonly IUserManager _userManager;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;

    public AuthorizeController(
        IUserManager userManager,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager)
    {
        _userManager = userManager;
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync(CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest()
                      ?? throw new InvalidOperationException("Could not retrieve OpenID Connect request.");

        if (request.HasPrompt(Prompts.Login))
        {
            return Challenge();
        }

        string subject = User.GetClaim(Claims.Subject)
                         ?? throw new InvalidOperationException("Could not retrieve subject claim.");

        var user = await _userManager.FindByIdAsync(subject, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Could not retrieve user details.");
        }

        string client = request.ClientId
                        ?? throw new InvalidOperationException("Could not retrieve client from request.");

        object? application = await _applicationManager.FindByIdAsync(client, cancellationToken);

        if (application is null)
        {
            throw new InvalidOperationException("Could not retrieve application details.");
        }

        var scopes = request.GetScopes();

        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            subject,
            client,
            scopes,
            cancellationToken);

        // Implicitly authorize when enabled/application already accepted
        
        string? consentType = await _applicationManager.GetConsentTypeAsync(application, cancellationToken);

        return request.HasPrompt(Prompts.Consent) switch
        {
            false when permanentAuthorizations.Any() => await AuthorizeAsync(
                user, 
                client, 
                scopes,
                authorization: permanentAuthorizations.LastOrDefault(), 
                cancellationToken),
            
            false when consentType == ConsentTypes.Implicit => await AuthorizeAsync(
                user, 
                client, 
                scopes,
                authorization: null, 
                cancellationToken),
            
            _ => View()
        };
    }

    [HttpPost]
    [FormValueRequired("submit.Accept"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptAsync(CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("Could not retrieve OpenID Connect request.");
        
        string subject = User.GetClaim(Claims.Subject)
                         ?? throw new InvalidOperationException("Could not retrieve subject claim.");

        var user = await _userManager.FindByIdAsync(subject, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Could not retrieve user details.");
        }

        string client = request.ClientId
                        ?? throw new InvalidOperationException("Could not retrieve client from request.");

        object? application = await _applicationManager.FindByIdAsync(client, cancellationToken);

        if (application is null)
        {
            throw new InvalidOperationException("Could not retrieve application details.");
        }

        var scopes = request.GetScopes();

        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            subject,
            client,
            scopes,
            cancellationToken);

        return await AuthorizeAsync(
            user,
            client,
            scopes,
            authorization: permanentAuthorizations.LastOrDefault(),
            cancellationToken);
    }

    [HttpPost]
    [FormValueRequired("submit.Cancel"), ValidateAntiForgeryToken]
    public IActionResult CancelAsync()
    {
        return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IList<object>> GetPermanentAuthorizationsAsync(
        string subject,
        string client,
        ImmutableArray<string> scopes,
        CancellationToken ct)
    {
        return await _authorizationManager.FindAsync(
            subject: subject,
            client: client,
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: scopes,
            cancellationToken: ct).ToListAsync(cancellationToken: ct);
    }

    private async Task<IActionResult> AuthorizeAsync(
        UserModel user,
        string client,
        ImmutableArray<string> scopes,
        object? authorization,
        CancellationToken ct)
    {
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Subject, user.Id.ToString());
        identity.SetClaim(Claims.Username, user.Username);

        identity.SetClaim(Claims.ClientId, client);

        identity.SetScopes(scopes);

        identity.SetDestinations(static claim => claim.Type switch
        {
            Claims.Subject => [Destinations.IdentityToken, Destinations.AccessToken],
            Claims.Username => [Destinations.IdentityToken],

            _ => [Destinations.AccessToken]
        });

        authorization ??= await _authorizationManager.CreateAsync(
            identity: identity,
            subject: user.Id.ToString(),
            client: client,
            type: AuthorizationTypes.Permanent,
            scopes: scopes,
            cancellationToken: ct);

        identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization, ct));

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}