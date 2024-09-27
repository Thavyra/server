using Thavyra.Rest.Security.Resource.Application;

namespace Thavyra.Rest.Security;

public static partial class RegisterHandlers
{
    private static IServiceCollection AddApplicationHandlers(this IServiceCollection services) => services
        .AddAuthorizationHandler<ClientCanReadObjectives>()
        .AddAuthorizationHandler<OwnerCanCreate>()
        .AddAuthorizationHandler<OwnerCanDelete>()
        .AddAuthorizationHandler<OwnerCanRead>()
        .AddAuthorizationHandler<OwnerCanReadTransactions>()
        .AddAuthorizationHandler<OwnerCanResetClientSecret>()
        .AddAuthorizationHandler<OwnerCanUpdate>();
}