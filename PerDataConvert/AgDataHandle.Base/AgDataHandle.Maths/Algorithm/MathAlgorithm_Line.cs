using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        #region 判断点是否在直线上
        /// <summary>
        /// 点是否在直线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static bool IsOnLine(Vector2d point, Vector2d head, Vector2d tail, double tolerance = 1e-8)
        {
            Vector2d v1 = tail - head;
            Vector2d v2 = tail - point;

            double angle = v1.Angle(v2);
            return angle < tolerance || Math.Abs(angle - 180) < tolerance;
        }
        /// <summary>
        /// 点是否在直线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static bool IsOnLine(Vector3 point, Vector3 head, Vector3 tail, double tolerance = 1e-8)
        {
            Vector3 v1 = tail - head;
            Vector3 v2 = tail - point;

            double angle = v1.Angle(v2);
            return angle < tolerance || Math.Abs(angle - 180) < tolerance;
        }
        #endregion

        #region 计算点在直线上的垂点
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point_a">直线a点</param>
        /// <param name="point_b">直线b点</param>
        /// <param name="point_c">待投影点</param>
        /// <returns></returns>
        public static Vector2d ComputeProjectPoint(Vector2d point_a, Vector2d point_b, Vector2d point_c)
        {
            Vector2d vec_ab = point_b - point_a;
            Vector2d vec_ac = point_c - point_a;
            Vector2d vec_Normalize = vec_ab.NormalizeToOther<Vector2d>();
            //计算出投影向量的模
            double projection_length = vec_ac.Dot(vec_Normalize);
            //计算出投影向量
            Vector2d vec_projection = vec_Normalize.MultiplyToOther<Vector2d>(projection_length);
            return vec_projection + point_a;
        }
        #endregion
    }
}
