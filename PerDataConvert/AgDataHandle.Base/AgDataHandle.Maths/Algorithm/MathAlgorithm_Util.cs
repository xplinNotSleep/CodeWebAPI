using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm
    {
        /// <summary>
        /// 返回最小值
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <param name="val3">The third value.</param>
        /// <returns>The minimum value.</returns>
        public static double Min(double val1, double val2, double val3)
        {
            return (val1 < val2 ? (val1 < val3 ? val1 : val3) : (val2 < val3 ? val2 : val3));
        }

        public static float Min(params float[] values)
        {
            var min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if(values[i]<min)
                {
                    min = values[i];
                }
            }
            return min;
        }
        public static int Min(params int[] values)
        {
            var min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] < min)
                {
                    min = values[i];
                }
            }
            return min;
        }
        public static float Max(params float[] values)
        {
            var max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
            }
            return max;
        }
        public static int Max(params int[] values)
        {
            var max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
            }
            return max;
        }

        public static Vector3 Min(params Vector3[] values)
        {
            var minX = values[0].X;
            var minY = values[0].Y;
            var minZ = values[0].Z;
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].X < minX)
                {
                    minX = values[i].X;
                }
                if (values[i].Y < minY)
                {
                    minY = values[i].Y;
                }
                if (values[i].Z < minZ)
                {
                    minZ = values[i].Z;
                }
            }
            return new Vector3(minX,minY,minZ);
        }
        public static Vector3 Max(params Vector3[] values)
        {
            var maxX = values[0].X;
            var maxY = values[0].Y;
            var maxZ = values[0].Z;
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].X > maxX)
                {
                    maxX = values[i].X;
                }
                if (values[i].Y > maxY)
                {
                    maxY = values[i].Y;
                }
                if (values[i].Z > maxZ)
                {
                    maxZ = values[i].Z;
                }
            }
            return new Vector3(maxX, maxY, maxZ);
        }

        /// <summary>
        /// 限制一个值在最大最小之间
        /// </summary>
        /// <param name="value">限制的值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>限制后的值</returns>
        public static float Clamp(float value, float min, float max)
        {
            return (value >= min ? (value <= max ? value : max) : min);
        }
        public static int Clamp(int value, int min, int max)
        {
            return (value >= min ? (value <= max ? value : max) : min);
        }

        /// <summary>
        /// 取小数部分，如1.1为0.1；-0.1为0.9
        /// </summary>
        /// <param name="value"></param>
        public static void Fract(ref Vector2 value)
        {
            var x = value.X - (int)value.X;
            var y = value.Y - (int) value.Y;
            if (x < 0)
                x = 1 + x;
            if(y<0)
                y = 1 + y;
            value.X = x;
            value.Y = y;
        }
        public static float Fract(float value)
        {
            var result = value - (int)value;
            if (result < 0)
                result = 1 + result;
            return result;
        }

        /// <summary>
        /// 计算三个分量的方差
        /// </summary>
        /// <param name="vector3s"></param>
        /// <returns></returns>
        public static Vector3 ComputeVariance(List<Vector3> vector3s)
        {
            var sum = new Vector3(0,0,0);
            foreach (var item in vector3s)
            {
                sum += item;
            }
            var average =sum/ vector3s.Count;
            sum = new Vector3(0,0,0);
            foreach (var item in vector3s)
            {
                sum.X += (float)Math.Pow(item.X - average.X, 2);
                sum.Y += (float)Math.Pow(item.Y - average.Y, 2);
                sum.Z += (float)Math.Pow(item.Z - average.Z, 2);
            }
            return sum / vector3s.Count;
        }
        public static float ComputeVariance(List<float> values)
        {
            var sum = 0f;
            foreach (var item in values)
            {
                sum += item;
            }
            var average = sum / values.Count;
            sum = 0;
            foreach (var item in values)
            {
                sum += (float)Math.Pow(item - average, 2);
            }
            return sum / values.Count;
        }

        /// <summary>
        /// 当使用自带的Math.Powe 有精度问题，可采用此方法
        /// </summary>
        /// <param name="v"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float Pow(float v,int num)
        {
            var result = v; 
            for (int i = 1; i < num; i++)
            {
                result *= v;
            }
            return result;
        }

        public static void Swap(Vector3 p0,Vector3 p1)
        {
            var x = p0.X;
            var y = p0.Y;
            var z = p0.Z;
            p0.X = p1.X;
            p0.Y = p1.Y;
            p0.Z = p1.Z;
            p1.X = x;
            p1.Y = y;
            p1.Z = z;

        }
    }
}
