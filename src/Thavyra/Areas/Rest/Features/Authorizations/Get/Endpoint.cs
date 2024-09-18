using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Authorization;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Authorizations.Get;

public class Endpoint : Endpoint<Request, AuthorizationResponse>
{
    private readonly IRequestClient<Authorization_GetById> _getAuthorization;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IRequestClient<Authorization_GetById> getAuthorization, IAuthorizationService authorizationService)
    {
        _getAuthorization = getAuthorization;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/authorizations/{Id}");
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
            await _authorizationService.AuthorizeAsync(User, authorization,
                Security.Policies.Operation.Authorization.Read);

        if (authorizationResult.Failed())
        {
            await SendForbiddenAsync(ct);
            return;
        }

        await SendAsync(new AuthorizationResponse
        {
            Id = authorization.Id,
            ApplicationId = authorization.ApplicationId ?? default(JsonOptional<Guid>),
            UserId = authorization.UserId ?? default(JsonOptional<Guid>),
            Type = authorization.Type,
            Status = authorization.Status,
            CreatedAt = authorization.CreatedAt
        }, cancellation: ct);
    }
}