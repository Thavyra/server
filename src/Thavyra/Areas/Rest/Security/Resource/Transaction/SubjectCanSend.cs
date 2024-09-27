using Thavyra.Contracts.Transaction;

namespace Thavyra.Rest.Security.Resource.Transaction;

/// <summary>
/// Authorizes a create operation if the user will be the subject.
/// </summary>
public class SubjectCanSend : AuthorizationHandler<SendTransactionRequirement, Transaction_Create>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Transaction_Create resource) => Task.FromResult(state

        .AllowUser(resource.SubjectId)
        .RequireScope(Constants.Scopes.Transactions.All));
}