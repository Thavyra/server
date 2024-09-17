using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.User;
using Thavyra.Contracts.User.Register;

namespace Thavyra.Data.Consumers;

public class RegisterConsumer : 
    IConsumer<User_Register>,
    IConsumer<User_RegisterWithDiscord>,
    IConsumer<User_RegisterWithGitHub>
{
    private readonly IRequestClient<User_GetById> _getUser;
    private readonly IRequestClient<User_Create> _createUser;
    private readonly IRequestClient<PasswordLogin_Create> _createPassword;
    private readonly IRequestClient<DiscordLogin_GetByDiscordId> _getDiscord;
    private readonly IRequestClient<DiscordLogin_Create> _createDiscord;
    private readonly IRequestClient<GitHubLogin_GetByGitHubId> _getGitHub;
    private readonly IRequestClient<GitHubLogin_Create> _createGitHub;

    public RegisterConsumer(
        IRequestClient<User_GetById> getUser,
        IRequestClient<User_Create> createUser,
        IRequestClient<PasswordLogin_Create> createPassword,
        IRequestClient<DiscordLogin_GetByDiscordId> getDiscord,
        IRequestClient<DiscordLogin_Create> createDiscord,
        IRequestClient<GitHubLogin_GetByGitHubId> getGitHub,
        IRequestClient<GitHubLogin_Create> createGitHub)
    {
        _getUser = getUser;
        _createUser = createUser;
        _createPassword = createPassword;
        _getDiscord = getDiscord;
        _createDiscord = createDiscord;
        _getGitHub = getGitHub;
        _createGitHub = createGitHub;
    }
    
    public async Task Consume(ConsumeContext<User_Register> context)
    {
        var userResponse = await _createUser.GetResponse<User>(new User_Create
        {
            Username = context.Message.Username
        });

        var passwordResponse = await _createPassword.GetResponse<PasswordLogin>(new PasswordLogin_Create
        {
            UserId = userResponse.Message.Id,
            Password = context.Message.Password
        });

        await context.RespondAsync(new PasswordRegistration
        {
            User = userResponse.Message,
            Login = passwordResponse.Message
        });
    }

    public async Task Consume(ConsumeContext<User_RegisterWithDiscord> context)
    {
        Response existsResponse = await _getDiscord.GetResponse<DiscordLogin, NotFound>(new DiscordLogin_GetByDiscordId
        {
            DiscordId = context.Message.DiscordId
        });

        var login = existsResponse switch
        {
            (_, NotFound) => null,
            (_, DiscordLogin l) => l,
            _ => throw new InvalidOperationException()
        };

        var userResponse = login switch
        {
            null => await _createUser.GetResponse<User>(new User_Create
            {
                Username = context.Message.Username
            }),

            not null => await _getUser.GetResponse<User>(new User_GetById
            {
                Id = login.UserId
            })
        };

        if (login is null)
        {
            var loginResponse = await _createDiscord.GetResponse<DiscordLogin>(new DiscordLogin_Create
            {
                UserId = userResponse.Message.Id,
                DiscordId = context.Message.DiscordId
            });

            login = loginResponse.Message;
        }

        await context.RespondAsync(new DiscordRegistration
        {
            User = userResponse.Message,
            Login = login
        });
    }

    public async Task Consume(ConsumeContext<User_RegisterWithGitHub> context)
    {
        Response existsResponse = await _getGitHub.GetResponse<GitHubLogin, NotFound>(new GitHubLogin_GetByGitHubId
        {
            GitHubId = context.Message.GitHubId
        });

        var login = existsResponse switch
        {
            (_, NotFound) => null,
            (_, GitHubLogin l) => l,
            _ => throw new InvalidOperationException()
        };

        var userResponse = login switch
        {
            null => await _createUser.GetResponse<User>(new User_Create
            {
                Username = context.Message.Username
            }),

            not null => await _getUser.GetResponse<User>(new User_GetById
            {
                Id = login.UserId
            })
        };

        if (login is null)
        {
            var loginResponse = await _createGitHub.GetResponse<GitHubLogin>(new GitHubLogin_Create
            {
                UserId = userResponse.Message.Id,
                GitHubId = context.Message.GitHubId
            });

            login = loginResponse.Message;
        }

        await context.RespondAsync(new GitHubRegistration
        {
            User = userResponse.Message,
            Login = login
        });
    }
}