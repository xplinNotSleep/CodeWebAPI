using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        #region 判断线段相交
        /// 线段是否相交
        /// </summary>
        /// <param name="p1">线段P1P2的P1点</param>
        /// <param name="p2">线段P1P2的P2点</param>
        /// <param name="q1">线段Q1Q2的Q1点</param>
        /// <param name="q2">线段Q1Q2的Q2点</param>
        /// <returns></returns>
        public static bool IsIntersect(Vector2d p1, Vector2d p2, Vector2d q1, Vector2d q2)
        {
            //排斥试验，判断p1p2在q1q2为对角线的矩形区之外
            if (Math.Max(p1.X, p2.X) < Math.Min(q1.X, q2.X))
            {//P1P2中最大的X比Q1Q2中的最小X还要小，说明P1P2在Q1Q2的最左点的左侧，不可能相交。
                return false;
            }

            if (Math.Min(p1.X, p2.X) > Math.Max(q1.X, q2.X))
            {//P1P2中最小的X比Q1Q2中的最大X还要大，说明P1P2在Q1Q2的最右点的右侧，不可能相交。
                return false;
            }

            if (Math.Max(p1.Y, p2.Y) < Math.Min(q1.Y, q2.Y))
            {//P1P2中最大的Y比Q1Q2中的最小Y还要小，说明P1P2在Q1Q2的最低点的下方，不可能相交。
                return false;
            }

            if (Math.Min(p1.Y, p2.Y) > Math.Max(q1.Y, q2.Y))
            {//P1P2中最小的Y比Q1Q2中的最大Y还要大，说明P1P2在Q1Q2的最高点的上方，不可能相交。
                return false;
            }

            //跨立试验
            double crossP1P2Q1 = Cross(p1, p2, q1);
            double crossP1Q2P2 = Cross(p1, q2, p2);
            double crossQ1Q2P1 = Cross(q1, q2, p1);
            double crossQ1P2Q2 = Cross(q1, p2, q2);

            bool isIntersect = (crossP1P2Q1 * crossP1Q2P2 >= 0) && (crossQ1Q2P1 * crossQ1P2Q2 >= 0);
            return isIntersect;
        }
        #endregion

        #region 判断线段是否包含另一线段（暂时不考虑部分重叠的问题）
        public static bool IsInclude(Vector2d p1, Vector2d p2, Vector2d q1, Vector2d q2)
        {
            //判断包含关系
            double dis1 = GetDistance(p1, p2);
            double dis2 = GetDistance(q1, q2);
            if (dis1 < dis2)
            {
                //判断短线段的点是否在长线段上
                if (IsOnLineSegment(p1, q1, q2) && IsOnLineSegment(p2, q1, q2))
                {
                    return true;
                }
            }
            else
            {
                if (IsOnLineSegment(q1, p1, p2) && IsOnLineSegment(q2, p1, p2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否第一条线包含第二条
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static bool IsIncludeByFirst(Vector2d p1, Vector2d p2, Vector2d q1, Vector2d q2)
        {
            //判断包含关系
            double dis1 = GetDistance(p1, p2);
            double dis2 = GetDistance(q1, q2);
            if (dis1 >= dis2)
            {
                //判断短线段的点是否在长线段上
                if (IsOnLineSegment(q1, p1, p2) && IsOnLineSegment(q2, p1, p2))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 判断点是否在线段上
        public static bool IsOnLineSegment(Vector2d point, Vector2d head, Vector2d tail, double tolerance = 1e-8)
        {
            //先判断有无超出线段
            if (point.X < Math.Min(head.X, tail.X) || point.X > Math.Max(head.X, tail.X) || point.Y < Math.Min(head.Y, tail.Y) || point.Y > Math.Max(head.Y, tail.Y))
            {
                return false;
            }
            return IsOnLine(point, head, tail, tolerance);
        }
        #endregion

        #region 判断线段是否首尾相接
        /// <summary>
        /// 线段是否首尾相接
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static bool IsHeadTailConnect(Vector2d p1, Vector2d p2, Vector2d q1, Vector2d q2)
        {
            return p1.Equals(q1)||p1.Equals(q2)||p2.Equals(q1)||p2.Equals(q2);
        }
        #endregion

        #region 二维向量叉乘
        /// <summary>
        /// 二维向量叉乘（center->point1,center->point2）
        /// </summary>
        public static double Cross(Vector2d center, Vector2d point1, Vector2d point2)
        {
            return Cross(point1 - center, point2 - center);
        }

        public static double Cross(Vector2 center, Vector2 point1, Vector2 point2)
        {
            return Cross(point1 - center, point2 - center);
        }

        public static double Cross(Vector2 vector1, Vector2 vector2)
        {
            return vector1.X * vector2.Y - vector2.X * vector1.Y;
        }

        public static double Cross(Vector2d vector1, Vector2d vector2)
        {
            return vector1.X * vector2.Y - vector2.X * vector1.Y;
        }
        #endregion

        #region 距离
        public static double GetDistance(Vector2d p1, Vector2d p2)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }
        public static Vector2d GetDirection(Vector2d p1, Vector2d p2)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;

            return new Vector2d(dx, dy);
        }
        public static double GetDistanceSq(Vector2d p1, Vector2d p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }
        #endregion

        #region 计算交点
        /// <summary>
        /// 计算两条线段的交点，方法来源于Cesium的方法Cesium.Intersections2D.computeLineSegmentLineSegmentIntersection
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static Vector2d ComputeTwoLineSegmentIntersectPoint(Vector2d p1, Vector2d p2, Vector2d q1, Vector2d q2)
        {
            double numerator1A = (q2.X - q1.X) * (p1.Y - q1.Y) - (q2.Y - q1.Y) * (p1.X - q1.X);
            double numerator1B = (p2.X - p1.X) * (p1.Y - q1.Y) - (p2.Y - p1.Y) * (p1.X - q1.X);
            double denominator1 = (q2.Y - q1.Y) * (p2.X - p1.X) - (q2.X - q1.X) * (p2.Y - p1.Y);
            if (denominator1 == 0)
            {
                return null;
            }
            double ua1 = numerator1A / denominator1;
            double ub1 = numerator1B / denominator1;
            Vector2d result = new Vector2d();
            if (ua1 >= 0 && ua1 <= 1 && ub1 >= 0 && ub1 <= 1)
            {
                result.X = p1.X + ua1 * (p2.X - p1.X);
                result.Y = p1.Y + ua1 * (p2.Y - p1.Y);
                return result;
            }
            return null;
        }

        #endregion

        /// <summary>
        /// DDA画线法
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public static List<Vector2> DDALines(int x0, int y0, int x1, int y1) 
        {
            var points = new List<Vector2>();
            float dx, dy, k;
            dx = x1 - x0;
            dy = y1 - y0;
            if (dx == 0)//斜率不存在情况
            {
                int y;
                for (y = y0; y <= y1; y++)
                {
                    points.Add(new Vector2(x0, y));
                }
                for (y = y1; y <= y0; y++)
                {
                    points.Add(new Vector2(x0, y));
                }
            }
            else
            {
                k = dy / dx;
                if (Math.Abs(k) <= 1)//斜率小于1情况。
                {
                    int x;
                    float y = y0;
                    for (x = x0; x <= x1; x++)//x0小于x1情况。
                    {
                        points.Add(new Vector2(x, (int)Math.Floor(y + 0.5)));
                        y += k;
                    }
                    y = y1;
                    for (x = x1; x <= x0; x++)//x1小于x0情况。
                    {
                        points.Add(new Vector2(x, (int)Math.Floor(y + 0.5)));
                        y += k;
                    }
                }
                if (Math.Abs(k) > 1)//斜率大于1情况。
                {
                    float x = x0;
                    int y;
                    for (y = y0; y <= y1; y++)//y0小于y1情况。
                    {
                        points.Add(new Vector2((int)Math.Floor(x + 0.5), y));
                        x += 1 / k;
                    }
                    x = x1;
                    for (y = y1; y <= y0; y++)//y1小于y0情况。
                    {
                        points.Add(new Vector2((int)Math.Floor(x + 0.5), y));
                        x += 1 / k;
                    }
                }
            }
            return points;
        }
    }
}
