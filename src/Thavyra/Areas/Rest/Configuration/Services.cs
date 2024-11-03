using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using OpenIddict.Validation.AspNetCore;
using Thavyra.Rest.Features.Applications;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Processors;
using Thavyra.Rest.Security;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Configuration;

public static class Services
{
    public static IServiceCollection AddRestApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorizationHandlers();

        services.AddTransient<IUserService, UserService>();
        
        services.AddFastEndpoints();

        foreach (var section in configuration.GetChildren())
            switch (section.Key)
            {
                case "DiceBear":
                    services.Configure<DiceBearOptions>(section);
                    services.AddHttpClient<IIconService, DiceBearIconService>();
                    break;
            }
        
        return services;
    }

    public static IApplicationBuilder UseRestApi(this WebApplication app)
    {
        return app
            .UseRestExceptionHandler(useGenericReason: !app.Environment.IsDevelopment())
            .UseFastEndpoints(options =>
            {
                options.Endpoints.RoutePrefix = "api";
                options.Endpoints.Configurator = ep =>
                {
                    ep.PreProcessor<UserSlugParser>(Order.Before);
                    ep.PreProcessor<ApplicationSlugParser>(Order.Before);
                    ep.AuthSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
                    ep.PostProcessor<EndpointResponseLogger>(Order.After);
                };
            
                options.Errors.UseProblemDetails();

                options.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.Serializer.Options.Converters.Add(new OptionalConverterFactory());
                options.Serializer.Options.Converters.Add(new NullableConverterFactory());
        });
    }
}