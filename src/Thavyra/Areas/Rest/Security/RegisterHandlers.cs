using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security.Resource;

namespace Thavyra.Rest.Security;

public static class RegisterHandlers
{
    private static IServiceCollection AddAuthorizationHandler<T>(this IServiceCollection services)
        where T : class, IAuthorizationHandler
    {
        return services.AddScoped<IAuthorizationHandler, T>();
    }

    public static IServiceCollection AddAuthorizationHandlers(this IServiceCollection services) => services
        .AddAuthorizationHandler<AnyoneCanReadProfile>()
        .AddAuthorizationHandler<AdminCanManageUserRoles>()
        .AddAuthorizationHandler<SubjectCanReadUserApplications>()
        .AddAuthorizationHandler<SubjectCanReadUserBalance>()
        .AddAuthorizationHandler<SubjectCanReadUserLogins>()
        .AddAuthorizationHandler<SubjectCanReadUserAuthorizations>()
        .AddAuthorizationHandler<SubjectCanReadUserRoles>()
        .AddAuthorizationHandler<SubjectCanReadUserTransactions>()
        .AddAuthorizationHandler<SubjectCanUpdateUserProfile>()
        
        .AddAuthorizationHandler<SubjectCanSetPassword>()

        .AddAuthorizationHandler<AdminCanReadApplication>()
        .AddAuthorizationHandler<OwnerCanCreateApplication>()
        .AddAuthorizationHandler<OwnerCanReadApplication>()
        .AddAuthorizationHandler<OwnerCanReadApplicationObjectives>()
        .AddAuthorizationHandler<OwnerCanReadApplicationTransactions>()
        .AddAuthorizationHandler<OwnerCanReadClientSecret>()
        .AddAuthorizationHandler<OwnerCanUpdateApplication>()
        .AddAuthorizationHandler<OwnerCanDeleteApplication>()
        .AddAuthorizationHandler<ClientCanReadApplicationObjectives>()
        .AddAuthorizationHandler<SubjectCanReadApplication>()
        .AddAuthorizationHandler<SubjectCanReadApplicationObjectives>()
        .AddAuthorizationHandler<SubjectCanReadApplicationTransactions>()
        .AddAuthorizationHandler<SubjectCanResetClientSecret>()
        .AddAuthorizationHandler<SubjectCanUpdateApplication>()

        .AddAuthorizationHandler<AdminCanManagePrivilegedPermissions>()
        .AddAuthorizationHandler<AnyoneCanManageBasicPermissions>()

        .AddAuthorizationHandler<SubjectCanReadAuthorization>()
        .AddAuthorizationHandler<SubjectCanRevokeAuthorization>()

        .AddAuthorizationHandler<SubjectCanSendTransaction>()
        .AddAuthorizationHandler<SubjectCanSendTransfer>()
        .AddAuthorizationHandler<SubjectOrRecipientCanReadTransaction>()

        .AddAuthorizationHandler<ApplicationOwnerCanCreateObjective>()
        .AddAuthorizationHandler<ApplicationOwnerCanReadObjective>()
        .AddAuthorizationHandler<ApplicationOwnerCanUpdateObjective>()
        .AddAuthorizationHandler<ApplicationOwnerCanDeleteObjective>()
        .AddAuthorizationHandler<ClientCanReadObjective>()

        .AddAuthorizationHandler<PrincipalCanCreateScore>()
        .AddAuthorizationHandler<ClientCanReadScore>();
}