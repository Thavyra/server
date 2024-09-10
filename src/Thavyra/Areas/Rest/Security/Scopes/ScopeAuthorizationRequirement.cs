using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security.Scopes;

public class ScopeAuthorizationRequirement : IAuthorizationRequirement
{
    public string Name { get; }

    public ScopeAuthorizationRequirement(string name)
    {
        Name = name;
    }
}