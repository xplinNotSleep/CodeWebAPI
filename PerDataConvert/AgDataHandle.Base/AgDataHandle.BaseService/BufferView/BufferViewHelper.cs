using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;

namespace AgDataHandle.BaseService.BufferView
{
    public class BufferViewHelper
    {
        /// <summary>
        /// padding buffers with boundary
        /// </summary>
        /// <param name="buffers"></param>
        /// <param name="boundary"></param>
        public static void PaddingBuffers(List<byte> buffers, int boundary = 4)
        {
            var length = buffers.Count;
            var remainder = length % boundary;
            if (remainder != 0)
            {
                var padding = boundary - remainder;
                for (var i = 0; i < padding; i++)
                {
                    buffers.Add(0);
                }
            }
        }
        public static GLTFBufferView CreateBufferView(BufferViewUsage buf_time, int byteOffset, int byteLength, int byteStride = 0)
        {
            GLTFBufferView bfv = new GLTFBufferView();
            bfv.BufferIndex = 0;
            if (buf_time == BufferViewUsage.Triangle || buf_time == BufferViewUsage.BorderLine)
            {
                bfv.Target = GLTFTargets.ELEMENT_ARRAY_BUFFER.GetHashCode();
            }
            else if (buf_time == BufferViewUsage.Vertical || buf_time == BufferViewUsage.Normal || buf_time == BufferViewUsage.UV || buf_time == BufferViewUsage.BatchId || buf_time == BufferViewUsage.Colors || buf_time == BufferViewUsage.UVRegions)
            {
                bfv.Target = GLTFTargets.ARRAY_BUFFER.GetHashCode();
            }
            bfv.ByteStride = byteStride;
            bfv.ByteOffset = byteOffset;
            bfv.ByteLength = byteLength;
            return bfv;
        }

        public static GLTFBufferView CreateBufferView(GLTFTargets target, int byteOffset, int byteLength, int byteStride = 0)
        {
            GLTFBufferView bfv = new GLTFBufferView();
            bfv.BufferIndex = 0;
            bfv.Target = target.GetHashCode();
            //bfv.ByteStride = byteStride;
            bfv.ByteOffset = byteOffset;
            bfv.ByteLength = byteLength;
            return bfv;
        }
    }

    public enum BufferViewUsage
    {
        Vertical = 0,
        UV = 1,
        Normal = 2,
        Triangle = 3,
        BatchId = 4,
        BorderLine = 5,
        Colors = 7,
        UVRegions = 8
    }
}
