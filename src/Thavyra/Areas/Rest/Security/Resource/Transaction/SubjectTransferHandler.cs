using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Transaction;

namespace Thavyra.Rest.Security.Resource.Transaction;

/// <summary>
/// Authorizes a transfer operation if the user will be the subject.
/// </summary>
public class SubjectTransferHandler : AuthorizationHandler<OperationAuthorizationRequirement, Transfer_Create>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Transfer_Create resource)
    {
        if (requirement.Name != Operations.Transaction.Transfer.Name)
        {
            return Task.CompletedTask;
        }

        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.SubjectId.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}