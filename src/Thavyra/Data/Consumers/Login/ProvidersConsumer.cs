using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Providers;
using Thavyra.Contracts.User;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;
using Thavyra.Storage;

namespace Thavyra.Data.Consumers.Login;

public class ProvidersConsumer :
    IConsumer<ProviderLogin>,
    IConsumer<LinkProvider>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IRequestClient<CreateUser> _createUser;
    private readonly IAvatarStorageService _avatarStorageService;

    public ProvidersConsumer(
        ThavyraDbContext dbContext,
        IRequestClient<CreateUser> createUser,
        IAvatarStorageService avatarStorageService)
    {
        _dbContext = dbContext;
        _createUser = createUser;
        _avatarStorageService = avatarStorageService;
    }
    
    public async Task Consume(ConsumeContext<ProviderLogin> context)
    {
        var login = await _dbContext.Logins
            .Where(x => x.Type == context.Message.Provider)
            .Where(x => x.ProviderAccountId == context.Message.AccountId)
            .Where(x => !x.User!.DeletedAt.HasValue)
            .Include(x => x.User)
            .FirstOrDefaultAsync(context.CancellationToken);
        
        if (login is null)
        {
            var message = await CreateUser(context.Message, context.CancellationToken);
            await context.RespondAsync(message);
            
            return;
        }

        login.ProviderUsername = context.Message.Username;
        login.ProviderAvatarUrl = context.Message.AvatarUrl;

        await context.RespondAsync(new LoginSucceeded
        {
            UserId = login.UserId,
            Username = login.User!.Username
        });

        var attempt = new LoginAttemptDto
        {
            LoginId = login.Id,
            Succeeded = true,
            CreatedAt = DateTime.UtcNow,
        };
        
        _dbContext.LoginAttempts.Add(attempt);
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }

    private async Task<UserRegistered> CreateUser(ProviderLogin message, CancellationToken cancellationToken)
    {
        var user = await _createUser.GetResponse<UserCreated>(new CreateUser(), cancellationToken);

        try
        {
            await _avatarStorageService.UploadAvatarAsync(AvatarType.User, user.Message.UserId,
                message.AvatarUrl, cancellationToken);
        }
        catch (Exception)
        {
            // this is allowed to fail silently
        }

        var login = new LoginDto
        {
            UserId = user.Message.UserId,
            Type = message.Provider,

            ProviderAccountId = message.AccountId,
            ProviderUsername = message.Username,
            ProviderAvatarUrl = message.AvatarUrl,

            CreatedAt = user.Message.Timestamp,
            UpdatedAt = user.Message.Timestamp
        };

        _dbContext.Logins.Add(login);
            
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserRegistered
        {
            UserId = login.UserId,
            Username = user.Message.Username
        };
    }

    public async Task Consume(ConsumeContext<LinkProvider> context)
    {
        var login = await _dbContext.Logins
            .Where(x => x.Type == context.Message.Provider)
            .Where(x => x.ProviderAccountId == context.Message.AccountId)
            .Where(x => !x.User!.DeletedAt.HasValue)
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