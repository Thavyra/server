using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Objectives.Patch;

public class Endpoint : Endpoint<Request, ObjectiveResponse>
{
    private readonly IRequestClient<Objective_GetById> _getClient;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Objective_Update> _updateClient;

    public Endpoint(
        IRequestClient<Objective_GetById> getClient, 
        IAuthorizationService authorizationService, 
        IRequestClient<Objective_Update> updateClient)
    {
        _getClient = getClient;
        _authorizationService = authorizationService;
        _updateClient = updateClient;
    }

    public override void Configure()
    {
        Patch("/objectives/{Id}");
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
            await _authorizationService.AuthorizeAsync(User, objective, Security.Policies.Operation.Objective.Update);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var updateRequest = new Objective_Update
        {
            Id = objective.Id
        };

        if (req.Name.HasValue)
        {
            updateRequest = updateRequest with { Name = req.Name.Value };
        }

        if (req.DisplayName.HasValue)
        {
            updateRequest = updateRequest with { DisplayName = req.DisplayName.Value };
        }
        
        var updateResponse = await _updateClient.GetResponse<Objective>(updateRequest, ct);

        await SendAsync(new ObjectiveResponse
        {
            Id = objective.Id,
            ApplicationId = objective.ApplicationId,
            
            Name = updateResponse.Message.Name,
            DisplayName = updateResponse.Message.DisplayName,
            
            CreatedAt = objective.CreatedAt
        }, cancellation: ct);
    }
}