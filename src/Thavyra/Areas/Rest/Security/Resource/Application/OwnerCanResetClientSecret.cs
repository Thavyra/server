namespace Thavyra.Rest.Security.Resource.Application;

public class OwnerCanResetClientSecret : AuthorizationHandler<ResetClientSecretRequirement, Contracts.Application.Application>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Application.Application resource) => Task.FromResult(state

        .AllowUser(resource.OwnerId)
        .RequireScope(Constants.Scopes.Sudo));
}