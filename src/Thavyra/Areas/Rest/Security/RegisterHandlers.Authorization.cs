using Thavyra.Rest.Security.Resource.Authorization;

namespace Thavyra.Rest.Security;

public static partial class RegisterHandlers
{
    private static IServiceCollection AddAuthorizationEntityHandlers(this IServiceCollection services) => services
        .AddAuthorizationHandler<UserCanDelete>()
        .AddAuthorizationHandler<UserCanRead>();
}