using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Data;
using Thavyra.Data.Contexts;
using Thavyra.Data.Security.Hashing;

namespace Thavyra.Data.Consumers.Login;

public class ChangePasswordConsumer :
    IConsumer<ChangePassword>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IHashService _hashService;

    public ChangePasswordConsumer(ThavyraDbContext dbContext, IHashService hashService)
    {
        _dbContext = dbContext;
        _hashService = hashService;
    }
    
    public async Task Consume(ConsumeContext<ChangePassword> context)
    {
        var login = await _dbContext.Logins
            .Where(x => x.Type == Constants.LoginTypes.Password)
            .Where(x => x.UserId == context.Message.UserId)
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

        var hash = await _hashService.HashAsync(context.Message.Password);
        
        login.PasswordHash = hash;
        login.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new PasswordChanged
        {
            LoginId = login.Id,
            Timestamp = DateTime.UtcNow
        });
    }
}