using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scores.Post;

public class Endpoint : Endpoint<Request, ScoreResponse>
{
    private readonly IRequestClient<Objective_GetById> _objectiveClient;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Score_Create> _createClient;

    public Endpoint(
        IRequestClient<Objective_GetById> objectiveClient, 
        IAuthorizationService authorizationService, 
        IRequestClient<Score_Create> createClient)
    {
        _objectiveClient = objectiveClient;
        _authorizationService = authorizationService;
        _createClient = createClient;
    }

    public override void Configure()
    {
        Post("/scores", "/scoreboard/objectives/{ObjectiveId}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var objectiveResponse = await _objectiveClient.GetResponse<Objective, NotFound>(new Objective_GetById
        {
            Id = req.ObjectiveId
        }, ct);

        if (objectiveResponse.Is(out Response<NotFound>? _))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var createRequest = new Score_Create
        {
            ObjectiveId = req.ObjectiveId,
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
        
        var response = await _createClient.GetResponse<Score>(createRequest, ct);

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