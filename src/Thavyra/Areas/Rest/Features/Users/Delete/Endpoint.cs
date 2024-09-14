using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.User;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Users.Delete;

public class Endpoint : Endpoint<UserRequest>
{
    private readonly IRequestClient<User_Delete> _deleteClient;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IRequestClient<User_Delete> deleteClient, IAuthorizationService authorizationService)
    {
        _deleteClient = deleteClient;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Delete("/users/{UserSlug}");
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<RequestState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.User.Delete);
        
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

        var response = await _deleteClient.GetResponse<Success>(new User_Delete
        {
            Id = user.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}