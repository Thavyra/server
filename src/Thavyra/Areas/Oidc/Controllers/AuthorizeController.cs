using System.Collections.Immutable;
using System.Security.Claims;
using MassTransit;
using MassTransit.Internals;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.Server.AspNetCore;
using Thavyra.Contracts.User;
using Thavyra.Oidc.Managers;
using Thavyra.Oidc.Models.Internal;
using Thavyra.Oidc.Models.View;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Thavyra.Oidc.Controllers;

[Area("Oidc")]
[Route("/accounts/connect")]
public class AuthorizeController : Controller
{
    private readonly IUserManager _userManager;
    private readonly OpenIddictApplicationManager<ApplicationModel> _applicationManager;
    private readonly OpenIddictAuthorizationManager<AuthorizationModel> _authorizationManager;
    private readonly OpenIddictScopeManager<ScopeModel> _scopeManager;
    private readonly IRequestClient<User_GetById> _getUser;

    public AuthorizeController(
        IUserManager userManager,
        OpenIddictApplicationManager<ApplicationModel> applicationManager,
        OpenIddictAuthorizationManager<AuthorizationModel> authorizationManager,
        OpenIddictScopeManager<ScopeModel> scopeManager,
        IRequestClient<User_GetById> getUser)
    {
        _userManager = userManager;
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
        _getUser = getUser;
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync(CancellationToken cancellationToken)
    {
        // Validate OpenID Connect request

        var request = HttpContext.GetOpenIddictServerRequest();

        if (request is not { ClientId: not null })
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidRequest
                }));
        }

        // Authenticate subject, handle prompts

        var sessionResult = await ValidateSessionAsync(cancellationToken);

        if (!sessionResult.Succeeded ||
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

        var user = sessionResult.User!;

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId, cancellationToken);

        if (application is null)
        {
            throw new InvalidOperationException("The client should have been validated by middleware.");
        }

        if (request.HasScope(Constants.Scopes.LinkProvider))
        {
            if (!request.HasResponseType(ResponseTypes.None))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidRequest,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            $"Invalid response type for {Constants.Scopes.LinkProvider} flow."
                    }));
            }

            if (request.GetParameter(Constants.Parameters.Provider) is not
                { Value: Constants.Providers.Discord or Constants.Providers.GitHub })
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidRequest,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Provider is invalid or not specified."
                    }));
            }
        }

        // Names of requested scopes
        var scopeNames = request.GetScopes();

        // Scopes retrieved from database - will not include openid, offline_access
        var scopes = await GetScopesAsync(scopeNames, cancellationToken);

        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            user.Id.ToString(),
            application.Id,
            scopes: [..scopes.Select(x => x.Name!)],
            cancellationToken);

        // Implicitly authorize when enabled/application already accepted

        var consentType = await _applicationManager.GetConsentTypeAsync(application, cancellationToken);

        return request.HasPrompt(Prompts.Consent) switch
        {
            false when permanentAuthorizations.Any() => await AuthorizeAsync(
                user.Id,
                application,
                scopeNames,
                authorization: permanentAuthorizations.LastOrDefault(),
                type: AuthorizationTypes.Permanent,
                cancellationToken),

            false when consentType == ConsentTypes.Implicit => await AuthorizeAsync(
                user.Id,
                application,
                scopeNames,
                authorization: null,
                type: AuthorizationTypes.AdHoc,
                cancellationToken),

            _ => View(new AuthorizeViewModel
            {
                UserId = user.Id.ToString(),
                Username = user.Username,
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
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request is not { ClientId: not null })
        {
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // Authenticate subject

        var sessionResult = await ValidateSessionAsync(cancellationToken);

        if (!sessionResult.Succeeded)
        {
            return Challenge(
                authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + Request.QueryString
                });
        }

        var user = sessionResult.User!;

        // Retrieve request details

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId, cancellationToken);

        if (application is null)
        {
            throw new InvalidOperationException("The application should have been validated by middleware.");
        }

        if (request.HasScope(Constants.Scopes.LinkProvider))
        {
            if (!request.HasResponseType(ResponseTypes.None))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidRequest,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            $"Invalid response type for {Constants.Scopes.LinkProvider} flow."
                    }));
            }

            return request.GetParameter(Constants.Parameters.Provider) switch
            {
                { Value: Constants.Providers.Discord } => Challenge(
                    authenticationSchemes: OidcConstants.AuthenticationSchemes.LinkDiscord,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = request.RedirectUri
                    }),

                { Value: Constants.Providers.GitHub } => Challenge(
                    authenticationSchemes: OidcConstants.AuthenticationSchemes.LinkGitHub,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = request.RedirectUri
                    }),

                _ => Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidRequest,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Provider is invalid or not specified."
                    }))
            };
        }

        var scopeNames = request.GetScopes();

        var scopes = await GetScopesAsync(scopeNames, cancellationToken);

        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            user.Id.ToString(),
            application.Id,
            scopes: [..scopes.Select(x => x.Name!)],
            cancellationToken);

        return await AuthorizeAsync(
            user.Id,
            application,
            scopes: [..scopes.Select(x => x.Name!)],
            authorization: permanentAuthorizations.LastOrDefault(),
            type: AuthorizationTypes.Permanent,
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

    private async Task<(bool Succeeded, User? User)> ValidateSessionAsync(CancellationToken cancellationToken)
    {
        var authenticationResult =
            await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticationResult.Succeeded)
        {
            return (false, null);
        }

        if (!Guid.TryParse(authenticationResult.Principal?.GetClaim(ClaimTypes.NameIdentifier), out var userId))
        {
            return (false, null);
        }

        Response response = await _getUser.GetResponse<User, NotFound>(new User_GetById
        {
            Id = userId
        }, cancellationToken);

        return response switch
        {
            (_, User user) => (true, user),
            _ => (false, null)
        };
    }

    private async Task<IList<AuthorizationModel>> GetPermanentAuthorizationsAsync(
        string subject,
        Guid applicationId,
        ImmutableArray<string> scopes,
        CancellationToken ct)
    {
        return await _authorizationManager.FindAsync(
            subject: subject,
            client: applicationId.ToString(),
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: scopes,
            cancellationToken: ct).ToListAsync(cancellationToken: ct);
    }

    private async Task<IList<ScopeModel>> GetScopesAsync(ImmutableArray<string> scopes,
        CancellationToken cancellationToken)
    {
        List<ScopeModel> result = [];

        await foreach (var scope in _scopeManager.FindByNamesAsync(scopes, cancellationToken))
        {
            result.Add(scope);
        }

        return result;
    }

    private async Task<IActionResult> AuthorizeAsync(
        Guid userId,
        ApplicationModel application,
        ImmutableArray<string> scopes,
        AuthorizationModel? authorization,
        string type,
        CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(userId, ct);

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Name, User.GetClaim(ClaimTypes.Name));
        identity.SetClaim(Claims.Picture, User.GetClaim(Claims.Picture));

        identity.SetClaim(Claims.Subject, userId.ToString());
        identity.SetClaim(Claims.ClientId, application.ClientId);
        identity.SetClaim(Constants.Claims.ApplicationId, application.Id.ToString());

        foreach (var role in roles)
        {
            identity.AddClaim(Claims.Role, role.Name);
        }

        identity.SetScopes(scopes);

        identity.SetDestinations(static claim => claim.Type switch
        {
            Claims.Subject => [Destinations.IdentityToken, Destinations.AccessToken],
            Claims.Name => [Destinations.IdentityToken, Destinations.AccessToken],
            Claims.Picture => [Destinations.IdentityToken],

            Claims.Role => [Destinations.IdentityToken],

            _ => [Destinations.AccessToken]
        });

        if (authorization is null || type == AuthorizationTypes.AdHoc)
        {
            authorization = await _authorizationManager.CreateAsync(
                identity: identity,
                subject: userId.ToString(),
                client: application.Id.ToString(),
                type: type,
                scopes: scopes,
                cancellationToken: ct);
        }

        identity.SetAuthorizationId(authorization.Id.ToString());

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}