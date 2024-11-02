using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts.Login.Data;
using Thavyra.Data.Contexts;

namespace Thavyra.Data.Consumers.Login;

public class LoginDataConsumer :
    IConsumer<GetUserLogins>
{
    private readonly ThavyraDbContext _dbContext;

    public LoginDataConsumer(ThavyraDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<GetUserLogins> context)
    {
        var logins = await _dbContext.Logins
            .Where(x => x.UserId == context.Message.UserId)
            .Include(x => x.Attempts)
            .ToListAsync(context.CancellationToken);
        
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
                UsedAt = login.Attempts.FirstOrDefault()?.CreatedAt ?? login.CreatedAt,
            }).ToList()
        });
    }
}