using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Patch;

public class Endpoint : Endpoint<Request, ApplicationResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Application_Update> _client;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Application_Update> client)
    {
        _authorizationService = authorizationService;
        _client = client;
    }

    public override void Configure()
    {
        Patch("/applications/{Application}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.Application is not { } application)
        {
            throw new InvalidOperationException("Could not retrieve application.");
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User,
            application, Security.Policies.Operation.Application.Update);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var updateRequest = new Application_Update
        {
            Id = application.Id,
        };

        if (req.Name.HasValue)
        {
            updateRequest = updateRequest with { Name = req.Name.Value };
        }

        if (req.Description.HasValue)
        {
            updateRequest = updateRequest with { Description = req.Description.Value };
        }

        var response = await _client.GetResponse<Application>(updateRequest, ct);

        await SendAsync(new ApplicationResponse
        {
            Id = application.Id.ToString(),
            OwnerId = application.OwnerId.ToString(),

            Name = response.Message.Name,
            Description = response.Message.Description,

            CreatedAt = application.CreatedAt
        }, cancellation: ct);
    }
}