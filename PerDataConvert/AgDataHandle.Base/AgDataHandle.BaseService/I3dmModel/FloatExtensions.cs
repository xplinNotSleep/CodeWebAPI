namespace AgDataHandle.BaseService.I3dmModel
{
    public static class FloatExtensions
    {
        public static byte[] ToBytes(this IEnumerable<float> fs)
        {
            List<byte> bytes = new List<byte>();
            foreach (float f in fs)
            {
                bytes.AddRange(BitConverter.GetBytes(f));
            }
            return bytes.ToArray();
        }
    }
}
