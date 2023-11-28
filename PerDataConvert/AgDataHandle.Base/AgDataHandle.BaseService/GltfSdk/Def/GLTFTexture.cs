using AgDataHandle.BaseService.CommonHelper;
using Newtonsoft.Json;
using System.ComponentModel;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLTFTexture : GLTFElement
    {
        #region Properties

        [Description("即 'name' 属性")]
        [JsonProperty("name", Required = Required.DisallowNull)]
        public string Name { get; set; }

        [Description("即 'sampler' 属性")]
        [JsonProperty("sampler", Required = Required.DisallowNull)]
        public int SamplerIndex { get; set; }

        [Description("即 'source' 属性")]
        [JsonProperty("source", Required = Required.DisallowNull)]
        public int ImageIndex { get; set; }

        #endregion

        #region Methods

        [Obsolete("还没确定")]
        public void SetImage()
        {

        }

        public GLTFImage GetImage()
        {
            if (GLTF.Images.Count != 0)
                if (GLTF.Images.Count > ImageIndex)
                    return GLTF.Images[ImageIndex];
                else return null;
            else return null;
        }

        internal static GLTFTexture Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read(); // token : startArray -> startObject
            if (reader.TokenType == JsonToken.EndArray) //rainer 20210106
                return null;

            bool hasData = false;
            GLTFTexture tex = new GLTFTexture();
            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                hasData = true;
                string curPropName = reader.Value.ToString();
                switch (curPropName)
                {
                    case "source":
                        tex.ImageIndex = C73.GetInt32(reader);
                        break;
                    case "sampler":
                        tex.SamplerIndex = C73.GetInt32(reader);
                        break;
                    case "name":
                        tex.Name = reader.ReadAsString();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            if (!hasData)
                return null;

            tex.GLTF = model;
            return tex;
        }

        public GLTFTexture Clone()
        {
            GLTFTexture texture = new GLTFTexture() { GLTF = null };
            texture.Name = Name;
            texture.SamplerIndex = SamplerIndex;
            texture.ImageIndex = ImageIndex;

            return texture;
        }

        #endregion
    }
}
