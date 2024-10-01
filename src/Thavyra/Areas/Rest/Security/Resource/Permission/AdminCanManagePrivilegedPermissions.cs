using MassTransit;
using OpenIddict.Abstractions;
using Thavyra.Contracts;
using Thavyra.Contracts.Role;

namespace Thavyra.Rest.Security.Resource.Permission;

public class
    AdminCanManagePrivilegedPermissions : AuthorizationHandler<ManagePermissionRequirement,
    Contracts.Permission.Permission>
{
    private readonly IRequestClient<User_HasRole> _hasRole;

    private static readonly string[] Permissions =
    [
        OpenIddictConstants.Permissions.Endpoints.Introspection,
        OpenIddictConstants.Permissions.Endpoints.Logout,
        
        OpenIddictConstants.Permissions.ResponseTypes.IdToken,
        
        OpenIddictConstants.Permissions.GrantTypes.Implicit,
        
        Constants.Permissions.ConsentTypes.Implicit,
        
        Constants.Permissions.Scopes.Sudo,
        Constants.Permissions.Scopes.Admin,
        Constants.Permissions.Scopes.Account.All,
        Constants.Permissions.Scopes.Account.Logins,
        Constants.Permissions.Scopes.Authorizations.All,
        Constants.Permissions.Scopes.Authorizations.Read
    ];

    public AdminCanManagePrivilegedPermissions(IRequestClient<User_HasRole> hasRole)
    {
        _hasRole = hasRole;
    }

    protected override async Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Permission.Permission resource)
    {
        if (!Permissions.Contains(resource.Name))
        {
            return state;
        }

        var response = await _hasRole.GetResponse<Correct, Incorrect>(new User_HasRole
        {
            UserId = state.Subject,
            RoleName = Constants.Roles.Admin
        });

        if (!response.Is(out Response<Correct> _))
        {
            return state;
        }

        return state
            .Succeed()
            .RequireScope(Constants.Scopes.Admin);
    }
}