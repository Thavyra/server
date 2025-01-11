using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scoreboard.Post;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IRequestClient<Objective_GetById> _getObjectiveById;
    private readonly IRequestClient<Objective_GetByName> _getObjectiveByName;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Score_Create> _createScore;

    public Endpoint(
        IRequestClient<Objective_GetById> getObjectiveById,
        IRequestClient<Objective_GetByName> getObjectiveByName,
        IAuthorizationService authorizationService,
        IRequestClient<Score_Create> createScore)
    {
        _getObjectiveById = getObjectiveById;
        _getObjectiveByName = getObjectiveByName;
        _authorizationService = authorizationService;
        _createScore = createScore;
    }

    public override void Configure()
    {
        Post("/scoreboard");

        Summary(x => { x.Summary = "Create Score"; });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var objectiveResponse = req switch
        {
            { ObjectiveId: { HasValue: true, Value: var objectiveId } } => await _getObjectiveById
                .GetResponse<Objective, NotFound>(new Objective_GetById
                {
                    Id = objectiveId,
                }, ct),
            
            { ObjectiveName: { HasValue: true, Value: var objectiveName } } => await _getObjectiveByName
                .GetResponse<Objective, NotFound>(new Objective_GetByName
                {
                    Name = objectiveName,
                    ApplicationId = req.ApplicationId
                }, ct),
            
            _ => throw new InvalidOperationException()
        };

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

        await SendAsync(statusCode: 201, response: new Response
        {
            Id = response.Message.Id,
            ObjectiveId = response.Message.ObjectiveId,
            UserId = response.Message.UserId,
            Score = response.Message.Value,
            CreatedAt = response.Message.CreatedAt
        }, cancellation: ct);
    }
}