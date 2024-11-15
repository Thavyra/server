using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OpenIddict.Abstractions;
using Thavyra.Contracts;
using Thavyra.Contracts.Token;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers;

public class TokenConsumer :
    IConsumer<Token_Count>,
    IConsumer<Token_Create>,
    IConsumer<Token_Delete>,
    IConsumer<Token_Get>,
    IConsumer<Token_GetByApplication>,
    IConsumer<Token_GetByAuthorization>,
    IConsumer<Token_GetById>,
    IConsumer<Token_GetByReferenceId>,
    IConsumer<Token_GetBySubject>,
    IConsumer<Token_List>,
    IConsumer<Token_Prune>,
    IConsumer<Token_RevokeByAuthorization>,
    IConsumer<Token_Update>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public TokenConsumer(ThavyraDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    private static Token Map(TokenDto token)
    {
        return new Token
        {
            Id = token.Id,
            ApplicationId = token.ApplicationId,
            AuthorizationId = token.AuthorizationId,
            Subject = token.Subject,

            ReferenceId = token.ReferenceId,
            Type = token.Type,
            Status = token.Status,
            Payload = token.Payload,

            CreatedAt = token.CreatedAt,
            RedeemedAt = token.RedeemedAt,
            ExpiresAt = token.ExpiresAt
        };
    }
    
    public async Task Consume(ConsumeContext<Token_Count> context)
    {
        long count = await _dbContext.Tokens.LongCountAsync(context.CancellationToken);

        await context.RespondAsync(new Count(count));
    }

    public async Task Consume(ConsumeContext<Token_Create> context)
    {
        var token = new TokenDto
        {
            Id = context.Message.Id,
            ApplicationId = context.Message.ApplicationId,
            AuthorizationId = context.Message.AuthorizationId,
            Subject = context.Message.Subject,

            ReferenceId = context.Message.ReferenceId,
            Type = context.Message.Type,
            Status = context.Message.Status,
            Payload = context.Message.Payload,

            CreatedAt = context.Message.CreatedAt,
            RedeemedAt = context.Message.RedeemedAt,
            ExpiresAt = context.Message.ExpiresAt
        };

        _dbContext.Tokens.Add(token);
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        
        await context.RespondAsync(Map(token));
    }

    public async Task Consume(ConsumeContext<Token_Delete> context)
    {
        await _dbContext.Tokens
            .Where(x => x.Id == context.Message.Id)
            .ExecuteDeleteAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<Token_Get> context)
    {
        var tokens = await _dbContext.Tokens
            .Where(x => x.Subject == context.Message.Subject)
            .Where(x => x.ApplicationId == context.Message.ApplicationId)
            .Where(x => context.Message.Type == null || x.Type == context.Message.Type)
            .Where(x => context.Message.Status == null || x.Status == context.Message.Status)
            .ToListAsync(context.CancellationToken);

        await context.RespondAsync(new Multiple<Token>(tokens.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Token_GetByApplication> context)
    {
        var tokens = await _dbContext.Tokens
            .Where(x => x.ApplicationId == context.Message.ApplicationId)
            .ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Token>(tokens.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Token_GetByAuthorization> context)
    {
        var tokens = await _dbContext.Tokens
            .Where(x => x.AuthorizationId == context.Message.AuthorizationId)
            .ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Token>(tokens.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Token_GetById> context)
    {
        var token = await _dbContext.Tokens
            .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

        if (token is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(Map(token));
    }

    public async Task Consume(ConsumeContext<Token_GetByReferenceId> context)
    {
        var token = await _dbContext.Tokens
            .FirstOrDefaultAsync(x => x.ReferenceId == context.Message.ReferenceId, context.CancellationToken);

        if (token is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        await context.RespondAsync(Map(token));
    }

    public async Task Consume(ConsumeContext<Token_GetBySubject> context)
    {
        var tokens = await _dbContext.Tokens
            .Where(x => x.Subject == context.Message.Subject)
            .ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Token>(tokens.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Token_List> context)
    {
        var tokens = await _dbContext.Tokens.ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Token>(tokens.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Token_Prune> context)
    {
        var tokens = _dbContext.Tokens
            .Where(x => x.CreatedAt < context.Message.Threshold)
            .Where(x =>
                (x.Status != OpenIddictConstants.Statuses.Inactive && x.Status != OpenIddictConstants.Statuses.Valid) ||
                (x.Authorization != null && x.Authorization.Status != OpenIddictConstants.Statuses.Valid) ||
                x.ExpiresAt < DateTime.UtcNow);

        long count = await tokens.LongCountAsync(context.CancellationToken);
        
        await tokens.ExecuteDeleteAsync(context.CancellationToken);
        
        await context.RespondAsync(new Count(count));
    }

    public async Task Consume(ConsumeContext<Token_RevokeByAuthorization> context)
    {
        var tokens = _dbContext.Tokens
            .Where(x => x.AuthorizationId == context.Message.AuthorizationId);

        long count = await tokens.LongCountAsync(context.CancellationToken);
        
        await tokens.ExecuteDeleteAsync(context.CancellationToken);
        
        await context.RespondAsync(new Count(count));
    }

    public async Task Consume(ConsumeContext<Token_Update> context)
    {
        var token = await _dbContext.Tokens.FindAsync([context.Message.Id], context.CancellationToken);

        if (token is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        token.ReferenceId = context.Message.ReferenceId.IsChanged ? context.Message.ReferenceId.Value : token.ReferenceId;
        token.Type = context.Message.Type.IsChanged ? context.Message.Type.Value : token.Type;
        token.Status = context.Message.Status.IsChanged ? context.Message.Status.Value : token.Status;
        token.Payload = context.Message.Payload.IsChanged ? context.Message.Payload.Value : token.Payload;
        
        token.RedeemedAt = context.Message.RedeemedAt.IsChanged ? context.Message.RedeemedAt.Value : token.RedeemedAt;
        token.ExpiresAt = context.Message.ExpiresAt.IsChanged ? context.Message.ExpiresAt.Value : token.ExpiresAt;

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        
        await context.RespondAsync(Map(token));
    }
}