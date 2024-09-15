using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scores.Get;

public class Endpoint : Endpoint<Request, ScoreResponse>
{
    private readonly IRequestClient<Score_GetById> _client;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IRequestClient<Score_GetById> client, IAuthorizationService authorizationService)
    {
        _client = client;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/scores/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _client.GetResponse<Score, NotFound>(new Score_GetById
        {
            Id = req.Id
        }, ct);

        if (response.Is(out Response<NotFound> _))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!response.Is(out Response<Score>? r) || r.Message is not { } score)
        {
            throw new InvalidOperationException("Could not retrieve score.");
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, score, Security.Policies.Operation.Score.Read);

        if (authorizationResult.Failed())
        {
            await SendForbiddenAsync(ct);
            return;
        }

        await SendAsync(new ScoreResponse
        {
            Id = score.Id,
            ObjectiveId = score.ObjectiveId,
            UserId = score.UserId,
            Score = score.Value,
            CreatedAt = score.CreatedAt
        }, cancellation: ct);
    }
}