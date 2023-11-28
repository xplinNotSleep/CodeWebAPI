using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFPbrMetallicRoughnessConvertor : GLTFBaseConvertor<GLTFPbrMetallicRoughness>
    {
        public override GLTFPbrMetallicRoughness ReadJson(JsonReader reader, Type objectType, GLTFPbrMetallicRoughness existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFPbrMetallicRoughness;
        }

        public override void CheckSchema(GLTFPbrMetallicRoughness pbrmr)
        {
            if (pbrmr.BaseColorFactor == null)
                return;
            if (pbrmr.BaseColorFactor.Length != 4)
                throw new Exception("[GLTFPbrMetallicRoughness Schema Error] baseColorFactor 必须是 4 维数组");
            bool hasErrorNumber = pbrmr.BaseColorFactor.All(v => v > 1.0 || v < 0);
            if (hasErrorNumber)
                throw new Exception("[GLTFPbrMetallicRoughness Schema Error] baseColorFactor 数字必须在0~1内");
            if (pbrmr.MetallicFactor > 1.0 || pbrmr.MetallicFactor < 0)
                throw new Exception("[GLTFPbrMetallicRoughness Schema Error] metallicFactor 数字必须在0~1内");
            if (pbrmr.RoughnessFactor > 1.0 || pbrmr.RoughnessFactor < 0)
                throw new Exception("[GLTFPbrMetallicRoughness Schema Error] roughnessFactor 数字必须在0~1内");
        }

        public override void WriteJson(JsonWriter writer, GLTFPbrMetallicRoughness value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            CheckSchema(value);
            if (value.BaseColorFactor != null && !value.BaseColorFactor.All(v => v == 1.0))
            {
                writer.WritePropertyName("baseColorFactor");
                serializer.Serialize(writer, value.BaseColorFactor);
            }
            if (value.BaseColorTexture != null)
            {
                writer.WritePropertyName("baseColorTexture");
                serializer.Serialize(writer, value.BaseColorTexture);
            }
            if (value.MetallicFactor != 1.0)
                WriteProperty(writer, "metallicFactor", value.MetallicFactor);
            if (value.RoughnessFactor != 1.0)
                WriteProperty(writer, "roughnessFactor", value.RoughnessFactor);
            if (value.MetallicRoughnessTexture != null)
            {
                writer.WritePropertyName("metallicRoughnessTexture");
                serializer.Serialize(writer, value.MetallicRoughnessTexture);
            }

            writer.WriteEndObject();
        }
    }
}
