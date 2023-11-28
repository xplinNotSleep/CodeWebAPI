using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AgDataHandle.Maths.SpatialStructure
{
    /// <summary>
    /// 光线追踪过程中的采样
    /// </summary>
    public class RayTracingSampling
    {
        private static Vector3 Top =new Vector3(0,0,1);
        public static List<Vector3> AllDir;

        /// <summary>
        /// 根据采样方式获取漫反射反射方向
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Vector3 GetSamplingDirForDiffuseReflection(DiffuseReflectionSamplingMode mode= DiffuseReflectionSamplingMode.Hemisphere)
        {
            Vector3 refDir = null;
            while(refDir==null)
            {
                var random = new Random(Guid.NewGuid().GetHashCode());
                var index = random.Next(0,AllDir.Count);
                var dir = AllDir[index];
                var cosx = dir.Dot(Top);
                var pdfV = HemispherePDF(cosx);
                var rv = random.NextDouble();
                if(rv<=pdfV)
                {
                    refDir = dir;
                }
            }
            return refDir;
        }
        public static Vector3 GetHemisphereSurfaceDir()
        {
            var constant = 2 * Math.PI * 0.618;
            var num = 360;
            var random = new Random(Guid.NewGuid().GetHashCode());
            var n = 0;
            var z = 0f;
            do
            {
                n = random.Next(0, 360);
                z = (2 * n - 1f) / num - 1f;
            } while (z < 0);
          
            var zz = Math.Pow(z, 2);
            var rzz = Math.Sqrt(1f - zz);
            var x = rzz * Math.Cos(constant * n);
            var y = rzz * Math.Sin(constant * n);
            var dir = new Vector3(x, y, z);
            return dir;
        }

        /// <summary>
        /// 漫反射半球的pdf概率函数
        /// </summary>
        /// <param name="cosx"></param>
        /// <returns></returns>
        private static float HemispherePDF(float cosx)
        {
            return cosx / (float)Math.PI;
        }
    }

    public enum DiffuseReflectionSamplingMode
    {
        [Description("半球采样")]
        Hemisphere,
        [Description("brdf采样")]
        BRDF
    }
}
