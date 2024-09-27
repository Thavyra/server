using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Objectives.Delete;

public class Endpoint : Endpoint<Request>
{
    private readonly IRequestClient<Objective_GetById> _getClient;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Objective_Delete> _deleteClient;

    public Endpoint(IRequestClient<Objective_GetById> getClient, IAuthorizationService authorizationService, IRequestClient<Objective_Delete> deleteClient)
    {
        _getClient = getClient;
        _authorizationService = authorizationService;
        _deleteClient = deleteClient;
    }

    public override void Configure()
    {
        Delete("/objectives/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var getResponse = await _getClient.GetResponse<Objective, NotFound>(new Objective_GetById
        {
            Id = req.Id
        }, ct);

        if (getResponse.Is(out Response<NotFound> _))
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        if (!getResponse.Is(out Response<Objective>? r) || r is not { Message: { } objective })
        {
            throw new InvalidOperationException("Could not retrieve objective.");
        }
        
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, objective, Security.Policies.Operation.Objective.Delete);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var deleteResponse = await _deleteClient.GetResponse<Success>(new Objective_Delete
        {
            Id = objective.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}