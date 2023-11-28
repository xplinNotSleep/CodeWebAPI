using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLTFOcclusionTextureInfo : GLTFTextureInfo
    {
        #region Properties

        [JsonProperty("strength", Required = Required.DisallowNull)]
        public double Strength { get; set; }

        #endregion

        #region Methods

        public new GLTFOcclusionTextureInfo Clone()
        {
            var info = new GLTFOcclusionTextureInfo { GLTF = null };

            info.Index = Index;
            info.TexCoord = TexCoord;
            info.Strength = Strength;

            return info;
        }

        internal new static GLTFOcclusionTextureInfo Deserialize(JsonReader reader, GLTF model)
        {
            var oTexInfo = new GLTFOcclusionTextureInfo();

            reader.Read(); // propertyName -> startObject

            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string propName = reader.Value.ToString();

                switch (propName)
                {
                    case "texCoord":
                        oTexInfo.TexCoord = (int)reader.ReadAsInt32();
                        break;
                    case "strength":
                        oTexInfo.Strength = (double)reader.ReadAsDouble();
                        break;
                    case "index":
                        oTexInfo.Index = (int)reader.ReadAsInt32();
                        break;
                    default:
                        break;
                }
            }

            oTexInfo.GLTF = model;
            return oTexInfo;
        }

        #endregion
    }
}
