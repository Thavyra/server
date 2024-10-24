using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using Thavyra.Contracts;
using Thavyra.Contracts.Authorization;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Authorizations.Delete;

public class Endpoint : Endpoint<Request>
{
    private readonly IRequestClient<Authorization_GetById> _getAuthorization;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Authorization_Revoke> _revokeAuthorization;

    public Endpoint(
        IRequestClient<Authorization_GetById> getAuthorization, 
        IAuthorizationService authorizationService, 
        IRequestClient<Authorization_Revoke> revokeAuthorization)
    {
        _getAuthorization = getAuthorization;
        _authorizationService = authorizationService;
        _revokeAuthorization = revokeAuthorization;
    }

    public override void Configure()
    {
        Delete("/authorizations/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _getAuthorization.GetResponse<Authorization, NotFound>(new Authorization_GetById
        {
            Id = req.Id
        }, ct);

        if (!response.Is(out Response<Authorization>? r) || r.Message is not { } authorization)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User,
                authorization, Security.Policies.Operation.Authorization.Revoke);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        await _revokeAuthorization.GetResponse<Success>(new Authorization_Revoke
        {
            AuthorizationId = authorization.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}