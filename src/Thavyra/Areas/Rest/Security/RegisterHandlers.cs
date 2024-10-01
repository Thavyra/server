using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security;

public static partial class RegisterHandlers
{
    private static IServiceCollection AddAuthorizationHandler<T>(this IServiceCollection services)
        where T : class, IAuthorizationHandler
    {
        return services.AddScoped<IAuthorizationHandler, T>();
    }

    public static IServiceCollection AddAuthorizationHandlers(this IServiceCollection services) => services
        .AddUserHandlers()
        .AddApplicationHandlers()
        .AddPermissionHandlers()
        .AddAuthorizationEntityHandlers()
        .AddLoginHandlers()
        .AddObjectiveHandlers()
        .AddScoreHandlers()
        .AddTransactionHandlers();
}