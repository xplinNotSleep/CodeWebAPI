using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLTFBuffer : GLTFResourceElement
    {
        public GLTFBuffer()
        {
            StreamStartPosition = 0;
            //BufferType = GLTFBufferType.Memory;
        }

        public GLTFBuffer(GLTFBufferType bufferType)
        {
            BufferType = bufferType;
        }

        private int _ByteLength;
        [JsonProperty("byteLength", Required = Required.Always)]
        public int ByteLength
        {
            get
            {
                if (BufferType == GLTFBufferType.BinaryData)
                {
                    _ByteLength = BinaryData.Count;
                    return _ByteLength;
                }
                _ByteLength = (int)(DataStream?.Length ?? 0);
                return _ByteLength;
            }
            set { _ByteLength = value; }
        }

        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        #region Methods

        public byte[] ReadData(int from = -1, int length = -1)
        {
            if (BufferType == GLTFBufferType.BinaryData)
            {
                if (from == -1)
                {
                    return BinaryData.ToArray();
                }
                else
                {
                    return BinaryData.Skip(from).Take(length).ToArray();
                }
            }
            if (DataStream == null)
                return null;
            if (BufferType == GLTFBufferType.GLBFileStream && StreamStartPosition == 0)
                return null;
            if (length == -1)
                length = (int)DataStream.Length - StreamStartPosition;

            List<byte> data = new List<byte>(length);
            if (from == -1)
                from = StreamStartPosition;
            else
                from += StreamStartPosition;
            DataStream.Position = from;

            int dt;
            int i = 0;
            while ((dt = DataStream.ReadByte()) != -1 && i < length)
            {
                i++;
                data.Add((byte)dt);
            }

            ResetPosition();

            return data.ToArray();
        }

        internal static GLTFBuffer Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray) // 如果不是，Read()的token就是属性了
                reader.Read();

            GLTFBuffer bf = new GLTFBuffer();
            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var curProp = reader.Value as string;
                switch (curProp)
                {
                    case "uri":
                    case "url":
                        bf.Uri = reader.ReadAsString();
                        break;
                    case "byteLength":
                        bf.ByteLength = (int)reader.ReadAsInt32();
                        break;
                    case "name":
                        bf.Name = reader.ReadAsString();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            reader.Read(); // token -> endObject

            return bf;
        }

        #endregion
    }

    public enum GLTFBufferType
    {
        FileStream,
        Memory,
        GLBFileStream,
        BinaryData
    }
}
