using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public abstract class GLTFResourceElement : GLTFElement
    {
        [JsonIgnore]
        public Stream DataStream { get; set; }
        [JsonIgnore]
        public List<byte> BinaryData { get; set; }
        [JsonIgnore]
        public GLTFBufferType BufferType { get; set; }
        [JsonIgnore]
        public long CurrentPosition { get { return DataStream.Position; } }

        [JsonProperty("uri", NullValueHandling = NullValueHandling.Ignore)]
        public string Uri { get; set; }

        [JsonIgnore]
        public int StreamStartPosition { get; set; }

        [JsonIgnore]
        public bool IsReadOnly { get; set; }

        public void ResetPosition()
        {
            DataStream.Position = StreamStartPosition;
        }

        /// <summary>
        /// 增加字节
        /// </summary>
        /// <param name="b"></param>
        public void Add(byte b)
        {
            if (BufferType == GLTFBufferType.BinaryData)
            {
                BinaryData.Add(b);
            }
            else
            {
                DataStream.WriteByte(b);
            }
        }

        /// <summary>
        /// 增加字节数组
        /// </summary>
        /// <param name="bs"></param>
        public void AddRange(byte[] bs)
        {
            if (BufferType == GLTFBufferType.BinaryData)
            {
                BinaryData.AddRange(bs);
            }
            else
            {
                DataStream.Write(bs, 0, bs.Length);
            }
        }

        /// <summary>
        /// 获取所有字节
        /// </summary>
        /// <returns></returns>
        public List<byte> GetByte()
        {
            if (BufferType == GLTFBufferType.BinaryData)
            {
                return BinaryData;
            }
            else
            {
                byte[] bs = new byte[DataStream.Length];
                DataStream.Read(bs, 0, bs.Length);
                // 设置当前流的位置为流的开始
                DataStream.Seek(0, SeekOrigin.Begin);
                return bs.ToList();
            }
        }
    }
}
