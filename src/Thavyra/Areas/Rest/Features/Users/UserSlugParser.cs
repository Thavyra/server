using FastEndpoints;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.User;

namespace Thavyra.Rest.Features.Users;

public class UserSlugParser : GlobalPreProcessor<AuthenticationState>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public UserSlugParser(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task PreProcessAsync(IPreProcessorContext context, AuthenticationState authenticationState, CancellationToken ct)
    {
        if (context.Request is not UserRequest request)
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();

        var idClient = scope.Resolve<IRequestClient<User_GetById>>();
        var usernameClient = scope.Resolve<IRequestClient<User_GetByUsername>>();

        Response? userResponse = null;

        switch (request.User)
        {
            case { } guid when Guid.TryParse(guid, out var id):
                userResponse = await idClient.GetResponse<User, NotFound>(new User_GetById
                {
                    Id = id
                }, ct);
                
                break;
            
            case "@me":
                if (request.Subject == default)
                {
                    await context.HttpContext.Response.SendUnauthorizedAsync(ct);
                    return;
                }
            
                userResponse = await idClient.GetResponse<User, NotFound>(new User_GetById
                {
                    Id = request.Subject
                }, ct);
                
                break;
            
            case { } username when username.StartsWith('@'):
                userResponse = await usernameClient.GetResponse<User, NotFound>(new User_GetByUsername
                {
                    Username = request.User[1..]
                }, ct);
                
                break;
        }
        
        switch (userResponse)
        {
            case (_, NotFound):
                await context.HttpContext.Response.SendNotFoundAsync(cancellation: ct);
                return;
            case (_, User user):
                authenticationState.User = user;
                break;
        }
    }
}