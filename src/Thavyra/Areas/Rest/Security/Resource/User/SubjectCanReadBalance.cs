using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security.Resource.User;

/// <summary>
/// Authorizes any operation on a user if they are the current subject.
/// </summary>
public class SubjectCanReadBalance : AuthorizationHandler<ReadUserBalanceRequirement, Contracts.User.User>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.User.User resource) => Task.FromResult(state

        .AllowSubject(resource.Id)
        .RequireScope(Constants.Scopes.Transactions.All));
}