using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        #region 三角形面积
        /// Function:   GetTriangleArea
        ///
        /// Summary:    Gets triangle area.获取三角形面积
        ///
        /// Author:蔡周峻
        ///
        /// Date:   2022/1/13
        ///
        /// Parameters:
        /// points -    The points. 
        ///
        /// Returns:    The triangle area.

        public static double GetTriangleArea(List<Vector2> points)
        {
            var a = points[0].DistanceTo(points[1]);
            var b = points[0].DistanceTo(points[2]);
            var c = points[1].DistanceTo(points[2]);
            var p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }
        public static double GetTriangleArea(List<Vector3> points)
        {
            var a = points[0].DistanceTo(points[1]);
            var b = points[0].DistanceTo(points[2]);
            var c = points[1].DistanceTo(points[2]);
            var p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }
        public static double GetTriangleArea(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            var a = p0.DistanceTo(p1);
            var b = p0.DistanceTo(p2);
            var c = p1.DistanceTo(p2);
            var p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }
        #endregion

        #region 多边形面积
        public static double GetArea(List<Vector2> Points)
        {
            var area = 00.0;
            for (var i = 0; i < Points.Count; i++)
            {
                var dy = Points[(i + 1) % Points.Count].Y + Points[i].Y;
                var dx = Points[(i + 1) % Points.Count].X - Points[i].X;
                area += dx * dy;
            }
            return Math.Abs(area) / 2;
        }
        public static double GetArea(List<Vector3> Points)
        {
            var area = 00.0;
            for (var i = 0; i < Points.Count; i++)
            {
                var dy = Points[(i + 1) % Points.Count].Y + Points[i].Y;
                var dx = Points[(i + 1) % Points.Count].X - Points[i].X;
                area += dx * dy;
            }
            return Math.Abs(area) / 2;
        }
        /// <summary>
        /// 替换其中的一个点后的面积
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="ri"></param>
        /// <param name="pp1"></param>
        /// <returns></returns>
        internal static double GetAreaWhenReplace(List<Vector2> Points, int ri, Vector2 pp1)
        {
            var area = 00.0;
            for (var i = 0; i < Points.Count; i++)
            {
                var j = (i + 1) % Points.Count;
                if (i == ri)
                {
                    var dy = Points[j].Y + pp1.Y;
                    var dx = Points[j].X - pp1.X;
                    area += dx * dy;
                }
                else if (j == ri)
                {
                    var dy = pp1.Y + Points[i].Y;
                    var dx = pp1.X - Points[i].X;
                    area += dx * dy;
                }
                else
                {
                    var dy = Points[(i + 1) % Points.Count].Y + Points[i].Y;
                    var dx = Points[(i + 1) % Points.Count].X - Points[i].X;
                    area += dx * dy;
                }

            }
            return Math.Abs(area) / 2;
        }
        #endregion
    }
}
