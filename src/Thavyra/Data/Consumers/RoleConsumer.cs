using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts;
using Thavyra.Contracts.Role;
using Thavyra.Data.Contexts;

namespace Thavyra.Data.Consumers;

public class RoleConsumer :
    IConsumer<User_GrantRole>,
    IConsumer<User_HasRole>,
    IConsumer<User_DenyRole>,
    IConsumer<Role_GetByUser>
{
    private readonly ThavyraDbContext _dbContext;

    public RoleConsumer(ThavyraDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<User_GrantRole> context)
    {
        var user = await _dbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == context.Message.UserId, context.CancellationToken);

        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        if (user.Roles.Any(x => x.Id == context.Message.RoleId))
        {
            await context.RespondAsync(new Success());
            return;
        }
        
        var role = await _dbContext.Roles
            .FirstOrDefaultAsync(x => x.Id == context.Message.RoleId, context.CancellationToken);

        if (role is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        user.Roles.Add(role);

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        
        await context.RespondAsync(new Success());
    }

    public async Task Consume(ConsumeContext<User_HasRole> context)
    {
        var user = await _dbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == context.Message.UserId, context.CancellationToken);

        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        if (user.Roles.Any(x => x.Name == context.Message.RoleName))
        {
            await context.RespondAsync(new Correct());
            return;
        }

        await context.RespondAsync(new Incorrect());
    }

    public async Task Consume(ConsumeContext<User_DenyRole> context)
    {
        var user = await _dbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == context.Message.UserId, context.CancellationToken);
        
        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        var role = user.Roles.FirstOrDefault(x => x.Id == context.Message.RoleId);

        if (role is null)
        {
            await context.RespondAsync(new Success());
            return;
        }
        
        user.Roles.Remove(role);

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new Success());
    }

    public async Task Consume(ConsumeContext<Role_GetByUser> context)
    {
        var user = await _dbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == context.Message.UserId, context.CancellationToken);
        
        if (user is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(new Multiple<Role>(user.Roles.Select(x => new Role
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName
        }).ToList()));
    }
}