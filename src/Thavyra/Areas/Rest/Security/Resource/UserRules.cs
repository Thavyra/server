using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Role;

namespace Thavyra.Rest.Security.Resource;

public class ReadUserProfileRequirement : IOperationAuthorizationRequirement;
public class ReadUserBalanceRequirement : IOperationAuthorizationRequirement;
public class ReadUserRolesRequirement : IOperationAuthorizationRequirement;

public class ManageUserRolesRequirement : IOperationAuthorizationRequirement;
public class ReadUserApplicationsRequirement : IOperationAuthorizationRequirement;
public class ReadUserAuthorizationsRequirement : IOperationAuthorizationRequirement;
public class ReadUserLoginsRequirement : IOperationAuthorizationRequirement;
public class ReadUserTransactionsRequirement : IOperationAuthorizationRequirement;
public class UpdateUserProfileRequirement : IOperationAuthorizationRequirement;
public class ChangeUsernameRequirement : IOperationAuthorizationRequirement;
public class DeleteUserRequirement : IOperationAuthorizationRequirement;


public class AnyoneCanReadProfile : FluentAuthorizationHandler<ReadUserProfileRequirement, Contracts.User.User>;

public class AdminCanManageUserRoles : FluentAuthorizationHandler<ManageUserRolesRequirement, Contracts.User.User>
{
    private readonly IRequestClient<User_HasRole> _hasRole;

    public AdminCanManageUserRoles(IRequestClient<User_HasRole> hasRole)
    {
        _hasRole = hasRole;
        Scope(Constants.Scopes.Admin);
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageUserRolesRequirement requirement, Contracts.User.User resource)
    {
        var response = await _hasRole.GetResponse<Correct, Incorrect>(new User_HasRole
        {
            UserId = context.User.GetSubject(),
            RoleName = Constants.Roles.Admin
        });

        if (response.Is(out Response<Correct> _))
        {
            context.Succeed(requirement);
        }
    }
}

public class
    SubjectCanReadUserApplications : FluentAuthorizationHandler<ReadUserApplicationsRequirement, Contracts.User.User>
{
    public SubjectCanReadUserApplications()
    {
        Scope(Constants.Scopes.Applications.Read);
        Subject(x => x.Id);
    }
}

public class SubjectCanReadUserBalance : FluentAuthorizationHandler<ReadUserBalanceRequirement, Contracts.User.User>
{
    public SubjectCanReadUserBalance()
    {
        Scope(Constants.Scopes.Transactions.All, Constants.Scopes.Account.ReadTransactions);
        Subject(x => x.Id);
    }
}

public class SubjectCanReadUserLogins : FluentAuthorizationHandler<ReadUserLoginsRequirement, Contracts.User.User>
{
    public SubjectCanReadUserLogins()
    {
        Scope(Constants.Scopes.Account.Logins);
        Subject(x => x.Id);
    }
}

public class
    SubjectCanReadUserAuthorizations : FluentAuthorizationHandler<ReadUserAuthorizationsRequirement, Contracts.User.User>
{
    public SubjectCanReadUserAuthorizations()
    {
        Scope(Constants.Scopes.Authorizations.Read);
        Subject(x => x.Id);
    }
}

public class SubjectCanReadUserRoles : FluentAuthorizationHandler<ReadUserRolesRequirement, Contracts.User.User>
{
    public SubjectCanReadUserRoles()
    {
        Scope(Constants.Scopes.Account.ReadProfile);
        Subject(x => x.Id);
    }
}

public class
    SubjectCanReadUserTransactions : FluentAuthorizationHandler<ReadUserTransactionsRequirement, Contracts.User.User>
{
    public SubjectCanReadUserTransactions()
    {
        Scope(Constants.Scopes.Account.ReadTransactions);
        Subject(x => x.Id);
    }
}

public class SubjectCanUpdateUserProfile : FluentAuthorizationHandler<UpdateUserProfileRequirement, Contracts.User.User>
{
    public SubjectCanUpdateUserProfile()
    {
        Scope(Constants.Scopes.Account.Profile);
        Subject(x => x.Id);
    }
}
