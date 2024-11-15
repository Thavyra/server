using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Thavyra.Contracts;
using Thavyra.Contracts.User;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;
using Thavyra.Data.Services;

namespace Thavyra.Data.Consumers.User;

public class UserConsumer :
    IConsumer<CreateUser>,
    IConsumer<User_Delete>,
    IConsumer<User_ExistsByUsername>,
    IConsumer<User_GetById>,
    IConsumer<User_GetByUsername>,
    IConsumer<User_Update>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly IFallbackUsernameService _fallbackUsername;

    public UserConsumer(
        ThavyraDbContext dbContext, 
        IMemoryCache cache,
        IFallbackUsernameService fallbackUsername)
    {
        _dbContext = dbContext;
        _cache = cache;
        _fallbackUsername = fallbackUsername;
    }

    private static Contracts.User.User Map(UserDto user)
    {
        return new Contracts.User.User
        {
            Id = user.Id,
            Username = user.Username,
            Description = user.Description,
            Balance = user.Balance,
            CreatedAt = user.CreatedAt
        };
    }
    
    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        var username = context.Message.Username ?? await _fallbackUsername.GenerateFallbackUsernameAsync();
        
        var user = new UserDto
        {
            Username = username,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        var message = new UserCreated
        {
            UserId = user.Id,
            Username = user.Username,
            Timestamp = user.CreatedAt
        };
        
        await context.RespondAsync(message);
        await context.Publish(message);
    }

    public async Task Consume(ConsumeContext<User_Delete> context)
    {
        await _dbContext.Users
            .Where(x => x.Id == context.Message.Id)
            .ExecuteUpdateAsync(
                x => x.SetProperty(user => user.DeletedAt, DateTime.UtcNow), 
                context.CancellationToken);
        
        _cache.Remove(context.Message.Id);
        
        await context.RespondAsync(new Success());
    }

    public async Task Consume(ConsumeContext<User_ExistsByUsername> context)
    {
        bool exists = await _dbContext.Users
            .AnyAsync(x => x.Username == context.Message.Username, context.CancellationToken);

        await context.RespondAsync(exists ? new UsernameExists() : new NotFound());
    }

    public async Task Consume(ConsumeContext<User_GetById> context)
    {
        var user = _cache.Get<UserDto>(context.Message.Id);

        if (user is null)
        {
            user = await _dbContext.Users
                .Where(x => !x.DeletedAt.HasValue)
                .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

            if (user is null)
            {
                await context.RespondAsync(new NotFound());
                return;
            }

            _cache.Set(user.Id, user, TimeSpan.FromMinutes(1));
        }

        await context.RespondAsync(Map(user));
    }

    public async Task Consume(ConsumeContext<User_GetByUsername> context)
    {
        var user = await _dbContext.Users
            .Where(x => !x.DeletedAt.HasValue)
            .FirstOrDefaultAsync(x => x.Username == context.Message.Username, context.CancellationToken);

        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(Map(user));
    }

    public async Task Consume(ConsumeContext<User_Update> context)
    {
        var user = await _dbContext.Users
            .Where(x => !x.DeletedAt.HasValue)
            .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        user.Username = context.Message.Username.IsChanged ? context.Message.Username.Value : user.Username;
        user.Description = context.Message.Description.IsChanged ? context.Message.Description.Value : user.Description;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(Map(user));

        _cache.Remove(user.Id);
    }
}