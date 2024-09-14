using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Features.Applications;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security.Scopes;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Configuration;

public static class Services
{
    public static IServiceCollection AddRestApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthorizationHandler, ScopeAuthorizationHandler>();
        
        services.AddSingleton<IAuthorizationHandler, Security.Resource.User.SameSubjectHandler>();
        
        services.AddSingleton<IAuthorizationHandler, Security.Resource.Application.OwnerHandler>();
        services.AddSingleton<IAuthorizationHandler, Security.Resource.Application.OwnerCreateHandler>();
        services.AddSingleton<IAuthorizationHandler, Security.Resource.Application.OwnerCollectHandler>();

        services.AddSingleton<IAuthorizationHandler, Security.Resource.Transaction.OwnerCollectHandler>();
        services.AddSingleton<IAuthorizationHandler, Security.Resource.Transaction.OwnerReadHandler>();
        services.AddSingleton<IAuthorizationHandler, Security.Resource.Transaction.SubjectSendHandler>();
        services.AddSingleton<IAuthorizationHandler, Security.Resource.Transaction.SubjectTransferHandler>();
        services.AddSingleton<IAuthorizationHandler, Security.Resource.Transaction.SubjectOrRecipientCollectHandler>();
        services.AddSingleton<IAuthorizationHandler, Security.Resource.Transaction.SubjectOrRecipientReadHandler>();

        services.AddTransient<IUserService, UserService>();
        
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