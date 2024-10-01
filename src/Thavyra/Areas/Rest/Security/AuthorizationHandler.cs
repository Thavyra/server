using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Scope;

namespace Thavyra.Rest.Security;

public abstract class AuthorizationHandler<TOperationRequirement, TResource> : IAuthorizationHandler where TOperationRequirement : IOperationAuthorizationRequirement
{
    private static bool HasScopes(ClaimsPrincipal user, IEnumerable<string> scopes)
    {
        var results = scopes.ToDictionary(key => key, _ => false);
        
        foreach (string name in results.Keys)
        {
            // Scopes are formed of n parts separated by '.' e.g. account.profile.read
            string[] parts = name.Split('.');

            // Possessing a parent scope allows use to its children
            // e.g. account.profile automatically allows account.profile.read and account.profile.edit
        
            // Incrementally recombine the parts of the required scope until the user possesses it or authorization fails
            for (int i = 1; i <= parts.Length; i++)
            {
                string combined = string.Join('.', parts[..i]);

                if (user.HasScope(combined))
                {
                    results[name] = true;
                    break;
                }
            }
        }

        return results.All(x => x.Value);
    }
    
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.Resource is not TResource resource)
        {
            return;
        }

        if (context.Requirements.OfType<TOperationRequirement>().FirstOrDefault() is not { } operation)
        {
            return;
        }
        
        var state = await HandleAsync(new AuthorizationHandlerState(context.User), resource);
        
        switch (context.Requirements.OfType<ScopeAuthorizationRequirement>().FirstOrDefault())
        {
            case { } scope when state.Succeeded && HasScopes(context.User, state.RequiredScopes):
                context.Succeed(operation);
                context.Succeed(scope);
                break;
            
            case null when state.Succeeded:
                context.Succeed(operation);
                break;
        }
    }

    protected abstract Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state, TResource resource);
}

public class AuthorizationHandlerState
{
    

    public AuthorizationHandlerState(ClaimsPrincipal user)
    {
        User = user;
    }

    public bool Succeeded { get; private set; }
    public List<string> RequiredScopes { get; } = [];
    public ClaimsPrincipal User { get; }
    public Guid Subject => new(User.GetClaim(OpenIddictConstants.Claims.Subject) ?? throw new InvalidOperationException());

    public AuthorizationHandlerState Succeed()
    {
        Succeeded = true;

        return this;
    }
    
    public AuthorizationHandlerState AllowSubject(Guid subject)
    {
        if (User.GetClaim(OpenIddictConstants.Claims.Subject) == subject.ToString())
        {
            Succeed();
        }
        
        return this;
    }

    public AuthorizationHandlerState AllowClient(Guid applicationId)
    {
        if (User.GetClaim(Constants.Claims.ApplicationId) == applicationId.ToString())
        {
            Succeed();
        }

        return this;
    }

    public AuthorizationHandlerState AllowPrincipal(Guid subject, Guid applicationId)
    {
        if (User.GetClaim(OpenIddictConstants.Claims.Subject) == subject.ToString()
            && User.GetClaim(Constants.Claims.ApplicationId) == applicationId.ToString())
        {
            Succeed();
        }

        return this;
    }

    public AuthorizationHandlerState RequireScope(params string[] names)
    {
        RequiredScopes.AddRange(names);

        return this;
    }
}