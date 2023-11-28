using AgDataHandle.Maths.Numerics;
using AgDataHandle.Maths;
using System;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        /// <summary>
        /// 逆时针计算法线
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Vector3 ComputeNormalByAnticlockwise(Vector3[] vertices)
        {
            Vector3 a = vertices[1] - vertices[0];
            Vector3 b = vertices[2] - vertices[0];
            return a.Cross(b);
        }
        public static Vector3 ComputeNormalByClockwise(Vector3[] vertices)
        {
            Vector3 a = vertices[1] - vertices[0];
            Vector3 b = vertices[2] - vertices[0];
            return b.Cross(a);
        }

        /// <summary>
        /// 由p1-p2-p3组成的三角角度
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static double TriangleDegree(Vector2d p1, Vector2d p2, Vector2d p3)
        {
            Vector2d dir1 = GetDirection(p1, p2);
            Vector2d dir2 = GetDirection(p3, p2);
            double d = dir1.X * dir2.X + dir1.Y * dir2.Y;

            double d1 = Math.Sqrt(dir1.X * dir1.X + dir1.Y * dir1.Y);
            double d2 = Math.Sqrt(dir2.X * dir2.X + dir2.Y * dir2.Y);

            double costh = d / (d1 * d2);
            return Math.Acos(costh) * 180 / Math.PI;
        }
    }
}
