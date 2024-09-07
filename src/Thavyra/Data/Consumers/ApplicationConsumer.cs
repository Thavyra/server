using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;
using Thavyra.Data.Security;

namespace Thavyra.Data.Consumers;

public class ApplicationConsumer :
    IConsumer<Application_CheckClientSecret>,
    IConsumer<Application_Count>,
    IConsumer<Application_Create>,
    IConsumer<Application_Delete>,
    IConsumer<Application_GetByClientId>,
    IConsumer<Application_GetById>,
    IConsumer<Application_GetByRedirect>,
    IConsumer<Application_List>,
    IConsumer<Application_Update>
{
    private readonly ThavyraDbContext _dbContext;

    public ApplicationConsumer(ThavyraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private Application Map(ApplicationDto application)
    {
        return new Application
        {
            Id = application.Id,
            OwnerId = application.OwnerId,

            ClientId = application.ClientId,
            ClientType = application.ClientType,
            ConsentType = application.ConsentType,
            Type = application.Type,
            Name = application.Name,
            Description = application.Description,
            CreatedAt = application.CreatedAt
        };
    }
    
    public async Task Consume(ConsumeContext<Application_CheckClientSecret> context)
    {
        var application = await _dbContext.Applications.FindAsync(context.Message.ApplicationId);

        if (application is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        if (application.ClientSecret == context.Message.Secret)
        {
            await context.RespondAsync(new Correct());
            return;
        }
        
        await context.RespondAsync(new Incorrect());
    }

    public async Task Consume(ConsumeContext<Application_Count> context)
    {
        long count = await _dbContext.Applications.LongCountAsync();

        await context.RespondAsync(new Count(count));
    }

    public async Task Consume(ConsumeContext<Application_Create> context)
    {
        var application = new ApplicationDto
        {
            OwnerId = context.Message.OwnerId,

            ClientId = Secret.NewSecret(20).ToString(),
            ConsentType = context.Message.ConsentType,

            Type = context.Message.Type,
            Name = context.Message.Name,
            Description = context.Message.Description,
            
            CreatedAt = DateTime.UtcNow
        };

        switch (context.Message.Type)
        {
            case OpenIddictConstants.ApplicationTypes.Web:
                var secret = Secret.NewSecret(32);
            
                application.ClientSecret = secret.ToString();
                application.ClientType = OpenIddictConstants.ClientTypes.Confidential;
                
                break;
            
            case OpenIddictConstants.ApplicationTypes.Native:
                application.ClientType = OpenIddictConstants.ClientTypes.Public;
                break;
            
            default:
                throw new InvalidOperationException($"Unsupported application type: {context.Message.Type}");
        }
        
        await _dbContext.Applications.AddAsync(application);
        await _dbContext.SaveChangesAsync();

        await context.RespondAsync(Map(application));
    }

    public async Task Consume(ConsumeContext<Application_Delete> context)
    {
        await _dbContext.Applications
            .Where(x => x.Id == context.Message.Id)
            .ExecuteDeleteAsync();
    }

    public async Task Consume(ConsumeContext<Application_GetByClientId> context)
    {
        var application = await _dbContext.Applications
            .FirstOrDefaultAsync(x => x.ClientId == context.Message.ClientId);

        if (application is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(Map(application));
    }

    public async Task Consume(ConsumeContext<Application_GetById> context)
    {
        var application = await _dbContext.Applications
            .FirstOrDefaultAsync(x => x.Id == context.Message.Id);

        if (application is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        await context.RespondAsync(Map(application));
    }

    public async Task Consume(ConsumeContext<Application_GetByRedirect> context)
    {
        var applications = await _dbContext.Redirects
            .Where(x => x.Uri == context.Message.Uri)
            .Select(x => x.Application)
            .ToListAsync();
        
        await context.RespondAsync(new Multiple<Application>(applications.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Application_List> context)
    {
        var applications = await _dbContext.Applications.ToListAsync();
        
        await context.RespondAsync(new Multiple<Application>(applications.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Application_Update> context)
    {
        var application = await _dbContext.Applications.FindAsync(context.Message.Id);

        if (application is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        application.Name = context.Message.Name.IsChanged ? context.Message.Name.Value : application.Name;
        application.Description = context.Message.Description.IsChanged ? context.Message.Description.Value : application.Description;

        await _dbContext.SaveChangesAsync();
    }
}