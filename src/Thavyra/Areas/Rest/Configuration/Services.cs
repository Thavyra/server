using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;
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
        
        services.AddScoped<IAuthorizationHandler, Security.Resource.User.SameSubjectHandler>();

        services.AddScoped<IAuthorizationHandler, Security.Resource.Login.SubjectHandler>();
        
        services.AddScoped<IAuthorizationHandler, Security.Resource.Application.OwnerHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Application.OwnerCreateHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Application.OwnerCollectHandler>();

        services.AddScoped<IAuthorizationHandler, Security.Resource.Authorization.SubjectHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Authorization.SubjectCollectHandler>();

        services.AddScoped<IAuthorizationHandler, Security.Resource.Transaction.OwnerCollectHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Transaction.OwnerReadHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Transaction.SubjectSendHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Transaction.SubjectTransferHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Transaction.SubjectOrRecipientCollectHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Transaction.SubjectOrRecipientReadHandler>();

        services.AddScoped<IAuthorizationHandler, Security.Resource.Objective.ClientCollectHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Objective.ClientReadHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Objective.OwnerCollectHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Objective.OwnerCreateHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Objective.OwnerHandler>();

        services.AddScoped<IAuthorizationHandler, Security.Resource.Score.ClientAndSubjectCreateHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Score.ClientAndSubjectDeleteHandler>();
        services.AddScoped<IAuthorizationHandler, Security.Resource.Score.ClientReadHandler>();

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
                options.Endpoints.RoutePrefix = "api";
                options.Endpoints.Configurator = ep =>
                {
                    ep.PreProcessor<UserSlugParser>(Order.Before);
                    ep.PreProcessor<ApplicationSlugParser>(Order.Before);
                    ep.AuthSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
                };
            
                options.Errors.UseProblemDetails();

                options.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.Serializer.Options.Converters.Add(new OptionalConverterFactory());
                options.Serializer.Options.Converters.Add(new NullableConverterFactory());
        });
    }
}