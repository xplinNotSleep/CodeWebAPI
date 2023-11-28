using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 包围盒最大值最小值
    /// </summary>
    public class MinMax
    {
        public MinMax()
        {
            Min = double.MaxValue;
            Max = double.MinValue;
        }
        public double Min { get; set; }

        public double Max { get; set; }

        public bool IsValid()
        {
            return Min <= Max;
        }

        public override string ToString()
        {
            return $"(Min: {Min}, Max: {Max})";
        }

        /// <summary>
        /// update bounding box with double array
        /// </summary>
        /// <param name="vs"></param>
        /// <param name="minMax"></param>

        public static void UpdateMinMax(float[] vs, MinMax minMax)
        {
            var min = vs.Min();
            var max = vs.Max();
            if (minMax.Min > min)
            {
                minMax.Min = min;
            }
            if (minMax.Max < max)
            {
                minMax.Max = max;
            }
        }
    }
}
