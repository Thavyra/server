using Newtonsoft.Json;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Documentation;

public class NullableDocumentationConverter<T> : JsonConverter<JsonNullable<T>> where T : notnull
{
    public override void WriteJson(JsonWriter writer, JsonNullable<T> value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.Value);
    }

    public override JsonNullable<T> ReadJson(JsonReader reader, Type objectType, JsonNullable<T> existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }
}