using Thavyra.Contracts.Login;

namespace Thavyra.Rest.Security.Resource.Login;

public class UserCanSetPassword : AuthorizationHandler<SetPasswordRequirement, PasswordLogin>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        PasswordLogin resource) => Task.FromResult(state

        .AllowUser(resource.UserId)
        .RequireScope(Constants.Scopes.Account.Logins));
}