using Newtonsoft.Json;
using System.ComponentModel;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    /*
     * 反序列化/序列化测试通过 2020年8月7日
     */

    public class GLTFAccessor : GLTFElement
    {
        public GLTFAccessor()
        {

        }

        #region Properties

        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("bufferView", Required = Required.DisallowNull)]
        public int? BufferViewIndex { get; set; }

        [JsonProperty("byteOffset", Required = Required.DisallowNull)]
        public int? ByteOffset { get; set; }

        [JsonProperty("componentType", Required = Required.Always)]
        [Description("5123: unsigned_short; 5126: float;5121:unsigned_byte")]
        public int ComponentType { get; set; }

        [JsonProperty("count", Required = Required.Always)]
        public int Count { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty("max", Required = Required.DisallowNull)]
        public List<float> Max { get; set; }

        [JsonProperty("min", Required = Required.DisallowNull)]
        public List<float> Min { get; set; }

        #endregion

        #region Methods

        public static GLTFAccessor Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read();

            GLTFAccessor acc = new GLTFAccessor();

            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var curProp = reader.Value as string;
                switch (curProp)
                {
                    case "byteOffset":
                        acc.ByteOffset = (int)reader.ReadAsInt32();
                        break;
                    case "bufferView":
                        acc.BufferViewIndex = (int)reader.ReadAsInt32();
                        break;
                    case "componentType":
                        acc.ComponentType = (int)reader.ReadAsInt32();
                        break;
                    case "count":
                        acc.Count = (int)reader.ReadAsInt32();
                        break;
                    case "type":
                        acc.Type = reader.ReadAsString();
                        break;
                    case "max":
                        acc.Max = new List<float>();
                        reader.Read(); // 进入 startArray
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            acc.Max.Add(float.Parse(reader.Value.ToString()));
                        break;
                    case "min":
                        acc.Min = new List<float>();
                        reader.Read(); // 进入 startArray
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            acc.Min.Add(float.Parse(reader.Value.ToString()));
                        break;
                    case "name":
                        acc.Name = reader.ReadAsString();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            reader.Read();
            return acc;
        }

        #endregion
    }
}
