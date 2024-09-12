using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thavyra.Rest.Json;

public class NullableConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(JsonNullable<>))
        {
            return false;
        }
        
        return true;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        
        return (JsonConverter) Activator.CreateInstance(
            type: typeof(NullableConverter<>).MakeGenericType(valueType),
            bindingAttr: BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null
        )!;
    }
    
    private class NullableConverter<T> : JsonConverter<JsonNullable<T>> where T : notnull
    {
        public override JsonNullable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = JsonSerializer.Deserialize<T>(ref reader, options);

            return value;
        }

        public override void Write(Utf8JsonWriter writer, JsonNullable<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (T?)value, options);
        }
    }
}