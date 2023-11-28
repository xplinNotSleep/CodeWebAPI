using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;

namespace AgDataHandle.BaseService.BufferView
{
    public class AccessorHelper
    {
        public static GLTFAccessor CreateAccessor(int bufferViewIndex, int count, List<float> maxValues, List<float> minValues, BufferViewUsage buf_time, int nByteOffset = 0, bool IsUseUShort = false)
        {
            GLTFAccessor accessor = new GLTFAccessor();
            accessor.BufferViewIndex = bufferViewIndex;
            accessor.ByteOffset = nByteOffset;
            accessor.Count = count;
            if (buf_time == BufferViewUsage.Vertical || buf_time == BufferViewUsage.Normal)
            {
                accessor.ComponentType = GLTFAccessorComponentType.FLOAT.GetHashCode();
                accessor.Type = GLTFAccessorType.VEC3.ToString();
            }
            else if (buf_time == BufferViewUsage.Triangle || buf_time == BufferViewUsage.BorderLine)
            {
                if (!IsUseUShort)
                {
                    accessor.ComponentType = GLTFAccessorComponentType.UNSIGNED_INT.GetHashCode();
                }
                else
                {
                    accessor.ComponentType = GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode();
                }
                accessor.Type = GLTFAccessorType.SCALAR.ToString();
            }
            else if (buf_time == BufferViewUsage.UV)
            {
                accessor.ComponentType = GLTFAccessorComponentType.FLOAT.GetHashCode();
                accessor.Type = GLTFAccessorType.VEC2.ToString();
            }
            else if (buf_time == BufferViewUsage.BatchId)
            {
                if (IsUseUShort)
                {
                    accessor.ComponentType = GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode();
                }
                else
                {
                    accessor.ComponentType = GLTFAccessorComponentType.UNSIGNED_INT.GetHashCode();
                }
                accessor.Type = GLTFAccessorType.SCALAR.ToString();
            }
            else if (buf_time == BufferViewUsage.Colors)
            {
                accessor.ComponentType = GLTFAccessorComponentType.FLOAT.GetHashCode();
                accessor.Type = GLTFAccessorType.VEC4.ToString();
            }
            else
            {
                accessor.ComponentType = GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode();
                accessor.Type = GLTFAccessorType.SCALAR.ToString();
            }
            accessor.Max = maxValues;
            accessor.Min = minValues;
            return accessor;
        }

        public static GLTFAccessor CreateAccessor(int bufferViewIndex, int count, List<float> maxValues, List<float> minValues, GLTFAccessorComponentType componentType, GLTFAccessorType accessorType, int nByteOffset = 0)
        {
            GLTFAccessor accessor = new GLTFAccessor();
            accessor.BufferViewIndex = bufferViewIndex;
            accessor.ByteOffset = nByteOffset;
            accessor.Count = count;
            accessor.ComponentType = componentType.GetHashCode();
            accessor.Type = accessorType.ToString();
            accessor.Max = maxValues;
            accessor.Min = minValues;
            return accessor;
        }

    }
}
