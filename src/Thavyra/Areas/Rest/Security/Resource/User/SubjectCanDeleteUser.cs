namespace Thavyra.Rest.Security.Resource.User;

public class SubjectCanDeleteUser : AuthorizationHandler<DeleteUserRequirement, Contracts.User.User>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.User.User resource) => Task.FromResult(state

        .AllowSubject(resource.Id)
        .RequireScope(Constants.Scopes.Sudo));
}