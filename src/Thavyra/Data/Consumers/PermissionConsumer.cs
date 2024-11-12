using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Permission;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers;

public class PermissionConsumer :
    IConsumer<Permission_GetByApplication>,
    IConsumer<Permission_GetByNames>,
    IConsumer<Application_ModifyPermissions>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public PermissionConsumer(ThavyraDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task Consume(ConsumeContext<Permission_GetByApplication> context)
    {
        var permissions = _cache.Get<List<PermissionDto>>($"permissions:{context.Message.ApplicationId}");

        if (permissions is null)
        {
            permissions = await _dbContext.Applications
                .Where(x => x.Id == context.Message.ApplicationId)
                .Select(x => x.Permissions.ToList())
                .FirstOrDefaultAsync(context.CancellationToken);

            if (permissions is null)
            {
                await context.RespondAsync(new NotFound());
                return;
            }
            
            _cache.Set($"permissions:{context.Message.ApplicationId}", permissions, TimeSpan.FromMinutes(1));
        }

        await context.RespondAsync(new Multiple<Permission>(permissions.Select(x => new Permission
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName
        }).ToList()));
    }
    
    public async Task Consume(ConsumeContext<Permission_GetByNames> context)
    {
        var permissions = await _dbContext.Permissions
            .Where(x => context.Message.Names.Contains(x.Name))
            .ToListAsync(context.CancellationToken);

        await context.RespondAsync(new Multiple<Permission>(permissions.Select(x => new Permission
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName
        }).ToList()));
    }

    public async Task Consume(ConsumeContext<Application_ModifyPermissions> context)
    {
        var application = await _dbContext.Applications
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.Id == context.Message.ApplicationId, context.CancellationToken);

        if (application is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        var grantPermissions = await _dbContext.Permissions
            .Where(x => context.Message.Grant.Contains(x.Id))
            .Where(x => !application.Permissions.Contains(x))
            .ToListAsync(context.CancellationToken);

        foreach (var permission in grantPermissions)
        {
            application.Permissions.Add(permission);
        }
        
        var denyPermissions = application.Permissions
            .Where(x => context.Message.Deny.Contains(x.Id))
            .ToList();

        foreach (var permission in denyPermissions)
        {
            application.Permissions.Remove(permission);
        }

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new PermissionsChanged
        {
            ApplicationId = application.Id,
            
            Granted = grantPermissions.Select(x => new Permission
            {
                Id = x.Id,
                Name = x.Name,
                DisplayName = x.DisplayName
            }).ToList(),
            
            Denied = denyPermissions.Select(x => new Permission
            {
                Id = x.Id,
                Name = x.Name,
                DisplayName = x.DisplayName
            }).ToList(),
            
            CurrentPermissions = application.Permissions.Select(x => new Permission
            {
                Id = x.Id,
                Name = x.Name,
                DisplayName = x.DisplayName
            }).ToList()
        });
        
        _cache.Remove($"permissions:{application.Id}");
    }
}