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
        Summary(x =>
        {
            x.Summary = "Get User";
            x.RequestParam(r => r.User, "&lt;guid&gt; or '@me' or '@&lt;username&gt;'");
        });
        Description(x =>
        {
            x.Produces(404);
        });
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var response = await _userService.GetResponseAsync(user, HttpContext, ct);
        
        await SendAsync(response, cancellation: ct);
    }
}