using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFTextureInfoConvertor : GLTFBaseConvertor<GLTFTextureInfo>
    {
        public override GLTFTextureInfo ReadJson(JsonReader reader, Type objectType, GLTFTextureInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFTextureInfo;
        }

        public override void WriteJson(JsonWriter writer, GLTFTextureInfo value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value.TexCoord < 0)
                throw new Exception("[GLTFTextureInfo Schema Error] texCoord 必须是非负整数");
            WriteProperty(writer, "texCoord", value.TexCoord);
            WriteProperty(writer, "index", value.Index);
            if (value.Extensions != null && value.Extensions.Count != 0)
            {
                writer.WritePropertyName("extensions");
                serializer.Serialize(writer, value.Extensions);
            }
            writer.WriteEndObject();
        }
    }
}
