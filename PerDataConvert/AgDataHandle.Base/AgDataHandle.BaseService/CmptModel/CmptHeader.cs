using System.Text;

namespace AgDataHandle.BaseService.CmptModel
{
    public class CmptHeader
    {
        public string Magic { get; set; }
        public int Version { get; set; }
        public int ByteLength { get; set; }
        public int TilesLength { get; set; }


        public CmptHeader()
        {
            Magic = "cmpt";
            Version = 1;
            ByteLength = 0;
            TilesLength = 0;
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Encoding.UTF8.GetBytes(Magic).ToList());
            bytes.AddRange(BitConverter.GetBytes(Version));
            bytes.AddRange(BitConverter.GetBytes(ByteLength));
            bytes.AddRange(BitConverter.GetBytes(TilesLength));

            return bytes.ToArray();
        }
    }

    public static class BufferPadding
    {
        private static int boundary = 8;
        public static byte[] AddPadding(byte[] bytes, int offset = 0)
        {
            if (bytes == null) bytes = new byte[0];
            var remainder = (offset + bytes.Length) % boundary;
            var padding = remainder == 0 ? 0 : boundary - remainder;
            var whitespace = new string(' ', padding);
            var paddingBytes = Encoding.UTF8.GetBytes(whitespace);
            var res = bytes.Concat(paddingBytes);
            return res.ToArray();
        }
        public static string AddPadding(string input, int offset = 0)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var paddedBytes = AddPadding(bytes, offset);
            var result = Encoding.UTF8.GetString(paddedBytes);
            return result;
        }
    }
}
