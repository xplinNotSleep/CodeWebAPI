using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AgDataHandle.Maths.Geometry
{
    public class Polygon : IDisposable
    {
        public Polygon()
        { 
        }
        public Polygon(Polygon other)
        {
            this.Points = other.Points;
            this.ID = other.ID;
        }
        public Polygon(IEnumerable<Vector2d> points)
        {
            this.Points = points.ToList();
        }

        private long m_permid = -1;

        #region 属性
        public long ID
        {
            get
            {
                return m_permid;
            }
            set
            {
                if (m_permid == -1)
                {
                    m_permid = value;
                }
            }
        }

        public List<Vector2d> Points { get; set; } = new List<Vector2d>();
        

        public Vector2d this[int index]
        {
            get
            {
                return Points[index];
            }
        }

        public int Count { get { return Points.Count; } }

        private Vector2d m_center { get; set; } = null;

        public Vector2d Center
        {
            get
            {
                if (m_center != null) return m_center;
                Vector2d total = new Vector2d();
                PointsNoRepeat.ForEach(p => total += p);
                return total / PointsNoRepeat.Count;
            }
        }

        private BoundingBox m_boundingbox;
        public BoundingBox Boundingbox
        {
            get
            {
                if (m_boundingbox != null)
                {
                    return m_boundingbox;
                }
                m_boundingbox = new BoundingBox();
                for (int i = 0; i < Points.Count; i++)
                {
                    m_boundingbox.Update(Points[i]);
                }
                return m_boundingbox;
            }
        }
        #endregion

        #region 不重复的点
        private List<Vector2d> m_pointsNoRepeat;
        /// <summary>
        /// 获取首尾不重复的点数组
        /// </summary>
        public List<Vector2d> PointsNoRepeat
        {
            get
            {
                //后面的判断条件是为了多边形合并后更新m_pointsNoRepeat数组
                if (m_pointsNoRepeat != null && m_pointsNoRepeat.Count == Points.Count - 1)
                    return m_pointsNoRepeat;

                if (!Points[0].Equals(Points[Points.Count - 1]))
                {
                    return Points;
                }
                m_pointsNoRepeat = Points.ToList();
                m_pointsNoRepeat.RemoveAt(m_pointsNoRepeat.Count - 1);
                return m_pointsNoRepeat;
            }
        }
        public void ResetPointsNoRepeat()
        {
            m_pointsNoRepeat = null;
        }
        #endregion

        #region 集成组合
        public void Add(Vector2d p)
        {
            Points.Add(p);
        }
        public void Add(double x, double y)
        {
            Points.Add(new Vector2d(x, y));
        }

        public void Clear()
        {
            Points.Clear();
        }
        #endregion

        public bool AlmostEquals(Polygon other)
        {
            if (this.Count != other.Count) return false;
            if (!this.Boundingbox.Equals(other.Boundingbox)) return false;

            //相同的点作为起点，考虑可能有多个点重复的情况
            List<int> startIndices = new List<int>();
            List<Vector2d> polygon1 = this.PointsNoRepeat;
            List<Vector2d> polygon2 = other.PointsNoRepeat;
            for (int i = 0; i < polygon2.Count; i++)
            {
                if (Vector2d.Equals(polygon1[0], polygon2[i]))
                {
                    startIndices.Add(i);
                }
            }
            //开始从相同的点开始遍历
            foreach (int startIndex in startIndices)
            {
                bool bEquals = true;
                for (int i = 0; i < polygon1.Count; i++)
                {
                    Vector2d p1 = polygon1[i];
                    int currentIndex = i + startIndex;
                    Vector2d p2 = polygon2[currentIndex >= polygon2.Count ? currentIndex - polygon2.Count : currentIndex];
                    if (!Vector2d.Equals(p1, p2))
                    {
                        bEquals = false;
                    }
                }
                //如果当前起点下全部相等，则判断两个多边形相等
                if (bEquals)
                {
                    return true;
                }
            }
            return false;
        }

        private double? m_area = null;
        /// <summary>
        /// 多边形面积获取
        /// </summary>
        public double Area
        {
            get
            {
                if (m_area == null)
                {
                    m_area = Math.Abs(Points.Take(Points.Count - 1).Select((p, i) => (Points[i + 1].X - p.X) * (Points[i + 1].Y + p.Y)).Sum() / 2);
                }
                return (double)m_area;
            }
        }

        public void Dispose()
        {
            Points.Clear();
            Points = null;
            if (m_pointsNoRepeat != null)
            {
                m_pointsNoRepeat.Clear();
                m_pointsNoRepeat = null;
            }
        }
    }
}
