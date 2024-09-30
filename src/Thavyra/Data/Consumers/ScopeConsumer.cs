using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Thavyra.Contracts;
using Thavyra.Contracts.Scope;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers;

public class ScopeConsumer :
    IConsumer<Scope_Count>,
    IConsumer<Scope_Create>,
    IConsumer<Scope_Delete>,
    IConsumer<Scope_GetByApplication>,
    IConsumer<Scope_GetById>,
    IConsumer<Scope_GetByName>,
    IConsumer<Scope_GetByNames>,
    IConsumer<Scope_List>,
    IConsumer<Scope_Update>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public ScopeConsumer(ThavyraDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    private static Scope Map(ScopeDto scope)
    {
        return new Scope
        {
            Id = scope.Id,
            Name = scope.Name,
            DisplayName = scope.DisplayName,
            Description = scope.Description
        };
    }
    
    public async Task Consume(ConsumeContext<Scope_Count> context)
    {
        long count = await _dbContext.Scopes.LongCountAsync(context.CancellationToken);

        await context.RespondAsync(new Count(count));
    }

    public async Task Consume(ConsumeContext<Scope_Create> context)
    {
        var scope = new ScopeDto
        {
            Name = context.Message.Name,
            DisplayName = context.Message.DisplayName,
            Description = context.Message.Description
        };
        
        _dbContext.Scopes.Add(scope);
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(Map(scope));
    }

    public async Task Consume(ConsumeContext<Scope_Delete> context)
    {
        await _dbContext.Scopes
            .Where(x => x.Id == context.Message.Id)
            .ExecuteDeleteAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<Scope_GetByApplication> context)
    {
        var scopes = await _dbContext.Scopes.ToListAsync(context.CancellationToken);

        await context.RespondAsync(new Multiple<Scope>(scopes.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Scope_GetById> context)
    {
        var scope = await _dbContext.Scopes
            .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

        if (scope is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(Map(scope));
    }

    public async Task Consume(ConsumeContext<Scope_GetByName> context)
    {
        var scope = _cache.Get<ScopeDto>($"Scope:{context.Message.Name}");

        if (scope is null)
        {
            scope = await _dbContext.Scopes
                .FirstOrDefaultAsync(x => x.Name == context.Message.Name, context.CancellationToken);
            
            if (scope is null)
            {
                await context.RespondAsync(new NotFound());
                return;
            }

            _cache.Set($"Scope:{scope.Name}", scope, TimeSpan.FromMinutes(1));
        }
        
        await context.RespondAsync(Map(scope));
    }

    public async Task Consume(ConsumeContext<Scope_GetByNames> context)
    {
        var scopes = await _dbContext.Scopes
            .Where(x => context.Message.Names.Contains(x.Name))
            .ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Scope>(scopes.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Scope_List> context)
    {
        var scopes = await _dbContext.Scopes.ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Scope>(scopes.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Scope_Update> context)
    {
        var scope = await _dbContext.Scopes.FindAsync([context.Message.Id], context.CancellationToken);

        if (scope is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        scope.DisplayName = context.Message.DisplayName.IsChanged ? context.Message.DisplayName.Value : scope.DisplayName;
        scope.Description = context.Message.Description.IsChanged ? context.Message.Description.Value : scope.Description;
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        
        await context.RespondAsync(Map(scope));
    }
}