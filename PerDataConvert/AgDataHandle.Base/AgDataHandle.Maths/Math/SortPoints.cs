using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public class SortPoints
    {
        public static List<Vector2> SortPoint2d(List<Vector2> points)
        {
            var letfButtom = points[0];
            for (var i = 1; i < points.Count; i++)
            {
                if (letfButtom.X > points[i].X)
                    letfButtom = points[i];
            }
            var list = new List<Point2dInfo>();
            var dir = new Vector2(0, 1);
            for (var j = 0; j < points.Count; j++)
            {
                var p = points[j].MinusToOther(dir);
                var angel = 0f;
                if (p.X != 0 && p.Y != 0)
                    angel = p.Dot(dir);
                list.Add(new Point2dInfo() { Point = points[j], Angle = angel });
            }
            list.Sort((a,b)=>a.Angle.CompareTo(b.Angle));
            var sortPoints = new List<Vector2>();
            foreach (var item in list)
            {
                sortPoints.Add(item.Point);
            }
            return sortPoints;
        }
    }

    internal class Point2dInfo
    {
        public Vector2 Point { get; set; }
        public float Angle { get; set; }
    }
}
