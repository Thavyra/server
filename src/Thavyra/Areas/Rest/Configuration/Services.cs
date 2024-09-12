using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using Thavyra.Rest.Features.Applications;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Configuration;

public static class Services
{
    public static IServiceCollection AddRestApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<Security.Resource.User.SameSubjectHandler>();
        
        services.AddSingleton<Security.Resource.Application.OwnerHandler>();
        services.AddSingleton<Security.Resource.Application.OwnerCreateHandler>();
        services.AddSingleton<Security.Resource.Application.OwnerCollectHandler>();

        services.AddSingleton<Security.Resource.Transaction.OwnerCollectHandler>();
        services.AddSingleton<Security.Resource.Transaction.OwnerReadHandler>();
        services.AddSingleton<Security.Resource.Transaction.SubjectCreateHandler>();
        services.AddSingleton<Security.Resource.Transaction.SubjectOrRecipientCollectHandler>();
        services.AddSingleton<Security.Resource.Transaction.SubjectOrRecipientReadHandler>();

        services.AddSingleton<IUserService, UserService>();
        
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