using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    internal class GLTFTextureConvertor : GLTFBaseConvertor<GLTFTexture>
    {
        public override GLTFTexture ReadJson(JsonReader reader, Type objectType, GLTFTexture existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFTexture;
        }

        public override void WriteJson(JsonWriter writer, GLTFTexture value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value.SamplerIndex != -1)
                WriteProperty(writer, "sampler", value.SamplerIndex);
            if (value.ImageIndex != -1)
                WriteProperty(writer, "source", value.ImageIndex);

            writer.WriteEndObject();
        }
    }
}
