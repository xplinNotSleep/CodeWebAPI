using System.Text;

namespace AgDataHandle.BaseService.B3dmModel
{
    public class BI3dmHeader
    {
        public string magic { get; set; }
        public int version { get; set; }
        public int byteLength { get; set; }
        public int featureTableJSONByteLength { get; set; }
        public int featureTableBinaryByteLength { get; set; }
        public int batchTableJSONByteLength { get; set; }
        public int batchTableBinaryByteLength { get; set; }
        public int gltfFormat { get; set; }
        public byte[] EndBytes { get; internal set; }

        // 每个头部信息占用的字节长度
        private static readonly int MAGIC_LENGTH = 4;
        private static readonly int VERSION_LENGTH = 4;
        private static readonly int BYTE_LENGTH = 4;
        private static readonly int FEATURE_TABLE_JSON_BYTE_LENGTH = 4;
        private static readonly int FEATURE_TABLE_BINARY_BYTE_LENGTH = 4;
        private static readonly int BATCH_TABLE_JSON_BYTE_LENGTH = 4;
        private static readonly int BATCH_TABLE_BINARY_BYTE_LENGTH = 4;
        // i3dm独有的头部信息
        private static readonly int GLTF_FORMAT_LENGTH = 4;

        public int Length
        {
            get
            {
                int b3dmHeader = MAGIC_LENGTH +
                    VERSION_LENGTH +
                    BYTE_LENGTH +
                    FEATURE_TABLE_JSON_BYTE_LENGTH +
                    FEATURE_TABLE_BINARY_BYTE_LENGTH +
                    BATCH_TABLE_JSON_BYTE_LENGTH +
                    BATCH_TABLE_BINARY_BYTE_LENGTH;
                if (magic == "b3dm")
                {
                    return b3dmHeader;
                }
                else
                {
                    return b3dmHeader + GLTF_FORMAT_LENGTH;
                }
            }
        }

        public BI3dmHeader(string magic)
        {
            this.magic = magic;
            version = 1;
            featureTableJSONByteLength = 0;
            featureTableBinaryByteLength = 0;
            batchTableJSONByteLength = 0;
            batchTableBinaryByteLength = 0;
            gltfFormat = 1;
        }

        /// <summary>
        /// 获取头部字节
        /// </summary>
        /// <returns></returns>
        public byte[] GetHeadBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Encoding.UTF8.GetBytes(magic).ToList());
            bytes.AddRange(BitConverter.GetBytes(version));
            bytes.AddRange(BitConverter.GetBytes(byteLength));
            bytes.AddRange(BitConverter.GetBytes(featureTableJSONByteLength));
            bytes.AddRange(BitConverter.GetBytes(featureTableBinaryByteLength));
            bytes.AddRange(BitConverter.GetBytes(batchTableJSONByteLength));
            bytes.AddRange(BitConverter.GetBytes(batchTableBinaryByteLength));
            if (magic == "i3dm")
            {
                bytes.AddRange(BitConverter.GetBytes(gltfFormat));
            }
            return bytes.ToArray();
        }
    }
}
