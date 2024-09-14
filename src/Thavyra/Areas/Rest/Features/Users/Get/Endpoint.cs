using FastEndpoints;
using Thavyra.Contracts;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Users.Get;

public class Endpoint : Endpoint<UserRequest, UserResponse>
{
    private readonly IUserService _userService;

    public Endpoint(IUserService userService)
    {
        _userService = userService;
    }

    public override void Configure()
    {
        Get("/users/{User}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<RequestState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var response = await _userService.GetResponseAsync(user, HttpContext, ct);
        
        await SendAsync(response, cancellation: ct);
    }
}