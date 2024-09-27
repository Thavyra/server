using Thavyra.Rest.Security.Resource.Objective;

namespace Thavyra.Rest.Security;

public static partial class RegisterHandlers
{
    private static IServiceCollection AddObjectiveHandlers(this IServiceCollection services) => services
        .AddAuthorizationHandler<ClientCanRead>()
        .AddAuthorizationHandler<OwnerCanCreate>()
        .AddAuthorizationHandler<OwnerCanUpdate>();
}