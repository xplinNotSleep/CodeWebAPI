using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.Geometry
{
    /// <summary>
    /// 圆
    /// </summary>
    public class Circle
    {
        /// <summary>
        /// 圆心
        /// </summary>
        public Vector2 Center { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// 分割圆
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<Vector2> SplitCircle(Circle circle, int count)
        {
            var result = new List<Vector2>();
            var offset = 360 / count;
            for (int i = 0; i < 360; i += offset)
            {
                var x = circle.Radius * Math.Cos(i * 2 * Math.PI / 360)+circle.Center.X;
                var y = circle.Radius * Math.Sin(i * 2 * Math.PI / 360) + circle.Center.Y;
                result.Add(new Vector2(x, y));
            }
            return result;
        }

        /// <summary>
        /// 是否是圆,否的时候返回空值
        /// </summary>
        /// <param name="vector2s"></param>
        /// <returns></returns>
        public static Circle IsCircle(List<Vector2> vector2s,float tolerance = 0.000001f)
        {
            var box = new BoundingBox2();
            box.Update(vector2s);
            var center = box.Center();
            var disList = new List<float>();
            var sum = 0f;
            foreach (var vector in vector2s)
            {
                var v = vector.DistanceTo(center.X, center.Y);
                sum += v;
                disList.Add(v);
            }
            var offset = 0f;
            var ave = sum / disList.Count;
            foreach (var item in disList)
            {
                offset += (item - ave) * (item - ave);
            }
            if (offset / disList.Count < tolerance)
                return new Circle() { Center = center, Radius = ave };
            else
                return null;
        }
    }
}
