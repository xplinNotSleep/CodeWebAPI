using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFOcclusionTextureInfoConvertor : GLTFBaseConvertor<GLTFOcclusionTextureInfo>
    {
        public override GLTFOcclusionTextureInfo ReadJson(JsonReader reader, Type objectType, GLTFOcclusionTextureInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFOcclusionTextureInfo;
        }

        public override void WriteJson(JsonWriter writer, GLTFOcclusionTextureInfo value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value.Strength > 1.0 || value.Strength < 0)
                throw new Exception("[GLTFMaterial Schema Error] occlusionTextureInfo.strength 必须在0~1内");
            if (value.Strength != 1.0)
                WriteProperty(writer, "strength", value.Strength);

            WriteProperty(writer, "index", value.Index);
            WriteProperty(writer, "texCoord", value.TexCoord);

            writer.WriteEndObject();
        }
    }
}
