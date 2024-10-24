using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OpenIddict.Abstractions;
using Thavyra.Contracts;
using Thavyra.Contracts.Authorization;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers;

public class AuthorizationConsumer :
    IConsumer<Authorization_Count>,
    IConsumer<Authorization_Create>,
    IConsumer<Authorization_Delete>,
    IConsumer<Authorization_Get>,
    IConsumer<Authorization_GetByApplication>,
    IConsumer<Authorization_GetById>,
    IConsumer<Authorization_GetByUser>,
    IConsumer<Authorization_List>,
    IConsumer<Authorization_Prune>,
    IConsumer<Authorization_Update>,
    IConsumer<Authorization_Revoke>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public AuthorizationConsumer(ThavyraDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    private static Authorization Map(AuthorizationDto authorization)
    {
        return new Authorization
        {
            Id = authorization.Id,

            ApplicationId = authorization.ApplicationId,
            Subject = authorization.Subject,

            Type = authorization.Type,
            Status = authorization.Status,

            Scopes = authorization.Scopes.Select(x => x.Name).ToList(),

            CreatedAt = authorization.CreatedAt
        };
    }
    
    public async Task Consume(ConsumeContext<Authorization_Count> context)
    {
        long count = await _dbContext.Applications.LongCountAsync(context.CancellationToken);

        await context.RespondAsync(new Count(count));
    }

    public async Task Consume(ConsumeContext<Authorization_Create> context)
    {
        var scopes = await _dbContext.Scopes
            .Where(x => context.Message.Scopes.Contains(x.Name))
            .ToListAsync(context.CancellationToken);
        
        var authorization = new AuthorizationDto
        {
            Id = context.Message.Id,
            ApplicationId = context.Message.ApplicationId,
            Subject = context.Message.Subject,
            Type = context.Message.Type,
            Status = context.Message.Status,
            CreatedAt = DateTime.UtcNow,
            
            Scopes = scopes
        };

        _dbContext.Add(authorization);
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(Map(authorization));
    }

    public async Task Consume(ConsumeContext<Authorization_Delete> context)
    {
        await _dbContext.Authorizations
            .Where(x => x.Id == context.Message.Id)
            .ExecuteDeleteAsync(context.CancellationToken);

        await context.RespondAsync(new Success());
    }

    public async Task Consume(ConsumeContext<Authorization_Get> context)
    {
        var authorizations = await _dbContext.Authorizations
            .Where(x => x.Subject == context.Message.Subject)
            .Where(x => x.ApplicationId == context.Message.ApplicationId)
            .Where(x => context.Message.Type == null || x.Type == context.Message.Type)
            .Where(x => context.Message.Status == null || x.Status == context.Message.Status)
            .Include(x => x.Scopes)
            .ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Authorization>(authorizations.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Authorization_GetByApplication> context)
    {
        var authorizations = await _dbContext.Authorizations
            .Where(x => x.ApplicationId == context.Message.ApplicationId)
            .Include(x => x.Scopes)
            .ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Authorization>(authorizations.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Authorization_GetById> context)
    {
        var authorization = _cache.Get<AuthorizationDto>(context.Message.Id);

        if (authorization is null)
        {
            authorization = await _dbContext.Authorizations
                .Include(x => x.Scopes)
                .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);
            
            if (authorization is null)
            {
                await context.RespondAsync(new NotFound());
                return;
            }

            _cache.Set(authorization.Id, authorization, TimeSpan.FromMinutes(1));
        }
        
        await context.RespondAsync(Map(authorization));
    }

    public async Task Consume(ConsumeContext<Authorization_GetByUser> context)
    {
        var authorizations = await _dbContext.Authorizations
            .Where(x => x.Subject == context.Message.Subject)
            .Where(x => x.Status == OpenIddictConstants.Statuses.Valid)
            .Include(x => x.Scopes)
            .ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Authorization>(authorizations.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Authorization_List> context)
    {
        var authorization = await _dbContext.Authorizations
            .Include(x => x.Scopes)
            .ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Authorization>(authorization.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Authorization_Prune> context)
    {
        var authorizations = _dbContext.Authorizations
            .Where(x => x.CreatedAt < context.Message.Threshold)
            .Where(x => x.Status != OpenIddictConstants.Statuses.Valid ||
                        x.Type == OpenIddictConstants.AuthorizationTypes.AdHoc && !x.Tokens.Any());

        long count = await authorizations.LongCountAsync(context.CancellationToken);
        await authorizations.ExecuteDeleteAsync(context.CancellationToken);

        await context.RespondAsync(new Count(count));
    }

    public async Task Consume(ConsumeContext<Authorization_Update> context)
    {
        var authorization = await _dbContext.Authorizations.FindAsync([context.Message.Id], context.CancellationToken);

        if (authorization is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        authorization.Type = context.Message.Type.IsChanged ? context.Message.Type.Value : authorization.Type;
        authorization.Status = context.Message.Status.IsChanged ? context.Message.Status.Value : authorization.Status;

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        
        _cache.Remove(authorization.Id);
        
        await context.RespondAsync(Map(authorization));
    }

    public async Task Consume(ConsumeContext<Authorization_Revoke> context)
    {
        var authorization =
            await _dbContext.Authorizations.FindAsync([context.Message.AuthorizationId], context.CancellationToken);

        if (authorization is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        if (authorization.Type == OpenIddictConstants.Statuses.Revoked)
        {
            await context.RespondAsync(new Success());
            return;
        }
        
        authorization.Status = OpenIddictConstants.Statuses.Revoked;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new Success());
    }
}