namespace Thavyra.Rest.Security.Resource.Application;

public class OwnerCanDelete : AuthorizationHandler<DeleteApplicationRequirement, Contracts.Application.Application>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Application.Application resource) => Task.FromResult(state

        .AllowSubject(resource.OwnerId)
        .RequireScope(Constants.Scopes.Sudo));
}