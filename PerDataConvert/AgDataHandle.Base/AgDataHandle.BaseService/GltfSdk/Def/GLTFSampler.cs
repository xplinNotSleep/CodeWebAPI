using AgDataHandle.BaseService.CommonHelper;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLTFSampler : GLTFElement
    {
        public GLTFSampler()
        {

        }

        #region Properties

        [JsonProperty("magFilter", Required = Required.DisallowNull)]
        public int MagFilter { get; set; } = 9729;
        [JsonProperty("minFilter", Required = Required.DisallowNull)]
        public int MinFilter { get; set; } = 9986;
        [JsonProperty("wrapS", Required = Required.DisallowNull)]
        public int WrapS { get; set; } = 10497;
        [JsonProperty("wrapT", Required = Required.DisallowNull)]
        public int WrapT { get; set; } = 10497;

        #endregion

        public GLTFSampler Clone()
        {
            GLTFSampler sampler = new GLTFSampler();
            sampler.MagFilter = MagFilter;
            sampler.MinFilter = MinFilter;
            sampler.WrapS = WrapS;
            sampler.WrapT = WrapT;

            return sampler;
        }

        internal static GLTFSampler Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read(); // token : startArray -> startObject
            if (reader.TokenType == JsonToken.EndArray) //rainer 20210106
                return new GLTFSampler();
            GLTFSampler sampler = new GLTFSampler();

            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string curPropName = reader.Value.ToString();
                switch (curPropName)
                {
                    case "magFilter":
                        sampler.MagFilter = C73.GetInt32(reader);
                        break;
                    case "minFilter":
                        sampler.MinFilter = C73.GetInt32(reader);
                        break;
                    case "wrapS":
                        sampler.WrapS = C73.GetInt32(reader);
                        break;
                    case "wrapT":
                        sampler.WrapS = C73.GetInt32(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            sampler.GLTF = model;
            return sampler;
        }

    }

    public enum GLTFSamplerFilter
    {

    }
}
