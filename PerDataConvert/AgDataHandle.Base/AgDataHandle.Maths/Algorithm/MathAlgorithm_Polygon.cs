using AgDataHandle.Maths.Geometry;
using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        #region 多边形验证
        /// <summary>
        /// 多边形是否包含点
        /// </summary>
        /// <param name="polygonVertices"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool PolygonContains(List<PointF> polygon, PointF point)
        {
            if (polygon == null)
            {
                return false;
            }
            //删除最后一个顶点
            int polygonVerticesCount = polygon.Count;
            var polygonVertices = polygon;
            if (polygonVertices[0].Equals(polygonVertices[polygonVertices.Count - 1]))
            {
                polygonVerticesCount -= 1;
            }

            double A, B, C, D;
            int count = 0;
            for (int i = 0; i < polygonVerticesCount; i++)
            {//[0,1]..[n-2,n-1]
                var p1 = polygonVertices[i];
                var p2 = polygonVertices[(i + 1) % polygonVertices.Count];
                bool t1 = p1.X > p2.X;
                bool t2 = p1.X > point.X;
                bool t3 = p2.X > point.X;
                if (t2 != t3)
                {
                    A = p2.Y - p1.Y;
                    B = p1.X - p2.X;
                    C = -A * p1.X - B * p1.Y;
                    D = A * point.X + B * point.Y + C;
                    if (D < 0 && t1)
                    {
                        count++;
                    }
                    if (D > 0 && t1 == false)
                    {
                        count++;
                    }
                }
                else if ((t2 == false) && (t3 == false))
                {
                    if (t1 && p2.X == point.X)
                    {
                        count++;
                    }
                    if ((t1 == false) && p1.X == point.X)
                    {
                        count++;
                    }

                }
            }

            if (count % 2 == 1)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 简化多边形
        public static void SimplityPolyonBySameLine(Polygon polygon)
        {
            bool hasUpdate = true;
            while (hasUpdate)
            {
                hasUpdate = false;
                for (var i = 0; i < polygon.Count; i++)
                {
                    //防止简化后点过少，出现异常
                    if (polygon.Count <= 4)
                        return;

                    var p1 = polygon[(i - 1 + polygon.Count) % polygon.Count];
                    var p2 = polygon[i];
                    var p3 = polygon[(i + 1 + polygon.Count) % polygon.Count];

                    var th = MathAlgorithm.TriangleDegree(p1, p2, p3);
                    if (th > 90)
                    {
                        th = 180 - th;
                    }
                    if (th < 15)
                    {
                        polygon.Points.RemoveAt(i);
                        hasUpdate = true;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 对多边形进行简化，就是距离相近的点归纳为一点
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static List<Vector2d> SimplityPolyon(List<Vector2d> polygon, bool selfUpdate, double pointBuffer = 3)
        {
            List<int> toRemovePointIndex = new List<int>();

            var currentIndex = 1;

            var p1 = polygon[0];
            var p2 = polygon[currentIndex];
            var p3 = polygon[2];
            while (currentIndex < polygon.Count)
            {
                var d = GetDistance(p2, p3);
                if (d < pointBuffer)
                {
                    toRemovePointIndex.Add(currentIndex + 1);
                }
                else
                {
                    var th = TriangleDegree(p3, p1, p2);
                    var d2 = GetDistance(p1, p3);
                    var diffth = d2 < pointBuffer ? 15 : (400.0 / d2);
                    if (th < diffth)
                    {
                        toRemovePointIndex.Add(currentIndex + 1);
                    }
                    else
                    {
                        p1 = p2;
                        p2 = p3;
                    }
                }
                if (currentIndex + 3 < polygon.Count)
                {
                    p3 = polygon[currentIndex + 3];
                    currentIndex++;
                }
                else
                {
                    break;
                }
            }

            if (selfUpdate)
            {
                for (var i = toRemovePointIndex.Count - 1; i > -1; i--)
                {
                    polygon.RemoveAt(toRemovePointIndex[i]);
                }
                return polygon;
            }
            else
            {
                List<Vector2d> myPolygon = new List<Vector2d>();
                for (var i = 0; i < polygon.Count; i++)
                {
                    if (toRemovePointIndex.Contains(i) == false)
                    {
                        myPolygon.Add(polygon[i]);
                    }
                }
                return myPolygon;
            }
        }
        #endregion

        #region 计算内坍缩的多边形，生成mesh
        const double threshold_minValue = 5.0 * Math.PI / 180;
        const double threshold_maxValue = 355.0 * Math.PI / 180;
        /// <summary>
        /// 计算内坍缩的多边形（用于女儿墙等），多边形方向为顺时针方向
        /// </summary>
        /// <param name="outerPolygon"></param>
        /// <param name="width">内插的边与原多边形的边的垂直距离（内插边与原多边形的边平行）</param>
        /// <returns></returns>
        public static Polygon CreateInnerPolygon(Polygon outerPolygon, float width)
        {
            //用于计算方位角的X轴正方向标准向量
            Vector2d standard = new Vector2d(1, 0);
            Polygon innerPolygon = new Polygon();
            for (int i = 0; i < outerPolygon.Count - 1; i++)
            {
                int aIndex = i == 0 ? outerPolygon.Count - 2 : i - 1;
                int bIndex = i == outerPolygon.Count - 2 ? 0 : i + 1;
                Vector2d pointA = outerPolygon[aIndex];
                Vector2d pointO = outerPolygon[i];
                Vector2d pointB = outerPolygon[bIndex];
                Vector2d vectorOA = pointA - pointO;
                Vector2d vectorOB = pointB - pointO;
                //OA->OB的夹角
                double angle = vectorOA.Angle(vectorOB);
                //将夹角改为0-360°范围
                angle = vectorOA.Cross(vectorOB) > 0 ? angle : Math.PI * 2 - angle;
                //定义角平分线为OC，halfAngle为 OA->OC 或 OC->OB 的角度
                double halfAngle = angle / 2;
                //在OC方向上，点C相对点O的距离（点C到边OA和边OB垂直距离均等于width）
                double lengthOC = width / Math.Sin(halfAngle);

                //处理OA与OB的夹角为一个角度很小的尖角的特殊情况
                if (angle < threshold_minValue || Math.PI * 2 - angle < threshold_maxValue)
                {
                    lengthOC = width;
                }

                //OA相对于(1,0)的夹角
                double angleOA = standard.Angle(vectorOA);
                angleOA = standard.Cross(vectorOA) > 0 ? angleOA : Math.PI * 2 - angleOA;
                //计算出OC相对于X轴正方向的夹角
                double angleOC = angleOA + halfAngle;
                //计算OC在X轴和Y轴上的分量
                Vector2d sub = new Vector2d(lengthOC * Math.Cos(angleOC), lengthOC * Math.Sin(angleOC));
                Vector2d pointC = new Vector2d(pointO.X + sub.X, pointO.Y + sub.Y);
                innerPolygon.Add(pointC);
            }
            //尾部加入重复点，首尾相接
            innerPolygon.Add(innerPolygon[0]);
            return innerPolygon;
        }
        #endregion

        #region 包含判断    
        /// <summary>
        /// 判断点是否在多边形内.
        /// ----------原理----------
        /// 注意到如果从P作水平向左的射线的话，如果P在多边形内部，那么这条射线与多边形的交点必为奇数，
        /// 如果P在多边形外部，则交点个数必为偶数(0也在内)。
        /// </summary>
        /// <param name="polygon">多边形的顶点</param>
        /// <param name="checkPoint">要判断的点</param>
        public static bool PolygonContainPoint(Polygon polygon, Vector2d checkPoint)
        {
            //先判断该点是否在包围盒内
            BoundingBox box = polygon.Boundingbox;
            if (!box.Contains(checkPoint.X, checkPoint.Y))
                return false;

            //是否应该补充一个 点是否在多边形上 的判断，以减少误判(暂不)

            //射线法
            bool inside = false;
            int pointCount = polygon.Count;
            Vector2d p1, p2;
            for (int i = 0, j = pointCount - 1; i < pointCount; j = i, i++)//第一个点和最后一个点作为第一条线，之后是第一个点和第二个点作为第二条线，之后是第二个点与第三个点，第三个点与第四个点...
            {
                p1 = polygon[i];
                p2 = polygon[j];
                if (checkPoint.Y < p2.Y)
                {//p2在射线之上
                    if (p1.Y <= checkPoint.Y)
                    {//p1正好在射线中或者射线下方
                        if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) > (checkPoint.X - p1.X) * (p2.Y - p1.Y))//斜率判断,在P1和P2之间且在P1P2右侧
                        {
                            //射线与多边形交点为奇数时则在多边形之内，若为偶数个交点时则在多边形之外。
                            //由于inside初始值为false，即交点数为零。所以当有第一个交点时，则必为奇数，则在内部，此时为inside=(!inside)
                            //所以当有第二个交点时，则必为偶数，则在外部，此时为inside=(!inside)
                            inside = (!inside);
                        }
                    }
                }
                else if (checkPoint.Y < p1.Y)
                {
                    //p2正好在射线中或者在射线下方，p1在射线上
                    if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) < (checkPoint.X - p1.X) * (p2.Y - p1.Y))//斜率判断,在P1和P2之间且在P1P2右侧
                    {
                        inside = (!inside);
                    }
                }
            }
            return inside;
        }
        /// <summary>
        /// 判断多边形polygon是否包含多边形checkPolygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="checkPolygon"></param>
        /// <returns></returns>
        public static bool PolygonContainPolygon(Polygon polygon, Polygon checkPolygon, bool checkBoundingBoxFirst = false)
        {
            //避免重复判断包围盒是否相交，加入控制开关
            if (checkBoundingBoxFirst)
            {
                double intersectArea = polygon.Boundingbox.ComputeIntersectArea(checkPolygon.Boundingbox);
                if (intersectArea != checkPolygon.Boundingbox.GetArea())
                {
                    return false;
                }
            }

            bool result = true;
            foreach (Vector2d item in checkPolygon.Points)
            {
                if (!PolygonContainPoint(polygon, item))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        #endregion

        #region 多边形相交
        /// <summary>
        /// 判断多边形是否与另一个多边形相交
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="checkPolygon"></param>
        /// <returns></returns>
        public static bool PolygonIntersectPolygon(Polygon polygon, Polygon checkPolygon, bool checkBoundingBoxFirst = false)
        {
            //避免重复判断包围盒是否相交，加入控制开关
            if (checkBoundingBoxFirst)
            {
                if (!polygon.Boundingbox.IntersectXY(checkPolygon.Boundingbox))
                {
                    return false;
                }
            }

            for (int i = 0; i < polygon.Points.Count; i++)
            {
                Vector2d beginPoint = polygon.Points[i];
                Vector2d endPoint = i != polygon.Points.Count - 1 ? polygon.Points[i + 1] : polygon.Points[0];
                if (beginPoint.X == endPoint.X && beginPoint.Y == endPoint.Y) continue;
                for (int j = 0; j < checkPolygon.Points.Count; j++)
                {
                    Vector2d beginCheckPoint = checkPolygon.Points[j];
                    Vector2d endCheckPoint = j != checkPolygon.Points.Count - 1 ? checkPolygon.Points[j + 1] : checkPolygon.Points[0];
                    if (beginCheckPoint.X == endCheckPoint.X && beginCheckPoint.Y == endCheckPoint.Y) continue;
                    if (IsIntersect(beginPoint, endPoint, beginCheckPoint, endCheckPoint))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 多边形是否和包围盒相交
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static bool IsIntersectantIgnoreZ(List<Vector3> polygon, BoundingBox box)
        {
            var flag = false;
            for (int j = 0; j < polygon.Count - 1; j++)
            {
                var p1 = polygon[j];
                var p2 = polygon[j + 1];
                var intersectant = IsLineIntersectRectangle(p1.X, p1.Y, p2.X, p2.Y,
                    (float)box.MinX, (float)box.MaxY, (float)box.MaxX, (float)box.MinY);
                if (intersectant)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        #endregion

        #region 判断面是否重叠
        /// <summary>
        /// 判断多边形是否与另一个多边形重叠
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="checkPolygon"></param>
        /// <returns></returns>
        public static bool PolygonOverlayPolygon(Polygon polygon, Polygon checkPolygon)
        {
            if (!polygon.Boundingbox.IntersectXY(checkPolygon.Boundingbox))
                return false;
            return PolygonContainPolygon(polygon, checkPolygon) || PolygonIntersectPolygon(polygon, checkPolygon);
        }
        #endregion

        #region 两个多边形间最短距离
        public static double GetMinDistanceTwoPolygon(Polygon polygon1, Polygon polygon2)
        {
            double minDistance = double.MaxValue;
            //还需要考虑两个多边形相交的情况
            if (polygon1.Boundingbox.GetArea() > polygon2.Boundingbox.GetArea() ? PolygonOverlayPolygon(polygon1, polygon2) : PolygonOverlayPolygon(polygon2, polygon1))
                return 0;

            //这种只适用于不相交的情况
            foreach (Vector2d p in polygon1.PointsNoRepeat)
            {
                minDistance = Math.Min(GetMinDistancePoint2Polygon(p, polygon2), minDistance);
            }
            foreach (Vector2d p in polygon2.PointsNoRepeat)
            {
                minDistance = Math.Min(GetMinDistancePoint2Polygon(p, polygon1), minDistance);
            }
            return minDistance;
        }
        public static double GetMinDistancePoint2Polygon(Vector2d point, Polygon polygon)
        {
            double min_distance = double.MaxValue;
            for (int i = 0; i < polygon.Count - 1; ++i)
            {
                double dis = GetMinDistancePoint2LineSegment(point, polygon[i], polygon[i + 1]);
                min_distance = Math.Min(min_distance, dis);
            }

            return min_distance;
        }
        public static double GetMinDistancePoint2LineSegment(Vector2d point, Vector2d begin, Vector2d end)
        {
            double d0 = GetDistance(begin, end);//point对边的长度
            double d1 = GetDistance(point, begin);//point和begin组成的边的长度
            double d2 = GetDistance(point, end);//point和end组成的边的长度

            if (d0 * d0 + d1 * d1 < d2 * d2)//begin处的夹角为钝角
                return d1;
            if (d0 * d0 + d2 * d2 < d1 * d1)//end处的夹角为钝角
                return d2;

            //海伦公式求高
            double halfP = (d0 + d1 + d2) / 2;//周长的一半
            double s = Math.Sqrt(halfP * (halfP - d0) * (halfP - d1) * (halfP - d2));//面积
            return 2 * s / d0;
        }
        #endregion

        #region 线和多边形相交
        /// <summary>
        /// 线段与矩形是否相交
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="rectangleLeftTop"></param>
        /// <param name="rectangleRightBottom"></param>
        /// <returns></returns>
        public static bool IsLineIntersectRectangle(Vector2 point1, Vector2 point2, Vector2 rectangleLeftTop, Vector2 rectangleRightBottom)
        {
            return IsLineIntersectRectangle(point1.X, point1.Y, point2.X, point2.Y, rectangleLeftTop.X,
                rectangleLeftTop.Y, rectangleRightBottom.X, rectangleRightBottom.Y);
        }
        public static bool IsLineIntersectRectangle(Vector2d point1, Vector2d point2, Vector2d rectangleLeftTop, Vector2d rectangleRightBottom)
        {
            return IsLineIntersectRectangle(point1.X, point1.Y, point2.X, point2.Y, rectangleLeftTop.X,
                rectangleLeftTop.Y, rectangleRightBottom.X, rectangleRightBottom.Y);
        }
        /// <summary>
        /// 线段与矩形是否相交
        /// </summary>
        /// <param name="linePointX1"></param>
        /// <param name="linePointY1"></param>
        /// <param name="linePointX2"></param>
        /// <param name="linePointY2"></param>
        /// <param name="rectangleLeftTopX"></param>
        /// <param name="rectangleLeftTopY"></param>
        /// <param name="rectangleRightBottomX"></param>
        /// <param name="rectangleRightBottomY"></param>
        /// <returns></returns>
        public static bool IsLineIntersectRectangle(
            float linePointX1,
            float linePointY1,
            float linePointX2,
            float linePointY2,
            float rectangleLeftTopX,
            float rectangleLeftTopY,
            float rectangleRightBottomX,
            float rectangleRightBottomY)
        {
            float lineHeight = linePointY1 - linePointY2;
            float lineWidth = linePointX2 - linePointX1;  // 计算叉乘 
            float c = linePointX1 * linePointY2 - linePointX2 * linePointY1;
            if ((lineHeight * rectangleLeftTopX + lineWidth * rectangleLeftTopY + c >= 0 && lineHeight * rectangleRightBottomX + lineWidth * rectangleRightBottomY + c <= 0)
                || (lineHeight * rectangleLeftTopX + lineWidth * rectangleLeftTopY + c <= 0 && lineHeight * rectangleRightBottomX + lineWidth * rectangleRightBottomY + c >= 0)
                || (lineHeight * rectangleLeftTopX + lineWidth * rectangleRightBottomY + c >= 0 && lineHeight * rectangleRightBottomX + lineWidth * rectangleLeftTopY + c <= 0)
                || (lineHeight * rectangleLeftTopX + lineWidth * rectangleRightBottomY + c <= 0 && lineHeight * rectangleRightBottomX + lineWidth * rectangleLeftTopY + c >= 0))
            {

                if (rectangleLeftTopX > rectangleRightBottomX)
                {
                    float temp = rectangleLeftTopX;
                    rectangleLeftTopX = rectangleRightBottomX;
                    rectangleRightBottomX = temp;
                }
                if (rectangleLeftTopY < rectangleRightBottomY)
                {
                    float temp1 = rectangleLeftTopY;
                    rectangleLeftTopY = rectangleRightBottomY;
                    rectangleRightBottomY = temp1;
                }
                if ((linePointX1 < rectangleLeftTopX && linePointX2 < rectangleLeftTopX)
                    || (linePointX1 > rectangleRightBottomX && linePointX2 > rectangleRightBottomX)
                    || (linePointY1 > rectangleLeftTopY && linePointY2 > rectangleLeftTopY)
                    || (linePointY1 < rectangleRightBottomY && linePointY2 < rectangleRightBottomY))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }
        public static bool IsLineIntersectRectangle(
           double linePointX1,
           double linePointY1,
           double linePointX2,
           double linePointY2,
           double rectangleLeftTopX,
           double rectangleLeftTopY,
           double rectangleRightBottomX,
           double rectangleRightBottomY)
        {
            double lineHeight = linePointY1 - linePointY2;
            double lineWidth = linePointX2 - linePointX1;  // 计算叉乘 
            double c = linePointX1 * linePointY2 - linePointX2 * linePointY1;
            if ((lineHeight * rectangleLeftTopX + lineWidth * rectangleLeftTopY + c >= 0 && lineHeight * rectangleRightBottomX + lineWidth * rectangleRightBottomY + c <= 0)
                || (lineHeight * rectangleLeftTopX + lineWidth * rectangleLeftTopY + c <= 0 && lineHeight * rectangleRightBottomX + lineWidth * rectangleRightBottomY + c >= 0)
                || (lineHeight * rectangleLeftTopX + lineWidth * rectangleRightBottomY + c >= 0 && lineHeight * rectangleRightBottomX + lineWidth * rectangleLeftTopY + c <= 0)
                || (lineHeight * rectangleLeftTopX + lineWidth * rectangleRightBottomY + c <= 0 && lineHeight * rectangleRightBottomX + lineWidth * rectangleLeftTopY + c >= 0))
            {

                if (rectangleLeftTopX > rectangleRightBottomX)
                {
                    double temp = rectangleLeftTopX;
                    rectangleLeftTopX = rectangleRightBottomX;
                    rectangleRightBottomX = temp;
                }
                if (rectangleLeftTopY < rectangleRightBottomY)
                {
                    double temp1 = rectangleLeftTopY;
                    rectangleLeftTopY = rectangleRightBottomY;
                    rectangleRightBottomY = temp1;
                }
                if ((linePointX1 < rectangleLeftTopX && linePointX2 < rectangleLeftTopX)
                    || (linePointX1 > rectangleRightBottomX && linePointX2 > rectangleRightBottomX)
                    || (linePointY1 > rectangleLeftTopY && linePointY2 > rectangleLeftTopY)
                    || (linePointY1 < rectangleRightBottomY && linePointY2 < rectangleRightBottomY))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }
        #endregion
    }
}
