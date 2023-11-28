using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialStructure
{
    /// <summary>
    /// 空间拓扑网络
    /// </summary>
    public class SpatialTopologicalNetwork
    {
        private SpatialTopologicalObject[,,] m_dataSource;
        private int m_xSize;
        private int m_ySize;
        private int m_zSize;

        public SpatialTopologicalNetwork(SpatialTopologicalObject[,,] spatialTopologicalObjects)
        {
            m_dataSource = spatialTopologicalObjects;
            m_xSize = m_dataSource.GetLength(0);
            m_ySize = m_dataSource.GetLength(1);
            m_zSize = m_dataSource.GetLength(2);
        }

        /// <summary>
        /// 构建拓扑关系
        /// </summary>
        public void BuildTopotaxy()
        {
            for (int x = 0; x < m_xSize; x++)
            {
                for (int y = 0; y < m_ySize; y++)
                {
                    for (int z = 0; z < m_zSize; z++)
                    {
                        if (m_dataSource[x, y, z] == null)
                            continue;
                        m_dataSource[x, y, z].HaveTop = HaveTop(x,y,z);
                        m_dataSource[x, y, z].HaveBottom = HaveBottom(x, y, z);
                        m_dataSource[x, y, z].HaveLeft = HaveLeft(x, y, z);
                        m_dataSource[x, y, z].HaveRight = HaveRight(x, y, z);
                        m_dataSource[x, y, z].HaveFront = HaveFront(x, y, z);
                        m_dataSource[x, y, z].HaveBack = HaveBack(x, y, z);
                    }
                }
            }
        }

        public bool HaveTop(int x,int y,int z)
        {
            if(z+1>=m_zSize)
                return false;
            return m_dataSource[x, y, z+1] != null;
        }
        public bool HaveBottom(int x, int y, int z)
        {
            if (z - 1 < 0)
                return false;
            return m_dataSource[x, y, z-1] != null;
        }
        public bool HaveLeft(int x, int y, int z)
        {
            if (x - 1 < 0)
                return false;
            return m_dataSource[x-1, y, z] != null;
        }
        public bool HaveRight(int x, int y, int z)
        {
            if (x + 1 >= m_xSize)
                return false;
            return m_dataSource[x + 1, y, z] != null;
        }
        public bool HaveFront(int x, int y, int z)
        {
            if (y - 1 < 0)
                return false;
            return m_dataSource[x , y - 1, z] != null;
        }
        public bool HaveBack(int x, int y, int z)
        {
            if (y + 1 >= m_ySize)
                return false;
            return m_dataSource[x, y + 1, z] != null;
        }
    }
    /// <summary>
    /// 空间拓扑对象
    /// </summary>
    public class SpatialTopologicalObject
    {
        public bool HaveTop { get; set; } = false;
        public bool HaveBottom { get; set; } = false;
        public bool HaveLeft { get; set; } = false;
        public bool HaveRight { get; set; } = false;
        public bool HaveFront { get; set; } = false;
        public bool HaveBack { get; set; } = false;
    }
}
