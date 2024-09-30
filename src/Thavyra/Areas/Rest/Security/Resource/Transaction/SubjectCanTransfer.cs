using Thavyra.Contracts.Transaction;

namespace Thavyra.Rest.Security.Resource.Transaction;

/// <summary>
/// Authorizes a transfer operation if the user will be the subject.
/// </summary>
public class SubjectCanTransfer : AuthorizationHandler<SendTransferRequirement, Transfer_Create>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Transfer_Create resource) => Task.FromResult(state

        .AllowSubject(resource.SubjectId)
        .RequireScope(Constants.Scopes.Transactions.All));
}