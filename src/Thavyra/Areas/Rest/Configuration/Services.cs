using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;
using NSwag;
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

        services.AddFastEndpoints()
            .SwaggerDocument(o =>
            {
                o.DocumentSettings = s =>
                {
                    s.Title = "Thavyra";
                    s.Version = "v1";
                    s.AddAuth("OpenIdConnect", new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OpenIdConnect,
                        OpenIdConnectUrl = "/.well-known/openid-configuration",
                    });
                    s.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(
                        typeof(UserQuery),
                        schema =>
                        {
                            schema.Type = JsonObjectType.String;
                            schema.Format = "guid or '@me' or '@{username}'";
                        }));
                    s.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(
                        typeof(ApplicationQuery),
                        schema =>
                        {
                            schema.Type = JsonObjectType.String;
                            schema.Format = "guid or '@me'";
                        }));
                    s.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(
                        typeof(JsonOptional<string>),
                        schema =>
                        {
                            schema.Type = JsonObjectType.String;
                            schema.Title = "optional";
                        }));
                    s.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(
                        typeof(JsonOptional<string?>),
                        schema =>
                        {
                            schema.Type = JsonObjectType.String | JsonObjectType.Null;
                            schema.Title = "optional";
                        }));
                    s.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(
                        typeof(JsonOptional<double>),
                        schema =>
                        {
                            schema.Type = JsonObjectType.Number;
                            schema.Title = "optional";
                        }));
                    s.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(
                        typeof(JsonOptional<Guid>),
                        schema =>
                        {
                            schema.Type = JsonObjectType.String;
                            schema.Format = "guid";
                            schema.Title = "optional";
                        }));
                    s.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(
                        typeof(JsonOptional<bool>),
                        schema =>
                        {
                            schema.Type = JsonObjectType.Boolean;
                            schema.Title = "optional";
                        }));
                    s.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(
                        typeof(JsonNullable<string>),
                        schema =>
                        {
                            schema.Type = JsonObjectType.String | JsonObjectType.Null;
                        }));
                };
                o.ExcludeNonFastEndpoints = true;
            });

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
                    ep.PreProcessor<UserQueryProcessor>(Order.Before);
                    ep.PreProcessor<ApplicationQueryProcessor>(Order.Before);
                    ep.AuthSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
                    ep.PostProcessor<EndpointResponseLogger>(Order.After);
                };

                options.Errors.UseProblemDetails();

                options.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.Serializer.Options.Converters.Add(new OptionalConverterFactory());
                options.Serializer.Options.Converters.Add(new NullableConverterFactory());
            })
            .UseOpenApi()
            .UseReDoc(options =>
            {
                options.Path = "/docs/rest";
            });
    }
}