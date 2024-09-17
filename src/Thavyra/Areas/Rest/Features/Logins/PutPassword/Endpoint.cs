using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Login;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Logins.PutPassword;

public class Endpoint : Endpoint<Request>
{
    private readonly IRequestClient<PasswordLogin_GetByUser> _getPassword;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<PasswordLogin_Update> _updatePassword;

    public Endpoint(
        IRequestClient<PasswordLogin_GetByUser> getPassword, 
        IAuthorizationService authorizationService,
        IRequestClient<PasswordLogin_Update> updatePassword)
    {
        _getPassword = getPassword;
        _authorizationService = authorizationService;
        _updatePassword = updatePassword;
    }

    public override void Configure()
    {
        Put("/users/{User}/logins/@password");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var state = ProcessorState<RequestState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var readResult =
            await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.Login.Read);

        if (readResult.Failed())
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var passwordResponse = await _getPassword.GetResponse<PasswordLogin, NotFound>(new PasswordLogin_GetByUser
        {
            UserId = user.Id
        }, ct);

        if (passwordResponse.Is(out Response<NotFound> _))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (passwordResponse.Is(out Response<PasswordLogin>? login))
        {
            var updateResult =
                await _authorizationService.AuthorizeAsync(User, login, Security.Policies.Operation.Login.Password);

            if (updateResult.Failed())
            {
                await SendForbiddenAsync(ct);
                return;
            }
        }
        else
        {
            return;
        }

        var updateResponse = await _updatePassword.GetResponse<PasswordLogin>(new PasswordLogin_Update
        {
            Id = user.Id,
            Password = req.Password
        }, ct);

        await SendAsync(new PasswordLoginResponse
        {
            ChangedAt = updateResponse.Message.ChangedAt,
            CreatedAt = updateResponse.Message.CreatedAt
        }, cancellation: ct);
    }
}