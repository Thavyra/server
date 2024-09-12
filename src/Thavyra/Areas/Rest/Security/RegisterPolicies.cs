using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security.Resource;
using Thavyra.Rest.Security.Scopes;

namespace Thavyra.Rest.Security;

public static class RegisterPolicies
{
    public static AuthorizationBuilder AddOperationPolicies(this AuthorizationBuilder builder)
    {
        return builder
            
            .AddPolicy(Policies.Operation.User.Read, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.ReadProfile)
                .AddRequirements(Operations.Read)
                .Build())
            .AddPolicy(Policies.Operation.User.Update, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.EditProfile)
                .AddRequirements(Operations.Update)
                .Build())
            .AddPolicy(Policies.Operation.User.Username, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.Username)
                .AddRequirements(Operations.User.Username)
                .Build())
            .AddPolicy(Policies.Operation.User.Password, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.Logins)
                .AddRequirements(Operations.User.Password)
                .Build())
            .AddPolicy(Policies.Operation.User.Delete, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Account.Delete)
                .AddRequirements(Operations.Delete)
                .Build())
            
            
            .AddPolicy(Policies.Operation.Application.Create, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Applications.Create)
                .AddRequirements(Operations.Create)
                .Build())
            .AddPolicy(Policies.Operation.Application.Read, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Applications.Read)
                .AddRequirements(Operations.Read)
                .Build())
            .AddPolicy(Policies.Operation.Application.Update, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Applications.Edit)
                .AddRequirements(Operations.Update)
                .Build())
            .AddPolicy(Policies.Operation.Application.Delete, new AuthorizationPolicyBuilder()
                .RequireScope(ScopeNames.Applications.Delete)
                .AddRequirements(Operations.Delete)
                .Build());
    }
}