using AgDataHandle.Maths.Geometry;
using System;
using System.Collections.Generic;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 通用的数学方法
    /// </summary>
    public partial class MathAlgorithm
    {
        /// <summary>
        /// 矫正方向，通常在BIM模型里面观察方向都是垂直坐标轴的，为避免精度问题，需要矫正
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3 RectifyDirection(Vector3 direction)
        {
            if (Math.Abs(direction.X) > 0.99f)
                return direction.X > 0 ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            if (Math.Abs(direction.Y) > 0.99f)
                return direction.Y > 0 ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
            if (Math.Abs(direction.Z) > 0.99f)
                return direction.Z > 0 ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
            return new Vector3(direction);
        }
        /// <summary>
        /// 计算在xoy平面上，三角形和直线的相交线段
        /// </summary>
        /// <param name="p1">三角形的点1</param>
        /// <param name="p2">三角形的点2</param>
        /// <param name="p3">三角形的点3</param>
        /// <param name="k2">直线斜率</param>
        /// <param name="point">直线上一点</param>
        /// <returns>相交的线段</returns>
        public static Line ComputeIntersectionLineInXOYPlane(Vector3 p1, Vector3 p2, Vector3 p3, double k2, Vector2 point)
        {
            var list = new List<Vector3>();
            var intP0 = ComputeIntersectionPoint(p1, p2, k2, point);
            if (intP0 != null)
                list.Add(intP0);
            var intP1 = ComputeIntersectionPoint(p2, p3, k2, point);
            if (intP1 != null)
                list.Add(intP1);
            var intP2 = ComputeIntersectionPoint(p3, p1, k2, point);
            if (intP2 != null)
                list.Add(intP2);
            if (list.Count != 2)
                return null;
            return new Line(list[0], list[1]);
        }
        
        /// <summary>
        /// 获得路径中某点垂直于切线直线斜率
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="index">点索引</param>
        /// <returns></returns>
        public static double GetSlopeOfVerticalTangentIgnoreZ(List<Vector3> path, int index)
        {
            var k = GetSlopeIgnoreZ(path,index);
            if (k == double.MaxValue)
                k = 0;
            else if (k == 0)
                k = double.MaxValue;
            else
                k = -1 / k;
            return k;
        }

        /// <summary>
        /// 获得路径中某点斜率
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="index">点索引</param>
        /// <returns></returns>
        public static double GetSlopeIgnoreZ(List<Vector3> path, int index)
        {
            var k = 0d;
            if (index == 0)
            {
                k = ComputeSlopeIgnoreZ(path[index], path[index + 1]);
            }
            else if (index == path.Count - 1)
            {
                k = ComputeSlopeIgnoreZ(path[index - 1], path[index]);
            }
            else
            {
                var v0 = path[index] - path[index - 1];
                var v1 = path[index + 1] - path[index];
                var dir0 = new Vector2(v0.X, v0.Y);
                var dir1 = new Vector2(v1.X, v1.Y);
                dir0.Normalize<Vector2>();
                dir1.Normalize<Vector2>();
                var dir = dir0 + dir1;
                k = ComputeSlopeIgnoreZ(new Vector3d(dir.X, dir.Y, 0), Vector3d.Zero);
            }
            return k;
        }
        /// <summary>
        /// 根据两点计算斜率
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static double ComputeSlopeIgnoreZ(Vector3 p0, Vector3 p1)
        {
            var k = 0d;
            var xOffset = (double)p1.X - p0.X;
            if (xOffset == 0)
            {
                k = double.MaxValue;
            }
            else
            {
                k = (p1.Y - p0.Y) / xOffset;
            }
            return k;
        }
        public static double ComputeSlope(Vector2 p0, Vector2 p1)
        {
            var k = 0d;
            var xOffset = (double)p1.X - p0.X;
            if (xOffset == 0)
            {
                k = double.MaxValue;
            }
            else
            {
                k = (p1.Y - p0.Y) / xOffset;
            }
            return k;
        }
        public static double ComputeSlope(Vector2d p0, Vector2d p1)
        {
            var k = 0d;
            var xOffset = p1.X - p0.X;
            if (xOffset == 0)
            {
                k = double.MaxValue;
            }
            else
            {
                k = (p1.Y - p0.Y) / xOffset;
            }
            return k;
        }
        public static double ComputeSlopeIgnoreZ(Vector3d p0, Vector3d p1)
        {
            var k = 0d;
            var xOffset = p1.X - p0.X;
            if (xOffset == 0)
            {
                k = double.MaxValue;
            }
            else
            {
                k = (p1.Y - p0.Y) / xOffset;
            }
            return k;
        }


        /// <summary>
        /// 计算交点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="k2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 ComputeIntersectionPoint(Vector3 sp1, Vector3 sp2, double k2, Vector2 point)
        {
            var xmin = Math.Min(sp1.X, sp2.X);
            var xmax = Math.Max(sp1.X, sp2.X);
            var ymin = Math.Min(sp1.Y, sp2.Y);
            var ymax = Math.Max(sp1.Y, sp2.Y);
            var p1 = new Vector3d(sp1.X, sp1.Y, sp1.Z);
            var p2 = new Vector3d(sp2.X, sp2.Y, sp2.Z);
            var k1 = ComputeSlopeIgnoreZ(p1, p2);
            if (k1 == k2)
                return null;
            if (k2 == double.MaxValue)
            {
                if (point.X < xmin || point.X > xmax)
                    return null;
                var y = p1.Y + Math.Abs(point.X - p1.X) * (p2.Y - p1.Y) / (xmax - xmin);
                var z = p1.Z + Math.Abs(point.X - p1.X) * (p2.Z - p1.Z) / (xmax - xmin);
                return new Vector3(point.X, y, z);
            }
            if (k1 == double.MaxValue)
            {
                var b2 = point.Y - k2 * point.X;
                var y = k2 * p1.X + b2;
                if (y < ymin || y > ymax)
                    return null;
                var z = p1.Z + Math.Abs(y - p1.Y) * (p2.Z - p1.Z) / (ymax - ymin);
                return new Vector3(p1.X, y, z);
            }
            var b1 = p1.Y - k1 * p1.X;
            var intersectionX = (b1 - point.Y + k2 * point.X) / (k2 - k1);
            var intersectionY = k1 * intersectionX + b1;
            if (intersectionX < xmin || intersectionX > xmax)
                return null;
            var intersectionZ = p1.Z + Math.Abs(intersectionX - p1.X) * (p2.Z - p1.Z) / (xmax - xmin);
            return new Vector3(intersectionX, intersectionY, intersectionZ);
        }
        public static Vector2 ComputeIntersectionPoint(Vector2 sp1, Vector2 sp2, float k2, Vector2 point)
        {
            if(sp1.Equals(point)|| sp2.Equals(point))
                return point;
            var xmin = Math.Min(sp1.X, sp2.X);
            var xmax = Math.Max(sp1.X, sp2.X);
            var ymin = Math.Min(sp1.Y, sp2.Y);
            var ymax = Math.Max(sp1.Y, sp2.Y);
            var p1 = new Vector2d(sp1.X, sp1.Y);
            var p2 = new Vector2d(sp2.X, sp2.Y);
            var k1 = MathAlgorithm.ComputeSlope(p1, p2);
            if (k1 == k2)
                return null;
            if (k2 == double.MaxValue)
            {
                if (point.X < xmin || point.X > xmax)
                    return null;
                var y = p1.Y + Math.Abs(point.X - p1.X) * (p2.Y - p1.Y) / (xmax - xmin);
                return new Vector2(point.X, y);
            }
            if (k1 == double.MaxValue)
            {
                var b2 = point.Y - k2 * point.X;
                var y = k2 * p1.X + b2;
                if (y < ymin || y > ymax)
                    return null;
                return new Vector2(p1.X, y);
            }
            var b1 = p1.Y - k1 * p1.X;
            var intersectionX = (b1 - point.Y + k2 * point.X) / (k2 - k1);
            var intersectionY = k1 * intersectionX + b1;
            if (intersectionX < xmin || intersectionX > xmax)
                return null;
            return new Vector2(intersectionX, intersectionY);
        }
    }
}
