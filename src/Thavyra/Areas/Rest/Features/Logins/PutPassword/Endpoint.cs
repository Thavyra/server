using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Data;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Logins.PutPassword;

public class Endpoint : Endpoint<Request, LoginResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<ChangePassword> _changePassword;
    private readonly IRequestClient<GetLoginById> _getLogin;

    public Endpoint(
        IAuthorizationService authorizationService,
        IRequestClient<ChangePassword> changePassword,
        IRequestClient<GetLoginById> getLogin)
    {
        _authorizationService = authorizationService;
        _changePassword = changePassword;
        _getLogin = getLogin;
    }

    public override void Configure()
    {
        Put("/users/{User}/logins/@password");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (ProcessorState<AuthenticationState>().User is not { } user)
        {
            throw new InvalidOperationException();
        }

        if (await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.Login.SetPassword) 
            is { Succeeded: false, Failure: var failure })
        {
            await this.SendAuthorizationFailureAsync(failure, ct);
            return;
        }

        Response response = await _changePassword.GetResponse<PasswordChanged, LoginFailed, LoginNotFound>(
            new ChangePassword
            {
                UserId = user.Id,
                CurrentPassword = req.CurrentPassword,
                Password = req.Password
            }, ct);

        switch (response)
        {
            case (_, LoginNotFound):
                await SendNotFoundAsync(ct);
                return;
            
            case (_, LoginFailed):
                AddError(x => x.CurrentPassword, 
                    req.CurrentPassword is null ? "Current password is required." : "Current password is incorrect.");

                await SendErrorsAsync(cancellation: ct);
                
                return;
            
            case (_, PasswordChanged success):
                var login = await _getLogin.GetResponse<LoginResult>(new GetLoginById
                {
                    LoginId = success.LoginId
                }, ct);

                await SendAsync(new LoginResponse
                {
                    Id = success.LoginId.ToString(),
                    Type = Constants.LoginTypes.Password,

                    ChangedAt = success.Timestamp,
                    UsedAt = login.Message.UsedAt
                }, cancellation: ct);
                
                break;
        }
    }
}