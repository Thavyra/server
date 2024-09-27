using Thavyra.Rest.Security.Resource.Transaction;

namespace Thavyra.Rest.Security;

public static partial class RegisterHandlers
{
    private static IServiceCollection AddTransactionHandlers(this IServiceCollection services) => services
        .AddAuthorizationHandler<SubjectCanSend>()
        .AddAuthorizationHandler<SubjectCanTransfer>()
        .AddAuthorizationHandler<SubjectOrRecipientCanRead>();
}