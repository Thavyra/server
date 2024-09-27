namespace Thavyra.Rest.Security.Resource.User;

public class AnyoneCanReadProfile : AuthorizationHandler<ReadUserProfileRequirement, Contracts.User.User>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.User.User resource) => Task.FromResult(state.Succeed());
}