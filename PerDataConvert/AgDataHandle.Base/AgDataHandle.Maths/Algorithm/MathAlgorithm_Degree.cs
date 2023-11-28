using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        #region 三点角度
        /// <summary>
        /// 计算三点构成的夹角，其中以P2为夹角点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static double GetInclinationDegree(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            if (p1.Equals(p2))
            {
                return 0;
            }
            if (p2.Equals(p3))
            {
                return 0;
            }
            Vector2 v1 = new Vector2(p1.X - p2.X, p1.Y - p2.Y);
            Vector2 v2 = new Vector2(p3.X - p2.X, p3.Y - p2.Y);
            float dot = v1.X * v2.X + v1.Y * v2.Y;
            double len1 = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
            double len2 = Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);
            float q = v1.X * v2.Y - v1.Y * v2.X;
            double t = dot / (len1 * len2);
            if (t > 1) t = 1;
            if (t < -1) t = -1;
            double costh = Math.Acos(t) * 180 / Math.PI;
            if (q < 0)
                return 360 - costh;
            return costh;
        }
        #endregion
    }
}
