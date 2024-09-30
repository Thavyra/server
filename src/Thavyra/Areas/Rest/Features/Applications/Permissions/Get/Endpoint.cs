using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Permission;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Permissions.Get;

public class Endpoint : Endpoint<ApplicationRequest, List<PermissionResponse>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Permission_GetByApplication> _getPermissions;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Permission_GetByApplication> getPermissions)
    {
        _authorizationService = authorizationService;
        _getPermissions = getPermissions;
    }

    public override void Configure()
    {
        Get("/applications/{Application}/permissions");
    }

    public override async Task HandleAsync(ApplicationRequest req, CancellationToken ct)
    {
        var application = ProcessorState<AuthenticationState>().Application;

        if (application is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application, Security.Policies.Operation.Application.Read);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var response = await _getPermissions.GetResponse<Multiple<Permission>>(new Permission_GetByApplication
        {
            ApplicationId = application.Id
        }, ct);

        await SendAsync(response.Message.Items.Select(x => new PermissionResponse
        {
            Id = x.Id.ToString(),
            Name = x.Name,
            DisplayName = x.DisplayName
        }).ToList(), cancellation: ct);
    }
}