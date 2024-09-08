using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts;
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

    public UserConsumer(ThavyraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private User Map(UserDto user)
    {
        return new User
        {
            Id = user.Id,
            Username = user.Username,
            Description = user.Description,
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
        
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        await context.RespondAsync(Map(user));
    }

    public async Task Consume(ConsumeContext<User_Delete> context)
    {
        await _dbContext.Users
            .Where(x => x.Id == context.Message.Id)
            .ExecuteDeleteAsync();
    }

    public async Task Consume(ConsumeContext<User_ExistsByUsername> context)
    {
        bool exists = await _dbContext.Users
            .AnyAsync(x => x.Username == context.Message.Username);

        await context.RespondAsync(exists ? new UsernameExists() : new NotFound());
    }

    public async Task Consume(ConsumeContext<User_GetById> context)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == context.Message.Id);

        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(Map(user));
    }

    public async Task Consume(ConsumeContext<User_GetByUsername> context)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Username == context.Message.Username);

        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(Map(user));
    }

    public async Task Consume(ConsumeContext<User_Update> context)
    {
        var user = await _dbContext.Users.FindAsync(context.Message.Id);

        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        user.Username = context.Message.Username.IsChanged ? context.Message.Username.Value : user.Username;
        user.Description = context.Message.Description.IsChanged ? context.Message.Description.Value : user.Description;
    }
}