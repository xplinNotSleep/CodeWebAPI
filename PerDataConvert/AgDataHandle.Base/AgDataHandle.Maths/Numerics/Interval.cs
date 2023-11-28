using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgDataHandle.Maths.Numerics
{
    public class Interval
    {
        private bool m_minClosed = false;
        private bool m_maxClosed = false;
        public float Min { get; private set; }
        public float Max { get; private set; }
        public Interval(string str)
        {
            try
            {
                var array = str.Split('-');
                var value1 = float.Parse(str.Split('-')[0]);
                var value2 = float.Parse(str.Split('-')[1]);
                Min = Math.Min(value1, value2);
                Max = Math.Max(value1, value2);
            }
            catch (Exception)
            {
                throw new Exception("输入格式不正确");
            }


        }
        public Interval(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public Interval(float min, float max, bool minClosed, bool maxClosed)
        {
            Min = min;
            Max = max;
            m_minClosed = minClosed;
            m_maxClosed = maxClosed;
        }
        public bool IsInInterval(float value)
        {
            if (IsBeyondMax(value) && IsBeyondMin(value))
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return Min + "-" + Max;
        }
        private bool IsBeyondMax(float value)
        {
            var flag = false;
            if (m_maxClosed)
                flag = value <= Max ? true : false;
            else
                flag = value < Max ? true : false;
            return flag;
        }
        private bool IsBeyondMin(float value)
        {
            var flag = false;
            if (m_minClosed)
                flag = value >= Min ? true : false;
            else
                flag = value > Min ? true : false;
            return flag;
        }
    }
}
