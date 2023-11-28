using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFMaterialConvertor : GLTFBaseConvertor<GLTFMaterial>
    {
        public override GLTFMaterial ReadJson(JsonReader reader, Type objectType, GLTFMaterial existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFMaterial;
        }
        public override void CheckSchema(GLTFMaterial el)
        {
            if (el.EmissiveFactor.Length != 3 || el.EmissiveFactor.All(v => v > 1.0 || v < 0))
                throw new Exception("[GLTFMaterial Schema Error] emissiveFactor 必须是三维数组; emissiveFactor 数字必须在 [0,1]区间上。");
        }
        public override void WriteJson(JsonWriter writer, GLTFMaterial value, JsonSerializer serializer)
        {
            CheckSchema(value);

            writer.WriteStartObject();

            WriteProperty(writer, "name", value.Name);
            WriteProperty(writer, "doubleSided", value.DoubleSided);
            if (value.AlphaCutOff != 0.5)
            {
                if (value.AlphaCutOff < 0)
                    throw new Exception("[GLTFMaterial Schema Error] alphaCutOff 必须大于0.");
                else if (!(
                    value.AlphaMode.Equals("OPAQUE") ||
                    value.AlphaMode.Equals("MASK") ||
                    value.AlphaMode.Equals("BLEND")
                    ))
                    throw new Exception("[GLTFMaterial Schema Error] alphaMode 只能是 BLEND/MASK/OPAQUE");
                WriteProperty(writer, "alphaCutOff", value.AlphaCutOff);
            }
            WriteProperty(writer, "alphaMode", value.AlphaMode);
            if (!value.EmissiveFactor.All(v => v == 0))
            {
                writer.WritePropertyName("emissiveFactor");
                serializer.Serialize(writer, value.EmissiveFactor);
            }
            if (value.EmissiveTexture != null)
            {
                writer.WritePropertyName("emissiveTexture");
                serializer.Serialize(writer, value.EmissiveTexture);
            }
            if (value.PbrMetallicRoughness != null)
            {
                if (!value.PbrMetallicRoughness.IsAllDefault)
                {
                    writer.WritePropertyName("pbrMetallicRoughness");
                    serializer.Serialize(writer, value.PbrMetallicRoughness);
                }
            }
            if (value.OcclusionTexture != null)
            {
                writer.WritePropertyName("occlusionTexture");
                serializer.Serialize(writer, value.OcclusionTexture);
            }
            if (value.NormalTexture != null)
            {
                writer.WritePropertyName("normalTexture");
                serializer.Serialize(writer, value.NormalTexture);
            }
            if (value.Extensions.Count != 0)
            {
                writer.WritePropertyName("extensions");
                serializer.Serialize(writer, value.Extensions);
            }
            writer.WriteEndObject();
        }
    }
}
