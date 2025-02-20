using FastEndpoints;
using FastEndpoints.Swagger;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Objectives.Delete;

public class Endpoint : Endpoint<Request>
{
    private readonly IRequestClient<Objective_GetById> _getClient;
    private readonly IRequestClient<Objective_Delete> _deleteClient;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(
        IRequestClient<Objective_GetById> getClient,
        IRequestClient<Objective_Delete> deleteClient,
        IAuthorizationService authorizationService)
    {
        _getClient = getClient;
        _deleteClient = deleteClient;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Delete("/applications/{Application}/objectives/{Id}");
        
        Description(x => x.AutoTagOverride("Scoreboard"));
        
        Summary(x =>
        {
            x.Summary = "Delete Scoreboard Objective";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (ProcessorState<AuthenticationState>().Application is not { } application)
        {
            throw new InvalidOperationException();
        }
        
        var getResponse = await _getClient.GetResponse<Objective, NotFound>(new Objective_GetById
        {
            Id = req.Id
        }, ct);

        if (getResponse.Is(out Response<NotFound>? _))
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        if (!getResponse.Is(out Response<Objective>? r) || r is not { Message: { } objective })
        {
            throw new InvalidOperationException("Could not retrieve objective.");
        }

        if (objective.Id != application.Id)
        {
            await SendNotFoundAsync(ct);
            return;
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