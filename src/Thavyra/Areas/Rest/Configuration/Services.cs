using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Configuration;

public static class Services
{
    public static IServiceCollection AddRestApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFastEndpoints();
        
        return services;
    }

    public static IApplicationBuilder UseRestApi(this IApplicationBuilder app)
    {
        return app.UseFastEndpoints(options =>
        {
            options.Errors.UseProblemDetails();

            options.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            options.Serializer.Options.Converters.Add(new OptionalConverterFactory());
            options.Serializer.Options.Converters.Add(new NullableConverterFactory());
        });
    }
}