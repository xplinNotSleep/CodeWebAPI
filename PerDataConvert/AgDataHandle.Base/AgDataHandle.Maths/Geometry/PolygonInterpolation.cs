using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.Geometry
{
    /// <summary>
    /// 多边形插值
    /// </summary>
    public class PolygonInterpolation
    {
        /// <summary>
        /// 通过距离插值
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <returns></returns>
        public static List<Vector2> InterpolateByDistance(List<Vector2> sourcePoints,float distance)
        {
            var re = new List<Vector2>();
            float lastlength = 0;
            for (int i = 0; i < sourcePoints.Count; i++)
            {               
                if(i!=sourcePoints.Count-1)
                {
                    re.AddRange(Interpolation.InterpolateByDistance(sourcePoints[i], sourcePoints[i + 1], distance,ref lastlength));
                }
                else
                {
                    re.AddRange(Interpolation.InterpolateByDistance(sourcePoints[i], sourcePoints[0], distance,ref lastlength));
                }
            }
            return re;
        }
    }
}
