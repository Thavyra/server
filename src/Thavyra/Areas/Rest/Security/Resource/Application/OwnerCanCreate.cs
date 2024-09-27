using Thavyra.Contracts.Application;

namespace Thavyra.Rest.Security.Resource.Application;

/// <summary>
/// Authorizes a create operation if the user will be the owner.
/// </summary>
public class OwnerCanCreate : AuthorizationHandler<CreateApplicationRequirement, Application_Create>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Application_Create resource) => Task.FromResult(state

        .AllowUser(resource.OwnerId)
        .RequireScope(Constants.Scopes.Applications.All));
}