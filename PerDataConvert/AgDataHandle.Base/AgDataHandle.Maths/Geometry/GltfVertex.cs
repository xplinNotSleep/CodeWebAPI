using AgDataHandle.Maths;

namespace AgDataHandle.Maths.Geometry
{
    /// <summary>
    /// 附带batchID的顶点数据
    /// </summary>
    public class GltfVertex
    {
        public Vertex Position { get; set; }
        public int BatchID { get; set; }
        public Vector3 Normal { get; set; } = new Vector3(1, 1, 1);
        public Vector2 UV { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="id">batchID</param>
        public GltfVertex(Vertex position, int id)
        {
            Position = position;
            BatchID = id;
        }
    }
}
