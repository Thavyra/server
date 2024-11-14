using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.User;
using Thavyra.Rest.Security;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Users.Patch;

public class Endpoint : Endpoint<Request, UserResponse>
{
    private readonly IUserService _userService;
    private readonly IRequestClient<User_GetById> _getClient;
    private readonly IRequestClient<User_Update> _updateClient;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IUserService userService, IRequestClient<User_GetById> getClient, IRequestClient<User_Update> updateClient, IAuthorizationService authorizationService)
    {
        _userService = userService;
        _getClient = getClient;
        _updateClient = updateClient;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Patch("/users/{User}");
        Summary(x =>
        {
            x.Summary = "Modify User";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.User.UpdateProfile);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var updateRequest = new User_Update
        {
            Id = user.Id
        };

        if (req.Username.HasValue)
        {
            updateRequest = updateRequest with { Username = req.Username.Value };
        }

        if (req.Description.HasValue)
        {
            updateRequest = updateRequest with { Description = req.Description.Value };
        }

        var updateResponse = await _updateClient.GetResponse<User>(updateRequest, ct);

        var response = await _userService.GetResponseAsync(updateResponse.Message, HttpContext, ct);
        
        await SendAsync(response, cancellation: ct);
    }
}