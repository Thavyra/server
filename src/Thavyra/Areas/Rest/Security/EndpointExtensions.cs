using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security;

public static class EndpointExtensions
{
    public static Task SendAuthorizationFailureAsync(this IEndpoint endpoint, AuthorizationFailure failure, CancellationToken ct = default)
    {
        endpoint.HttpContext.MarkResponseStart();

        foreach (var reason in failure.FailureReasons)
        {
            endpoint.ValidationFailures.Add(new ValidationFailure("Message", reason.Message));
        }

        if (endpoint.ValidationFailures.Count == 0)
        {
            endpoint.ValidationFailures.Add(
                new ValidationFailure("Forbidden", "Not authorized to perform this action."));
        }
        
        return endpoint.HttpContext.Response.SendErrorsAsync(endpoint.ValidationFailures, statusCode: 403,
            cancellation: ct);
    }
}