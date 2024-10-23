namespace Thavyra.Rest.Security.Resource.Application;

public class OwnerCanReadObjectives : AuthorizationHandler<ReadApplicationObjectivesRequirement, Contracts.Application.Application>
{
    protected override async Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state, Contracts.Application.Application resource)
    {
        return state
            .AllowSubject(resource.OwnerId)
            .RequireScope(Constants.Scopes.Applications.Read);
    }
}