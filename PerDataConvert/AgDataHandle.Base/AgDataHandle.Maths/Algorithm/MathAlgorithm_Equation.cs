using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        /// <summary>
        /// 求解二元一次方程组
        /// </summary>
        /// <param name="coefficient0">方程1的系数，如ax+by=c，则Vector3(a,b,c)</param>
        /// <param name="coefficient1">方程2的系数，如ax+by=c，则Vector3(a,b,c)</param>
        /// <returns>Tuple<float,float>,item1为x，item2为y，当无解时返回null</returns>
        public static Tuple<float,float> SolveLinearEquationInTwoUnknowns(Vector3 coefficient0,Vector3 coefficient1)
        {
            var x = (coefficient0.Z * coefficient1.Y - coefficient1.Z * coefficient0.Y) / (coefficient0.X * coefficient1.Y - coefficient1.X * coefficient0.Y);
            var y =Math.Abs(coefficient0.Y) <=0.001 ? (coefficient1.Z - coefficient1.X * x) / coefficient1.Y:(coefficient0.Z - coefficient0.X * x) / coefficient0.Y;
            if (float.IsNaN(x) || float.IsNaN(y)||float.IsInfinity(x) || float.IsInfinity(y))
                return null;
            return new Tuple<float,float>(x,y);
        }

        /// <summary>
        /// 根据两点计算直线方程系数k和b，y = kx+b，当直线垂直x轴时，系数为空
        /// </summary>
        /// <param name="LineP0"></param>
        /// <param name="LineP1"></param>
        /// <returns></returns>
        public static Tuple<float,float> ComputeKB(Vector2 LineP0,Vector2 LineP1)
        {
            var xOffset = LineP1.X - LineP0.X;
            var yOffset = LineP1.Y - LineP0.Y;
            if (xOffset == 0)
                return null;
            var k = yOffset / xOffset;
            var b = LineP1.Y - k * LineP1.X;
            return new Tuple<float, float>(k, b);
        }
        public static Tuple<float, float> ComputeKB(Vector2Int LineP0, Vector2Int LineP1)
        {
            var xOffset = LineP1.X - LineP0.X;
            var yOffset = LineP1.Y - LineP0.Y;
            if (xOffset == 0)
                return null;
            var k = yOffset / (float)xOffset;
            var b = LineP1.Y - k * LineP1.X;
            return new Tuple<float, float>(k, b);
        }
    }
}
