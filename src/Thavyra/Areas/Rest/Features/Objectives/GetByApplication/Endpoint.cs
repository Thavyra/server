using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Features.Applications;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Objectives.GetByApplication;

public class Endpoint : Endpoint<ApplicationRequest, List<ObjectiveResponse>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Objective_GetByApplication> _client;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Objective_GetByApplication> client)
    {
        _authorizationService = authorizationService;
        _client = client;
    }

    public override void Configure()
    {
        Get("/applications/{Application}/objectives");
    }

    public override async Task HandleAsync(ApplicationRequest req, CancellationToken ct)
    {
        var state = ProcessorState<RequestState>();

        if (state.Application is not { } application)
        {
            throw new InvalidOperationException("Could not retrieve application.");
        }

        var collectRequest = new Objective_GetByApplication
        {
            ApplicationId = application.Id
        };

        var authorizationResult = 
            await _authorizationService.AuthorizeAsync(User, collectRequest, Security.Policies.Operation.Objective.Read);

        if (authorizationResult.Failed())
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var response = await _client.GetResponse<Multiple<Objective>>(collectRequest, ct);

        await SendAsync(response.Message.Items.Select(x => new ObjectiveResponse
        {
            Id = x.Id,
            ApplicationId = x.ApplicationId,
            Name = x.Name,
            CreatedAt = x.CreatedAt
        }).ToList(), cancellation: ct);
    }
}