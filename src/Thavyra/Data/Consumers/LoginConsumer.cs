using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.User;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers;

public class LoginConsumer :
    IConsumer<PasswordLogin_Check>,
    IConsumer<PasswordLogin_Create>,
    IConsumer<PasswordLogin_UpdateOrCreate>,
    IConsumer<DiscordLogin_GetOrCreate>,
    IConsumer<GitHubLogin_GetOrCreate>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IRequestClient<User_Create> _createUserClient;
    private readonly IRequestClient<User_GetById> _getUserClient;

    public LoginConsumer(
        ThavyraDbContext dbContext, 
        IRequestClient<User_Create> createUserClient,
        IRequestClient<User_GetById> getUserClient)
    {
        _dbContext = dbContext;
        _createUserClient = createUserClient;
        _getUserClient = getUserClient;
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
        var response = await _createUserClient.GetResponse<User>(new User_Create
        {
            Username = context.Message.Username
        }, context.CancellationToken);

        var login = new PasswordLoginDto
        {
            UserId = response.Message.Id,
            Password = context.Message.Password,
            CreatedAt = DateTime.UtcNow
        };
        
        await _dbContext.Passwords.AddAsync(login);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new PasswordLogin
        {
            Id = login.Id,
            User = response.Message,
            CreatedAt = login.CreatedAt
        });
    }
    
    public async Task Consume(ConsumeContext<PasswordLogin_UpdateOrCreate> context)
    {
        var login = await _dbContext.Passwords
            .FirstOrDefaultAsync(x => x.UserId == context.Message.UserId, context.CancellationToken);

        if (login is null)
        {
            login = new PasswordLoginDto
            {
                UserId = context.Message.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Passwords.Add(login);
        }
        
        login.Password = context.Message.Password;
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<DiscordLogin_GetOrCreate> context)
    {
        var login = await _dbContext.DiscordLogins
            .FirstOrDefaultAsync(x => x.DiscordId == context.Message.DiscordId, context.CancellationToken);

        bool create = login is null;

        if (login is null)
        {
            login = new DiscordLoginDto
            {
                DiscordId = context.Message.DiscordId,
                CreatedAt = DateTime.UtcNow
            };
            
            _dbContext.DiscordLogins.Add(login);
        }

        var response = create switch
        {
            true => await _createUserClient.GetResponse<User>(new User_Create
            {
                Username = context.Message.Username
            }, context.CancellationToken),
            
            false => await _getUserClient.GetResponse<User>(new User_GetById
            {
                Id = login.UserId
            }, context.CancellationToken)
        };

        login.UserId = response.Message.Id;
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new DiscordLogin
        {
            Id = login.Id,
            DiscordId = login.DiscordId,
            User = response.Message,
            CreatedAt = login.CreatedAt
        });
    }

    public async Task Consume(ConsumeContext<GitHubLogin_GetOrCreate> context)
    {
        var login = await _dbContext.GitHubLogins
            .FirstOrDefaultAsync(x => x.GitHubId == context.Message.GitHubId, context.CancellationToken);
        
        bool create = login is null;

        if (login is null)
        {
            login = new GitHubLoginDto
            {
                GitHubId = context.Message.GitHubId,
                CreatedAt = DateTime.UtcNow
            };
            
            _dbContext.GitHubLogins.Add(login);
        }

        var response = create switch
        {
            true => await _createUserClient.GetResponse<User>(new User_Create
            {
                Username = context.Message.Username,
            }, context.CancellationToken),

            false => await _getUserClient.GetResponse<User>(new User_GetById
            {
                Id = login.UserId
            }, context.CancellationToken)
        };

        login.UserId = response.Message.Id;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new GitHubLogin
        {
            Id = login.Id,
            GitHubId = login.GitHubId,
            User = response.Message,
            CreatedAt = login.CreatedAt
        });
    }
}