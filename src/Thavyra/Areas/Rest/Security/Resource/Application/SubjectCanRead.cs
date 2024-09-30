namespace Thavyra.Rest.Security.Resource.Application;

public class SubjectCanRead : AuthorizationHandler<ReadApplicationRequirement, Contracts.Application.Application>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state, Contracts.Application.Application resource)
    {
        return Task.FromResult(state
            .AllowSubject(resource.Id)
            .RequireScope(Constants.Scopes.Applications.Read));
    }
}