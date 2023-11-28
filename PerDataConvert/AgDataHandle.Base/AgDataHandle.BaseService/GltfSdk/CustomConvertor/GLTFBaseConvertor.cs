using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public abstract class GLTFBaseConvertor<GLTFElement> : JsonConverter<GLTFElement>
    {
        public virtual void CheckSchema(GLTFElement el) { }
        public virtual void WriteArray(JsonWriter writer, string name, IEnumerable arr) { }
        public virtual void WriteProperty(JsonWriter writer, string name, object value)
        {
            if (value != null)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
            }
        }

        public virtual void WriteJObject(JsonWriter writer, Dictionary<string, JObject> value, string name)
        {
            if (value == null)
                return;
            writer.WritePropertyName(name);
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
