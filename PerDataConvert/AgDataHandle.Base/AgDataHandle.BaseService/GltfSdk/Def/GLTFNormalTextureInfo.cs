using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLTFNormalTextureInfo : GLTFTextureInfo
    {
        public GLTFNormalTextureInfo()
        {
            Scale = 1.0;
        }

        #region Properties

        [JsonProperty("scale", Required = Required.DisallowNull)]
        public double Scale { get; set; }

        #endregion

        #region Methods

        public new GLTFNormalTextureInfo Clone()
        {
            var info = new GLTFNormalTextureInfo { GLTF = null };

            info.Index = Index;
            info.TexCoord = TexCoord;
            info.Scale = Scale;

            return info;
        }

        internal static new GLTFNormalTextureInfo Deserialize(JsonReader reader, GLTF model)
        {
            var nTexInfo = new GLTFNormalTextureInfo();

            reader.Read(); // propertyName -> startObject

            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string propName = reader.Value.ToString();

                switch (propName)
                {
                    case "texCoord":
                        string readStr = reader.ReadAsString();
                        if (int.TryParse(readStr, out int intValue))
                        {
                            nTexInfo.TexCoord = intValue;
                        }
                        else if (double.TryParse(readStr, out double doubleValue))
                        {
                            nTexInfo.TexCoord = (int)doubleValue;
                        }
                        else
                        {
                            nTexInfo.TexCoord = 0;
                        }
                        break;
                    case "scale":
                        nTexInfo.Scale = (double)reader.ReadAsDouble();
                        break;
                    case "index":
                        nTexInfo.Index = (int)reader.ReadAsInt32();
                        break;
                    case "extensions":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        var jpExts = JToken.ReadFrom(reader) as JProperty;
                        nTexInfo.Extensions = nTexInfo.Extensions ?? new Dictionary<string, JObject>();
                        nTexInfo.LoadExtension(jpExts, nTexInfo.Extensions);
                        break;
                    default:
                        break;
                }
            }

            nTexInfo.GLTF = model;
            return nTexInfo;
        }

        #endregion
    }
}
