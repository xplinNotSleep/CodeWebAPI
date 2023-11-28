using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFExtensionConvertor : JsonConverter<Dictionary<string, JObject>>
    {
        public override Dictionary<string, JObject> ReadJson(JsonReader reader, Type objectType, Dictionary<string, JObject> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as Dictionary<string, JObject>;
        }

        public override void WriteJson(JsonWriter writer, Dictionary<string, JObject> value, JsonSerializer serializer)
        {
            if (value == null)
                return;
            writer.WriteStartObject();
            foreach (var kv in value)
            {
                writer.WritePropertyName(kv.Key);
                writer.WriteStartObject();

                var children = kv.Value.Children();
                foreach (var v in children)
                {
                    writer.WriteRaw(v.ToString());
                    if (v.Next != null)
                        writer.WriteRaw(",");
                }

                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }
    }
}
