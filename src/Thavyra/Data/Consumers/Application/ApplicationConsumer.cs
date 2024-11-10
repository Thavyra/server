using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OpenIddict.Abstractions;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;
using Thavyra.Data.Security;
using Thavyra.Data.Security.Hashing;

namespace Thavyra.Data.Consumers.Application;

public class ApplicationConsumer :
    IConsumer<Application_CheckClientSecret>,
    IConsumer<Application_Count>,
    IConsumer<Application_Create>,
    IConsumer<Application_Delete>,
    IConsumer<Application_GetByClientId>,
    IConsumer<Application_GetById>,
    IConsumer<Application_GetByOwner>,
    IConsumer<Application_GetByRedirect>,
    IConsumer<Application_List>,
    IConsumer<Application_ResetClientSecret>,
    IConsumer<Application_Update>,
    IConsumer<Redirect_Create>,
    IConsumer<Redirect_Delete>,
    IConsumer<Redirect_GetByApplication>,
    IConsumer<Redirect_GetById>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly IHashService _hashService;

    public ApplicationConsumer(ThavyraDbContext dbContext, IMemoryCache cache, IHashService hashService)
    {
        _dbContext = dbContext;
        _cache = cache;
        _hashService = hashService;
    }

    private static Contracts.Application.Application Map(ApplicationDto application)
    {
        return new Contracts.Application.Application
        {
            Id = application.Id,
            OwnerId = application.OwnerId,

            ClientId = application.ClientId,
            ClientType = application.ClientSecretHash switch
            {
                not null => OpenIddictConstants.ClientTypes.Confidential,
                _ => OpenIddictConstants.ClientTypes.Public
            },
            Type = application.Type,
            Name = application.Name,
            Description = application.Description,
            CreatedAt = application.CreatedAt
        };
    }

    public async Task Consume(ConsumeContext<Application_CheckClientSecret> context)
    {
        var application = await _dbContext.Applications
            .Where(x => !x.DeletedAt.HasValue)
            .FirstOrDefaultAsync(x => x.Id == context.Message.ApplicationId, context.CancellationToken);

        if (application?.ClientSecretHash is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        var result = await _hashService.CheckAsync(context.Message.Secret, application.ClientSecretHash);

        if (!result.Succeeded)
        {
            await context.RespondAsync(new Incorrect());
            return;
        }

        if (result.Rehash is { } rehash)
        {
            application.ClientSecretHash = rehash;

            await _dbContext.SaveChangesAsync(context.CancellationToken);
        }

        await context.RespondAsync(new Correct());
    }

    public async Task Consume(ConsumeContext<Application_Count> context)
    {
        long count = await _dbContext.Applications
            .Where(x => !x.DeletedAt.HasValue)
            .LongCountAsync(context.CancellationToken);

        await context.RespondAsync(new Count(count));
    }

    public async Task Consume(ConsumeContext<Application_Create> context)
    {
        var application = new ApplicationDto
        {
            OwnerId = context.Message.OwnerId,

            ClientId = Secret.NewSecret(20).ToString(),

            Type = context.Message.Type,
            Name = context.Message.Name,
            Description = context.Message.Description,

            CreatedAt = DateTime.UtcNow
        };

        string? clientSecret = null;

        switch (context.Message.Type)
        {
            case OpenIddictConstants.ApplicationTypes.Web:
            case Constants.ApplicationTypes.Service:
                
                clientSecret = Secret.NewSecret(32).ToString();
                string hash = await _hashService.HashAsync(clientSecret);

                application.ClientSecretHash = hash;

                break;

            case OpenIddictConstants.ApplicationTypes.Native:
                break;

            default:
                throw new InvalidOperationException($"Unsupported application type: {context.Message.Type}");
        }

        _dbContext.Applications.Add(application);
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        var message = new ApplicationCreated
        {
            Id = application.Id,
            Application = Map(application),
            ClientSecret = clientSecret
        };
        
        await context.RespondAsync(message);
        await context.Publish(message);
    }

    public async Task Consume(ConsumeContext<Application_Delete> context)
    {
        await _dbContext.Applications
            .Where(x => x.Id == context.Message.Id)
            .ExecuteUpdateAsync(
                x => x.SetProperty(a => a.DeletedAt, DateTime.UtcNow), 
                context.CancellationToken);

        _cache.Remove(context.Message.Id);
        
        await context.RespondAsync(new Success());
    }

    public async Task Consume(ConsumeContext<Application_GetByClientId> context)
    {
        var application = await _dbContext.Applications
            .Where(x => !x.DeletedAt.HasValue)
            .FirstOrDefaultAsync(x => x.ClientId == context.Message.ClientId, context.CancellationToken);

        if (application is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(Map(application));
    }

    public async Task Consume(ConsumeContext<Application_GetById> context)
    {
        var application = _cache.Get<ApplicationDto>(context.Message.Id);

        if (application is null)
        {
            application = await _dbContext.Applications
                .Where(x => !x.DeletedAt.HasValue)
                .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

            if (application is null)
            {
                await context.RespondAsync(new NotFound());
                return;
            }

            _cache.Set(application.Id, application, TimeSpan.FromMinutes(1));
        }

        await context.RespondAsync(Map(application));
    }

    public async Task Consume(ConsumeContext<Application_GetByOwner> context)
    {
        var applications = await _dbContext.Applications
            .Where(x => !x.DeletedAt.HasValue)
            .Where(x => x.OwnerId == context.Message.OwnerId)
            .ToListAsync(context.CancellationToken);

        await context.RespondAsync(new Multiple<Contracts.Application.Application>(applications.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Application_GetByRedirect> context)
    {
        var applications = await _dbContext.Redirects
            .Where(x => x.Uri == context.Message.Uri)
            .Select(x => x.Application!)
            .Where(x => !x.DeletedAt.HasValue)
            .ToListAsync(context.CancellationToken);

        await context.RespondAsync(new Multiple<Contracts.Application.Application>(applications.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Application_List> context)
    {
        var applications = await _dbContext.Applications
            .Where(x => !x.DeletedAt.HasValue)
            .ToListAsync(context.CancellationToken);

        await context.RespondAsync(new Multiple<Contracts.Application.Application>(applications.Select(Map).ToList()));
    }
    
    public async Task Consume(ConsumeContext<Application_ResetClientSecret> context)
    {
        var application = await _dbContext.Applications
            .Where(x => !x.DeletedAt.HasValue)
            .FirstOrDefaultAsync(x => x.Id == context.Message.ApplicationId, context.CancellationToken);
        
        if (application is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        string secret = Secret.NewSecret(32).ToString();
        string hash = await _hashService.HashAsync(secret);
        
        application.ClientSecretHash = hash;
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new ClientSecretCreated
        {
            ApplicationId = application.Id,
            ClientSecret = secret
        });
    }

    public async Task Consume(ConsumeContext<Application_Update> context)
    {
        var application = await _dbContext.Applications
            .Where(x => !x.DeletedAt.HasValue)
            .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

        if (application is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        application.Name = context.Message.Name.IsChanged ? context.Message.Name.Value : application.Name;
        application.Description = context.Message.Description.IsChanged
            ? context.Message.Description.Value
            : application.Description;

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        
        _cache.Remove(application.Id);

        await context.RespondAsync(Map(application));
    }

    public async Task Consume(ConsumeContext<Redirect_Create> context)
    {
        var application = await _dbContext.Applications
            .Where(x => !x.DeletedAt.HasValue)
            .FirstOrDefaultAsync(x => x.Id == context.Message.ApplicationId, context.CancellationToken);

        if (application is null)
        {
            if (context.IsResponseAccepted<NotFound>())
            {
                await context.RespondAsync(new NotFound());
                return;
            }

            throw new InvalidOperationException("Application not found.");
        }

        var redirect = new RedirectDto
        {
            ApplicationId = context.Message.ApplicationId,
            Uri = context.Message.Uri,
            CreatedAt = DateTime.UtcNow
        };
        
        application.Redirects.Add(redirect);
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new Redirect
        {
            Id = redirect.Id,
            ApplicationId = redirect.ApplicationId,
            Uri = redirect.Uri,
            CreatedAt = redirect.CreatedAt
        });
    }

    public async Task Consume(ConsumeContext<Redirect_Delete> context)
    {
        await _dbContext.Redirects
            .Where(x => !x.Application!.DeletedAt.HasValue)
            .Where(x => x.ApplicationId == context.Message.ApplicationId)
            .Where(x => x.Id == context.Message.Id)
            .ExecuteDeleteAsync(context.CancellationToken);

        await context.RespondAsync(new Success());
    }

    public async Task Consume(ConsumeContext<Redirect_GetByApplication> context)
    {
        var redirects = await _dbContext.Redirects
            .Where(x => !x.Application!.DeletedAt.HasValue)
            .Where(x => x.ApplicationId == context.Message.ApplicationId)
            .ToListAsync(context.CancellationToken);

        var response = redirects.Select(x => new Redirect
        {
            Id = x.Id,
            ApplicationId = x.ApplicationId,
            Uri = x.Uri,
            CreatedAt = x.CreatedAt
        });

        await context.RespondAsync(new Multiple<Redirect>(response.ToList()));
    }

    public async Task Consume(ConsumeContext<Redirect_GetById> context)
    {
        var redirect = await _dbContext.Redirects
            .Where(x => !x.Application!.DeletedAt.HasValue)
            .Where(x => x.ApplicationId == context.Message.ApplicationId)
            .Where(x => x.Id == context.Message.Id)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (redirect is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(new Redirect
        {
            Id = redirect.Id,
            ApplicationId = redirect.ApplicationId,
            Uri = redirect.Uri,
            CreatedAt = redirect.CreatedAt
        });
    }
}