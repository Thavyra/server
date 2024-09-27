using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scores.Delete;

public class Endpoint : Endpoint<Request>
{
    private readonly IRequestClient<Score_GetById> _getClient;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Score_Delete> _deleteClient;

    public Endpoint(
        IRequestClient<Score_GetById> getClient, 
        IAuthorizationService authorizationService, 
        IRequestClient<Score_Delete> deleteClient)
    {
        _getClient = getClient;
        _authorizationService = authorizationService;
        _deleteClient = deleteClient;
    }

    public override void Configure()
    {
        Delete("/scores/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var getResponse = await _getClient.GetResponse<Score, NotFound>(new Score_GetById
        {
            Id = req.Id
        }, ct);

        if (getResponse.Is(out Response<NotFound> _))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!getResponse.Is(out Response<Score>? r) || r.Message is not { } score)
        {
            throw new InvalidOperationException("Could not retrieve score.");
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, score, Security.Policies.Operation.Score.Delete);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        await _deleteClient.GetResponse<Success>(new Score_Delete
        {
            Id = score.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}