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

    public Endpoint(
        IAuthorizationService authorizationService,
        IRequestClient<User_Delete> deleteClient)
    {
        _deleteClient = deleteClient;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Delete("/users/{User}");
        Summary(x =>
        {
            x.Summary = "Delete User";
        });
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.User.Delete);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        _ = await _deleteClient.GetResponse<Success>(new User_Delete
        {
            Id = user.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}