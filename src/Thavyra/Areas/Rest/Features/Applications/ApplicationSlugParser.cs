using FastEndpoints;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;

namespace Thavyra.Rest.Features.Applications;

public class ApplicationSlugParser : GlobalPreProcessor<ApplicationRequestState>
{
    private readonly IRequestClient<Application_GetById> _client;

    public ApplicationSlugParser(IRequestClient<Application_GetById> client)
    {
        _client = client;
    }
    
    public override async Task PreProcessAsync(IPreProcessorContext context, ApplicationRequestState requestState, CancellationToken ct)
    {
        if (context.Request is not ApplicationRequest request)
        {
            return;
        }
        
        Response? applicationResponse = null;

        if (Guid.TryParse(request.Application, out var id))
        {
            applicationResponse = await _client.GetResponse<Application, NotFound>(new Application_GetById
            {
                Id = id
            }, ct);
        }
        
        if (request.Application == "@me")
        {
            if (request.Subject == default)
            {
                await context.HttpContext.Response.SendUnauthorizedAsync(ct);
                return;
            }
            
            applicationResponse = await _client.GetResponse<Application, NotFound>(new Application_GetById
            {
                Id = request.Subject
            }, ct);
        }
        
        switch (applicationResponse)
        {
            case (_, NotFound):
                await context.HttpContext.Response.SendNotFoundAsync(cancellation: ct);
                return;
            case (_, Application application):
                requestState.Application = application;
                break;
        }
    }
}