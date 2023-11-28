using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;
using System.Collections;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFConvertor : JsonConverter<GLTF>
    {
        public override GLTF ReadJson(JsonReader reader, Type objectType, GLTF existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTF;
        }

        public void WriteProperty(JsonWriter writer, string name, object value)
        {
            if (value != null)
            {
                if (value is ICollection && (value as ICollection).Count == 0)
                {
                    return;
                }
                writer.WritePropertyName(name);
                writer.WriteValue(value);
            }
        }

        public override void WriteJson(JsonWriter writer, GLTF value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            serializer.Serialize(writer, value);

            writer.WriteEndObject();
        }
    }
}
