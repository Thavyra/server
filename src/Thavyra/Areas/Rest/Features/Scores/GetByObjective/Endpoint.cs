using FastEndpoints;
using FastEndpoints.Swagger;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scores.GetByObjective;

public class Endpoint : Endpoint<Request, List<ScoreResponse>>
{
    private readonly IRequestClient<Objective_GetById> _client;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IRequestClient<Objective_GetById> client, IAuthorizationService authorizationService)
    {
        _client = client;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/objectives/{ObjectiveId}/scores");
        
        Description(x => x.AutoTagOverride("Scoreboard"));
        
        Summary(x =>
        {
            x.Summary = "Get Objective Scores";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _client.GetResponse<Objective, NotFound>(new Objective_GetById
        {
            Id = req.ObjectiveId
        }, ct);

        if (response.Is(out Response<NotFound>? _))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!response.Is(out Response<Objective>? r) || r.Message is not { } objective)
        {
            throw new InvalidOperationException("Could not retrieve objective.");
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, objective, Security.Policies.Operation.Objective.Read);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        await SendAsync(objective.Scores.Select(x => new ScoreResponse
        {
            Id = x.Id,
            ObjectiveId = x.ObjectiveId,
            UserId = x.UserId,
            Score = x.Value,
            CreatedAt = x.CreatedAt
        }).ToList(), cancellation: ct);
    }
}