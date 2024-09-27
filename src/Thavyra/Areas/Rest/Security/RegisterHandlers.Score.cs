using Thavyra.Rest.Security.Resource.Score;

namespace Thavyra.Rest.Security;

public static partial class RegisterHandlers
{
    private static IServiceCollection AddScoreHandlers(this IServiceCollection services) => services
        .AddAuthorizationHandler<ClientCanRead>()
        .AddAuthorizationHandler<PrincipalCanCreate>();
}