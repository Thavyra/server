using FastEndpoints;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;

namespace Thavyra.Rest.Features.Applications;

public class ApplicationQueryProcessor : GlobalPreProcessor<AuthenticationState>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public ApplicationQueryProcessor(IServiceScopeFactory scopeFactory)
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

        switch (request.Application)
        {
            case ApplicationIdQuery q:
                applicationResponse = await client.GetResponse<Application, NotFound>(new Application_GetById
                {
                    Id = q.ApplicationId
                }, ct);
                
                break;
            
            case SelfQuery:
                if (request.ApplicationId == default)
                {
                    await context.HttpContext.Response.SendUnauthorizedAsync(ct);
                    return;
                }

                applicationResponse = await client.GetResponse<Application, NotFound>(new Application_GetById
                {
                    Id = request.ApplicationId
                }, ct);
                
                break;
        }
        
        switch (applicationResponse)
        {
            case null or (_, NotFound):
                await context.HttpContext.Response.SendNotFoundAsync(cancellation: ct);
                return;
            case (_, Application application):
                authenticationState.Application = application;
                break;
        }
    }
}