using MassTransit;
using Microsoft.Extensions.Options;
using Thavyra.Contracts.Application;
using Thavyra.Data.Configuration;

namespace Thavyra.Data.Consumers.Application;

public class ApplicationCreatedConsumer : IConsumer<ApplicationCreated>
{
    private readonly SystemOptions _options;

    public ApplicationCreatedConsumer(IOptions<SystemOptions> options)
    {
        _options = options.Value;
    }
    
    public async Task Consume(ConsumeContext<ApplicationCreated> context)
    {
        if (_options.DefaultPermissions.Count == 0)
        {
            return;
        }

        await context.Publish(new Application_ModifyPermissions
        {
            ApplicationId = context.Message.Application.Id,
            Grant = _options.DefaultPermissions,
            Deny = []
        });
    }
}