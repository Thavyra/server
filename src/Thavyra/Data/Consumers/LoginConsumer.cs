using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts;
using Thavyra.Contracts.Login;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers;

public class LoginConsumer : 
    IConsumer<PasswordLogin_Check>,
    IConsumer<PasswordLogin_Create>
{
    private readonly ThavyraDbContext _dbContext;

    public LoginConsumer(ThavyraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<PasswordLogin_Check> context)
    {
        var login = await _dbContext.Passwords
            .FirstOrDefaultAsync(x => x.UserId == context.Message.UserId);

        if (login is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        if (context.Message.Password == login.Password)
        {
            await context.RespondAsync(new Correct());
            return;
        }

        await context.RespondAsync(new Incorrect());
    }

    public async Task Consume(ConsumeContext<PasswordLogin_Create> context)
    {
        var login = new PasswordLoginDto
        {
            UserId = context.Message.UserId,
            Password = context.Message.Password,
            CreatedAt = DateTime.UtcNow
        };
        
        await _dbContext.Passwords.AddAsync(login);
        await _dbContext.SaveChangesAsync();

        await context.RespondAsync(new PasswordLogin
        {
            Id = login.Id,
            UserId = login.UserId,
            CreatedAt = login.CreatedAt
        });
    }
}