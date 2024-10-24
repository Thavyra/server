using System.Text.RegularExpressions;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.User;
using Thavyra.Contracts.User.Register;

namespace Thavyra.Data.Consumers;

public partial class RegisterConsumer :
    IConsumer<User_Register>,
    IConsumer<User_RegisterWithDiscord>,
    IConsumer<User_RegisterWithGitHub>
{
    private readonly IRequestClient<User_ExistsByUsername> _usernameExists;
    private readonly IRequestClient<User_GetById> _getUser;
    private readonly IRequestClient<User_Create> _createUser;
    private readonly IRequestClient<PasswordLogin_Create> _createPassword;
    private readonly IRequestClient<DiscordLogin_GetByDiscordId> _getDiscord;
    private readonly IRequestClient<DiscordLogin_Create> _createDiscord;
    private readonly IRequestClient<GitHubLogin_GetByGitHubId> _getGitHub;
    private readonly IRequestClient<GitHubLogin_Create> _createGitHub;

    public RegisterConsumer(
        IRequestClient<User_ExistsByUsername> usernameExists,
        IRequestClient<User_GetById> getUser,
        IRequestClient<User_Create> createUser,
        IRequestClient<PasswordLogin_Create> createPassword,
        IRequestClient<DiscordLogin_GetByDiscordId> getDiscord,
        IRequestClient<DiscordLogin_Create> createDiscord,
        IRequestClient<GitHubLogin_GetByGitHubId> getGitHub,
        IRequestClient<GitHubLogin_Create> createGitHub)
    {
        _usernameExists = usernameExists;
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
        }, context.CancellationToken);

        var passwordResponse = await _createPassword.GetResponse<PasswordLogin>(new PasswordLogin_Create
        {
            UserId = userResponse.Message.Id,
            Password = context.Message.Password
        }, context.CancellationToken);

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
        }, context.CancellationToken);

        var (login, userResponse) = existsResponse switch
        {
            (_, NotFound) => (null, await _createUser.GetResponse<User>(new User_Create
            {
                Username = await ThavyrafyUsername(context.Message.Username)
            }, context.CancellationToken)),

            (_, DiscordLogin l) => (l, await _getUser.GetResponse<User>(new User_GetById
            {
                Id = l.UserId
            }, context.CancellationToken)),

            _ => throw new InvalidOperationException()
        };

        if (login is null)
        {
            var loginResponse = await _createDiscord.GetResponse<DiscordLogin>(new DiscordLogin_Create
            {
                UserId = userResponse.Message.Id,
                DiscordId = context.Message.DiscordId,
                DiscordUsername = context.Message.Username
            }, context.CancellationToken);

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
        }, context.CancellationToken);

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
                Username = await ThavyrafyUsername(context.Message.Username)
            }, context.CancellationToken),

            not null => await _getUser.GetResponse<User>(new User_GetById
            {
                Id = login.UserId
            }, context.CancellationToken)
        };

        if (login is null)
        {
            var loginResponse = await _createGitHub.GetResponse<GitHubLogin>(new GitHubLogin_Create
            {
                UserId = userResponse.Message.Id,
                GitHubId = context.Message.GitHubId,
                GitHubUsername = context.Message.Username
            }, context.CancellationToken);

            login = loginResponse.Message;
        }

        await context.RespondAsync(new GitHubRegistration
        {
            User = userResponse.Message,
            Login = login
        });
    }

    /// <summary>
    /// Strips invalid characters from the specified username, and adds random digits if the result is already in use.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    private async Task<string> ThavyrafyUsername(string username)
    {
        string originalUsername = UsernameRegex().Replace(username, "");

        string newUsername = originalUsername;
        int attempts = 0;
        while (await _usernameExists.GetResponse<UsernameExists, NotFound>(
                   new User_ExistsByUsername { Username = newUsername }) is { Message: UsernameExists })
        {
            newUsername = originalUsername + Random.Shared.Next(1, 999);

            attempts++;

            if (attempts > 200)
            {
                throw new Exception("Too many attempts to Thavyrafy username.");
            }
        }

        return newUsername;
    }

    [GeneratedRegex(@"[^a-zA-Z0-9_\-\.]")]
    private static partial Regex UsernameRegex();
}