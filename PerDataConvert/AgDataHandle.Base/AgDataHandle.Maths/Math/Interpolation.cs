using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 插值
    /// </summary>
    public class Interpolation
    {
        /// <summary>
        /// 通过距离插值
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <returns></returns>
        public static List<Vector2> InterpolateByDistance(Vector2 start, Vector2 end, float distance, ref float lastlength)
        {
            var treepoints = new List<Vector2>();
            var length = (float)end.MinusToOther(start).Length();
            var originStart = start;
            if (lastlength+length<distance)
                lastlength += length;
            else
            {
                int count = (int)(lastlength + length) / (int)distance;
                float restDeta = 0;
                for (int i = 0; i < count; i++)
                {              
                    if (i == 0)
                    {                       
                        restDeta = distance-lastlength;
                        Vector2 point = restDeta / length * (end - originStart);
                        Vector2 infactPoint = point.Add(start);
                        treepoints.Add(infactPoint);
                        start = infactPoint;
                    }
                    else
                    {
                       float Deta = restDeta + distance * i;
                       Vector2 point = Deta / length * (end - originStart);
                       Vector2 infactPoint = point.Add(originStart);
                       treepoints.Add(infactPoint);
                       start = infactPoint;
                    }
                }
                lastlength = (float)end.MinusToOther(start).Length();       
            }  
            return treepoints;

        }

    }
}

