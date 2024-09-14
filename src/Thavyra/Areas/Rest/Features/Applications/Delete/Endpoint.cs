using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Delete;

public class Endpoint : Endpoint<ApplicationRequest>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Application_Delete> _client;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Application_Delete> client)
    {
        _authorizationService = authorizationService;
        _client = client;
    }

    public override void Configure()
    {
        Delete("/applications/{Application}");
    }

    public override async Task HandleAsync(ApplicationRequest req, CancellationToken ct)
    {
        var state = ProcessorState<RequestState>();

        if (state.Application is not { } application)
        {
            throw new InvalidOperationException("Could not retrieve application.");
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, application,
                Security.Policies.Operation.Application.Delete);
        
        if (authorizationResult.Failure?.FailureReasons is { } reasons)
            foreach (var reason in reasons)
            {
                AddError(reason.Message);
            }

        if (authorizationResult.Failed())
        {
            await SendErrorsAsync(StatusCodes.Status403Forbidden, ct);
            return;
        }

        var response = await _client.GetResponse<Success>(new Application_Delete
        {
            Id = application.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}