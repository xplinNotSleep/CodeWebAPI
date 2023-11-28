using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GltfSdk.Generator
{
    internal abstract class GLTFBuilder
    {
        public GLTFBuilder()
        {

        }

        protected virtual GLTFBufferView CreateBufferView(
            GLTF model,
            int byteLength,
            int target = 34962,
            int offset = -1)
        {
            GLTFBufferView bv = new GLTFBufferView
            {
                GLTF = model,
                BufferIndex = 0,
                ByteLength = byteLength
            };

            if (target == 34962 || target == 34963)
                bv.Target = target;

            if (offset > 0)
                bv.ByteOffset = offset;

            return bv;
        }

        protected virtual GLTFAccessor CreateAccessor(
            GLTFBufferView bv,
            string type,
            int componentType,
            int count,
            List<float> max = null,
            List<float> min = null)
        {
            GLTFAccessor acc = new GLTFAccessor
            {
                GLTF = bv.GLTF,
                Type = type,
                ComponentType = componentType,
                BufferViewIndex = bv.GLTF.BufferViews.IndexOf(bv),
                Max = max,
                Min = min,
                Count = count
            };
            return acc;
        }
    }
}
