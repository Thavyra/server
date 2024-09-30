namespace Thavyra.Rest.Security.Resource.Transaction;

/// <summary>
/// Authorizes a read operation if the user is a subject or recipient.
/// </summary>
public class SubjectOrRecipientCanRead : AuthorizationHandler<ReadTransactionRequirement, Contracts.Transaction.Transaction>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Transaction.Transaction resource)
    {
        state.AllowSubject(resource.SubjectId);

        if (resource.RecipientId.HasValue)
        {
            state.AllowSubject(resource.RecipientId.Value);
        }

        return Task.FromResult(state.RequireScope(Constants.Scopes.Account.ReadTransactions));
    }
}