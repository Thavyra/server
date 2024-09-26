using System.Collections.Immutable;
using System.Security.Claims;
using MassTransit.Internals;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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

        if (User.Identity?.IsAuthenticated is false || 
            request.HasPrompt(Prompts.Login) || request.HasPrompt(Prompts.SelectAccount))
        {
            // Avoid endless redirects by removing prompt parameters
            string prompt = string.Join(' ', request.GetPrompts()
                .Remove(Prompts.Login)
                .Remove(Prompts.SelectAccount));

            var parameters = Request.HasFormContentType
                ? Request.Form.Where(x => x.Key != Parameters.Prompt).ToList()
                : Request.Query.Where(x => x.Key != Parameters.Prompt).ToList();

            if (!string.IsNullOrWhiteSpace(prompt))
            {
                parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));
            }

            return Challenge(
                authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
                });
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
            applicationId,
            scopeNames,
            cancellationToken);

        // Implicitly authorize when enabled/application already accepted

        return request.HasPrompt(Prompts.Consent) switch
        {
            false when permanentAuthorizations.Any() => await AuthorizeAsync(
                user: user,
                clientId: clientId,
                applicationId: applicationId,
                scopeNames,
                authorization: permanentAuthorizations.LastOrDefault(),
                cancellationToken),

            false when application.ConsentType == ConsentTypes.Implicit => await AuthorizeAsync(
                user: user,
                clientId: clientId,
                applicationId: applicationId,
                scopeNames,
                authorization: null,
                cancellationToken),

            _ => View(new AuthorizeViewModel
            {
                Subject = user,
                Client = application,
                Scopes = scopes.AsReadOnly(),
                ReturnUrl = Request.PathBase + Request.Path + Request.QueryString
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

        string? applicationId = await _applicationManager.GetIdAsync(application, cancellationToken);

        if (applicationId is null)
        {
            throw new InvalidOperationException();
        }

        var scopes = request.GetScopes();

        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            subject,
            applicationId,
            scopes,
            cancellationToken);

        return await AuthorizeAsync(
            user: user,
            clientId: clientId,
            applicationId: applicationId,
            scopes,
            authorization: permanentAuthorizations.LastOrDefault(),
            cancellationToken);
    }

    [HttpPostWithField("submit.Cancel")]
    [ValidateAntiForgeryToken]
    public IActionResult CancelAsync()
    {
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.AccessDenied,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "The user denied the authorization request."
            }));
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
        string clientId,
        string applicationId,
        ImmutableArray<string> scopes,
        object? authorization,
        CancellationToken ct)
    {
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Username,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Username, user.Username);

        identity.SetClaim(Claims.Subject, user.Id.ToString());
        identity.SetClaim(Claims.ClientId, clientId);
        identity.SetClaim("application_id", applicationId);

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
            client: applicationId,
            type: AuthorizationTypes.Permanent,
            scopes: scopes,
            cancellationToken: ct);

        identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization, ct));

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}