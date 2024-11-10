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

    public Endpoint(
        IAuthorizationService authorizationService,
        IRequestClient<Application_Update> client)
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
        var application = ProcessorState<AuthenticationState>().Application;

        if (application is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User,
            application, Security.Policies.Operation.Application.Update);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var updated = await UpdateDetails(application, req, ct);

        var response = new ApplicationResponse
        {
            Id = application.Id.ToString(),
            OwnerId = application.OwnerId.ToString(),

            Name = updated.Name,
            Description = updated.Description,

            CreatedAt = application.CreatedAt
        };
        
        await SendAsync(response, cancellation: ct);
    }

    private async Task<Application> UpdateDetails(Application application, Request req, CancellationToken ct)
    {
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

        return response.Message;
    }
}