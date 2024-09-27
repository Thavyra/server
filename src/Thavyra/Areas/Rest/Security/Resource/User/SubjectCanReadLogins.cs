namespace Thavyra.Rest.Security.Resource.User;

public class SubjectCanReadLogins : AuthorizationHandler<ReadUserLoginsRequirement, Contracts.User.User>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.User.User resource) => Task.FromResult(state

        .AllowUser(resource.Id)
        .RequireScope(Constants.Scopes.Account.Logins));
}