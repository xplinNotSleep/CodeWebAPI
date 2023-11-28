using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 用于加速寻找相交的包围盒
    /// </summary>
    public class BoxFindHelper
    {
        private List<BoundingBox> m_boxList;
        /// <summary>
        /// xoy平面上的网格，忽略Z值
        /// </summary>
        private BoxGrid[,] m_boxGridXOY;
        /// <summary>
        /// xyz三维空间的网格
        /// </summary>
        private BoxGrid[,,] m_boxGridXYZ;
        /// <summary>
        /// 网格大小
        /// </summary>
        private float m_gridSize;
        /// <summary>
        /// 总的包围盒
        /// </summary>
        private BoundingBox m_allBox;

        public BoxFindHelper(List<BoundingBox> boundingBoxes,float gridSize)
        {
            m_boxList = boundingBoxes;
            m_gridSize = gridSize;
        }

        /// <summary>
        /// 寻找相交的包围盒
        /// </summary>
        /// <param name="point"></param>
        /// <returns>包围盒的索引</returns>
        public List<int> FindIntersectingBoxInXOY(Vector2 point)
        {
            if (m_boxGridXOY == null)
                m_boxGridXOY = BuildGridInXOY();
            var xIndex =(int)((point.X - m_allBox.Min.X) / m_gridSize);
            var yIndex = (int)((point.Y - m_allBox.Min.Y) / m_gridSize);
            if (m_boxGridXOY[xIndex, yIndex] == null)
                return new List<int>();
            return m_boxGridXOY[xIndex,yIndex].BoxList;
        }

        /// <summary>
        /// 构建在XOY平面的网格
        /// </summary>
        private BoxGrid[,] BuildGridInXOY()
        {
            m_allBox = new BoundingBox();
            foreach (var item in m_boxList)
            {
                m_allBox.Update(item);
            }
            var min = m_allBox.Min;
            var xSize = (int)Math.Ceiling(m_allBox.Width / m_gridSize)+1;
            var ySize = (int)Math.Ceiling(m_allBox.Height / m_gridSize) + 1;
            var grid = new BoxGrid[xSize, ySize];
            for (int i = 0; i < m_boxList.Count; i++)
            {
                var bMin = m_boxList[i].Min - min;
                var bMax = m_boxList[i].Max - min;
                var xMin = (int)Math.Floor(bMin.X / m_gridSize);
                var xMax = (int)Math.Floor(bMax.X / m_gridSize);
                var yMin = (int)Math.Floor(bMin.Y / m_gridSize);
                var yMax = (int)Math.Floor(bMax.Y / m_gridSize);
                for (int x = xMin; x <= xMax; x++)
                {
                    for (int y = yMin; y <= yMax; y++)
                    {
                        if (grid[x, y] == null)
                            grid[x, y] = new BoxGrid();
                        grid[x, y].BoxList.Add(i);
                    }
                }
            }
            return grid;
        }

    }

    /// <summary>
    /// 包围盒网格
    /// </summary>
    class BoxGrid
    {
        /// <summary>
        /// 存储的包围盒索引
        /// </summary>
        public List<int> BoxList { get; set; }=new List<int>();
    }
}
