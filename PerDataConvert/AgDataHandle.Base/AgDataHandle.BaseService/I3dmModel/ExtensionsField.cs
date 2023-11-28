using AgDataHandle.Maths.Numerics;

namespace AgDataHandle.BaseService.I3dmModel
{
    public class ExtensionsField
    {
        public BoundingBox boundingBox;
    }

    public static class ExtensionsFieldExtensions
    {
        public static byte[] ToBytes(this ExtensionsField field)
        {
            var res = new List<byte>();
            res.AddRange(field.boundingBox.Min.ToBytes());
            res.AddRange(field.boundingBox.Max.ToBytes());
            return res.ToArray();
        }

        public static byte[] ToBytes(this IEnumerable<ExtensionsField> fields)
        {
            var bytes = new List<byte>();
            foreach (var field in fields)
            {
                bytes.AddRange(field.ToBytes());
            }
            return bytes.ToArray();
        }
    }
}
