using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Permission;

namespace Thavyra.Rest.Security.Resource.User;

/// <summary>
/// Used to grant initial admin during system setup.
/// </summary>
public class SetupCanManageRoles : AuthorizationHandler<ManageUserRolesRequirement, Contracts.User.User>
{
    private readonly IRequestClient<Permission_GetByApplication> _getPermissions;

    public SetupCanManageRoles(IRequestClient<Permission_GetByApplication> getPermissions)
    {
        _getPermissions = getPermissions;
    }
    
    protected override async Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state, Contracts.User.User resource)
    {
        var response = await _getPermissions.GetResponse<Multiple<Contracts.Permission.Permission>>(
            new Permission_GetByApplication
            {
                ApplicationId = state.Subject
            });

        if (response.Message.Items.Any(x => x.Name == Constants.Permissions.Setup))
        {
            return state.Succeed();
        }

        return state;
    }
}