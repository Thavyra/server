using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Thavyra.Contracts;
using Thavyra.Contracts.User;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

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

    public UserConsumer(
        ThavyraDbContext dbContext, 
        IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
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
        var user = new UserDto
        {
            Username = context.Message.Username,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        var message = new UserCreated
        {
            UserId = user.Id,
            Timestamp = user.CreatedAt
        };
        
        await context.RespondAsync(message);
        await context.Publish(message);
    }

    public async Task Consume(ConsumeContext<User_Delete> context)
    {
        await _dbContext.Users
            .Where(x => x.Id == context.Message.Id)
            .ExecuteDeleteAsync(context.CancellationToken);

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
        var user = await _dbContext.Users.FindAsync([context.Message.Id], context.CancellationToken);

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