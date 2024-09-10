using MassTransit;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using Thavyra.Contracts;
using Thavyra.Contracts.Scope;

namespace Thavyra.Rest.Security.Scopes;

public class ScopeAuthorizationHandler : AuthorizationHandler<ScopeAuthorizationRequirement>
{
    private readonly IRequestClient<Scope_GetByName> _client;

    public ScopeAuthorizationHandler(IRequestClient<Scope_GetByName> client)
    {
        _client = client;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeAuthorizationRequirement requirement)
    {
        var response = await _client.GetResponse<Scope, NotFound>(new Scope_GetByName
        {
            Name = requirement.Name
        });

        // The scope must exist for the requirement to succeed, regardless of if the user possesses it
        if (!response.Is(out Response<Scope>? scope))
        {
            return;
        }

        // Scopes are formed of n parts separated by '.' e.g. account.profile.read
        string[] parts = scope.Message.Name.Split('.');

        // Possessing a parent scope allows use to its children
        // e.g. account.profile automatically allows account.profile.read and account.profile.edit
        
        // Incrementally recombine the parts of the required scope until the user possesses it or authorization fails
        for (int i = 1; i < parts.Length; i++)
        {
            string combined = string.Join('.', parts.Take(i));

            if (context.User.HasScope(combined))
            {
                context.Succeed(requirement);
            }
        }
    }
}