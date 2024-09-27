namespace Thavyra.Rest.Security.Resource.Authorization;

public class UserCanDelete : AuthorizationHandler<DeleteAuthorizationRequirement, Contracts.Authorization.Authorization>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Authorization.Authorization resource)
    {
        if (!resource.UserId.HasValue)
        {
            return Task.FromResult(state);
        }

        return Task.FromResult(state
            .AllowUser(resource.UserId.Value)
            .RequireScope(Constants.Scopes.Authorizations.All));
    }
}