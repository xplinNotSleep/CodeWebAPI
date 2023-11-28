using AgDataHandle.Maths;
using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Result
{
    /// <summary>
    /// 每一个node包含的几何结果集
    /// </summary>
    public class GLTFNodeResult
    {
        /// <summary>
        /// 生成的启动器集合
        /// </summary>
        public List<GLTFAccessor> gLTFAccessors { get; set; } = new List<GLTFAccessor>();
        /// <summary>
        /// 生成的bufferview集合
        /// </summary>
        public List<GLTFBufferView> gLTFBufferViews { get; set; } = new List<GLTFBufferView>();
        /// <summary>
        /// 生成的图元
        /// </summary>
        public GLTFPrimitiveCollection gLTFPrimitives { get; set; } = new GLTFPrimitiveCollection();
        /// <summary>
        /// 每个节点的扩展信息
        /// </summary>
        public Dictionary<string, JObject> Extensions { get; set; }
        /// <summary>
        /// 当前图元的二进制数据
        /// </summary>
        public List<byte> BinaryData { get; set; } = new List<byte>();
        /// <summary>
        /// 当前图元的二进制数据大小
        /// </summary>
        public int BinaryDataLength { get; set; }
        /// <summary>
        /// 是否相似
        /// </summary>
        public bool IsSimlilar { get; set; } = false;
        /// <summary>
        /// 矩阵
        /// </summary>
        public List<IMatrix4x4> SimlilarMatrixs { get; set; }

        /// <summary>
        /// 包围盒
        /// </summary>
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }
        public double MinZ { get; set; }
        public double MaxZ { get; set; }

    }
}
