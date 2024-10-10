using FastEndpoints;
using FluentValidation.Results;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Permission;
using Thavyra.Rest.Features.Applications.Permissions;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Patch;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Permission_GetByApplication> _getApplicationPermissions;
    private readonly IRequestClient<Application_Update> _client;
    private readonly IRequestClient<Permission_GetByNames> _getPermissions;
    private readonly IRequestClient<Application_ModifyPermissions> _modifyPermissions;

    public Endpoint(
        IAuthorizationService authorizationService,
        IRequestClient<Permission_GetByApplication> getApplicationPermissions,
        IRequestClient<Application_Update> client,
        IRequestClient<Permission_GetByNames> getPermissions,
        IRequestClient<Application_ModifyPermissions> modifyPermissions)
    {
        _authorizationService = authorizationService;
        _getApplicationPermissions = getApplicationPermissions;
        _client = client;
        _getPermissions = getPermissions;
        _modifyPermissions = modifyPermissions;
    }

    public override void Configure()
    {
        Patch("/applications/{Application}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var application = ProcessorState<AuthenticationState>().Application;

        if (application is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User,
            application, Security.Policies.Operation.Application.Update);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var updated = await UpdateDetails(application, req, ct);

        var response = new Response
        {
            Id = application.Id.ToString(),
            OwnerId = application.OwnerId.ToString(),

            Name = updated.Name,
            Description = updated.Description,

            CreatedAt = application.CreatedAt
        };

        if (req.Permissions.HasValue)
        {
            var updatedPermissions = await UpdatePermissions(application, req.Permissions.Value, ct);
            
            response.Permissions = updatedPermissions.Select(x => new PermissionResponse
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                DisplayName = x.DisplayName
            }).ToList();
        }
        
        await SendAsync(response, cancellation: ct);
    }

    private async Task<Application> UpdateDetails(Application application, Request req, CancellationToken ct)
    {
        var updateRequest = new Application_Update
        {
            Id = application.Id,
        };

        if (req.Name.HasValue)
        {
            updateRequest = updateRequest with { Name = req.Name.Value };
        }

        if (req.Description.HasValue)
        {
            updateRequest = updateRequest with { Description = req.Description.Value };
        }

        var response = await _client.GetResponse<Application>(updateRequest, ct);

        return response.Message;
    }

    private async Task<List<Permission>> UpdatePermissions(Application application, Permissions.Put.Request req, CancellationToken ct)
    {
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
            AddError(new ValidationFailure("permissions.grant", $"Not authorised to grant permission {permission.Name}"));
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
            AddError(new ValidationFailure("permissions.deny", $"Not authorised to deny permission {permission.Name}"));
        }

        if (!authorized)
        {
            ThrowIfAnyErrors(statusCode: 403);
        }

        var response = await _modifyPermissions.GetResponse<PermissionsChanged>(new Application_ModifyPermissions
        {
            ApplicationId = application.Id,
            Grant = grantPermissions.Message.Items.Select(x => x.Id).ToList(),
            Deny = denyPermissions.Message.Items.Select(x => x.Id).ToList()
        }, ct);

        return response.Message.CurrentPermissions;
    }
}