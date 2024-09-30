using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security.Resource.Permission;

public class
    AnyoneCanManageBasicPermissions : AuthorizationHandler<ManagePermissionRequirement, Contracts.Permission.Permission>
{
    private static readonly string[] Permissions =
    [
        OpenIddictConstants.Permissions.Endpoints.Authorization,
        OpenIddictConstants.Permissions.Endpoints.Token,

        OpenIddictConstants.Permissions.ResponseTypes.Code,

        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
            
        Constants.Permissions.Scopes.Account.Profile,
        Constants.Permissions.Scopes.Account.ReadProfile,
        Constants.Permissions.Scopes.Account.ReadTransactions,
            
        Constants.Permissions.Scopes.Applications.All,
        Constants.Permissions.Scopes.Applications.Read,
            
        Constants.Permissions.Scopes.Transactions.All
    ];
    
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Permission.Permission resource)
    {
        if (Permissions.Contains(resource.Name))
        {
            state.Succeed();
        }

        return Task.FromResult(state);
    }
}