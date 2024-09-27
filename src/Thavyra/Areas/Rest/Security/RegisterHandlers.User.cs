using Thavyra.Rest.Security.Resource.User;

namespace Thavyra.Rest.Security;

public static partial class RegisterHandlers
{
    private static IServiceCollection AddUserHandlers(this IServiceCollection services) => services
        .AddAuthorizationHandler<AnyoneCanReadProfile>()
        .AddAuthorizationHandler<SubjectCanChangeUsername>()
        .AddAuthorizationHandler<SubjectCanDeleteUser>()
        .AddAuthorizationHandler<SubjectCanReadApplications>()
        .AddAuthorizationHandler<SubjectCanReadAuthorizations>()
        .AddAuthorizationHandler<SubjectCanReadBalance>()
        .AddAuthorizationHandler<SubjectCanReadLogins>()
        .AddAuthorizationHandler<SubjectCanReadTransactions>()
        .AddAuthorizationHandler<SubjectCanUpdateProfile>();
}