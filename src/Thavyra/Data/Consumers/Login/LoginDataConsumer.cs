using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Data;
using Thavyra.Data.Contexts;

namespace Thavyra.Data.Consumers.Login;

public class LoginDataConsumer :
    IConsumer<GetUserLogins>,
    IConsumer<GetLoginById>
{
    private readonly ThavyraDbContext _dbContext;

    public LoginDataConsumer(ThavyraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<GetUserLogins> context)
    {
        var query = _dbContext.Logins
            .Where(x => x.UserId == context.Message.UserId)
            .Where(x => !x.User.DeletedAt.HasValue);
        
        var attempts = await query
            .Select(x => x.Attempts
                .Where(a => a.Succeeded)
                .OrderByDescending(a => a.CreatedAt)
                .Take(1))
            .Where(x => x.Any())
            .Select(x => x.First())
            .ToListAsync(context.CancellationToken);

        var logins = await query.ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new UserLoginResult
        {
            Logins = logins.Select(login => new LoginResult
            {
                Id = login.Id,
                UserId = login.UserId,
                Type = login.Type,
                ProviderUsername = login.ProviderUsername,
                ProviderAccountId = login.ProviderAccountId,
                ProviderAvatarUrl = login.ProviderAvatarUrl,
                CreatedAt = login.CreatedAt,
                UpdatedAt = login.UpdatedAt,
                UsedAt = attempts.FirstOrDefault(x => x.LoginId == login.Id)?.CreatedAt ?? login.CreatedAt
            }).ToList()
        });
    }

    public async Task Consume(ConsumeContext<GetLoginById> context)
    {
        var login = await _dbContext.Logins
            .Where(x => x.Id == context.Message.LoginId)
            .Where(x => !x.User.DeletedAt.HasValue)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (login is null)
        {
            if (context.IsResponseAccepted<LoginNotFound>())
            {
                await context.RespondAsync(new LoginNotFound());
                return;
            }
            
            throw new InvalidOperationException("Login not found.");
        }

        var attempt = await _dbContext.LoginAttempts
            .Where(x => x.LoginId == login.Id)
            .Where(x => x.Succeeded)
            .FirstOrDefaultAsync(context.CancellationToken);

        await context.RespondAsync(new LoginResult
        {
            Id = login.Id,
            UserId = login.UserId,
            Type = login.Type,
            ProviderUsername = login.ProviderUsername,
            ProviderAccountId = login.ProviderAccountId,
            ProviderAvatarUrl = login.ProviderAvatarUrl,
            CreatedAt = login.CreatedAt,
            UpdatedAt = login.UpdatedAt,
            UsedAt = attempt?.CreatedAt ?? login.CreatedAt,
        });
    }
}