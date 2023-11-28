using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 关于旋转的代码在这里
    /// </summary>
    public partial class MathAlgorithm
    {
        public static Vector3 RoateX(Vector3 vec, double d)
        {
            var cos = (float)Math.Cos(-d / 180 * Math.PI);
            var sin = (float)Math.Sin(-d / 180 * Math.PI);
            var x = vec[0];
            var y = cos * vec[1] - sin * vec[2];
            var z = sin * vec[1] + cos * vec[2];

            return new Vector3(x, y, z);
        }
        public static Vector3 RoateY(Vector3 vec, double d)
        {
            var cos = (float)Math.Cos(-d / 180 * Math.PI);
            var sin = (float)Math.Sin(-d / 180 * Math.PI);
            var x = cos * vec[0] + sin * vec[2];
            var y = vec[1];
            var z = -1 * vec[0] * sin + cos * vec[2];

            return new Vector3(x, y, z);
        }
        public static Vector3 RoateZ(Vector3 vec, double d)
        {
            var cos = (float)Math.Cos(-1 * d / 180 * Math.PI);
            var sin = (float)Math.Sin(-1 * d / 180 * Math.PI);
            var x = cos * vec[0] - sin * vec[1];
            var y = sin * vec[0] + cos * vec[1];
            var z = vec[2];

            return new Vector3(x, y, z);
        }
        public static Vector<float> RoateZ(Vector<float> vec, double d)
        {
            var cos = (float)Math.Cos(-1 * d / 180 * Math.PI);
            var sin = (float)Math.Sin(-1 * d / 180 * Math.PI);
            var x = cos * vec[0] - sin * vec[1];
            var y = sin * vec[0] + cos * vec[1];
            var z = vec[2];

            return new DenseVector(new float[] { x, y, z });
        }
        /// <summary>
        /// 角度逆时针为正
        /// </summary>
        /// <param name="vector2"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 RoteWithOrigin(Vector2 vector2, float angle)
        {
            var r = angle / 180 * Math.PI;
            var x = vector2.X * Math.Cos(r) - vector2.Y * Math.Sin(r);
            var y = vector2.X * Math.Sin(r) + vector2.Y * Math.Cos(r);
            return new Vector2((float)x, (float)y);
        }
    }
}
