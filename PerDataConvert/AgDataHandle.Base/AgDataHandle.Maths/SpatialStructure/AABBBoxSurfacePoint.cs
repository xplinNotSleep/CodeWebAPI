using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AgDataHandle.Maths.SpatialStructure
{
    /// <summary>
    /// aabb包围盒表面离散点,坐标系是右手坐标系，z轴朝上
    /// </summary>
    public class AABBBoxSurfacePoint
    {
        private BoundingBox m_box;
        private float m_unit;
        private int m_xSize;
        private int m_ySize;
        private int m_zSize;
        private List<VertexWithNorml> m_points;

        public AABBBoxSurfacePoint(BoundingBox box, float unit)
        {
            m_box = box; 
            m_unit = unit;
            m_xSize = (int)Math.Ceiling((m_box.Width / m_unit)) + 1;
            m_ySize = (int)Math.Ceiling((m_box.Height / m_unit)) + 1;
            m_zSize = (int)Math.Ceiling((m_box.ZWidth / m_unit)) + 1;
        }

        public List<VertexWithNorml> GetSurfacePoint(AABBBoxSurfacePointType aABBBoxSurfacePointType)
        {
            m_points = new List<VertexWithNorml>();
            switch (aABBBoxSurfacePointType)
            {
                case AABBBoxSurfacePointType.NoBottom:
                    m_points.AddRange(GetTopSurfacePoint());
                    m_points.AddRange(GetLeftSurfacePoint());
                    m_points.AddRange(GetRightSurfacePoint());
                    m_points.AddRange(GetFrontSurfacePoint());
                    m_points.AddRange(GetBackSurfacePoint());
                    break;
                default:
                    break;
            }
            //DebugPoints(@"D:\模型数据\OBJ\光追体素重建测试\汤坑分\sample.obj");
            return m_points;
        }
        private List<VertexWithNorml> GetFrontSurfacePoint()
        {
            var ps = new List<VertexWithNorml>();
            for (int x = 0; x < m_xSize; x++)
            {
                for (int z = 0; z < m_zSize; z++)
                {
                    var xV = m_box.Min.X + x * m_unit;
                    var zV = m_box.Min.Z + z * m_unit;
                    var position = new Vector3(xV, m_box.MinY, zV);
                    ps.Add(new VertexWithNorml(position,new Vector3(0,-1,0)));
                }
            }
            return ps;
        }
        private List<VertexWithNorml> GetBackSurfacePoint()
        {
            var ps = new List<VertexWithNorml>();
            for (int x = 0; x < m_xSize; x++)
            {
                for (int z = 0; z < m_zSize; z++)
                {
                    var xV = m_box.Min.X + x * m_unit;
                    var zV = m_box.Min.Z + z * m_unit;

                    var position = new Vector3(xV, m_box.MaxY, zV);
                    ps.Add(new VertexWithNorml(position, new Vector3(0, 1, 0)));

                }
            }
            return ps;
        }
        private List<VertexWithNorml> GetLeftSurfacePoint()
        {
            var ps = new List<VertexWithNorml>();
            for (int y = 0; y < m_ySize; y++)
            {
                for (int z = 0; z < m_zSize; z++)
                {
                    var yV = m_box.Min.Y + y * m_unit;
                    var zV = m_box.Min.Z + z * m_unit;

                    var position = new Vector3(m_box.MinX, yV, zV);
                    ps.Add(new VertexWithNorml(position, new Vector3(-1, 0, 0)));

                }
            }
            return ps;
        }
        private List<VertexWithNorml> GetRightSurfacePoint()
        {
            var ps = new List<VertexWithNorml>();
            for (int y = 0; y < m_ySize; y++)
            {
                for (int z = 0; z < m_zSize; z++)
                {
                    var yV = m_box.Min.Y + y * m_unit;
                    var zV = m_box.Min.Z + z * m_unit;
                    var position = new Vector3(m_box.MaxX, yV, zV);
                    ps.Add(new VertexWithNorml(position, new Vector3(1, 0, 0)));
                }
            }
            return ps;
        }
        private List<VertexWithNorml> GetTopSurfacePoint()
        {
            var ps = new List<VertexWithNorml>();
            for (int y = 0; y < m_ySize; y++)
            {
                for (int x = 0; x < m_xSize; x++)
                {
                    var yV = m_box.Min.Y + y * m_unit;
                    var xV = m_box.Min.X + x * m_unit;

                    var position = new Vector3(xV, yV, m_box.MaxZ);
                    ps.Add(new VertexWithNorml(position, new Vector3(0, 0, 1)));
                }
            }
            return ps;
        }

        #region 仅调试
        public void DebugPoints(string path)
        {
            var sb = new StringBuilder();
            foreach (var item in m_points)
            {
                sb.AppendLine($"v {item.Position.X} {item.Position.Y} {item.Position.Z}");
            }
            File.WriteAllText(path, sb.ToString());
        }
        #endregion
    }
    public class VertexWithNorml
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; } 
        public VertexWithNorml(Vector3 position,Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
    }
    public enum AABBBoxSurfacePointType
    {
        NoBottom
    }
}
