using System.Text;

namespace AgDataHandle.BaseService.B3dmModel
{
    public class BI3dmHelper
    {
        #region 读取头部
        /// <summary>
        /// b3dm文件的的所有二进制数据
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static BI3dmHeader ReadHead(BinaryReader br)
        {
            NavigateToHead(br);
            var head = new BI3dmHeader("b3dm");
            var mag = br.ReadBytes(4);

            head.magic = Encoding.UTF8.GetString(mag);
            head.version = (int)br.ReadUInt32();

            if (head.version > 2)
            {

            }
            head.byteLength = (int)br.ReadUInt32();
            head.featureTableJSONByteLength = (int)br.ReadUInt32();
            head.featureTableBinaryByteLength = (int)br.ReadUInt32();
            head.batchTableJSONByteLength = (int)br.ReadUInt32();
            head.batchTableBinaryByteLength = (int)br.ReadUInt32();
            if (head.magic == "i3dm")
            {
                head.gltfFormat = (int)br.ReadUInt32();
                head.EndBytes = BitConverter.GetBytes(head.gltfFormat);
            }

            return head;
        }

        public static B3dm ReadB3dm(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("文件不存在：" + filePath);

            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(fs))
                {
                    var header = ReadHead(reader);
                    var b3dm = new B3dm(reader, header);
                    return b3dm;
                }
            }
        }

        #endregion

        #region 获得头部字节
        public static byte[] GetHeadBytes(BI3dmHeader header)
        {
            List<byte> items = new List<byte>();
            items.AddRange(Encoding.UTF8.GetBytes(header.magic));
            items.AddRange(BitConverter.GetBytes(header.version));
            items.AddRange(BitConverter.GetBytes(header.byteLength));
            items.AddRange(BitConverter.GetBytes(header.featureTableJSONByteLength));
            items.AddRange(BitConverter.GetBytes(header.featureTableBinaryByteLength));
            items.AddRange(BitConverter.GetBytes(header.batchTableJSONByteLength));
            items.AddRange(BitConverter.GetBytes(header.batchTableBinaryByteLength));
            if (header.EndBytes != null && header.EndBytes.Length > 0)
            {
                items.AddRange(header.EndBytes);
            }
            return items.ToArray();
        }
        #endregion

        #region 其他
        private static void NavigateToHead(BinaryReader br)
        {
            var t = br.BaseStream.Position;
            var bytes = br.ReadBytes(6 * 32 + 4);
            var t2 = bytes[4] < 1 || bytes[4] > 4 || bytes[5] != 0 || bytes[6] != 0 || bytes[7] != 0;
            if (t2 == false)
            {
                br.BaseStream.Position = t;
                return;
            }
            var index = -4;
            t2 = bytes[4 + index] < 1 || bytes[4 + index] > 4 || bytes[5 + index] != 0 || bytes[6 + index] != 0 || bytes[7 + index] != 0;
            while (t2)
            {
                index++;
                t2 = bytes[4 + index] < 1 || bytes[4 + index] > 4 || bytes[5 + index] != 0 || bytes[6 + index] != 0 || bytes[7 + index] != 0;
            }

            br.BaseStream.Position = t + index;
        }
        #endregion
    }
}
