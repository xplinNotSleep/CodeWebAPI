using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        /// <summary>
        /// 获取大于或等于输入值的二次幂
        /// </summary>
        /// <param name="inputNum"></param>
        /// <param name="maxLimit"></param>
        /// <returns></returns>
        public static float TwoPowerWithCeiling(float inputNum, float maxLimit = float.MaxValue)
        {
            var result = 2.0;
            for (int i = 2; result < inputNum; i++)
            {
                result = Math.Pow(2, i);
                if (result > maxLimit)
                {
                    result = maxLimit;
                    break;
                }
            }
            return (float)result;
        }
        /// <summary>
        /// 获取小于或等于输入值的二次幂
        /// </summary>
        /// <param name="inputNum"></param>
        /// <param name="maxLimit"></param>
        /// <returns></returns>
        public static float TwoPowerWithFloor(float inputNum, float maxLimit = float.MaxValue)
        {
            var result = TwoPowerWithCeiling(inputNum, maxLimit);
            return (float)result / 2;
        }

        public static int TwoPowerWithNearest(float inputNum)
        {
            var min = TwoPowerWithFloor(inputNum);
            var max = TwoPowerWithCeiling(inputNum);
            return (int)(Math.Abs(inputNum - min) < Math.Abs(inputNum - max) ? min : max);
        }
        public static bool IsTwoPower(int num)
        {
            if (num < 1) return false;
            //若是2N次方，则2N次方-1的2进制 & 上2N次方各位均为0
            //例： 8：1000  7 ：0111   & 的结果为0
            return (num & num - 1) == 0;
        }
    }
}
