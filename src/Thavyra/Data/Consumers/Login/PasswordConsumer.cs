using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Password;
using Thavyra.Contracts.User;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;
using Thavyra.Data.Security.Hashing;

namespace Thavyra.Data.Consumers.Login;

public class PasswordConsumer :
    IConsumer<RegisterUser>,
    IConsumer<PasswordLogin>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IRequestClient<CreateUser> _createUser;
    private readonly IHashService _hashService;

    public PasswordConsumer(
        ThavyraDbContext dbContext,
        IRequestClient<CreateUser> createUser,
        IHashService hashService)
    {
        _dbContext = dbContext;
        _createUser = createUser;
        _hashService = hashService;
    }

    public async Task Consume(ConsumeContext<RegisterUser> context)
    {
        var user = await _createUser.GetResponse<UserCreated>(new CreateUser
        {
            Username = context.Message.Username
        }, context.CancellationToken);

        var hash = await _hashService.HashAsync(context.Message.Password);
        
        var login = new LoginDto
        {
            UserId = user.Message.UserId,
            Type = Constants.LoginTypes.Password,

            PasswordHash = hash,

            CreatedAt = user.Message.Timestamp,
            UsedAt = user.Message.Timestamp,
            UpdatedAt = user.Message.Timestamp,
        };

        _dbContext.Logins.Add(login);

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new UserRegistered
        {
            UserId = user.Message.UserId,
            Username = context.Message.Username,
        });
    }

    public async Task Consume(ConsumeContext<PasswordLogin> context)
    {
        var login = await _dbContext.Logins
            .Where(x => x.User.Username == context.Message.Username)
            .Where(x => x.Type == Constants.LoginTypes.Password)
            .Include(x => x.User)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (login?.PasswordHash is null)
        {
            if (context.IsResponseAccepted<LoginNotFound>())
            {
                await context.RespondAsync(new LoginNotFound());
                return;
            }

            throw new InvalidOperationException("Login not found.");
        }

        var hashResult = await _hashService.CheckAsync(context.Message.Password, login.PasswordHash);

        if (!hashResult.Succeeded)
        {
            var message = new LoginFailed
            {
                LoginId = login.Id,
                Timestamp = DateTime.UtcNow
            };

            await context.Publish(message);
            
            if (context.IsResponseAccepted<LoginFailed>())
            {
                await context.RespondAsync(message);
                return;
            }

            throw new InvalidOperationException("Login failed.");
        }

        await context.RespondAsync(new LoginSucceeded
        {
            UserId = login.UserId,
            Username = context.Message.Username
        });
        
        if (hashResult.Rehash is { } rehash)
        {
            login.PasswordHash = rehash;
            await _dbContext.SaveChangesAsync(context.CancellationToken);
        }
    }
}