using Thavyra.Rest.Security.Resource.Login;

namespace Thavyra.Rest.Security;

public static partial class RegisterHandlers
{
    private static IServiceCollection AddLoginHandlers(this IServiceCollection services) => services
        .AddAuthorizationHandler<UserCanRead>()
        .AddAuthorizationHandler<UserCanSetPassword>();
}