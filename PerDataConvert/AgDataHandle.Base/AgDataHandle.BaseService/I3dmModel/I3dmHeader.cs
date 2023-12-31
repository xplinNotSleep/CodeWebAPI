﻿using AgDataHandle.BaseService.CommonHelper;
using System.Text;

namespace AgDataHandle.BaseService.I3dmModel
{
    public class I3dmHeader
    {
        public string Magic { get; set; }
        public int Version { get; set; }
        public int ByteLength { get; set; }
        public int FeatureTableJsonByteLength { get; set; }
        public int FeatureTableBinaryByteLength { get; set; }
        public int BatchTableJsonByteLength { get; set; }
        public int BatchTableBinaryByteLength { get; set; }

        public int GltfFormat { get; set; }

        // I3DM每个头部信息占用的字节长度
        private static readonly int MAGIC_LENGTH = 4;
        private static readonly int VERSION_LENGTH = 4;
        private static readonly int BYTE_LENGTH = 4;
        private static readonly int FEATURE_TABLE_JSON_BYTE_LENGTH = 4;
        private static readonly int FEATURE_TABLE_BINARY_BYTE_LENGTH = 4;
        private static readonly int BATCH_TABLE_JSON_BYTE_LENGTH = 4;
        private static readonly int BATCH_TABLE_BINARY_BYTE_LENGTH = 4;
        private static readonly int GLTF_FORMAT_LENGTH = 4;

        public int HeaderLength
        {
            get
            {
                return MAGIC_LENGTH +
                    VERSION_LENGTH +
                    BYTE_LENGTH +
                    FEATURE_TABLE_JSON_BYTE_LENGTH +
                    FEATURE_TABLE_BINARY_BYTE_LENGTH +
                    BATCH_TABLE_JSON_BYTE_LENGTH +
                    BATCH_TABLE_BINARY_BYTE_LENGTH +
                    GLTF_FORMAT_LENGTH;
            }
        }

        public I3dmHeader()
        {
            Magic = "i3dm";
            Version = 1;
            FeatureTableJsonByteLength = 0;
            FeatureTableBinaryByteLength = 0;
            BatchTableJsonByteLength = 0;
            BatchTableBinaryByteLength = 0;
            GltfFormat = 1;
        }

        public I3dmHeader(BinaryReader reader)
        {
            Magic = Encoding.UTF8.GetString(reader.ReadBytes(4));
            Version = (int)reader.ReadUInt32();
            ByteLength = (int)reader.ReadUInt32();

            FeatureTableJsonByteLength = (int)reader.ReadUInt32();
            FeatureTableBinaryByteLength = (int)reader.ReadUInt32();
            BatchTableJsonByteLength = (int)reader.ReadUInt32();
            BatchTableBinaryByteLength = (int)reader.ReadUInt32();
            GltfFormat = (int)reader.ReadUInt32();
        }

        public int Length
        {
            get
            {
                return 32 + FeatureTableJsonByteLength + FeatureTableBinaryByteLength + BatchTableJsonByteLength + BatchTableBinaryByteLength;
            }
        }

        public byte[] AsBinary()
        {
            var magicBytes = Encoding.UTF8.GetBytes(Magic);
            var versionBytes = BitConverter.GetBytes(Version);
            var byteLengthBytes = BitConverter.GetBytes(ByteLength);
            var featureTableJsonByteLengthBytes = BitConverter.GetBytes(FeatureTableJsonByteLength);
            var featureTableBinaryByteLengthBytes = BitConverter.GetBytes(FeatureTableBinaryByteLength);
            var batchTableJsonByteLength = BitConverter.GetBytes(BatchTableJsonByteLength);
            var batchTableBinaryByteLength = BitConverter.GetBytes(BatchTableBinaryByteLength);
            var gltfFormatBytes = BitConverter.GetBytes(GltfFormat);

            return magicBytes.
                Concat(versionBytes).
                Concat(byteLengthBytes).
                Concat(featureTableJsonByteLengthBytes).
                Concat(featureTableBinaryByteLengthBytes).
                Concat(batchTableJsonByteLength).
                Concat(batchTableBinaryByteLength).
                Concat(gltfFormatBytes).
                ToArray();
        }

        public List<string> Validate()
        {
            var res = new List<string>();

            var headerByteLength = AsBinary().Count();
            var featureTableJsonByteOffset = headerByteLength;
            var featureTableBinaryByteOffset = featureTableJsonByteOffset + FeatureTableJsonByteLength;
            var batchTableJsonByteOffset = featureTableBinaryByteOffset + FeatureTableBinaryByteLength;
            var batchTableBinaryByteOffset = batchTableJsonByteOffset + BatchTableJsonByteLength;
            var glbByteOffset = batchTableBinaryByteOffset + BatchTableBinaryByteLength;

            if (featureTableBinaryByteOffset % 8 > 0)
            {
                res.Add("Feature table binary must be aligned to an 8-byte boundary.");
            }
            if (batchTableBinaryByteOffset % 8 > 0)
            {
                res.Add("Batch table binary must be aligned to an 8-byte boundary.");
            }
            if (glbByteOffset % 8 > 0)
            {
                res.Add("Glb must be aligned to an 8-byte boundary.");
            }

            return res;
        }

        private static byte[] GetBatchIdsBytes(List<ushort> inputs, string batchIdSerializeType)
        {
            byte[] res = null;
            switch (batchIdSerializeType)
            {
                case "UNSIGNED_BYTE":
                    res = ByteConvertor.ToBytes<byte>(inputs);
                    break;
                case "UNSIGNED_SHORT":
                    res = ByteConvertor.ToBytes<ushort>(inputs);
                    break;
                case "UNSIGNED_INT":
                    res = ByteConvertor.ToBytes<uint>(inputs);
                    break;
            }
            return res;
        }
    }
}
