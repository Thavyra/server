using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Features.Scores;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scoreboard.Objectives.Post;

public class Endpoint : Endpoint<Request, ScoreResponse>
{
    private readonly IRequestClient<Objective_GetByName> _getObjective;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Score_Create> _createScore;

    public Endpoint(
        IRequestClient<Objective_GetByName> getObjective, 
        IAuthorizationService authorizationService,
        IRequestClient<Score_Create> createScore)
    {
        _getObjective = getObjective;
        _authorizationService = authorizationService;
        _createScore = createScore;
    }

    public override void Configure()
    {
        Post("/scoreboard/objectives/@{Name}");
        
        Summary(x =>
        {
            x.Summary = "Create Score";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var objectiveResponse = await _getObjective.GetResponse<Objective, NotFound>(new Objective_GetByName
        {
            Name = req.Name,
            ApplicationId = req.ApplicationId
        }, ct);

        if (!objectiveResponse.Is(out Response<Objective>? objective))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var createRequest = new Score_Create
        {
            ObjectiveId = objective.Message.Id,
            UserId = req.Subject,
            Value = req.Score
        };

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, createRequest, Security.Policies.Operation.Score.Create);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }
        
        var response = await _createScore.GetResponse<Score>(createRequest, ct);

        await SendAsync(new ScoreResponse
        {
            Id = response.Message.Id,
            ObjectiveId = response.Message.ObjectiveId,
            UserId = response.Message.UserId,
            Score = response.Message.Value,
            CreatedAt = response.Message.CreatedAt
        }, cancellation: ct);
    }
}