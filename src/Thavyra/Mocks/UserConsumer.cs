using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.User;

namespace Thavyra.Mocks;

public class UserConsumer :
    IConsumer<User_Create>,
    IConsumer<User_ExistsByUsername>,
    IConsumer<User_GetById>,
    IConsumer<User_GetByUsername>,
    IConsumer<PasswordLogin_Check>,
    IConsumer<PasswordLogin_Create>
{
    
    public async Task Consume(ConsumeContext<User_Create> context)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = context.Message.Username,
            Description = null,
            CreatedAt = DateTime.UtcNow
        };
        
        Repository.Users.List.Add(user.Id.ToString(), user);

        await context.RespondAsync(user);
    }

    public async Task Consume(ConsumeContext<User_ExistsByUsername> context)
    {
        if (Repository.Users.List.Values.Any(x => x.Username == context.Message.Username))
        {
            await context.RespondAsync(new UsernameExists());
        }
        else
        {
            await context.RespondAsync(new NotFound());
        }
    }

    public async Task Consume(ConsumeContext<User_GetById> context)
    {
        if (Repository.Users.List.TryGetValue(context.Message.Id, out var user))
        {
            await context.RespondAsync(user);
        }
        else
        {
            await context.RespondAsync(new NotFound());
        }
    }

    public async Task Consume(ConsumeContext<User_GetByUsername> context)
    {
        if (Repository.Users.List.Values.FirstOrDefault(x => x.Username == context.Message.Username) is { } user)
        {
            await context.RespondAsync(user);
        }
        else
        {
            await context.RespondAsync(new NotFound());
        }
    }

    public async Task Consume(ConsumeContext<PasswordLogin_Check> context)
    {
        if (Repository.Users.Passwords[context.Message.UserId] == context.Message.Password)
        {
            await context.RespondAsync(new PasswordCorrect());
        }
        else
        {
            await context.RespondAsync(new PasswordIncorrect());
        }
    }

    public async Task Consume(ConsumeContext<PasswordLogin_Create> context)
    {
        Repository.Users.Passwords.Add(context.Message.UserId, context.Message.Password);
    }
}