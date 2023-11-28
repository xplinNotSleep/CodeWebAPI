using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFMeshConvertor : GLTFBaseConvertor<GLTFMesh>
    {
        // String to Object
        public override GLTFMesh ReadJson(JsonReader reader, Type objectType, GLTFMesh existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFMesh;
        }

        // Object to String
        public override void WriteJson(JsonWriter writer, GLTFMesh value1, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value1.Name != null && value1.Name != "")
                WriteProperty(writer, "name", value1.Name);
            if (value1.Extras != null)
                WriteProperty(writer, "extras", value1.Extras);
            if (value1.Extensions != null && value1.Extensions.Count != 0)
                WriteJObject(writer, value1.Extensions, "extensions");
            if (value1.Primitives != null && value1.Primitives.Count != 0)
            {
                writer.WritePropertyName("primitives");
                writer.WriteStartArray();
                value1.Primitives.ForEach(value =>
                {
                    writer.WriteStartObject();
                    var attrs = value.Attributes;
                    if (!attrs.ContainsKey("POSITION") || attrs.Count == 0)
                        throw new Exception("[GLTFPrimitive Schema Error] attributes[\"POSITION\"] 必须存在，现丢失；attributes数量：" + attrs.Count);

                    writer.WritePropertyName("attributes");
                    writer.WriteStartObject();
                    foreach (var kv in attrs)
                    {
                        writer.WritePropertyName(kv.Key);
                        writer.WriteValue(kv.Value);
                    }
                    writer.WriteEndObject();
                    if (value.GLRenderMode != 4)
                        WriteProperty(writer, "mode", value.GLRenderMode);
                    if (value.Indices != -1)
                        WriteProperty(writer, "indices", value.Indices);
                    if (value.MaterialIndex != -1)
                        WriteProperty(writer, "material", value.MaterialIndex);
                    if (value.Extensions != null && value.Extensions.Count != 0)
                    {
                        writer.WritePropertyName("extensions");
                        writer.WriteStartObject();
                        foreach (var kv in value.Extensions)
                        {
                            writer.WritePropertyName(kv.Key);
                            writer.WriteStartObject();
                            foreach (var kv2 in kv.Value)
                            {
                                writer.WritePropertyName(kv2.Key);
                                if (kv2.Key == "attributes")
                                {
                                    writer.WriteStartObject();
                                    foreach (JProperty kv3 in kv2.Value)
                                    {
                                        writer.WritePropertyName(kv3.Name);
                                        writer.WriteValue(kv3.Value);
                                    }
                                    writer.WriteEndObject();
                                }
                                else
                                {
                                    writer.WriteValue(kv2.Value);
                                }
                            }
                            writer.WriteEndObject();
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                });
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
    }
}
