using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.Geometry
{
    /// <summary>
    /// 三维空间的多边形
    /// </summary>
    public class Polygon3D
    {
        public bool HaveRemoveSamePoint { get; private set; } = false;
        /// <summary>
        /// 多边形是否闭合
        /// </summary>
        public bool Closed { get; set; } = false;
        public List<Vector3> Path { get; set; } = new List<Vector3>();

        /// <summary>
        /// 是否时闭合的
        /// </summary>
        /// <returns></returns>
        public bool IsClosed()
        {
            if (HaveRemoveSamePoint)
                return Closed;
            if (Path == null || Path.Count <= 4)
            {
                Closed = false;
                return Closed;
            }
            Closed = Path[0].Equals(Path[Path.Count - 1]);
            HaveRemoveSamePoint = true;
            return Closed;
        }

        /// <summary>
        /// 通过斜率的方式简化多边形
        /// </summary>
        public void SimplifyPolygon()
        {
            var polygon3D = this;
            var removeList = new List<int>();
            var path = polygon3D.Path;
            for (int i = 0; i < path.Count; i++)
            {
                Vector3 dir0 = null;
                Vector3 dir1 = null;
                if (i == 0)
                {
                    dir0 = path[polygon3D.Path.Count - 1] - path[i];
                    dir1 = path[i + 1] - path[i];
                }
                else if (i == path.Count - 1)
                {
                    dir0 = path[i - 1] - path[i];
                    dir1 = path[0] - path[i];
                }
                else
                {
                    dir0 = path[i - 1] - path[i];
                    dir1 = path[i + 1] - path[i];
                }
                dir0.Normalize<Vector3>();
                dir1.Normalize<Vector3>();
                var dotV = dir0.Dot(dir1);
                if (Math.Abs(dotV + 1) <= 0.0001f)
                {
                    removeList.Add(i);
                }
            }

            for (int i = removeList.Count - 1; i >= 0; i--)
            {
                path.RemoveAt(removeList[i]);
            }
        }
    }
}
