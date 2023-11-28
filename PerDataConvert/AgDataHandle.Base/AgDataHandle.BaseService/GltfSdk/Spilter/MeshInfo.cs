using AgDataHandle.Maths;

namespace AgDataHandle.BaseService.GltfSdk.Spilter
{
    public class MeshInfo
    {
        #region 属性
        public int MeshId { get; set; }
        public int NodeId { get; set; }
        public string MeshType { get; set; }
        public bool IsSimilar { get; set; }
        public List<IMatrix4x4> SimilarMatrixs { get; set; }
        public List<int> NodeIds { get; set; }
        public IMatrix4x4 Matrix { get; set; }
        //public Vector3 Translation { get; set; }
        #endregion
    }
}
