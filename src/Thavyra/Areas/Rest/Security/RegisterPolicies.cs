using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security.Resource;
using Thavyra.Rest.Security.Resource.Application;
using Thavyra.Rest.Security.Resource.Authorization;
using Thavyra.Rest.Security.Resource.Login;
using Thavyra.Rest.Security.Resource.Objective;
using Thavyra.Rest.Security.Resource.Score;
using Thavyra.Rest.Security.Resource.Transaction;
using Thavyra.Rest.Security.Resource.User;

namespace Thavyra.Rest.Security;

public static class RegisterPolicies
{
    private static AuthorizationPolicyBuilder AddOperation(this AuthorizationPolicyBuilder builder, IAuthorizationRequirement requirement)
    {
        return builder.AddRequirements(requirement, new ScopeAuthorizationRequirement());
    }
    
    public static AuthorizationBuilder AddOperationPolicies(this AuthorizationBuilder builder)
    {
        return builder

            // Users

            .AddPolicy(Policies.Operation.User.ReadProfile, policy =>
                policy.AddOperation(new ReadUserProfileRequirement()))

            .AddPolicy(Policies.Operation.User.ReadBalance, policy =>
                policy.AddOperation(new ReadUserBalanceRequirement()))

            .AddPolicy(Policies.Operation.User.ReadApplications, policy =>
                policy.AddOperation(new ReadUserApplicationsRequirement()))
            
            .AddPolicy(Policies.Operation.User.ReadAuthorizations, policy =>
                policy.AddOperation(new ReadUserAuthorizationsRequirement()))

            .AddPolicy(Policies.Operation.User.ReadLogins, policy =>
                policy.AddOperation(new ReadUserLoginsRequirement()))

            .AddPolicy(Policies.Operation.User.ReadTransactions, policy =>
                policy.AddOperation(new ReadUserTransactionsRequirement()))

            .AddPolicy(Policies.Operation.User.UpdateProfile, policy =>
                policy.AddOperation(new UpdateUserProfileRequirement()))

            .AddPolicy(Policies.Operation.User.ChangeUsername, policy =>
                policy.AddOperation(new ChangeUsernameRequirement()))

            .AddPolicy(Policies.Operation.User.Delete, policy =>
                policy.AddOperation(new DeleteUserRequirement()))

            // Logins

            .AddPolicy(Policies.Operation.Login.Read, policy =>
                policy.AddOperation(new ReadLoginRequirement()))

            .AddPolicy(Policies.Operation.Login.SetPassword, policy =>
                policy.AddOperation(new SetPasswordRequirement()))

            // Applications

            .AddPolicy(Policies.Operation.Application.Create, policy =>
                policy.AddOperation(new CreateApplicationRequirement()))

            .AddPolicy(Policies.Operation.Application.Read, policy =>
                policy.AddOperation(new ReadApplicationRequirement()))

            .AddPolicy(Policies.Operation.Application.ReadObjectives, policy =>
                policy.AddOperation(new ReadApplicationObjectivesRequirement()))

            .AddPolicy(Policies.Operation.Application.ReadTransactions, policy =>
                policy.AddOperation(new ReadApplicationTransactionsRequirement()))

            .AddPolicy(Policies.Operation.Application.Update, policy =>
                policy.AddOperation(new UpdateApplicationRequirement()))

            .AddPolicy(Policies.Operation.Application.ResetClientSecret, policy =>
                policy.AddOperation(new ResetClientSecretRequirement()))

            .AddPolicy(Policies.Operation.Application.Delete, policy =>
                policy.AddOperation(new DeleteApplicationRequirement()))

            // Authorizations

            .AddPolicy(Policies.Operation.Authorization.Read, policy =>
                policy.AddOperation(new ReadAuthorizationRequirement()))

            .AddPolicy(Policies.Operation.Authorization.Delete, policy =>
                policy.AddOperation(new DeleteAuthorizationRequirement()))

            // Transactions

            .AddPolicy(Policies.Operation.Transaction.Read, policy =>
                policy.AddOperation(new ReadTransactionRequirement()))

            .AddPolicy(Policies.Operation.Transaction.Send, policy =>
                policy.AddOperation(new SendTransactionRequirement()))

            .AddPolicy(Policies.Operation.Transaction.Transfer, policy =>
                policy.AddOperation(new SendTransferRequirement()))

            // Objectives

            .AddPolicy(Policies.Operation.Objective.Create, policy =>
                policy.AddOperation(new CreateObjectiveRequirement()))

            .AddPolicy(Policies.Operation.Objective.Read, policy =>
                policy.AddOperation(new ReadObjectiveRequirement()))

            .AddPolicy(Policies.Operation.Objective.Update, policy =>
                policy.AddOperation(new UpdateObjectiveRequirement()))

            .AddPolicy(Policies.Operation.Objective.Delete, policy =>
                policy.AddOperation(new DeleteObjectiveRequirement()))

            // Scores

            .AddPolicy(Policies.Operation.Score.Create, policy =>
                policy.AddOperation(new CreateScoreRequirement()))

            .AddPolicy(Policies.Operation.Score.Read, policy =>
                policy.AddOperation(new ReadScoreRequirement()));
    }
}