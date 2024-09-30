namespace Thavyra.Rest.Security.Resource.Authorization;

/// <summary>
/// Authorizes any operation if the subject is the authorization user.
/// </summary>
public class UserCanRead : AuthorizationHandler<ReadAuthorizationRequirement, Contracts.Authorization.Authorization>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Authorization.Authorization resource)
    {
        if (!resource.Subject.HasValue)
        {
            return Task.FromResult(state);
        }

        return Task.FromResult(state
            .AllowSubject(resource.Subject.Value)
            .RequireScope(Constants.Scopes.Authorizations.Read));
    }
}