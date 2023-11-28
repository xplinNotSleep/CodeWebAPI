using AgDataHandle.Maths;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AgDataHandle.BaseService.CommonHelper
{
    public class EncodingHelper
    {
        #region StringEncoding & StringConvertion

        public static byte[] ChangeEndian(byte[] array)
        {
            var resultList = new List<byte>(array);
            resultList.Reverse();
            return resultList.ToArray();
        }

        public static string BytesToString(byte[] byteArray)
        {
            return Encoding.UTF8.GetString(byteArray);
        }

        public static byte[] StringToBytes(string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        public static byte[] JsonFileToBytes(string path)
        {
            JObject jObject = null;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                StreamReader streamReader = new StreamReader(fileStream);
                JsonReader jsReader = new JsonTextReader(streamReader as TextReader);
                jObject = JToken.ReadFrom(jsReader) as JObject;
            }
            string jsonStr = JsonConvert.SerializeObject(jObject);

            return JsonTextToBytes(jsonStr);
        }

        public static byte[] JsonTextToBytes(string jsonStr)
        {
            return Encoding.Default.GetBytes(jsonStr);
        }

        #endregion

        #region Streams & Bytes

        /// <summary>
        /// 补齐空格byte[]数组至4byte的倍数
        /// </summary>
        /// <param name="byteArray">待补齐的byte数组</param>
        /// <returns>补齐后的byte数组</returns>
        public static byte[] FillBlank(byte[] byteArray, int bound = 4)
        {
            int remain = byteArray.Length % bound;
            if (remain == 0)
                return byteArray;

            StringBuilder fillContent = new StringBuilder("");
            for (int i = 0; i < bound - remain; i++)
            {
                fillContent.Append(" ");
            }

            return ArrayHelper.MergeArray(byteArray, StringToBytes(fillContent.ToString()));
        }

        public static byte[] ListToBytes(List<ushort> ls)
        {
            List<byte> result = new List<byte>();
            foreach (var element in ls)
            {
                result.AddRange(BitConverter.GetBytes(element));
            }
            return result.ToArray();
        }

        public static byte[] ListToBytes(List<float> ls)
        {
            List<byte> result = new List<byte>();
            foreach (var element in ls)
            {
                result.AddRange(BitConverter.GetBytes(element));
            }
            return result.ToArray();
        }

        public static byte[] ListToBytes(List<uint> ls)
        {
            List<byte> result = new List<byte>();
            foreach (var element in ls)
            {
                result.AddRange(BitConverter.GetBytes(element));
            }
            return result.ToArray();
        }

        public static byte[] ListToBytes(List<Vector3> ls)
        {
            List<float> expandLs = new List<float>();
            foreach (var v in ls)
            {
                expandLs.Add(v.X);
                expandLs.Add(v.Y);
                expandLs.Add(v.Z);
            }
            return ListToBytes(expandLs);
        }

        public static byte[] ListToBytes(List<Vector2> ls)
        {
            List<float> expandLs = new List<float>();
            foreach (var v in ls)
            {
                expandLs.Add(v.X);
                expandLs.Add(v.Y);
            }
            return ListToBytes(expandLs);
        }

        public static MemoryStream BytesToMemoryStream(byte[] data, int from = 0, int length = -1)
        {
            length = length == -1 ? data.Length : length;
            MemoryStream ms = new MemoryStream(data, from, length);
            return ms;
        }

        public static byte[] MemoryStreamToBytes(MemoryStream ms)
        {
            return ms.ToArray();
        }

        public static byte[] ReadByteFromStream(Stream s, int start = 0, int length = -1, bool isResetPosition = true)
        {
            if (!s.CanRead)
                return null;
            if (length == -1)
                length = (int)s.Length;

            int startPosition = (int)s.Position;
            s.Position += start;

            byte[] data = new byte[length];
            s.Read(data, 0, length);
            if (isResetPosition)
                s.Position = startPosition;

            return data;
        }
        public static byte[] ToU32Buffer(int[] arr)
        {
            var bytes = new List<byte>();
            foreach (var i in arr)
            {
                bytes.AddRange(BitConverter.GetBytes(i));
            }
            return bytes.ToArray();
        }

        public static byte[] ToU16Buffer(int[] arr)
        {
            var bytes = new List<byte>();
            foreach (var i in arr)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)i));
            }
            return bytes.ToArray();
        }
        #endregion
    }
}
