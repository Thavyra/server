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
using Thavyra.Oidc.Models.View;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Thavyra.Oidc.Controllers;

[Area("Oidc")]
[Route("/oauth/authorize")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AuthorizeController : Controller
{
    private readonly IUserManager _userManager;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    public AuthorizeController(
        IUserManager userManager,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager)
    {
        _userManager = userManager;
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
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

        string subject = User.GetClaim(ClaimTypes.NameIdentifier)
                         ?? throw new InvalidOperationException("Could not retrieve subject claim.");

        var user = await _userManager.FindByIdAsync(subject, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Could not retrieve user details.");
        }

        string clientId = request.ClientId
                        ?? throw new InvalidOperationException("Could not retrieve client from request.");

        object? client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);

        if (client is null)
        {
            throw new InvalidOperationException("Could not retrieve application details.");
        }

        string? applicationId = await _applicationManager.GetIdAsync(client, cancellationToken);

        if (applicationId is null)
        {
            throw new InvalidOperationException();
        }

        var application = new ApplicationModel
        {
            Id = new Guid(applicationId),
            ClientId = clientId,
            ClientType = await _applicationManager.GetClientTypeAsync(client, cancellationToken),
            ConsentType = await _applicationManager.GetConsentTypeAsync(client, cancellationToken),
            ApplicationType = await _applicationManager.GetApplicationTypeAsync(client, cancellationToken),
            DisplayName = await _applicationManager.GetDisplayNameAsync(client, cancellationToken),
        };

        var scopeNames = request.GetScopes();
        
        var scopes = await GetScopesAsync(scopeNames, cancellationToken);

        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            subject,
            clientId,
            scopeNames,
            cancellationToken);

        // Implicitly authorize when enabled/application already accepted

        return request.HasPrompt(Prompts.Consent) switch
        {
            false when permanentAuthorizations.Any() => await AuthorizeAsync(
                user, 
                clientId, 
                scopeNames,
                authorization: permanentAuthorizations.LastOrDefault(), 
                cancellationToken),
            
            false when application.ConsentType == ConsentTypes.Implicit => await AuthorizeAsync(
                user, 
                clientId, 
                scopeNames,
                authorization: null, 
                cancellationToken),
            
            _ => View(new AuthorizeViewModel
            {
                Subject = user,
                Client = application,
                Scopes = scopes.AsReadOnly(),
                ReturnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString
            })
        };
    }

    [HttpPostWithField("submit.Accept")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptAsync(CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("Could not retrieve OpenID Connect request.");
        
        string subject = User.GetClaim(ClaimTypes.NameIdentifier)
                         ?? throw new InvalidOperationException("Could not retrieve subject claim.");

        var user = await _userManager.FindByIdAsync(subject, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Could not retrieve user details.");
        }

        string clientId = request.ClientId
                        ?? throw new InvalidOperationException("Could not retrieve client from request.");

        object? application = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);

        if (application is null)
        {
            throw new InvalidOperationException("Could not retrieve application details.");
        }
        
        string? client = await _applicationManager.GetIdAsync(application, cancellationToken);

        if (client is null)
        {
            throw new InvalidOperationException();
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

    [HttpPostWithField("submit.Cancel")]
    [ValidateAntiForgeryToken]
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

    private async Task<IList<ScopeModel>> GetScopesAsync(ImmutableArray<string> scopes,
        CancellationToken cancellationToken)
    {
        List<ScopeModel> result = [];

        await foreach (object scope in _scopeManager.FindByNamesAsync(scopes, cancellationToken))
        {
            string? id = await _scopeManager.GetIdAsync(scope, cancellationToken);

            if (id is null)
            {
                throw new InvalidOperationException();
            }
            
            result.Add(new ScopeModel
            {
                Id = new Guid(id),
                Name = await _scopeManager.GetNameAsync(scope, cancellationToken),
                DisplayName = await _scopeManager.GetDisplayNameAsync(scope, cancellationToken),
                Description = await _scopeManager.GetDescriptionAsync(scope, cancellationToken)
            });
        }

        return result;
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