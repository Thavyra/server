using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security.Resource;
using Thavyra.Rest.Security.Scopes;

namespace Thavyra.Rest.Security;

public static class RegisterPolicies
{
    public static AuthorizationBuilder AddOperationPolicies(this AuthorizationBuilder builder)
    {
        return builder

            // Users
            
            .AddPolicy(Policies.Operation.User.Read, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.ReadProfile)
                .AddRequirements(Operations.Read)
                .Build())
            .AddPolicy(Policies.Operation.User.Update, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.Profile)
                .AddRequirements(Operations.Update)
                .Build())
            .AddPolicy(Policies.Operation.User.Username, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.Profile)
                .AddRequirements(Operations.User.Username)
                .Build())
            .AddPolicy(Policies.Operation.User.Delete, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.Delete)
                .AddRequirements(Operations.Delete)
                .Build())
            
            // Logins
            
            .AddPolicy(Policies.Operation.Login.Read, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.Logins)
                .AddRequirements(Operations.Read)
                .Build())
            .AddPolicy(Policies.Operation.Login.Password, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.Logins)
                .AddRequirements(Operations.Login.Password)
                .Build())

            // Applications
            
            .AddPolicy(Policies.Operation.Application.Create, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Applications.Group)
                .AddRequirements(Operations.Create)
                .Build())
            .AddPolicy(Policies.Operation.Application.Read, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Applications.Read)
                .AddRequirements(Operations.Read)
                .Build())
            .AddPolicy(Policies.Operation.Application.Update, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Applications.Group)
                .AddRequirements(Operations.Update)
                .Build())
            .AddPolicy(Policies.Operation.Application.ClientSecret, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Applications.Group)
                .AddRequirements(Operations.Application.ClientSecret)
                .Build())
            .AddPolicy(Policies.Operation.Application.Delete, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Applications.Group)
                .AddRequirements(Operations.Delete)
                .Build())
            
            // Authorizations
            
            .AddPolicy(Policies.Operation.Authorization.Read, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Authorizations.Group)
                .AddRequirements(Operations.Read)
                .Build())
            .AddPolicy(Policies.Operation.Authorization.Delete, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Authorizations.Group)
                .AddRequirements(Operations.Delete)
                .Build())

            // Transactions
            
            .AddPolicy(Policies.Operation.Transaction.Send, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Transactions.Group)
                .AddRequirements(Operations.Transaction.Send)
                .Build())
            .AddPolicy(Policies.Operation.Transaction.Transfer, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Transactions.Group)
                .AddRequirements(Operations.Transaction.Transfer)
                .Build())
            .AddPolicy(Policies.Operation.Transaction.Read, new AuthorizationPolicyBuilder()
                .AddRequirements(Operations.Read)
                .RequireScope(ScopeNames.Account.ReadTransactions)
                .Build())

            // Objectives
            
            .AddPolicy(Policies.Operation.Objective.Create, new AuthorizationPolicyBuilder()
                .AddRequirements(Operations.Create)
                .RequireScope(ScopeNames.Applications.Group)
                .Build())
            .AddPolicy(Policies.Operation.Objective.Read, new AuthorizationPolicyBuilder()
                .AddRequirements(Operations.Read)
                .RequireScope(ScopeNames.Applications.Group)
                .Build())
            .AddPolicy(Policies.Operation.Objective.Update, new AuthorizationPolicyBuilder()
                .AddRequirements(Operations.Update)
                .RequireScope(ScopeNames.Applications.Group)
                .Build())
            .AddPolicy(Policies.Operation.Objective.Delete, new AuthorizationPolicyBuilder()
                .AddRequirements(Operations.Delete)
                .RequireScope(ScopeNames.Applications.Group)
                .Build())

            // Scores
            
            .AddPolicy(Policies.Operation.Score.Create, new AuthorizationPolicyBuilder()
                .AddRequirements(Operations.Create)
                .Build())
            .AddPolicy(Policies.Operation.Score.Read, new AuthorizationPolicyBuilder()
                .AddRequirements(Operations.Read)
                .Build())
            .AddPolicy(Policies.Operation.Score.Delete, new AuthorizationPolicyBuilder()
                .AddRequirements(Operations.Delete)
                .Build());
    }
}