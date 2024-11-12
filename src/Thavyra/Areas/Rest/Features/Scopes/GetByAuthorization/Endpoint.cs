using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Authorization;
using Thavyra.Contracts.Scope;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scopes.GetByAuthorization;

public class Endpoint : Endpoint<Request, List<ScopeResponse>>
{
    private readonly IRequestClient<Authorization_GetById> _getAuthorization;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Scope_GetByNames> _getScopes;

    public Endpoint(
        IRequestClient<Authorization_GetById> getAuthorization, 
        IAuthorizationService authorizationService,
        IRequestClient<Scope_GetByNames> getScopes)
    {
        _getAuthorization = getAuthorization;
        _authorizationService = authorizationService;
        _getScopes = getScopes;
    }

    public override void Configure()
    {
        Get("/authorizations/{AuthorizationId}/scopes");
        
        Summary(x =>
        {
            x.Summary = "Get Authorization Scopes";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var authorizationResponse = await _getAuthorization.GetResponse<Authorization, NotFound>(
            new Authorization_GetById
            {
                Id = req.AuthorizationId
            }, ct);

        if (!authorizationResponse.Is(out Response<Authorization>? r) || r.Message is not { } authorization)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, authorization,
                Security.Policies.Operation.Authorization.Read);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var scopeResponse = await _getScopes.GetResponse<Multiple<Scope>>(new Scope_GetByNames
        {
            Names = authorization.Scopes.ToList()
        }, ct);

        await SendAsync(scopeResponse.Message.Items.Select(x => new ScopeResponse
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Description = x.Description
        }).ToList(), cancellation: ct);
    }
}