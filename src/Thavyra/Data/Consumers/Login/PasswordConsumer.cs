using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Data;
using Thavyra.Contracts.Login.Password;
using Thavyra.Contracts.User;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;
using Thavyra.Data.Security.Hashing;

namespace Thavyra.Data.Consumers.Login;

public class PasswordConsumer :
    IConsumer<RegisterUser>,
    IConsumer<PasswordLogin>,
    IConsumer<ChangePassword>
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
            .Where(x => x.User!.Username == context.Message.Username && !x.User.DeletedAt.HasValue)
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

        if (hashResult.Succeeded)
        {
            var message = new LoginSucceeded
            {
                UserId = login.UserId,
                Username = context.Message.Username
            };

            await context.Publish(message);
            await context.RespondAsync(message);

            if (hashResult.Rehash is { } rehash)
            {
                login.PasswordHash = rehash;
            }

            var attempt = new LoginAttemptDto
            {
                LoginId = login.Id,
                Succeeded = true,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.LoginAttempts.Add(attempt);
        }
        else
        {
            var message = new LoginFailed
            {
                LoginId = login.Id,
                Timestamp = DateTime.UtcNow
            };

            await context.Publish(message);
            await context.RespondAsync(message);

            var attempt = new LoginAttemptDto
            {
                LoginId = login.Id,
                Succeeded = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.LoginAttempts.Add(attempt);
        }

        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<ChangePassword> context)
    {
        var login = await _dbContext.Logins
            .Where(x => x.Type == Constants.LoginTypes.Password)
            .Where(x => x.UserId == context.Message.UserId && !x.User!.DeletedAt.HasValue)
            .FirstOrDefaultAsync(context.CancellationToken);

        var now = DateTime.UtcNow;

        switch (login)
        {
            case null:
                
                login = new LoginDto
                {
                    UserId = context.Message.UserId,
                    Type = Constants.LoginTypes.Password,
                    CreatedAt = now
                };

                _dbContext.Logins.Add(login);
                
                break;
            case { PasswordHash: not null } when context.Message.CurrentPassword is not null 
                && await _hashService.CheckAsync(
                    context.Message.CurrentPassword, login.PasswordHash) is { Succeeded: true }:

                break;
            default:
                
                if (context.IsResponseAccepted<LoginFailed>())
                {
                    await context.RespondAsync(new LoginFailed
                    {
                        LoginId = login.Id,
                        Timestamp = now
                    });

                    return;
                }

                throw new InvalidOperationException("Current password is incorrect.");
        }

        var hash = await _hashService.HashAsync(context.Message.Password);

        login.PasswordHash = hash;
        login.UpdatedAt = now;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new PasswordChanged
        {
            LoginId = login.Id,
            Timestamp = now
        });
    }
}