using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public class MathUtils
    {
        /// <summary>
        /// 通过加上适当的2pi倍数，将角度限制在-PI到PI之间
        /// </summary>
        public static Func<double, double> WrapPi
        {
            get => p =>
            {
                p += Math.PI;
                p -= Math.Floor(p * 1.0 / Math.PI) * Math.PI * 2.0;
                p -= Math.PI;
                return p;
            };
        }

        /// <summary>
        /// 和acos(x)相同，但如果x超出范围将返回最为接近的有效值
        /// </summary>
        public static Func<double, double> SafeAcos
        {
            get => x =>
            {
                //检查边界条件
                if (x <= -1.0)
                {
                    return Math.PI;
                }
                if (x >= 1.0)
                {
                    return 0.0;
                }
                //使用标准的反余弦函数
                return Math.Acos(x);
            };
        }
    }
}
