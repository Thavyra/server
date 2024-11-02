using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Permission;
using Thavyra.Data.Configuration;
using Thavyra.Data.Contexts;

namespace Thavyra.Data.Consumers.Application;

public class ApplicationCreatedConsumer : IConsumer<ApplicationCreated>
{
    private readonly ILogger<ApplicationCreatedConsumer> _logger;
    private readonly ThavyraDbContext _dbContext;
    private readonly IRequestClient<Permission_GetByNames> _getPermissions;
    private readonly ApplicationOptions _options;

    public ApplicationCreatedConsumer(
        ILogger<ApplicationCreatedConsumer> logger,
        ThavyraDbContext dbContext,
        IRequestClient<Permission_GetByNames> getPermissions,
        IOptions<ApplicationOptions> options)
    {
        _logger = logger;
        _dbContext = dbContext;
        _getPermissions = getPermissions;
        _options = options.Value;
    }
    
    public async Task Consume(ConsumeContext<ApplicationCreated> context)
    {
        var permissionNames = _options.DefaultPermissions.ToList();

        if (await _dbContext.Applications.CountAsync(context.CancellationToken) == 1)
        {
            _logger.LogInformation("Admin application created with ClientId: {ClientId} | ClientSecret: {ClientSecret}", 
                context.Message.Application.ClientId, context.Message.ClientSecret);
            
            permissionNames.Add(Constants.Permissions.Scopes.Admin);
        }
        
        if (permissionNames.Count == 0)
        {
            return;
        }

        var permissions = await _getPermissions.GetResponse<Multiple<Permission>>(new Permission_GetByNames
        {
            Names = permissionNames
        }, context.CancellationToken);

        await context.Publish(new Application_ModifyPermissions
        {
            ApplicationId = context.Message.Application.Id,
            Grant = permissions.Message.Items.Select(x => x.Id).ToList(),
            Deny = []
        });
    }
}