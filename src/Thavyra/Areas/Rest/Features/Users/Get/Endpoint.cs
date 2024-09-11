using FastEndpoints;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Users.Get;

public class Endpoint : Endpoint<RequestWithAuthentication, UserResponse>
{
    private readonly IUserService _userService;

    public Endpoint(IUserService userService)
    {
        _userService = userService;
    }

    public override void Configure()
    {
        Get("/users/{UserSlug}");
        AllowAnonymous();
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

        var response = await _userService.GetResponseAsync(user, HttpContext, ct);
        
        await SendAsync(response, cancellation: ct);
    }
}