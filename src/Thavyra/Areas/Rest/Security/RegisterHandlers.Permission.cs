using Thavyra.Rest.Security.Resource.Permission;

namespace Thavyra.Rest.Security;

public static partial class RegisterHandlers
{
    public static IServiceCollection AddPermissionHandlers(this IServiceCollection services) => services
        .AddAuthorizationHandler<AnyoneCanManageBasicPermissions>()
        .AddAuthorizationHandler<AdminCanManagePrivilegedPermissions>();
}