using System.Text;

namespace NetTopologySuite.GdalEx.Extension
{
    internal class EncodingUtils
    {
        static Encoding utf8 = Encoding.UTF8;
        static Encoding gb2312 = Encoding.GetEncoding("GB2312");
        internal static string Utf8ToGb2312(string str)
        {
            byte[] gb = utf8.GetBytes(str);
            return gb2312.GetString(gb);
        }

        internal static string Gb2312ToUtf8(string str)
        {
            byte[] gb = gb2312.GetBytes(str);
            return utf8.GetString(gb);
        }
    }
}
