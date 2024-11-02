using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Role;
using Thavyra.Contracts.Transaction;
using Thavyra.Contracts.User;
using Thavyra.Data.Configuration;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers.User;

public class UserCreatedConsumer :
    IConsumer<UserCreated>
{
    private readonly ThavyraDbContext _dbContext;
    private readonly IRequestClient<Application_Create> _createApplication;
    private readonly UserOptions _options;

    public UserCreatedConsumer(
        ThavyraDbContext dbContext,
        IRequestClient<Application_Create> createApplication,
        IOptions<UserOptions> options)
    {
        _dbContext = dbContext;
        _createApplication = createApplication;
        _options = options.Value;
    }
    
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var system = await _dbContext.System.FirstOrDefaultAsync(context.CancellationToken);

        if (await _dbContext.Users.CountAsync(context.CancellationToken) == 1)
        {
            var role = await _dbContext.Roles
                .FirstOrDefaultAsync(x => x.Name == Constants.Roles.Admin, context.CancellationToken);

            if (role is not null)
            {
                await context.Publish(new User_GrantRole
                {
                    UserId = context.Message.UserId,
                    RoleId = role.Id
                });
            }
        }
        
        if (system is null)
        {
            var response = await _createApplication.GetResponse<ApplicationCreated>(new Application_Create
            {
                OwnerId = context.Message.UserId,
                Type = Constants.ApplicationTypes.Service,
                Name = "System",
                Description = null
            }, context.CancellationToken);

            system = new SystemDto
            {
                ApplicationId = response.Message.Id
            };
            
            _dbContext.System.Add(system);

            await _dbContext.SaveChangesAsync(context.CancellationToken);
        }

        await context.Publish(new Transaction_Create
        {
            ApplicationId = system.ApplicationId,
            SubjectId = context.Message.UserId,
            Description = _options.WelcomeTransaction.Message,
            Amount = _options.WelcomeTransaction.Amount ?? Math.PI
        });
    }
}