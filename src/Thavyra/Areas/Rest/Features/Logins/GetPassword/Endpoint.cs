using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Login;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Logins.GetPassword;

public class Endpoint : Endpoint<UserRequest, PasswordLoginResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<PasswordLogin_GetByUser> _client;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<PasswordLogin_GetByUser> client)
    {
        _authorizationService = authorizationService;
        _client = client;
    }

    public override void Configure()
    {
        Get("/users/{User}/logins/@password");
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.User.ReadLogins);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        Response response = await _client.GetResponse<PasswordLogin, NotFound>(new PasswordLogin_GetByUser
        {
            UserId = user.Id
        }, ct);

        switch (response)
        {
            case (_, NotFound):
                await SendNotFoundAsync(ct);
                return;
            case (_, PasswordLogin login):
                await SendAsync(new PasswordLoginResponse
                {
                    ChangedAt = login.ChangedAt,
                    CreatedAt = login.CreatedAt
                }, cancellation: ct);
                break;
        }
    }
}