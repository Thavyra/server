using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.User;
using Thavyra.Rest.Security;
using Thavyra.Rest.Security.Scopes;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Users.Patch;

public class Endpoint : Endpoint<Request, UserResponse>
{
    private readonly IUserService _userService;
    private readonly IRequestClient<User_Update> _client;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IUserService userService, IRequestClient<User_Update> client, IAuthorizationService authorizationService)
    {
        _userService = userService;
        _client = client;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Patch("/users/{UserSlug}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
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

        if (req.Username.HasValue)
        {
            var authorizationResult =
                await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.User.Username);

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
        }

        if (req.Description.HasValue)
        {
            var authorizationResult =
                await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.User.Update);
            
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

        var updateResponse = await _client.GetResponse<User>(updateRequest, ct);

        var response = await _userService.GetResponseAsync(updateResponse.Message, HttpContext, ct);
        
        await SendAsync(response, cancellation: ct);
    }
}