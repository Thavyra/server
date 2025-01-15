using System.Reflection;
using NJsonSchema.Generation;

namespace Thavyra.Rest.Documentation;

public class AttributeSchemaNameGenerator : ISchemaNameGenerator
{
    private readonly ISchemaNameGenerator _fallback;

    public AttributeSchemaNameGenerator(ISchemaNameGenerator fallback)
    {
        _fallback = fallback;
    }
    
    public string Generate(Type type)
    {
        if (type.GetCustomAttribute<SchemaNameAttribute>() is { } attribute)
        {
            return attribute.Name;
        }
        
        return _fallback.Generate(type);
    }
}