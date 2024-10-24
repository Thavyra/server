using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security.Resource;

namespace Thavyra.Rest.Security;

public static class RegisterPolicies
{
    public static AuthorizationBuilder AddOperationPolicies(this AuthorizationBuilder builder)
    {
        return builder

            // Users

            .AddPolicy(Policies.Operation.User.ReadProfile, policy =>
                policy.AddRequirements(new ReadUserProfileRequirement()))

            .AddPolicy(Policies.Operation.User.ReadBalance, policy =>
                policy.AddRequirements(new ReadUserBalanceRequirement()))

            .AddPolicy(Policies.Operation.User.ReadRoles, policy =>
                policy.AddRequirements(new ReadUserRolesRequirement()))
            
            .AddPolicy(Policies.Operation.User.ReadApplications, policy =>
                policy.AddRequirements(new ReadUserApplicationsRequirement()))
            
            .AddPolicy(Policies.Operation.User.ReadAuthorizations, policy =>
                policy.AddRequirements(new ReadUserAuthorizationsRequirement()))

            .AddPolicy(Policies.Operation.User.ReadLogins, policy =>
                policy.AddRequirements(new ReadUserLoginsRequirement()))

            .AddPolicy(Policies.Operation.User.ReadTransactions, policy =>
                policy.AddRequirements(new ReadUserTransactionsRequirement()))

            .AddPolicy(Policies.Operation.User.UpdateProfile, policy =>
                policy.AddRequirements(new UpdateUserProfileRequirement()))

            .AddPolicy(Policies.Operation.User.ChangeUsername, policy =>
                policy.AddRequirements(new ChangeUsernameRequirement()))

            .AddPolicy(Policies.Operation.User.Delete, policy =>
                policy.AddRequirements(new DeleteUserRequirement()))
            
            // Roles
            
            .AddPolicy(Policies.Operation.Role.Grant, policy =>
                policy.AddRequirements(new ManageUserRolesRequirement()))
            
            .AddPolicy(Policies.Operation.Role.Deny, policy =>
                policy.AddRequirements(new ManageUserRolesRequirement()))

            // Logins

            .AddPolicy(Policies.Operation.Login.Read, policy =>
                policy.AddRequirements(new ReadLoginRequirement()))

            .AddPolicy(Policies.Operation.Login.SetPassword, policy =>
                policy.AddRequirements(new SetPasswordRequirement()))

            // Applications

            .AddPolicy(Policies.Operation.Application.Create, policy =>
                policy.AddRequirements(new CreateApplicationRequirement()))

            .AddPolicy(Policies.Operation.Application.Read, policy =>
                policy.AddRequirements(new ReadApplicationRequirement()))

            .AddPolicy(Policies.Operation.Application.ReadObjectives, policy =>
                policy.AddRequirements(new ReadApplicationObjectivesRequirement()))

            .AddPolicy(Policies.Operation.Application.ReadTransactions, policy =>
                policy.AddRequirements(new ReadApplicationTransactionsRequirement()))

            .AddPolicy(Policies.Operation.Application.Update, policy =>
                policy.AddRequirements(new UpdateApplicationRequirement()))

            .AddPolicy(Policies.Operation.Application.ResetClientSecret, policy =>
                policy.AddRequirements(new ResetClientSecretRequirement()))

            .AddPolicy(Policies.Operation.Application.Delete, policy =>
                policy.AddRequirements(new DeleteApplicationRequirement()))
            
            // Permissions
            
            .AddPolicy(Policies.Operation.Permission.Grant, policy => 
                policy.AddRequirements(new GrantPermissionRequirement()))
            
            .AddPolicy(Policies.Operation.Permission.Deny, policy =>
                policy.AddRequirements(new DenyPermissionRequirement()))

            // Authorizations

            .AddPolicy(Policies.Operation.Authorization.Read, policy =>
                policy.AddRequirements(new ReadAuthorizationRequirement()))

            .AddPolicy(Policies.Operation.Authorization.Revoke, policy =>
                policy.AddRequirements(new RevokeAuthorizationRequirement()))

            // Transactions

            .AddPolicy(Policies.Operation.Transaction.Read, policy =>
                policy.AddRequirements(new ReadTransactionRequirement()))

            .AddPolicy(Policies.Operation.Transaction.Send, policy =>
                policy.AddRequirements(new SendTransactionRequirement()))

            .AddPolicy(Policies.Operation.Transaction.Transfer, policy =>
                policy.AddRequirements(new SendTransferRequirement()))

            // Objectives

            .AddPolicy(Policies.Operation.Objective.Create, policy =>
                policy.AddRequirements(new CreateObjectiveRequirement()))

            .AddPolicy(Policies.Operation.Objective.Read, policy =>
                policy.AddRequirements(new ReadObjectiveRequirement()))

            .AddPolicy(Policies.Operation.Objective.Update, policy =>
                policy.AddRequirements(new UpdateObjectiveRequirement()))

            .AddPolicy(Policies.Operation.Objective.Delete, policy =>
                policy.AddRequirements(new DeleteObjectiveRequirement()))

            // Scores

            .AddPolicy(Policies.Operation.Score.Create, policy =>
                policy.AddRequirements(new CreateScoreRequirement()))

            .AddPolicy(Policies.Operation.Score.Read, policy =>
                policy.AddRequirements(new ReadScoreRequirement()));
    }
}