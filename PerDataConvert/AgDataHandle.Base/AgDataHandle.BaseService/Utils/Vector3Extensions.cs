using AgDataHandle.Maths;

namespace AgDataHandle.BaseService.Utils
{
    internal static class Vector3Extensions
    {
        public static byte[] ToBytes(this Vector3 vector)
        {
            var res = new List<byte>();
            res.AddRange(BitConverter.GetBytes(vector.X));
            res.AddRange(BitConverter.GetBytes(vector.Y));
            res.AddRange(BitConverter.GetBytes(vector.Z));
            return res.ToArray();
        }

        public static byte[] ToBytes(this IEnumerable<Vector3> vectors)
        {
            var bytes = new List<byte>();
            foreach (var vector in vectors)
            {
                bytes.AddRange(vector.ToBytes());
            }
            return bytes.ToArray();
        }
        public static List<Vector3> FromBytes(byte[] Binary, int offset, int vecCount)
        {
            List<Vector3> result = new List<Vector3>();

            var bytes = Binary.Skip(offset).Take(vecCount * 12).ToArray();
            var reader = new BinaryReader(new MemoryStream(bytes));
            for (int i = 0; i < vecCount; i++)
            {
                var vec = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                result.Add(vec);
            }
            return result;
        }
        public static Vector3 Transform(this Vector3 position, Matrix4x4 matrix)
        {
            return new Vector3((float)(position.X * matrix[0, 0] + position.Y * matrix[1, 0] + position.Z * matrix[2, 0] + matrix[3, 0]),
                              (float)(position.X * matrix[0, 1] + position.Y * matrix[1, 1] + position.Z * matrix[2, 1] + matrix[3, 1]),
                              (float)(position.X * matrix[0, 2] + position.Y * matrix[1, 2] + position.Z * matrix[2, 2] + matrix[3, 2]));
        }
        public static Vector3 Transform(this Vector3 position, float[] matrix)
        {
            return new Vector3(position.X * matrix[0] + position.Y * matrix[4] + position.Z * matrix[8] + matrix[12],
                               position.X * matrix[1] + position.Y * matrix[5] + position.Z * matrix[9] + matrix[13],
                               position.X * matrix[2] + position.Y * matrix[6] + position.Z * matrix[10] + matrix[14]);
        }
    }
}
