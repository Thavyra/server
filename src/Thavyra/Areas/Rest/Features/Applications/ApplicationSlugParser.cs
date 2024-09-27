using FastEndpoints;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;

namespace Thavyra.Rest.Features.Applications;

public class ApplicationSlugParser : GlobalPreProcessor<AuthenticationState>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public ApplicationSlugParser(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    public override async Task PreProcessAsync(IPreProcessorContext context, AuthenticationState authenticationState, CancellationToken ct)
    {
        if (context.Request is not ApplicationRequest request)
        {
            return;
        }
        
        using var scope = _scopeFactory.CreateScope();

        var client = scope.Resolve<IRequestClient<Application_GetById>>();
        
        Response? applicationResponse = null;

        if (Guid.TryParse(request.Application, out var id))
        {
            applicationResponse = await client.GetResponse<Application, NotFound>(new Application_GetById
            {
                Id = id
            }, ct);
        }
        
        if (request.Application == "@me")
        {
            if (request.ApplicationId == default)
            {
                await context.HttpContext.Response.SendUnauthorizedAsync(ct);
                return;
            }
            
            applicationResponse = await client.GetResponse<Application, NotFound>(new Application_GetById
            {
                Id = request.ApplicationId
            }, ct);
        }
        
        switch (applicationResponse)
        {
            case (_, NotFound):
                await context.HttpContext.Response.SendNotFoundAsync(cancellation: ct);
                return;
            case (_, Application application):
                authenticationState.Application = application;
                break;
        }
    }
}