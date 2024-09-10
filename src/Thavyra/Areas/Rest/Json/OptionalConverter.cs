using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thavyra.Rest.Json;

/// <summary>
/// Converter to handle optional JSON properties using the <see cref="JsonOptional{T}"/> type.
/// </summary>
public class OptionalConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(JsonOptional<>))
        {
            return false;
        }
        
        return true;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        
        return (JsonConverter) Activator.CreateInstance(
            type: typeof(OptionalConverter<>).MakeGenericType(valueType),
            bindingAttr: BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null
            )!;
    }

    private class OptionalConverter<T> : JsonConverter<JsonOptional<T>>
    {
        public override JsonOptional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = JsonSerializer.Deserialize<T>(ref reader, options);
            
            return JsonOptional<T>.Of(value!); // Value may be null, this is ok
        }

        public override void Write(Utf8JsonWriter writer, JsonOptional<T> value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                return;
            }
            
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}