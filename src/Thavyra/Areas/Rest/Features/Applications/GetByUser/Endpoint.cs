using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Security;
using Thavyra.Rest.Security.Resource;

namespace Thavyra.Rest.Features.Applications.GetByUser;

public class Endpoint : Endpoint<UserRequest, List<ApplicationResponse>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Application_GetByOwner> _client;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Application_GetByOwner> client)
    {
        _authorizationService = authorizationService;
        _client = client;
    }

    public override void Configure()
    {
        Get("/users/{User}/applications");
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<RequestState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var request = new Application_GetByOwner
        {
            OwnerId = user.Id,
        };
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, request,
            Security.Policies.Operation.Application.Read);

        if (authorizationResult.Failure?.FailureReasons is {} reasons)
            foreach (var reason in reasons)
            {
                AddError(reason.Message);
            }

        if (authorizationResult.Failed())
        {
            await SendErrorsAsync(StatusCodes.Status403Forbidden, ct);
            return;
        }

        var applicationResponse = await _client.GetResponse<Multiple<Application>>(request, ct);

        await SendAsync(applicationResponse.Message.Items.Select(application =>
            new ApplicationResponse
            {
                Id = application.Id.ToString(),
                OwnerId = application.OwnerId.ToString(),

                Name = application.Name,
                Description = application.Description,

                CreatedAt = application.CreatedAt
            }).ToList(), cancellation: ct);
    }
}