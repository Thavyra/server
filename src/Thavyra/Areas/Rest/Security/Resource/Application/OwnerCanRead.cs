namespace Thavyra.Rest.Security.Resource.Application;

/// <summary>
/// Authorizes any operation on an application if the user is its owner.
/// </summary>
public class OwnerCanRead : AuthorizationHandler<ReadApplicationRequirement, Contracts.Application.Application>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Application.Application resource) => Task.FromResult(state

        .AllowSubject(resource.OwnerId)
        .RequireScope(Constants.Scopes.Applications.Read));
}