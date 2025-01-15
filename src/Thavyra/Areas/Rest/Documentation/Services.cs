using FastEndpoints.Swagger;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using NSwag;
using Thavyra.Rest.Features.Applications;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Documentation;

public static class Services
{
    public static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        return services.SwaggerDocument(options =>
        {
            options.DocumentSettings = document =>
            {
                document.Title = "Thavyra";
                document.Version = "v1";

                document.AddAuth("OpenIdConnect", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = "/.well-known/openid-configuration",
                });

                document.SchemaSettings.ConfigureSchema();
            };

            options.ExcludeNonFastEndpoints = true;
            options.RemoveEmptyRequestSchema = true;
        });
    }

    public static void ConfigureSchema(this JsonSchemaGeneratorSettings settings)
    {
        settings.SchemaNameGenerator = new AttributeSchemaNameGenerator(settings.SchemaNameGenerator);
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(UserQuery),
            schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.Format = "user";
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(ApplicationQuery),
            schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.Format = "application";
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<string>),
            schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.Title = "optional";
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<string?>),
            schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.Title = "optional";
                schema.IsNullable(SchemaType.OpenApi3);
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<double>),
            schema =>
            {
                schema.Type = JsonObjectType.Number;
                schema.Format = JsonFormatStrings.Double;
                schema.Title = "optional";
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<double?>),
            schema =>
            {
                schema.Type = JsonObjectType.Number;
                schema.Format = JsonFormatStrings.Double;
                schema.Title = "optional";
                schema.IsNullable(SchemaType.OpenApi3);
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<Guid>),
            schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.Format = JsonFormatStrings.Guid;
                schema.Title = "optional";
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<Guid?>),
            schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.Format = JsonFormatStrings.Guid;
                schema.Title = "optional";
                schema.IsNullable(SchemaType.OpenApi3);
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<bool>),
            schema =>
            {
                schema.Type = JsonObjectType.Boolean;
                schema.Title = "optional";
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<bool?>),
            schema =>
            {
                schema.Type = JsonObjectType.Boolean;
                schema.Title = "optional";
                schema.IsNullable(SchemaType.OpenApi3);
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<DateTime>),
            schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.Format = JsonFormatStrings.DateTime;
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonOptional<DateTime?>),
            schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.Format = JsonFormatStrings.DateTime;
                schema.IsNullable(SchemaType.OpenApi3);
            }));
        
        settings.TypeMappers.Add(new PrimitiveTypeMapper(
            typeof(JsonNullable<string>),
            schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.IsNullable(SchemaType.OpenApi3);
            }));
    }
}