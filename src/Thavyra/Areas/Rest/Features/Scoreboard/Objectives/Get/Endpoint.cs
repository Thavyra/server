using FastEndpoints;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Features.Scores;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scoreboard.Objectives.Get;

public class Endpoint : Endpoint<RequestWithAuthentication, List<Response>>
{
    private readonly IRequestClient<Objective_GetByApplication> _client;

    public Endpoint(IRequestClient<Objective_GetByApplication> client)
    {
        _client = client;
    }

    public override void Configure()
    {
        Get("/scoreboard/objectives");
    }

    public override async Task HandleAsync(RequestWithAuthentication req, CancellationToken ct)
    {
        var response = await _client.GetResponse<Multiple<Objective>>(new Objective_GetByApplication
        {
            ApplicationId = req.ApplicationId
        }, ct);

        await SendAsync(response.Message.Items.Select(x => new Response
        {
            Id = x.Id,
            ApplicationId = x.ApplicationId,
            Name = x.Name,
            Scores = x.Scores.Select(score => new ScoreResponseWithoutObjective
            {
                Id = score.Id,
                UserId = score.UserId,
                Score = score.Value,
                CreatedAt = score.CreatedAt
            }).ToList(),
            CreatedAt = x.CreatedAt
        }).ToList(), cancellation: ct);
    }
}