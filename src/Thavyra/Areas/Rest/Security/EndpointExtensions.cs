using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security;

public static class EndpointExtensions
{
    public static Task SendAuthorizationFailureAsync(this IEndpoint endpoint, AuthorizationFailure failure, CancellationToken ct = default)
    {
        endpoint.HttpContext.MarkResponseStart();

        foreach (var requirement in failure.FailedRequirements)
        {
            endpoint.ValidationFailures.Add(requirement switch
            {
                ScopeAuthorizationRequirement => new ValidationFailure("scope", "Token does not have the required scope for this operation."),
                IOperationAuthorizationRequirement => new ValidationFailure("operation", "Not permitted to perform this operation."),
                _ => new ValidationFailure("forbidden", "Authorization failed.")
            });
        }
        
        return endpoint.HttpContext.Response.SendErrorsAsync(endpoint.ValidationFailures, statusCode: 403,
            cancellation: ct);
    }
}