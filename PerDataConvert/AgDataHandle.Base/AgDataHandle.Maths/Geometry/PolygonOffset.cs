using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.Geometry
{
    /// <summary>
    /// 多边形偏移
    /// </summary>
    public class PolygonOffset
    {
        /// <summary>
        /// 内缩
        /// </summary>
        /// <param name="points">多边形的点，顺时针</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public static List<Vector2> Indent(List<Vector2> points,float offset)
        {
            var re = new List<Vector2>();
            var lineList = new List<Vector2[]>();
            for (int i = 0; i < points.Count; i++)
            {
                if(i!=points.Count-1)
                {
                    var offsetPoints = LineOffset(points[i], points[i + 1], offset);
                    lineList.Add(offsetPoints);
                }
                else
                {
                    var offsetPoints = LineOffset(points[i], points[0], offset);
                    lineList.Add(offsetPoints);
                }
            }
            for (int i = 0; i < lineList.Count; i++)
            {
                if(i== lineList.Count-1)
                {
                    re.Add(LineIntersect(lineList[i],lineList[0]));
                }
                else
                {
                    re.Add(LineIntersect(lineList[i], lineList[i+1]));
                }
            }
            return re;
        }

        public static List<Vector2> Offset(List<Vector2>m_sourcePoints,float offset)
        {
            offset = JudgeOffsetOrientationV2(m_sourcePoints, offset);
            var n = m_sourcePoints.Count;
            var points =new List<Vector2>();
            for (var i = 0; i < n; i++)
            {
                var p = m_sourcePoints[i];
                var p1 = m_sourcePoints[i == 0 ? n - 1 : i - 1];
                var p2 = m_sourcePoints[i == n - 1 ? 0 : i + 1];

                var v1x = p1.X - p.X;
                var v1y = p1.Y - p.Y;
                var n1 = Math.Sqrt(v1x * v1x + v1y * v1y);

                v1x /= (float)n1;
                v1y /= (float)n1;

                var v2x = p2.X - p.X;
                var v2y = p2.Y - p.Y;
                var n2 = Math.Sqrt(v2x * v2x + v2y * v2y);

                v2x /= (float)n2;
                v2y /= (float)n2;

                var ceta = v1x * v2y - v2x * v1y;
                points.Add(new Vector2(p.X + offset / ceta * (v1x + v2x), p.Y + offset / ceta * (v1y + v2y)));

            }
            return points;
        }

        private static float JudgeOffsetOrientation(List<Vector2> m_sourcePoints, float offset)
        {
            var n = m_sourcePoints.Count;
            int clockwiseCount = 0, counterclockwiseCount = 0;
            for (int i = 0; i < m_sourcePoints.Count; i++)
            {
                var p = m_sourcePoints[i];
                var p1 = m_sourcePoints[i == 0 ? n - 1 : i - 1];
                var p2 = m_sourcePoints[i == n - 1 ? 0 : i + 1];
                var a = (p - p1).Length();
                var b = (p - p2).Length();
                var c = (p1 - p2).Length();
                var ans = (p.X - p1.X) * (p2.Y - p1.Y) - (p2.X - p1.X) * (p.Y - p1.Y);
                if (ans > 0)
                    clockwiseCount++;
                else
                    counterclockwiseCount++;
                    
            }
            if (clockwiseCount > counterclockwiseCount)
                return offset;
            else
                return offset * -1;
        }
        private static float JudgeOffsetOrientationV2(List<Vector2> m_sourcePoints, float offset)
        {
            var n = m_sourcePoints.Count;
            float area = 0;
            for (int i = 0; i < m_sourcePoints.Count; i++)
            {
                var p = m_sourcePoints[i];
                var p1 = m_sourcePoints[i == 0 ? n - 1 : i - 1];
                var p2 = m_sourcePoints[i == n - 1 ? 0 : i + 1];

                area+= p.X *p2.Y-p.Y*p2.X;            
            }
            if (area>0)
                return offset;
            else
                return offset * -1;
        }
        /// <summary>
        /// 计算线交点
        /// </summary>
        /// <param name="line0"></param>
        /// <param name="line1"></param>
        /// <returns></returns>
        private static Vector2 LineIntersect(Vector2[] line0,Vector2[] line1)
        {
            return LineIntersect(line0[0], line0[1], line1[0], line1[1]);
        }
        private static Vector2 LineIntersect(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var A1 = p1.Y - p0.Y;
            var B1 = p0.X - p1.X;
            var C1 = A1 * p0.X + B1 * p0.Y;
            var A2 = p3.Y - p2.Y;
            var B2 = p2.X - p3.X;
            var C2 = A2 * p2.X + B2 * p2.Y;
            var denominator = A1 * B2 - A2 * B1;
            return new Vector2((B2 * C1 - B1 * C2) / denominator, (A1 * C2 - A2 * C1) / denominator);
        }

        /// <summary>
        /// 线段偏移
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static Vector2[] LineOffset(Vector2 start, Vector2 end,float offset)
        {
            var vectorOffet = new Vector2(0, offset);
            var reEnd = end.MinusToOther(start);
            var line = reEnd.Normalize<Vector2>();
            var angle =(float)(Math.Acos(line.Dot(new Vector2(1, 0))) * 180 / Math.PI);
            if (reEnd.Y < 0)
                angle = -angle;

            reEnd = MathAlgorithm.RoteWithOrigin(reEnd, -angle).Add(vectorOffet);
            reEnd = MathAlgorithm.RoteWithOrigin(reEnd, angle).Add(start);

            var reSrart = MathAlgorithm.RoteWithOrigin(vectorOffet, angle).Add(start);
            return new Vector2[]{ reSrart,reEnd};
        }
    }
}
