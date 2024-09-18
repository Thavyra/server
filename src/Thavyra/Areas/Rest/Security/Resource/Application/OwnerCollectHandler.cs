using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Transaction;

namespace Thavyra.Rest.Security.Resource.Application;

public class OwnerCollectHandler : AuthorizationHandler<OperationAuthorizationRequirement, Transaction_GetByUser>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Transaction_GetByUser resource)
    {
        if (requirement.Name != Operations.Read.Name)
        {
            return Task.CompletedTask;
        }
        
        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.UserId.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}