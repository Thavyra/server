using Newtonsoft.Json;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Documentation;

public class OptionalDocumentationConverter<T> : JsonConverter<JsonOptional<T>>
{
    public override void WriteJson(JsonWriter writer, JsonOptional<T> value, JsonSerializer serializer)
    {
        if (!value.HasValue)
        {
            writer.WriteNull();
            return;
        }

        serializer.Serialize(writer, value.Value);
    }

    public override JsonOptional<T> ReadJson(JsonReader reader, Type objectType, JsonOptional<T> existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }
}