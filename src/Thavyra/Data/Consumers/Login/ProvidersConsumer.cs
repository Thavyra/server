using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Providers;
using Thavyra.Contracts.User;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers.Login;

public class ProvidersConsumer :
    IConsumer<ProviderLogin>,
    IConsumer<LinkProvider>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IRequestClient<CreateUser> _createUser;

    public ProvidersConsumer(
        ThavyraDbContext dbContext,
        IRequestClient<CreateUser> createUser)
    {
        _dbContext = dbContext;
        _createUser = createUser;
    }
    
    public async Task Consume(ConsumeContext<ProviderLogin> context)
    {
        var login = await _dbContext.Logins
            .Where(x => x.Type == context.Message.Provider)
            .Where(x => x.ProviderAccountId == context.Message.AccountId)
            .Include(x => x.User)
            .FirstOrDefaultAsync(context.CancellationToken);
        
        if (login is null)
        {
            var user = await _createUser.GetResponse<UserCreated>(new CreateUser(), context.CancellationToken);

            login = new LoginDto
            {
                UserId = user.Message.UserId,
                Type = context.Message.Provider,

                ProviderAccountId = context.Message.AccountId,
                ProviderUsername = context.Message.Username,
                ProviderAvatarUrl = context.Message.AvatarUrl,

                CreatedAt = user.Message.Timestamp,
                UsedAt = user.Message.Timestamp,
                UpdatedAt = user.Message.Timestamp
            };

            _dbContext.Logins.Add(login);
            
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            await context.RespondAsync(new UserRegistered
            {
                UserId = login.UserId,
                Username = login.ProviderUsername
            });
            
            return;
        }

        login.ProviderUsername = context.Message.Username;
        login.ProviderAvatarUrl = context.Message.AvatarUrl;
        login.UsedAt = DateTime.UtcNow;

        await context.RespondAsync(new LoginSucceeded
        {
            UserId = login.UserId,
            Username = login.User.Username ?? login.ProviderUsername
        });
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<LinkProvider> context)
    {
        var login = await _dbContext.Logins
            .Where(x => x.Type == context.Message.Provider)
            .Where(x => x.ProviderAccountId == context.Message.AccountId)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (login is not null)
        {
            await context.RespondAsync(new AccountAlreadyRegistered());
            return;
        }

        var now = DateTime.UtcNow;

        login = new LoginDto
        {
            UserId = context.Message.UserId,
            Type = context.Message.Provider,

            ProviderAccountId = context.Message.AccountId,
            ProviderUsername = context.Message.Username,
            ProviderAvatarUrl = context.Message.AvatarUrl,

            CreatedAt = now,
            UsedAt = now,
            UpdatedAt = now
        };

        _dbContext.Logins.Add(login);

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        var user = await _dbContext.Logins
            .Where(x => x.Id == login.Id)
            .Select(x => x.User)
            .FirstOrDefaultAsync(context.CancellationToken);

        await context.RespondAsync(new ProviderLinked
        {
            UserId = context.Message.UserId,
            Username = user?.Username ?? context.Message.Username
        });
    }
}