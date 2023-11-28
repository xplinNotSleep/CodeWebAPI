using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    [JsonObject]
    public class GLTFAsset : GLTFElement
    {
        public GLTFAsset()
        {
            Version = "2.0";
            Generator = "AgCIM Desktop GLTF SDK";
        }

        [JsonProperty("version", Required = Required.Always)]
        public string Version { get; set; }
        [JsonProperty("minVersion", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string MinVersion { get; set; }
        [JsonProperty("copyright", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Copyright { get; set; }
        [JsonProperty("generator", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Generator { get; set; }
        [JsonProperty("extras", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public JObject Extras { get; set; }
        [JsonProperty("upAxis", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string UpAxis { get; set; }

        internal static GLTFAsset Deserialize(JsonReader reader, GLTF model)
        {
            GLTFAsset asset = new GLTFAsset();
            reader.Read();
            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string curVal = reader.Value.ToString();
                switch (curVal)
                {
                    case "copyright":
                        asset.Copyright = reader.ReadAsString();
                        break;
                    case "generator":
                        asset.Generator = reader.ReadAsString();
                        break;
                    case "minVersion":
                        asset.MinVersion = reader.ReadAsString();
                        break;
                    case "version":
                        asset.Version = reader.ReadAsString();
                        break;
                    case "upAxis":
                        asset.UpAxis = reader.ReadAsString();
                        break;
                    case "extras":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        reader.Read();
                        var jpExtras = JToken.ReadFrom(reader) as JObject;
                        asset.Extras = jpExtras;
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            asset.GLTF = model;
            return asset;
        }
    }
}
