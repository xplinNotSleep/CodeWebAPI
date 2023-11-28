using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    /*
     * 反序列化/序列化测试通过 2020年8月7日
     */

    [JsonArray(false)]
    public class GLTFPrimitiveCollection : GLTFCollection<GLTFPrimitive>
    {
        [JsonIgnore]
        private GLTF gltf = null;
        public GLTFPrimitiveCollection() { }
        public GLTFPrimitiveCollection(GLTF model)
        {
            GLTF = model;
        }
        [JsonIgnore]
        public GLTF GLTF
        {
            get
            {
                return gltf;
            }
            set
            {
                gltf = value;
                foreach (var el in ls)
                {
                    el.GLTF = value;
                }
            }
        }

        /// <summary>
        /// 返回此集合的一个深拷贝，不包含图元所引用的数据
        /// </summary>
        /// <returns></returns>
        public GLTFPrimitiveCollection Clone()
        {
            GLTFPrimitiveCollection priCol = new GLTFPrimitiveCollection(null);

            foreach (var pri in this)
            {
                priCol.Add(pri.Clone());
            }

            return priCol;
        }
    }
}
