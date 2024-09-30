namespace Thavyra.Rest.Security.Resource.Permission;

/// <summary>
/// Used with client credentials flow for system setup.
/// </summary>
public class AnyoneCanDenySetupPermission : AuthorizationHandler<DenyPermissionRequirement, Contracts.Permission.Permission>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state, Contracts.Permission.Permission resource)
    {
        if (resource.Name != Constants.Permissions.Setup)
        {
            return Task.FromResult(state);
        }

        return Task.FromResult(state.Succeed());
    }
}