using FastEndpoints;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.User;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Users.Get;

public class Endpoint : Endpoint<UserRequest, UserResponse>
{
    private readonly IRequestClient<User_GetById> _getById;
    private readonly IRequestClient<User_GetByUsername> _getByUsername;
    private readonly IUserService _userService;

    public Endpoint(
        IRequestClient<User_GetById> getById,
        IRequestClient<User_GetByUsername> getByUsername,
        IUserService userService)
    {
        _getById = getById;
        _getByUsername = getByUsername;
        _userService = userService;
    }

    public override void Configure()
    {
        Get("/users/{User}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<UserRequestState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var response = await _userService.GetResponseAsync(user, HttpContext, ct);
        
        await SendAsync(response, cancellation: ct);
    }
}