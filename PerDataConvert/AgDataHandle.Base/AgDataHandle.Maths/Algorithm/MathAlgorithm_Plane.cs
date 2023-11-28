using AgDataHandle.Maths.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 本类负责平面的运算
    /// </summary>
    public partial class MathAlgorithm
    {
        /// <summary>
        /// 计算线和平面交点
        /// </summary>
        /// <param name="planeVector">平面方向</param>
        /// <param name="planePoint">平面上一点</param>
        /// <param name="lineVector">线方向</param>
        /// <param name="linePoint">线上一点</param>
        /// <returns></returns>
        public static Vector3 CalculateIntersectPoint(Plane3 plane, Vector3 lineVector, Vector3 linePoint)
        {
            float[] returnResult = new float[3];
            float vp1, vp2, vp3, n1, n2, n3, v1, v2, v3, m1, m2, m3, t, vpt;
            vp1 = plane.Normal[0];
            vp2 = plane.Normal[1];
            vp3 = plane.Normal[2];
            n1 = plane.P0[0];
            n2 = plane.P0[1];
            n3 = plane.P0[2];
            v1 = lineVector[0];
            v2 = lineVector[1];
            v3 = lineVector[2];
            m1 = linePoint[0];
            m2 = linePoint[1];
            m3 = linePoint[2];
            vpt = v1 * vp1 + v2 * vp2 + v3 * vp3;
            //首先判断直线是否与平面平行
            if (vpt == 0)
            {
                returnResult = null;
            }
            else
            {
                t = ((n1 - m1) * vp1 + (n2 - m2) * vp2 + (n3 - m3) * vp3) / vpt;
                returnResult[0] = m1 + v1 * t;
                returnResult[1] = m2 + v2 * t;
                returnResult[2] = m3 + v3 * t;
            }
            if (returnResult == null)
                return null;
            else
                return new Vector3(returnResult[0], returnResult[1], returnResult[2]);
        }
    }
}
