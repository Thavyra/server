using Thavyra.Contracts.Login;

namespace Thavyra.Rest.Security.Resource.Login;

public class UserCanRead : AuthorizationHandler<ReadLoginRequirement, PasswordLogin>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        PasswordLogin resource) => Task.FromResult(state

        .AllowSubject(resource.UserId)
        .RequireScope(Constants.Scopes.Account.Logins));
}