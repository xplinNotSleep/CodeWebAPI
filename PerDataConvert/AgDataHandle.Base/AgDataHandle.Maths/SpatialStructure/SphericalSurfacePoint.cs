using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AgDataHandle.Maths.SpatialStructure
{
    /// <summary>
    /// 球面点
    /// </summary>
    public class SphericalSurfacePoint
    {
        /// <summary>
        /// 获取球面点
        /// </summary>
        /// <param name="num"></param>
        /// <param name="r"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static List<VertexWithNorml> GetSphericalSurfacePoints(int num, float r,Vector3 center)
        {
            var ps = new List<VertexWithNorml>();
            var constant = 2 * Math.PI * 0.618;
            for (int n = 0; n < num; n++)
            {
                var z = (2 * n - 1f) / num - 1f;
                var zz = Math.Pow(z, 2);
                var rzz = Math.Sqrt(1f - zz);
                var x = rzz * Math.Cos(constant * n);
                var y = rzz * Math.Sin(constant * n);
                var position = new Vector3(x * r, y * r, z * r) + center;
                var normal = position - center;
                normal.Normalize<Vector3>();
                ps.Add(new VertexWithNorml(position, normal));
            }
            return ps;
        }
        /// <summary>
        /// 获取半球表面点
        /// </summary>
        /// <param name="num">点数量</param>
        /// <param name="r">半径</param>
        /// <param name="center">中心点</param>
        /// <returns></returns>
        public static List<VertexWithNorml> GetHemisphereSurfacePoints(int num, float r, Vector3 center)
        {
            var ps = new List<VertexWithNorml>();
            var constant = 2 * Math.PI * 0.618;
            for (int n = 0; n < num; n++)
            {
                var z = (2 * n - 1f) / num - 1f;
                if (z < 0)
                    continue;
                var zz = Math.Pow(z, 2);
                var rzz = Math.Sqrt(1f - zz);
                var x = rzz * Math.Cos(constant * n);
                var y = rzz * Math.Sin(constant * n);
                var position = new Vector3(x * r, y * r, z * r) + center;
                var normal = position - center;
                normal.Normalize<Vector3>();
                ps.Add(new VertexWithNorml(position, normal));
            }
            return ps;
        }
        /// <summary>
        /// 获取半球表面点方向
        /// </summary>
        /// <param name="num"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static List<Vector3> GetHemisphereSurfaceDir(int num, float r)
        {
            var ps = new List<Vector3>();
            var constant = 2 * Math.PI * 0.618;
            for (int n = 0; n < num; n++)
            {
                var z = (2 * n - 1f) / num - 1f;
                if (z < 0)
                    continue;
                var zz = Math.Pow(z, 2);
                var rzz = Math.Sqrt(1f - zz);
                var x = rzz * Math.Cos(constant * n);
                var y = rzz * Math.Sin(constant * n);
                var position = new Vector3(x * r, y * r, z * r);
                ps.Add(position);
            }
            return ps;
        }
    }
}
