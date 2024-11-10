using FluentValidation;
using MassTransit;
using OpenIddict.Abstractions;
using Thavyra.Contracts;
using Thavyra.Contracts.Permission;
using Thavyra.Contracts.Role;

namespace Thavyra.Rest.Security.Resource;

public class ManagePermissionRequirement : IOperationAuthorizationRequirement;
public class GrantPermissionRequirement : ManagePermissionRequirement;
public class DenyPermissionRequirement : ManagePermissionRequirement;

public class
    AdminCanManagePrivilegedPermissions : FluentAuthorizationHandler<ManagePermissionRequirement, Permission>
{
    private static readonly string[] Permissions =
    [
        OpenIddictConstants.Permissions.Endpoints.Introspection,
        OpenIddictConstants.Permissions.Endpoints.Logout,
        
        OpenIddictConstants.Permissions.ResponseTypes.IdToken,
        OpenIddictConstants.Permissions.ResponseTypes.None,
        
        OpenIddictConstants.Permissions.GrantTypes.Implicit,
        
        Constants.Permissions.ConsentTypes.Implicit,
        
        Constants.Permissions.Scopes.Sudo,
        Constants.Permissions.Scopes.Admin,
        Constants.Permissions.Scopes.LinkProvider,
        Constants.Permissions.Scopes.Account.All,
        Constants.Permissions.Scopes.Account.Logins,
        Constants.Permissions.Scopes.Authorizations.All,
        Constants.Permissions.Scopes.Authorizations.Read
    ];
    
    public AdminCanManagePrivilegedPermissions(
        IRequestClient<User_HasRole> hasRole)
    {
        Scope(Constants.Scopes.Admin);

        RuleFor(x => x.Context.User)
            .MustAsync(async (x, ct) =>
            {
                var response = await hasRole.GetResponse<Correct, Incorrect>(new User_HasRole
                {
                    UserId = x.GetSubject(),
                    RoleName = Constants.Roles.Admin
                }, ct);

                return response.Is(out Response<Correct>? _);
            })
            .Unless(x => x.Context.User.GetSubject() == x.Context.User.GetClient());
            
        RuleFor(x => x.Resource)
            .Must(x => Permissions.Contains(x.Name));
    }
}

public class
    AnyoneCanManageBasicPermissions : FluentAuthorizationHandler<ManagePermissionRequirement, Permission>
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
    
    public AnyoneCanManageBasicPermissions()
    {
        RuleFor(x => x.Resource)
            .Must(x => Permissions.Contains(x.Name));
    }
}
