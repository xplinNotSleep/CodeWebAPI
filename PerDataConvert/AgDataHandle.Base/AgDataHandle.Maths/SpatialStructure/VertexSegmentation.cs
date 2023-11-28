using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 顶点聚类分割
    /// </summary>
    public class VertexSegmentation
    {
        /// <summary>
        /// 网格大小
        /// </summary>
        private float m_cellSize;
        /// <summary>
        /// 原始点集
        /// </summary>
        private List<Vector3> m_sourcePoints;
        /// <summary>
        /// 顶点簇的包围盒
        /// </summary>
        public BoundingBox VertexClusterBox { get;private set; }

        public VertexSegmentation(List<Vector3> points,float cellSize)
        {
            m_cellSize = cellSize;
            m_sourcePoints = points;
        }

        /// <summary>
        /// 执行分割
        /// </summary>
        /// <returns>返回分割聚类后顶点的簇数量</returns>
        public List<RegionGroup> Execute()
        {
            var totalBox = new BoundingBox();
            totalBox.Update(m_sourcePoints);
            var min  =totalBox.Min;
            var xSzie = (int)Math.Ceiling(totalBox.Width / m_cellSize) + 1;
            var ySzie = (int)Math.Ceiling(totalBox.Height / m_cellSize) + 1;
            var zSzie = (int)Math.Ceiling(totalBox.ZWidth / m_cellSize) + 1;
            var clusters = new VertexCluster[xSzie,ySzie,zSzie];
            foreach (var item in m_sourcePoints)
            {
                var index = (item - min) / m_cellSize;
                var xIndex = (int)index.X;
                var yIndex = (int)index.Y;
                var zIndex = (int)index.Z;
                if (clusters[xIndex, yIndex, zIndex] == null)
                    clusters[xIndex, yIndex, zIndex] = new VertexCluster();
                clusters[xIndex, yIndex, zIndex].Points.Add(item);
            }
            var regionSegmentation3D = new RegionSegmentation3D();
            var list = regionSegmentation3D.SegmentInneighbourhood(clusters);
            VertexClusterBox = ComputeVertexClusterBox(list[0].Content, clusters);
            return list;
        }

        /// <summary>
        /// 计算顶点簇的包围盒
        /// </summary>
        /// <param name="vector3Ints"></param>
        /// <param name="vertexClusters"></param>
        /// <returns></returns>
        private BoundingBox ComputeVertexClusterBox(List<Vector3Int> vector3Ints, VertexCluster[,,] vertexClusters)
        {
            var box = new BoundingBox();
            foreach (var item in vector3Ints)
            {
                box.Update(vertexClusters[item.X,item.Y,item.Z].Points);
            }
            return box;
        }
    }
    /// <summary>
    /// 顶点簇
    /// </summary>
    public class VertexCluster
    {
        /// <summary>
        /// 点集
        /// </summary>
        public List<Vector3> Points { get; set; } = new List<Vector3>();
    }

}
