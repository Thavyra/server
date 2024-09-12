using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security.Resource.Transaction;

/// <summary>
/// Authorizes a read operation if the user is a subject or recipient.
/// </summary>
public class SubjectOrRecipientReadHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contracts.Transaction.Transaction>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Contracts.Transaction.Transaction resource)
    {
        if (requirement.Name != Operations.Read.Name)
        {
            return Task.CompletedTask;
        }

        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.SubjectId.ToString())
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (resource.RecipientId.HasValue && 
            context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.RecipientId.Value.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}