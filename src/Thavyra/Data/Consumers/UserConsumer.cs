using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Thavyra.Contracts;
using Thavyra.Contracts.Transaction;
using Thavyra.Contracts.User;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers;

public class UserConsumer :
    IConsumer<User_Create>,
    IConsumer<User_Delete>,
    IConsumer<User_ExistsByUsername>,
    IConsumer<User_GetById>,
    IConsumer<User_GetByUsername>,
    IConsumer<User_Update>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly IPublishEndpoint _publishEndpoint;

    public UserConsumer(ThavyraDbContext dbContext, IMemoryCache cache, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _cache = cache;
        _publishEndpoint = publishEndpoint;
    }

    private static User Map(UserDto user)
    {
        return new User
        {
            Id = user.Id,
            Username = user.Username,
            Description = user.Description,
            Balance = user.Balance,
            CreatedAt = user.CreatedAt
        };
    }
    
    public async Task Consume(ConsumeContext<User_Create> context)
    {
        var user = new UserDto
        {
            Username = context.Message.Username,
            CreatedAt = DateTime.UtcNow
        };
        
        _dbContext.Users.Add(user);
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(Map(user));

        await _publishEndpoint.Publish(new Transaction_Create
        {
            ApplicationId = new Guid("96ab99e3-6b3e-4265-a36a-8524e9a74f60"),
            SubjectId = user.Id,
            Description = "Welcome to Thavyra!",
            Amount = 1000
        }, context.CancellationToken);
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
        var user = await _dbContext.Users.FindAsync(context.Message.Id, context.CancellationToken);

        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        user.Username = context.Message.Username.IsChanged ? context.Message.Username.Value : user.Username;
        user.Description = context.Message.Description.IsChanged ? context.Message.Description.Value : user.Description;

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        
        await context.RespondAsync(Map(user));
    }
}