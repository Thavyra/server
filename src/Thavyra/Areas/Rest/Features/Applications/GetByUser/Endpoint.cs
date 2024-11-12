using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Security;

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
        
        Description(x => x
            .ProducesProblemDetails(403));
        
        Summary(x =>
        {
            x.Summary = "Get User Applications";
        });
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, user,
            Security.Policies.Operation.User.ReadApplications);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var request = new Application_GetByOwner
        {
            OwnerId = user.Id,
        };
        
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