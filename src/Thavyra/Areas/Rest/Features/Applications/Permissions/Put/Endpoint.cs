using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Permission;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Permissions.Put;

public class Endpoint : Endpoint<Request>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Permission_GetByNames> _getPermissions;
    private readonly IRequestClient<Application_ModifyPermissions> _modifyPermissions;

    public Endpoint(
        IAuthorizationService authorizationService, 
        IRequestClient<Permission_GetByNames> getPermissions,
        IRequestClient<Application_ModifyPermissions> modifyPermissions)
    {
        _authorizationService = authorizationService;
        _getPermissions = getPermissions;
        _modifyPermissions = modifyPermissions;
    }

    public override void Configure()
    {
        Put("/applications/{Application}/permissions");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var application = ProcessorState<AuthenticationState>().Application;

        if (application is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application,
                Security.Policies.Operation.Application.Update);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var grantPermissions = await _getPermissions.GetResponse<Multiple<Permission>>(new Permission_GetByNames
        {
            Names = req.Grant
        }, ct);

        bool authorized = true;
        
        foreach (var permission in grantPermissions.Message.Items)
        {
            var grantResult =
                await _authorizationService.AuthorizeAsync(User, permission,
                    Security.Policies.Operation.Permission.Grant);

            if (grantResult.Succeeded)
            {
                continue;
            }
            
            authorized = false;
            AddError(x => x.Grant, $"Not authorised to grant permission {permission.Name}");
        }
        
        var denyPermissions = await _getPermissions.GetResponse<Multiple<Permission>>(new Permission_GetByNames
        {
            Names = req.Deny
        }, ct);

        foreach (var permission in denyPermissions.Message.Items)
        {
            var denyResult =
                await _authorizationService.AuthorizeAsync(User, permission,
                    Security.Policies.Operation.Permission.Deny);

            if (denyResult.Succeeded)
            {
                continue;
            }
            
            authorized = false;
            AddError(x => x.Deny, $"Not authorised to deny permission {permission.Name}");
        }

        if (!authorized)
        {
            await SendErrorsAsync(403, ct);
            return;
        }

        var response = await _modifyPermissions.GetResponse<PermissionsChanged>(new Application_ModifyPermissions
        {
            ApplicationId = application.Id,
            Grant = grantPermissions.Message.Items.Select(x => x.Id).ToList(),
            Deny = denyPermissions.Message.Items.Select(x => x.Id).ToList()
        }, ct);

        await SendAsync(new Response
        {
            Granted = response.Message.Granted.Select(x => new PermissionResponse
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                DisplayName = x.DisplayName
            }).ToList(),

            Denied = response.Message.Denied.Select(x => new PermissionResponse
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                DisplayName = x.DisplayName
            }).ToList()
        }, cancellation: ct);
    }
}