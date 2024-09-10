using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security.Scopes;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, string scope)
    {
        return builder.AddRequirements(new ScopeAuthorizationRequirement(scope));
    }
}