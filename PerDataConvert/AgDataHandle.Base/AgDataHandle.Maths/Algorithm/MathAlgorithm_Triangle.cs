using AgDataHandle.Maths.Geometry;
using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        #region 判断点是否在三角形内
        /// <summary>
        /// 判断点是否在三角形内
        /// </summary>
        /// <returns></returns>
        public static bool IsInTriangle(Vector3d a, Vector3d b, Vector3d c, Vector3d point)
        {
            //先判断包围盒
            double minX = Math.Min(a.X, Math.Min(b.X, c.X));
            double minY = Math.Min(a.Y, Math.Min(b.Y, c.Y));
            double maxX = Math.Max(a.X, Math.Max(b.X, c.X));
            double maxY = Math.Max(a.Y, Math.Max(b.Y, c.Y));
            if (point.X < minX || point.X > maxX || point.Y < minY || point.Y > maxY)
            {
                return false;
            }

            Vector3d v0 = c - a;
            Vector3d v1 = b - a;
            Vector3d v2 = point - a;

            double dot00 = v0.X * v0.X + v0.Y * v0.Y;
            double dot01 = v0.X * v1.X + v0.Y * v1.Y;
            double dot02 = v0.X * v2.X + v0.Y * v2.Y;
            double dot11 = v1.X * v1.X + v1.Y * v1.Y;
            double dot12 = v1.X * v2.X + v1.Y * v2.Y;

            double inverDeno = (dot00 * dot11 - dot01 * dot01);

            double u = (dot11 * dot02 - dot01 * dot12) / inverDeno;

            if (u < 0 || u > 1)
                return false;

            double v = (dot00 * dot12 - dot01 * dot02) / inverDeno;

            if (v < 0 || v > 1)
                return false;

            return u + v <= 1;
        }
        public static bool IsInTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
        {
            double minX = Math.Min(A.X, Math.Min(B.X, C.X));
            double minY = Math.Min(A.Y, Math.Min(B.Y, C.Y));
            double maxX = Math.Max(A.X, Math.Max(B.X, C.X));
            double maxY = Math.Max(A.Y, Math.Max(B.Y, C.Y));
            if (P.X < minX || P.X > maxX || P.Y < minY || P.Y > maxY)
            {
                return false;
            }
            return PointinTriangle(A,B,C,  P);
        }
        private static bool SameSide(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
        {
            var AB = B - A;
            var AC = C - A;
            var AP = P - A;

            var v1 =AB.CrossToOther(AC);
            var v2 =AB.CrossToOther(AP);

            // v1 and v2 should point to the same direction

            return v1.Dot(v2) >= 0;
        }

        private static bool PointinTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
        {
            return SameSide(A, B, C, P) &&
             SameSide(B, C, A, P) &&
             SameSide(C, A, B, P);
        }
        private static bool  isSameFace(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
        {
            var ab = B - A;
            var ac = C - A;
            var ap = P - A;

            var ans = (ab.X * ac.Y * ap.Z + ab.Y * ac.Z * ap.X + ab.Z * ac.X * ap.Y - 
                ab.Z * ac.Y * ap.X - ab.X * ac.Z * ap.Y - ab.Y * ac.X * ap.Z);
            if (ans == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static bool IsInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 point)
        {
            double minX = Math.Min(a.X, Math.Min(b.X, c.X));
            double minY = Math.Min(a.Y, Math.Min(b.Y, c.Y));
            double maxX = Math.Max(a.X, Math.Max(b.X, c.X));
            double maxY = Math.Max(a.Y, Math.Max(b.Y, c.Y));
            if (point.X < minX || point.X > maxX || point.Y < minY || point.Y > maxY)
            {
                return false;
            }
            Vector2 v0 = c - a;
            Vector2 v1 = b - a;
            Vector2 v2 = point - a;

            double dot00 = v0.X * v0.X + v0.Y * v0.Y;
            double dot01 = v0.X * v1.X + v0.Y * v1.Y;
            double dot02 = v0.X * v2.X + v0.Y * v2.Y;
            double dot11 = v1.X * v1.X + v1.Y * v1.Y;
            double dot12 = v1.X * v2.X + v1.Y * v2.Y;

            double inverDeno = (dot00 * dot11 - dot01 * dot01);

            double u = (dot11 * dot02 - dot01 * dot12) / inverDeno;

            if (u < 0 || u > 1)
                return false;

            double v = (dot00 * dot12 - dot01 * dot02) / inverDeno;

            if (v < 0 || v > 1)
                return false;

            return u + v <= 1;
        }
        #endregion

        #region 三角形插值
        /// <summary>
        /// 计算三角形三个顶点分别对目标点的影响
        /// </summary>
        /// from cesium, which uses barycentric 
        /// https://github.com/CesiumGS/cesium/blob/9f5b492f96a0003c85f031f399584541452144d7/Source/Core/GoogleEarthEnterpriseTerrainData.js  row 466
        /// https://github.com/CesiumGS/cesium/blob/9f5b492f96a0003c85f031f399584541452144d7/Source/Core/Intersections2D.js  computeBarycentricCoordinates row 236
        /// https://blog.csdn.net/qq_35045096/article/details/95446984 根据重心坐标来插值的公式
        /// <returns></returns>
        public static double[] CalculateEachPointRatio(Vector3d a, Vector3d b, Vector3d c, Vector3d point)
        {
            double x1mx3 = a.X - c.X;
            double x3mx2 = c.X - b.X;
            double y2my3 = b.Y - c.Y;
            double y1my3 = a.Y - c.Y;
            double determinant = y2my3 * x1mx3 + x3mx2 * y1my3;
            //如果determinant为0，则三角形三个点共线
            //共线的情况直接按反距离权重计算
            if (determinant < 1e-10)
            {
                Vector2d a_2d = new Vector2d(a.X, a.Y);
                Vector2d b_2d = new Vector2d(b.X, b.Y);
                Vector2d c_2d = new Vector2d(c.X, c.Y);
                double inverse_distance_a = 1.0/a_2d.DistanceTo(point.X, point.Y);
                double inverse_distance_b = 1.0 / b_2d.DistanceTo(point.X, point.Y);
                double inverse_distance_c = 1.0 / c_2d.DistanceTo(point.X, point.Y);
                double inverse_distance_sum = inverse_distance_a + inverse_distance_b + inverse_distance_c;
                return new double[3] { inverse_distance_a / inverse_distance_sum, inverse_distance_b / inverse_distance_sum, inverse_distance_c / inverse_distance_sum };
            }
            else
            {
                double inverseDeterminant = 1.0 / determinant;
                double ymy3 = point.Y - c.Y;
                double xmx3 = point.X - c.X;
                double ratio1 = (y2my3 * xmx3 + x3mx2 * ymy3) * inverseDeterminant;
                double ratio2 = (-y1my3 * xmx3 + x1mx3 * ymy3) * inverseDeterminant;
                double ratio3 = 1.0 - ratio1 - ratio2;
                return new double[3] { ratio1, ratio2, ratio3 };
            }
        }
        public static double[] CalculateEachPointRatio(Vector3 a, Vector3 b, Vector3 c, Vector3 point)
        {
            double x1mx3 = a.X - c.X;
            double x3mx2 = c.X - b.X;
            double y2my3 = b.Y - c.Y;
            double y1my3 = a.Y - c.Y;
            double determinant = y2my3 * x1mx3 + x3mx2 * y1my3;
            //如果determinant为0，则三角形三个点共线
            //共线的情况直接按反距离权重计算
            if (determinant < 1e-10)
            {
                Vector2d a_2d = new Vector2d(a.X, a.Y);
                Vector2d b_2d = new Vector2d(b.X, b.Y);
                Vector2d c_2d = new Vector2d(c.X, c.Y);
                double inverse_distance_a = 1.0 / a_2d.DistanceTo(point.X, point.Y);
                double inverse_distance_b = 1.0 / b_2d.DistanceTo(point.X, point.Y);
                double inverse_distance_c = 1.0 / c_2d.DistanceTo(point.X, point.Y);
                double inverse_distance_sum = inverse_distance_a + inverse_distance_b + inverse_distance_c;
                return new double[3] { inverse_distance_a / inverse_distance_sum, inverse_distance_b / inverse_distance_sum, inverse_distance_c / inverse_distance_sum };
            }
            else
            {
                double inverseDeterminant = 1.0 / determinant;
                double ymy3 = point.Y - c.Y;
                double xmx3 = point.X - c.X;
                double ratio1 = (y2my3 * xmx3 + x3mx2 * ymy3) * inverseDeterminant;
                double ratio2 = (-y1my3 * xmx3 + x1mx3 * ymy3) * inverseDeterminant;
                double ratio3 = 1.0 - ratio1 - ratio2;
                return new double[3] { ratio1, ratio2, ratio3 };
            }
        }
        public static double[] CalculateEachPointRatio(Vector2 a, Vector2 b, Vector2 c, Vector2 point)
        {
            double x1mx3 = a.X - c.X;
            double x3mx2 = c.X - b.X;
            double y2my3 = b.Y - c.Y;
            double y1my3 = a.Y - c.Y;
            double determinant = y2my3 * x1mx3 + x3mx2 * y1my3;
            //如果determinant为0，则三角形三个点共线
            //共线的情况直接按反距离权重计算
            if (determinant < 1e-10)
            {
                double inverse_distance_a = 1.0 / a.DistanceTo(point.X, point.Y);
                double inverse_distance_b = 1.0 / b.DistanceTo(point.X, point.Y);
                double inverse_distance_c = 1.0 / c.DistanceTo(point.X, point.Y);
                double inverse_distance_sum = inverse_distance_a + inverse_distance_b + inverse_distance_c;
                return new double[3] { inverse_distance_a / inverse_distance_sum, inverse_distance_b / inverse_distance_sum, inverse_distance_c / inverse_distance_sum };
            }
            else
            {
                double inverseDeterminant = 1.0 / (y2my3 * x1mx3 + x3mx2 * y1my3);
                double ymy3 = point.Y - c.Y;
                double xmx3 = point.X - c.X;
                double ratio1 = (y2my3 * xmx3 + x3mx2 * ymy3) * inverseDeterminant;
                double ratio2 = (-y1my3 * xmx3 + x1mx3 * ymy3) * inverseDeterminant;
                double ratio3 = 1.0 - ratio1 - ratio2;
                return new double[3] { ratio1, ratio2, ratio3 };
            }
        }
        public static double[] CalculateEachPointRatio(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int point)
        {
            double x1mx3 = a.X - c.X;
            double x3mx2 = c.X - b.X;
            double y2my3 = b.Y - c.Y;
            double y1my3 = a.Y - c.Y;
            double determinant = y2my3 * x1mx3 + x3mx2 * y1my3;
            //如果determinant为0，则三角形三个点共线
            //共线的情况直接按反距离权重计算
            if (determinant < 1e-10)
            {
                double inverse_distance_a = 1.0 / a.DistanceTo(point.X, point.Y);
                double inverse_distance_b = 1.0 / b.DistanceTo(point.X, point.Y);
                double inverse_distance_c = 1.0 / c.DistanceTo(point.X, point.Y);
                double inverse_distance_sum = inverse_distance_a + inverse_distance_b + inverse_distance_c;
                return new double[3] { inverse_distance_a / inverse_distance_sum, inverse_distance_b / inverse_distance_sum, inverse_distance_c / inverse_distance_sum };
            }
            else
            {
                double inverseDeterminant = 1.0 / (y2my3 * x1mx3 + x3mx2 * y1my3);
                double ymy3 = point.Y - c.Y;
                double xmx3 = point.X - c.X;
                double ratio1 = (y2my3 * xmx3 + x3mx2 * ymy3) * inverseDeterminant;
                double ratio2 = (-y1my3 * xmx3 + x1mx3 * ymy3) * inverseDeterminant;
                double ratio3 = 1.0 - ratio1 - ratio2;
                return new double[3] { ratio1, ratio2, ratio3 };
            }
        }


        /// <summary>
        /// 插值得到高程
        /// </summary>
        /// <param name="indices">三角网的索引数组</param>
        /// <param name="points">三角网的点数组</param>
        /// <param name="targetPoint">待求值的点，z值无用</param>
        /// <returns></returns>
        public static bool InterpolateHeight(List<int> indices, List<Vector3d> points, Vector3d targetPoint, out double height)
        {
            height = double.MinValue;
            for (int i = 0; i < indices.Count; i += 3)
            {
                Vector3d a = points[indices[i]];
                Vector3d b = points[indices[i + 1]];
                Vector3d c = points[indices[i + 2]];

                if (IsInTriangle(a, b, c, targetPoint))
                {
                    double[] ratios = CalculateEachPointRatio(a, b, c, targetPoint);
                    height = a.Z * ratios[0] + b.Z * ratios[1] + c.Z * ratios[2];
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 三角形填充
        /// <summary>
        /// 使用DDA扫描线算法填充三角形
        /// </summary>
        /// <returns></returns>
        public static List<Vector2Int> FillTriangleByDDA(List<Vector2Int> points,int expand)
        {
            var fillPoints = new List<Vector2Int>();
            points.Sort((a, b) => -a.Y.CompareTo(b.Y));
            var p = points;
            if (points[1].Y == points[2].Y)
            {
                FillBottomFlatTriangle(points[0], points[1], points[2], expand,ref fillPoints);
            }
            else if (points[0].Y == points[1].Y)
            {
                FillTopFlatTriangle(points[0], points[1], points[2], expand,ref fillPoints);
            }
            else
            {
                var coefficient = ComputeKB(p[2],p[0]);
                Vector2Int mid = null;
                if (coefficient==null)
                {
                    mid = new Vector2Int((int)p[0].X, p[1].Y);
                }
                else
                {
                    var interX = (p[1].Y - coefficient.Item2) / coefficient.Item1;
                    mid = new Vector2Int((int)interX, p[1].Y);
                }
                
                FillBottomFlatTriangle(points[0], points[1], mid, expand,ref fillPoints);
                FillTopFlatTriangle(points[1], mid, points[2], expand,ref fillPoints);
            }
            
            return fillPoints;
        }

        public static List<Vector2> RasterizeTriangleByHalf(Vector2Int a, Vector2Int b, Vector2Int c, int expand)
        {
            var box = new BoundingBox2();
            box.Update(a.X, a.Y);
            box.Update(b.X, b.Y);
            box.Update(c.X, c.Y);
            var ps = new List<Vector2>();
            var ayby = a.Y - b.Y;
            var bxax = b.X - a.X;
            var ab = a.X*b.Y-a.Y*b.X;
            
            var bycy = b.Y - c.Y;
            var cxbx = c.X - b.X;
            var bc = b.X * c.Y - b.Y * c.X;

            var cyay = c.Y - a.Y;
            var axcx = a.X - c.X;
            var ca = c.X*a.Y-c.Y*a.X;

            var f1s = ayby * box.xMin + bxax * box.yMin + ab;
            var f2s = bycy * box.xMin + cxbx * box.yMin + bc;
            var f3s = cyay * box.xMin + axcx * box.yMin + ca;
            for (int y = (int)box.yMin; y <= box.yMax; y++)
            {
                var xf1s = f1s;
                var xf2s = f2s;
                var xf3s = f3s;
                for (int x = (int)box.xMin; x <= box.xMax; x++)
                {
                    if (xf1s >= 0&& xf2s >= 0&& xf3s >= 0)
                    {
                        ps.Add(new Vector2(x, y));
                    }
                    xf1s += ayby;
                    xf2s += bycy;
                    xf3s += cyay;
                }
                f1s += bxax;
                f2s += cxbx;
                f3s += axcx;
            }
            return ps;
        }

        public static List<Vector2> RasterizeTriangleByHalfFast(Vector2Int a, Vector2Int b, Vector2Int c, int expand)
        {
            var I1 = a.Y - b.Y;
            var I2 = b.Y - c.Y;
            var I3 = c.Y - a.Y;
            var J1 = b.X - a.X;
            var J2 = c.X - b.X;
            var J3 = a.X - c.X;
            var F1 = a.X * b.Y - a.Y * b.X;
            var F2 = b.X * c.Y - b.Y * c.X;
            var F3 = c.X * a.Y - c.Y * a.X;

            var cy1 = F1;
            var cy2 = F2;
            var cy3 = F3;
            var box = new BoundingBox2();
            box.Update(a.X, a.Y);
            box.Update(b.X, b.Y);
            box.Update(c.X, c.Y);
            var ps = new List<Vector2>();
            for (int y = (int)box.yMin; y <= box.yMax; y++)
            {
                var cx1 = cy1;
                var cx2 = cy2;
                var cx3 = cy3;
                for (int x = (int)box.xMin; x <= box.xMax; x++)
                {
                    if (cx1 >= 0 && cx2 >= 0 && cx3 >= 0)
                    {
                        ps.Add(new Vector2(x, y));
                    }
                    cx1 += I1;
                    cx2 += I2;
                    cx3 += I3;
                }
                cy1 += J1;
                cy2 += J2;
                cy3 += J3;
            }
            return ps;
        }

        /// <summary>
        /// 通过计算质心，实现三角填充
        /// </summary>
        /// <param name="points"></param>
        /// <param name="expand"></param>
        /// <returns></returns>
        public static List<Vector2> RasterizeTriangleByBarycentric(Vector2Int a, Vector2Int b, Vector2Int c, int expand)
        {
            var AB = b - a;
            var AC = c - a;
            var d = 1.0f / (AB.X * AC.Y - AB.Y * AC.Y);
            var k = 1.0f/AC.X;
            var xmin = MathAlgorithm.Min(a.X,b.X,c.X);
            var xmax= MathAlgorithm.Max(a.X, b.X, c.X);
            var ymin = MathAlgorithm.Min(a.Y, b.Y, c.Y);
            var ymax = MathAlgorithm.Max(a.Y, b.Y, c.Y);

            var ps = new List<Vector2>();
            for (var x = xmin; x <= xmax; x++)
            {
                for (var y = ymin; y <= ymax; y++)
                {
                    var PAy = a.Y - y;
                    var PAx = a.X - x;
                    var u = (PAy * AC.X - PAx * AC.Y)*d;
                    if (u < 0||u>1)
                        continue;
                    var v = (-PAy - u * AB.X) * k;
                    if (v < 0 || v > 1)
                        continue;
                    ps.Add(new Vector2(x,y));
                }
            }
           
            return ps;
        }
        
        private static void FillBottomFlatTriangle(Vector2Int v1, Vector2Int v2, Vector2Int v3, int expand, ref List<Vector2Int> fillPoints)
        {
            float slope1 = (v2.X - v1.X) / (float)(v2.Y - v1.Y);
            float slope2 = (v3.X - v1.X) / (float)(v3.Y - v1.Y);
            
            if(slope1<slope2)
            {
                var t = slope1;
                slope1 = slope2;
                slope2 = t;
            }
            var startX = v1.X;
            var endX = v1.X;

            var num = 1;
            for (var scanlineY = v1.Y; scanlineY >= v2.Y; scanlineY--)
            {
                for (int scanlineX = startX- expand; scanlineX <= endX+ expand; scanlineX++)
                {
                    fillPoints.Add(new Vector2Int(scanlineX, scanlineY));
                }
                startX = (int)Math.Round(v1.X - slope1*num);
                endX =(int)Math.Round(v1.X - slope2*num);
                num++;
            }
        }
        private static void FillTopFlatTriangle(Vector2Int v1, Vector2Int v2, Vector2Int v3, int expand, ref List<Vector2Int> fillPoints)
        {
            float slope1 = (float)(v3.X - v1.X) / (v3.Y - v1.Y);
            float slope2 = (float)(v3.X - v2.X) / (v3.Y - v2.Y);
            if (slope1 > slope2)
            {
                var t = slope1;
                slope1 = slope2;
                slope2 = t;
            }
            var startX = v3.X;
            var endX = v3.X;

            var num = 1;
            for (int scanlineY = v3.Y; scanlineY <= v1.Y; scanlineY++)
            {
                for (int scanlineX = startX- expand; scanlineX <= endX+ expand; scanlineX++)
                {
                    fillPoints.Add(new Vector2Int(scanlineX, scanlineY));
                }
                startX = (int)Math.Round(v3.X + slope1* num);
                endX = (int)Math.Round(v3.X + slope2* num);
                num++;
               
            }
        }
        #endregion

        #region 三角形水平相切
        /// <summary>
        /// 提供在h高度上三角形的切割，得到线段或者为空
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Line GetSliceLineOfTriangle(Vector3[] triangle, double h)
        {
            Vector3 p1 = null, p2 = null;
            for(var i = 0; i < 3; i++)
            {
                var A = triangle[i];
                var B = triangle[(i + 1) % 3];

                if ((A.Z > h && B.Z < h) || (A.Z < h && B.Z > h))
                {
                    var d1 = Math.Abs(A.Z - h);
                    var d2 = Math.Abs(B.Z - h);
                    if (d1 + d2 < 1e-5)
                        continue;
                    var t = new Vector3((A.X * d2 + B.X * d1) / (d1 + d2), (A.Y * d2 + B.Y * d1) / (d1 + d2), h);

                    if (p1 == null)
                        p1 = t;
                    else
                        p2 = t;
                }
            }
            if (p1 != null && p2 != null)
                return new Line(p1, p2);
            else
                return null;
        }
        #endregion
    }
}
