using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.User;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Managers;

public class UserManager : IUserManager
{
    private readonly IScopedClientFactory _clientFactory;
    private readonly IPublishEndpoint _publishEndpoint;

    public UserManager(IScopedClientFactory clientFactory, IPublishEndpoint publishEndpoint)
    {
        _clientFactory = clientFactory;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task<UserModel> RegisterAsync(PasswordRegisterModel login, CancellationToken cancellationToken)
    {
        var userClient = _clientFactory.CreateRequestClient<User_Create>();

        var userResponse = await userClient.GetResponse<User>(new User_Create
        {
            Username = login.Username
        }, cancellationToken);

        var user = userResponse.Message;

        await _publishEndpoint.Publish(new PasswordLogin_Create
        {
            UserId = user.Id,
            Password = login.Password
        }, cancellationToken);

        return new UserModel
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserModel?> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<User_GetById>();

        Response response = await client.GetResponse<User, NotFound>(new User_GetById
        {
            Id = id
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
            _ => null
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
}