using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace Thavyra.Rest;

public static class ExceptionHandler
{
    public static IApplicationBuilder UseRestExceptionHandler(this IApplicationBuilder app,
        bool useGenericReason = false)
    {
        app.UseExceptionHandler(error =>
        {
            error.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();

                if (feature is null)
                {
                    return;
                }

                string reason = feature.Error.Message;

                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(new ServerErrorResponse
                {
                    Status = context.Response.StatusCode,
                    Reason = useGenericReason ? "An unexpected error has occurred." : reason,
                    Note = "See application log for stack trace."
                });
            });
        });

        return app;
    }
}

public class ServerErrorResponse
{
    public required int Status { get; set; }
    public required string Reason { get; set; }
    public required string Note { get; set; }
}