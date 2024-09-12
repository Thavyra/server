using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using Thavyra.Rest.Features.Applications;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security.Resource.Application;
using Thavyra.Rest.Security.Resource.User;

namespace Thavyra.Rest.Configuration;

public static class Services
{
    public static IServiceCollection AddRestApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SameSubjectHandler>();
        
        services.AddSingleton<OwnerHandler>();
        services.AddSingleton<OwnerCreateHandler>();
        services.AddSingleton<OwnerCollectHandler>();
        
        services.AddFastEndpoints();
        
        return services;
    }

    public static IApplicationBuilder UseRestApi(this WebApplication app)
    {
        return app
            .UseDefaultExceptionHandler(useGenericReason: !app.Environment.IsDevelopment())
            .UseFastEndpoints(options =>
            {
                options.Endpoints.Configurator = ep =>
                {
                    ep.PreProcessor<UserSlugParser>(Order.Before);
                    ep.PreProcessor<ApplicationSlugParser>(Order.Before);
                };
            
            options.Errors.UseProblemDetails();

            options.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            options.Serializer.Options.Converters.Add(new OptionalConverterFactory());
            options.Serializer.Options.Converters.Add(new NullableConverterFactory());
        });
    }
}