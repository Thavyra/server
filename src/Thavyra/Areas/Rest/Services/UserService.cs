using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.User;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Services;

public class UserService : IUserService
{
    private readonly IRequestClient<User_ExistsByUsername> _usernameClient;
    private readonly IRequestClient<User_GetById> _getById;
    private readonly IRequestClient<User_GetByUsername> _getByUsername;
    private readonly IAuthorizationService _authorizationService;

    public UserService(
        IRequestClient<User_ExistsByUsername> usernameClient, 
        IRequestClient<User_GetById> getById, 
        IRequestClient<User_GetByUsername> getByUsername,
        IAuthorizationService authorizationService)
    {
        _usernameClient = usernameClient;
        _getById = getById;
        _getByUsername = getByUsername;
        _authorizationService = authorizationService;
    }
    
    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken)
    {
        Response response = await _usernameClient.GetResponse<UsernameExists, NotFound>(new User_ExistsByUsername
        {
            Username = username
        }, cancellationToken);

        return response switch
        {
            (_, UsernameExists) => false,
            (_, NotFound) => true,
            _ => throw new InvalidOperationException()
        };
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        Response response = await _getById.GetResponse<User, NotFound>(new User_GetById
        {
            Id = id
        }, cancellationToken);

        return response switch
        {
            (_, User) => true,
            (_, NotFound) => false,
            _ => throw new InvalidOperationException()
        };
    }

    public async Task<UserResponse> GetResponseAsync(User user, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var response = new UserResponse
        {
            Id = user.Id.ToString(),
            Username = user.Username
        };
        
        var profileAuthorizationResult = await _authorizationService.AuthorizeAsync(httpContext.User, user, Policies.Operation.User.Read);

        if (profileAuthorizationResult.Succeeded)
        {
            response.Description = user.Description;
        }

        return response;
    }
}