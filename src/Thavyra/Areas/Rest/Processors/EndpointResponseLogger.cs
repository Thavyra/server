using FastEndpoints;

namespace Thavyra.Rest.Processors;

public class EndpointResponseLogger : IGlobalPostProcessor
{
    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        var logger = context.HttpContext.Resolve<ILogger<EndpointResponseLogger>>();
        
        logger.LogInformation("{Method} {Path} {StatusCode}: {Response}", 
            context.HttpContext.Request.Method,
            context.HttpContext.Request.PathBase + context.HttpContext.Request.Path,
            context.HttpContext.Response.StatusCode, 
            context.Response);

        if (context.HasValidationFailures)
        {
            logger.LogWarning("{Count} validation failures", context.ValidationFailures.Count);

            foreach (var failure in context.ValidationFailures)
            {
                logger.LogWarning("{PropertyName}: {ErrorMessage}", failure.PropertyName, failure.ErrorMessage);
            }
        }

        if (context.HasExceptionOccurred)
        {
            logger.LogCritical("Exception Occurred: {Exception}", context.ExceptionDispatchInfo.SourceException.Message);
        }

        return Task.CompletedTask;
    }
}