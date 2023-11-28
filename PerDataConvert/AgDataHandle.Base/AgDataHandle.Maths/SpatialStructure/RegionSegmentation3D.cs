using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 基于区域蔓延的区域分割算法
    /// </summary>
    public class RegionSegmentation3D
    {
        private Stack<int> m_stackX = new Stack<int>();
        private Stack<int> m_stackY = new Stack<int>();
        private Stack<int> m_stackZ = new Stack<int>();
        private bool[,,] m_marked;
        private int m_xSize;
        private int m_ySize;
        private int m_zSize;
        private delegate void CheckDelegate(Object[,,] region, int xIndex, int yIndex, int zIndex, ref List<Vector3Int> group);

        public List<RegionGroup> Segment(Object[,,] region)
        {
            m_xSize = region.GetLength(0);
            m_ySize = region.GetLength(1);
            m_zSize = region.GetLength(2);
            m_marked = new bool[m_xSize, m_ySize, m_zSize];
            var groups = new List<RegionGroup>();

            for (int z = 0; z < m_zSize; z++)
            {
                for (int y = 0; y < m_ySize; y++)
                {
                    for (int x = 0; x < m_xSize; x++)
                    {
                        if (m_marked[x, y, z])
                            continue;
                        if (region[x, y, z] == null)
                            continue;

                        var list = GrowingForAll(region, x, y, z);
                        if (list.Count <= 0)
                            continue;
                        var group = new RegionGroup();
                        group.Add(list);
                        groups.Add(group);
                    }
                }
            }

            return groups;
        }

        /// <summary>
        /// 区域分割，考虑一阶邻域内的对象
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public List<RegionGroup> SegmentInneighbourhood(Object[,,] region)
        {
            m_xSize = region.GetLength(0);
            m_ySize = region.GetLength(1);
            m_zSize = region.GetLength(2);
            m_marked = new bool[m_xSize, m_ySize, m_zSize];
            var groups = new List<RegionGroup>();

            for (int z = 0; z < m_zSize; z++)
            {
                for (int y = 0; y < m_ySize; y++)
                {
                    for (int x = 0; x < m_xSize; x++)
                    {
                        if (m_marked[x, y, z])
                            continue;
                        if (region[x, y, z] == null)
                            continue;

                        var list = GrowingInNeighbourhood(region, x, y, z);
                        if (list.Count <= 0)
                            continue;
                        var group = new RegionGroup();
                        group.Add(list);
                        groups.Add(group);
                    }
                }
            }

            return groups;
        }


        /// <summary>
        /// 区域分割
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public List<RegionGroup> Segment(Object[,,] region, SegmentType segmentType)
        {
            m_xSize = region.GetLength(0);
            m_ySize = region.GetLength(1);
            m_zSize = region.GetLength(2);
            m_marked = new bool[m_xSize, m_ySize, m_zSize];
            var groups = new List<RegionGroup>();

            for (int z = 0; z < m_zSize; z++)
            {
                for (int y = 0; y < m_ySize; y++)
                {
                    for (int x = 0; x < m_xSize; x++)
                    {
                        if (m_marked[x, y, z])
                            continue;
                        if (region[x, y, z] == null)
                            continue;

                        var list = Growing(region, x, y, z, segmentType);
                        if (list.Count <= 0)
                            continue;
                        var group = new RegionGroup();
                        group.Add(list);
                        groups.Add(group);
                    }
                }
            }

            return groups;
        }

        /// <summary>
        /// 寻找组
        /// </summary>
        /// <param name="p"></param>
        /// <param name="xIndex"></param>
        /// <param name="yIndex"></param>
        /// <param name="newValue"></param>
        private List<Vector3Int> Growing(Object[,,] region, int xIndex, int yIndex, int zIndex,SegmentType segmentType)
        {
            m_stackX.Clear();
            m_stackY.Clear();
            m_stackZ.Clear();
            m_stackX.Push(xIndex);
            m_stackY.Push(yIndex);
            m_stackZ.Push(zIndex);
            var group = new List<Vector3Int>();
            group.Add(new Vector3Int(xIndex,yIndex,zIndex));
            m_marked[xIndex, yIndex, zIndex] = true;
            CheckDelegate checkDelegate = null;
            switch (segmentType)
            {
                case SegmentType.PerpendicularXAxis:
                    checkDelegate = CheckForXDir;
                    break;
                case SegmentType.PerpendicularYAxis:
                    checkDelegate = CheckForYDir;
                    break;
                case SegmentType.PerpendicularZAxis:
                    checkDelegate = CheckForZDir;
                    break;
            }
            while (m_stackX.Count > 0 && m_stackY.Count > 0 && m_stackZ.Count > 0)
            {
                xIndex = m_stackX.Pop();
                yIndex = m_stackY.Pop();
                zIndex = m_stackZ.Pop();
                checkDelegate(region,xIndex,yIndex,zIndex,ref group);
            }
            return group;
        }
        private List<Vector3Int> GrowingForAll(Object[,,] region, int xIndex, int yIndex, int zIndex)
        {
            m_stackX.Clear();
            m_stackY.Clear();
            m_stackZ.Clear();
            m_stackX.Push(xIndex);
            m_stackY.Push(yIndex);
            m_stackZ.Push(zIndex);
            var group = new List<Vector3Int>();
            group.Add(new Vector3Int(xIndex, yIndex, zIndex));
            m_marked[xIndex, yIndex, zIndex] = true;
            while (m_stackX.Count > 0 && m_stackY.Count > 0 && m_stackZ.Count > 0)
            {
                xIndex = m_stackX.Pop();
                yIndex = m_stackY.Pop();
                zIndex = m_stackZ.Pop();
                Check(region, xIndex+1, yIndex, zIndex, ref group);
                Check(region, xIndex-1, yIndex, zIndex, ref group);
                Check(region, xIndex, yIndex + 1, zIndex, ref group);
                Check(region, xIndex, yIndex - 1, zIndex, ref group);
                Check(region, xIndex, yIndex, zIndex + 1, ref group);
                Check(region, xIndex, yIndex, zIndex - 1, ref group);
            }
            return group;
        }
        private List<Vector3Int> GrowingInNeighbourhood(Object[,,] region, int xIndex, int yIndex, int zIndex)
        {
            m_stackX.Clear();
            m_stackY.Clear();
            m_stackZ.Clear();
            m_stackX.Push(xIndex);
            m_stackY.Push(yIndex);
            m_stackZ.Push(zIndex);
            var group = new List<Vector3Int>();
            group.Add(new Vector3Int(xIndex, yIndex, zIndex));
            m_marked[xIndex, yIndex, zIndex] = true;
            while (m_stackX.Count > 0 && m_stackY.Count > 0 && m_stackZ.Count > 0)
            {
                xIndex = m_stackX.Pop();
                yIndex = m_stackY.Pop();
                zIndex = m_stackZ.Pop();
                Check(region, xIndex + 1, yIndex, zIndex, ref group);
                Check(region, xIndex - 1, yIndex, zIndex, ref group);
                Check(region, xIndex, yIndex + 1, zIndex, ref group);
                Check(region, xIndex, yIndex - 1, zIndex, ref group);
                Check(region, xIndex, yIndex, zIndex + 1, ref group);
                Check(region, xIndex, yIndex, zIndex - 1, ref group);

                Check(region, xIndex-1, yIndex-1, zIndex, ref group);
                Check(region, xIndex-1, yIndex+1, zIndex, ref group);
                Check(region, xIndex+1, yIndex+1, zIndex, ref group);
                Check(region, xIndex+1, yIndex-1, zIndex, ref group);

                Check(region, xIndex - 1, yIndex, zIndex - 1, ref group);
                Check(region, xIndex - 1, yIndex, zIndex + 1, ref group);
                Check(region, xIndex + 1, yIndex, zIndex + 1, ref group);
                Check(region, xIndex + 1, yIndex, zIndex - 1, ref group);

                Check(region, xIndex, yIndex  - 1, zIndex- 1, ref group);
                Check(region, xIndex, yIndex  - 1, zIndex+ 1, ref group);
                Check(region, xIndex, yIndex  + 1, zIndex+ 1, ref group);
                Check(region, xIndex, yIndex + 1, zIndex - 1, ref group);
            }
            return group;
        }

        private void CheckForXDir(Object[,,] region, int xIndex, int yIndex, int zIndex,ref List<Vector3Int> group)
        {
            Check(region, xIndex, yIndex + 1, zIndex, ref group);
            Check(region, xIndex, yIndex - 1, zIndex, ref group);
            Check(region, xIndex, yIndex, zIndex + 1, ref group);
            Check(region, xIndex, yIndex, zIndex - 1, ref group);
        }
        private void CheckForYDir(Object[,,] region, int xIndex, int yIndex, int zIndex, ref List<Vector3Int> group)
        {
            Check(region, xIndex + 1, yIndex, zIndex, ref group);
            Check(region, xIndex - 1, yIndex, zIndex, ref group);
            Check(region, xIndex, yIndex, zIndex + 1, ref group);
            Check(region, xIndex, yIndex, zIndex - 1, ref group);
        }
        private void CheckForZDir(Object[,,] region, int xIndex, int yIndex, int zIndex, ref List<Vector3Int> group)
        {
            Check(region, xIndex, yIndex + 1, zIndex, ref group);
            Check(region, xIndex, yIndex - 1, zIndex, ref group);
            Check(region, xIndex + 1, yIndex, zIndex, ref group);
            Check(region, xIndex - 1, yIndex, zIndex, ref group);
        }

        private void Check(Object[,,] p, int x, int y,int z,ref List<Vector3Int> group)
        {
            if (!BaseCheck(p, x, y, z))
                return;
           
            if (CanAdd(x,y,z))
            {
                group.Add(new Vector3Int(x,y,z));
                m_marked[x, y, z] = true;
                m_stackX.Push(x);
                m_stackY.Push(y);
                m_stackZ.Push(z);
            }
        }

        /// <summary>
        /// 基本检查，所有对象都会执行该方法
        /// </summary>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private bool BaseCheck(Object[,,] p, int x, int y, int z)
        {
            if (x < 0 || x >= m_xSize || y < 0 || y >= m_ySize || z < 0 || z >= m_zSize)
                return false;
            if (p[x, y, z] == null)
                return false;
            if (m_marked[x, y, z])
                return false;
            return true;
        }

        /// <summary>
        /// 特定对象的检查，以检查该点是否可以添加至分割组
        /// </summary>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public virtual bool CanAdd(int x,int y,int z)
        {
            return true;
        }

    }
    
    /// <summary>
    /// 分割后的组
    /// </summary>
    public class RegionGroup
    {
        public List<Vector3Int> Content { get;private set; } = new List<Vector3Int>();

        public void Add(int x,int y,int z)
        {
            Content.Add(new Vector3Int(x,y,z));
        }
        public void Add(Vector3Int vector3Int)
        {
            Content.Add(vector3Int);
        }
        public void Add(List<Vector3Int> list)
        {
            Content.AddRange(list);
        }
    }

    public enum SegmentType
    {
        [Description("垂直于x轴分割")]
        PerpendicularXAxis,
        [Description("垂直于y轴分割")]
        PerpendicularYAxis,
        [Description("垂直于z轴分割")]
        PerpendicularZAxis
    }
}
