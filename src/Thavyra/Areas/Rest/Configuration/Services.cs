using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security.Resource.User;

namespace Thavyra.Rest.Configuration;

public static class Services
{
    public static IServiceCollection AddRestApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SameSubjectHandler>();
        
        services.AddFastEndpoints();
        
        return services;
    }

    public static IApplicationBuilder UseRestApi(this WebApplication app)
    {
        return app
            .UseDefaultExceptionHandler(useGenericReason: !app.Environment.IsDevelopment())
            .UseFastEndpoints(options =>
        {
            options.Errors.UseProblemDetails();

            options.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            options.Serializer.Options.Converters.Add(new OptionalConverterFactory());
            options.Serializer.Options.Converters.Add(new NullableConverterFactory());
        });
    }
}