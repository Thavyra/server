using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Role;
using Thavyra.Contracts.User;
using Thavyra.Contracts.User.Register;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Managers;

public class UserManager : IUserManager
{
    private readonly IScopedClientFactory _clientFactory;

    public UserManager(IScopedClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
    
    public async Task<UserModel> RegisterAsync(PasswordRegisterModel login, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<User_Register>();
        
        var response = await client.GetResponse<PasswordRegistration>(new User_Register
        {
            Username = login.Username,
            Password = login.Password
        }, cancellationToken);

        var user = response.Message.User;

        return new UserModel
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserModel?> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            return null;
        }
        
        var client = _clientFactory.CreateRequestClient<User_GetById>();

        Response response = await client.GetResponse<User, NotFound>(new User_GetById
        {
            Id = userId
        }, cancellationToken);

        return response switch
        {
            (_, User user) => new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAt = user.CreatedAt
            },
            (_, NotFound) => null,
            _ => throw new InvalidOperationException()
        };
    }

    public async Task<UserModel?> FindByLoginAsync(PasswordLoginModel login, CancellationToken cancellationToken)
    {
        var userClient = _clientFactory.CreateRequestClient<User_GetByUsername>();

        var userResponse = await userClient.GetResponse<User, NotFound>(new User_GetByUsername
        {
            Username = login.Username
        }, cancellationToken);

        if (! userResponse.Is(out Response<User>? user))
        {
            return null;
        }

        var loginClient = _clientFactory.CreateRequestClient<PasswordLogin_Check>();

        Response loginResponse = await loginClient.GetResponse<Correct, Incorrect>(new PasswordLogin_Check
        {
            UserId = user.Message.Id,
            Password = login.Password
        }, cancellationToken);

        return loginResponse switch
        {
            (_, Correct) => new UserModel
            {
                Id = user.Message.Id,
                Username = user.Message.Username,
                CreatedAt = user.Message.CreatedAt
            },
            (_, Incorrect or NotFound) => null,
            _ => throw new InvalidOperationException()
        };
    }

    public async Task<UserModel> RegisterWithDiscordAsync(DiscordLoginModel login, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<User_RegisterWithDiscord>();

        var response = await client.GetResponse<DiscordRegistration>(new User_RegisterWithDiscord
        {
            DiscordId = login.Id,
            Username = login.Username
        }, cancellationToken);

        var user = response.Message.User;

        return new UserModel
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserModel> RegisterWithGitHubAsync(GitHubLoginModel login, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<User_RegisterWithGitHub>();

        var response = await client.GetResponse<GitHubRegistration>(new User_RegisterWithGitHub
        {
            GitHubId = login.Id,
            Username = login.Username
        }, cancellationToken);
        
        var user = response.Message.User;

        return new UserModel
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<User_ExistsByUsername>();

        Response response = await client.GetResponse<UsernameExists, NotFound>(new User_ExistsByUsername
        {
            Username = username
        }, cancellationToken);

        return response switch
        {
            (_, UsernameExists) => false, // Username is not unique
            (_, NotFound) => true, // Username is unique
            _ => throw new InvalidOperationException()
        };
    }

    public async Task<IReadOnlyList<RoleModel>> GetRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Role_GetByUser>();

        var response = await client.GetResponse<Multiple<Role>>(new Role_GetByUser
        {
            UserId = userId
        }, cancellationToken);

        return response.Message.Items.Select(x => new RoleModel
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
    }
}