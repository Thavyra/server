namespace Thavyra.Rest.Security.Resource.Application;

public class OwnerCanReadTransactions : AuthorizationHandler<ReadApplicationTransactionsRequirement, Contracts.Application.Application>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Application.Application resource) => Task.FromResult(state
            
        .AllowSubject(resource.OwnerId)
        .RequireScope(Constants.Scopes.Applications.Read));
}