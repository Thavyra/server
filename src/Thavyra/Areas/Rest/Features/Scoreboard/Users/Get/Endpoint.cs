using FastEndpoints;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Features.Scores;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scoreboard.Users.Get;

public class Endpoint : Endpoint<RequestWithAuthentication, List<ScoreResponse>>
{
    private readonly IRequestClient<Score_GetByUser> _client;

    public Endpoint(IRequestClient<Score_GetByUser> client)
    {
        _client = client;
    }

    public override void Configure()
    {
        Get("/scoreboard/users");
        
        Summary(x =>
        {
            x.Summary = "Get Scoreboard by Users";
        });
    }

    public override async Task HandleAsync(RequestWithAuthentication req, CancellationToken ct)
    {
        var response = await _client.GetResponse<Multiple<Score>>(new Score_GetByUser
        {
            UserId = req.Subject
        }, ct);

        await SendAsync(response.Message.Items.Select(x => new ScoreResponse
        {
            Id = x.Id,

            ObjectiveId = x.ObjectiveId,
            UserId = x.UserId,
            Score = x.Value,
            CreatedAt = x.CreatedAt
        }).ToList(), cancellation: ct);
    }
}