using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLTFBufferView : GLTFElement
    {
        public GLTFBufferView()
        {
            ByteStride = 0;
        }

        #region Properties

        [JsonProperty(
            "name",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(
            "buffer",
            Required = Required.Always)]
        public int BufferIndex { get; set; }

        [JsonProperty(
            "byteLength",
            Required = Required.Always)]
        public int ByteLength { get; set; }

        [JsonProperty(
            "byteOffset",
            Required = Required.DisallowNull,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ByteOffset { get; set; }

        internal static GLTFBufferView Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read();

            GLTFBufferView bv = new GLTFBufferView();
            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string curProp = reader.Value.ToString();
                switch (curProp)
                {
                    case "byteOffset":
                        bv.ByteOffset = (int)reader.ReadAsInt32();
                        break;
                    case "byteLength":
                        bv.ByteLength = (int)reader.ReadAsInt32();
                        break;
                    case "byteStride":
                        bv.ByteStride = (int)reader.ReadAsInt32();
                        break;
                    case "buffer":
                        bv.BufferIndex = (int)reader.ReadAsInt32();
                        break;
                    case "target":
                        var tg = (int)reader.ReadAsInt32();
                        if (tg == 34962 || tg == 34963)
                            bv.Target = tg;
                        break;
                    case "name":
                        bv.Name = reader.ReadAsString();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            reader.Read();
            return bv;
        }

        [JsonProperty(
            "byteStride",
            Required = Required.DisallowNull)]
        public int ByteStride { get; set; }

        [JsonProperty(
            "target",
            Required = Required.DisallowNull)]
        public int Target { get; set; }

        #endregion

        #region Methods

        public GLTFBuffer GetBuffer()
        {
            return GLTF.Buffers[BufferIndex];
        }

        #endregion
    }
}
