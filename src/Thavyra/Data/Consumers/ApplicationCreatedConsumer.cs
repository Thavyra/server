using MassTransit;
using Microsoft.Extensions.Options;
using Thavyra.Contracts.Application;
using Thavyra.Data.Configuration;

namespace Thavyra.Data.Consumers;

public class ApplicationCreatedConsumer : IConsumer<ApplicationCreated>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly SystemOptions _options;

    public ApplicationCreatedConsumer(IOptions<SystemOptions> options, IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
        _options = options.Value;
    }
    
    public async Task Consume(ConsumeContext<ApplicationCreated> context)
    {
        if (_options.DefaultPermissions.Count == 0)
        {
            return;
        }

        await _publishEndpoint.Publish(new Application_ModifyPermissions
        {
            ApplicationId = context.Message.Application.Id,
            Grant = _options.DefaultPermissions,
            Deny = []
        }, context.CancellationToken);
    }
}