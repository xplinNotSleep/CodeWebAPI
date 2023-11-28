using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLTFTextureInfo : GLTFElement
    {
        public GLTFTextureInfo()
        {
            TexCoord = 0;
        }

        #region Properties

        [JsonProperty("index", Required = Required.Always)]
        public int Index { get; set; }

        [JsonProperty("texCoord", Required = Required.DisallowNull)]
        public int TexCoord { get; set; }
        [JsonProperty("extensions", Required = Required.DisallowNull)]
        public Dictionary<string, JObject> Extensions { get; set; }

        #endregion

        #region Methods

        public string GetTextureAttrName()
        {
            //return $"TEXCOORD_{TexCoord}";
            return "TEXCOORD_" + TexCoord;
        }

        public virtual GLTFTextureInfo Clone()
        {
            return new GLTFTextureInfo { GLTF = null, Index = Index, TexCoord = TexCoord };
        }

        internal static GLTFTextureInfo Deserialize(JsonReader reader, GLTF model)
        {
            var texInfo = new GLTFTextureInfo();

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
                            texInfo.TexCoord = intValue;
                        }
                        else if (double.TryParse(readStr, out double doubleValue))
                        {
                            texInfo.TexCoord = (int)doubleValue;
                        }
                        else
                        {
                            texInfo.TexCoord = 0;
                        }
                        break;
                    case "index":
                        texInfo.Index = (int)reader.ReadAsInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            texInfo.GLTF = model;
            return texInfo;
        }

        #endregion
    }
}
