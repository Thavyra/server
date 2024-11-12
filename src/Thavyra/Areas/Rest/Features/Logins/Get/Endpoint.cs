using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.Login.Data;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Logins.Get;

public class Endpoint : Endpoint<UserRequest, List<LoginResponse>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<GetUserLogins> _getLogins;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<GetUserLogins> getLogins)
    {
        _authorizationService = authorizationService;
        _getLogins = getLogins;
    }

    public override void Configure()
    {
        Get("/users/{User}/logins");
        
        Summary(x =>
        {
            x.Summary = "Get User Logins";
        });
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        if (ProcessorState<AuthenticationState>().User is not { } user)
        {
            throw new InvalidOperationException();
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.User.ReadLogins);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var response = await _getLogins.GetResponse<UserLoginResult>(new GetUserLogins
        {
            UserId = user.Id
        }, ct);

        await SendAsync(response.Message.Logins.Select(login => new LoginResponse
        {
            Id = login.Id.ToString(),
            Type = login.Type,

            ProviderUsername = login.ProviderUsername ?? default(JsonOptional<string>),
            ProviderAvatarUrl = login.ProviderAvatarUrl ?? default(JsonOptional<string>),

            UsedAt = login.UsedAt,
            ChangedAt = login.Type == Constants.LoginTypes.Password ? login.UpdatedAt : default(JsonOptional<DateTime>)
        }).ToList(), cancellation: ct);
    }
}