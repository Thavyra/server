using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.User;
using Thavyra.Rest.Security;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Users.Delete;

public class Endpoint : Endpoint<RequestWithAuthentication>
{
    private readonly IUserService _userService;
    private readonly IRequestClient<User_Delete> _client;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IUserService userService, IRequestClient<User_Delete> client, IAuthorizationService authorizationService)
    {
        _userService = userService;
        _client = client;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Delete("/users/{UserSlug}");
    }

    public override async Task HandleAsync(RequestWithAuthentication req, CancellationToken ct)
    {
        var userResult = await _userService.GetUserFromRequestAsync(req, ct);

        switch (userResult)
        {
            case (SlugClaimMissing, _):
                await SendUnauthorizedAsync(ct);
                return;
            case (SlugInvalid, _):
                await SendErrorsAsync(cancellation: ct);
                break;
            case (SlugNotFound, _):
                await SendNotFoundAsync(ct);
                return;
        }
        
        if (userResult is not (_, { } user))
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

        var response = await _client.GetResponse<Success>(new User_Delete
        {
            Id = user.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}