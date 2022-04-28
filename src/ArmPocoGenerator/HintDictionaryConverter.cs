using Microsoft.Json.Schema.ToDotNet.Hints;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArmTypeGenerator
{
    public class HintDictionaryConverter : JsonConverter<HintDictionary>
    {
        public override HintDictionary? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            var hintText = doc.RootElement.ToString();
            return new HintDictionary(hintText);
        }

        public override void Write(Utf8JsonWriter writer, HintDictionary value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
