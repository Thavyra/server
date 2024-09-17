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
    IConsumer<PasswordLogin_GetByUser>,
    IConsumer<PasswordLogin_Update>,
    IConsumer<DiscordLogin_Create>,
    IConsumer<DiscordLogin_GetByDiscordId>,
    IConsumer<DiscordLogin_GetByUser>,
    IConsumer<GitHubLogin_Create>,
    IConsumer<GitHubLogin_GetByGitHubId>,
    IConsumer<GitHubLogin_GetByUser>
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

    //
    // Passwords
    //
    
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
        var now = DateTime.UtcNow;
        
        var login = new PasswordLoginDto
        {
            UserId = context.Message.UserId,
            Password = context.Message.Password,
            ChangedAt = now,
            CreatedAt = now
        };

        await _dbContext.Passwords.AddAsync(login);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new PasswordLogin
        {
            Id = login.Id,
            UserId = login.UserId,
            ChangedAt = login.ChangedAt,
            CreatedAt = login.CreatedAt
        });
    }

    public async Task Consume(ConsumeContext<PasswordLogin_GetByUser> context)
    {
        var login = await _dbContext.Passwords
            .FirstOrDefaultAsync(x => x.UserId == context.Message.UserId, context.CancellationToken);

        if (login is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(new PasswordLogin
        {
            Id = login.Id,
            UserId = login.UserId,
            ChangedAt = login.ChangedAt,
            CreatedAt = login.CreatedAt
        });
    }
    
    public async Task Consume(ConsumeContext<PasswordLogin_Update> context)
    {
        var login = await _dbContext.Passwords.FindAsync(context.Message.Id, context.CancellationToken);

        if (login is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        login.Password = context.Message.Password;
        login.ChangedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new PasswordLogin
        {
            Id = login.Id,
            UserId = login.UserId,
            ChangedAt = login.ChangedAt,
            CreatedAt = login.CreatedAt
        });
    }

    //
    // Discord 
    //
    
    public async Task Consume(ConsumeContext<DiscordLogin_Create> context)
    {
        var login = new DiscordLoginDto
        {
            UserId = context.Message.UserId,
            DiscordId = context.Message.DiscordId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.DiscordLogins.Add(login);

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new DiscordLogin
        {
            Id = login.Id,
            DiscordId = login.DiscordId,
            UserId = login.UserId,
            CreatedAt = login.CreatedAt
        });
    }
    
    public async Task Consume(ConsumeContext<DiscordLogin_GetByDiscordId> context)
    {
        var login = await _dbContext.DiscordLogins
            .FirstOrDefaultAsync(x => x.DiscordId == context.Message.DiscordId, context.CancellationToken);

        if (login is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(new DiscordLogin
        {
            Id = login.Id,
            UserId = login.UserId,
            DiscordId = login.DiscordId,
            CreatedAt = login.CreatedAt
        });
    }
    
    public async Task Consume(ConsumeContext<DiscordLogin_GetByUser> context)
    {
        var login = await _dbContext.DiscordLogins
            .FirstOrDefaultAsync(x => x.UserId == context.Message.UserId, context.CancellationToken);

        if (login is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(new DiscordLogin
        {
            Id = login.Id,
            UserId = login.UserId,
            DiscordId = login.DiscordId,
            CreatedAt = login.CreatedAt
        });
    }
    
    // 
    // GitHub 
    //

    public async Task Consume(ConsumeContext<GitHubLogin_Create> context)
    {
        var login = new GitHubLoginDto
        {
            UserId = context.Message.UserId,
            GitHubId = context.Message.GitHubId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.GitHubLogins.Add(login);

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new GitHubLogin
        {
            Id = login.Id,
            GitHubId = login.GitHubId,
            UserId = login.UserId,
            CreatedAt = login.CreatedAt
        });
    }

    public async Task Consume(ConsumeContext<GitHubLogin_GetByGitHubId> context)
    {
        var login = await _dbContext.GitHubLogins
            .FirstOrDefaultAsync(x => x.GitHubId == context.Message.GitHubId, context.CancellationToken);

        if (login is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(new GitHubLogin
        {
            Id = login.Id,
            UserId = login.UserId,
            GitHubId = login.GitHubId,
            CreatedAt = login.CreatedAt
        });
    }

    public async Task Consume(ConsumeContext<GitHubLogin_GetByUser> context)
    {
        var login = await _dbContext.GitHubLogins
            .FirstOrDefaultAsync(x => x.UserId == context.Message.UserId, context.CancellationToken);

        if (login is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(new GitHubLogin
        {
            Id = login.Id,
            UserId = login.UserId,
            GitHubId = login.GitHubId,
            CreatedAt = login.CreatedAt
        });
    }
}