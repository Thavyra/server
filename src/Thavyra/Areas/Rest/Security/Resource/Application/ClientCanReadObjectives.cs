namespace Thavyra.Rest.Security.Resource.Application;

public class ClientCanReadObjectives : AuthorizationHandler<ReadApplicationObjectivesRequirement, Contracts.Application.Application>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Application.Application resource) => Task.FromResult(state
            
        .AllowClient(resource.Id));
}