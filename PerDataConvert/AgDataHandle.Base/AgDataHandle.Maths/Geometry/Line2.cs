using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;

namespace AgDataHandle.Maths.Geometry
{
    public class Line2
    {
        public Vector2 P1 { set; get; }
        public Vector2 P2 { set; get; }
    }

    /// <summary>
    /// DouglasPeucker多段线简化
    /// </summary>
    public class DouglasPeucker
    {
        private List<Vector2> m_pointsList;
        private double m_errorBound; // degree
        public DouglasPeucker(List<Vector2> pointsList, double errorBound)
        {
            m_pointsList = new List<Vector2>();
            if (pointsList != null)
            {
                Vector2 last = pointsList[0];
                m_pointsList.Add(last);

                for (int i = 1; i < pointsList.Count; i++)
                {
                    if (!last.Equals(pointsList[i]))
                    {
                        m_pointsList.Add(pointsList[i]);
                        last = pointsList[i];
                    }
                }
            }
            m_errorBound = errorBound;
        }

        public List<Vector2> Compress()
        {
            if (m_pointsList == null || m_pointsList.Count <= 2)
            {
                return m_pointsList;
            }

            List<Vector2> result = CompressHelper(m_pointsList);
            result.Add(m_pointsList[m_pointsList.Count-1]);

            return result;
        }

        private List<Vector2> CompressHelper(List<Vector2> pointsList)
        {
            if (pointsList.Count < 2)
            {
                return pointsList;
            }

            List<Vector2> result = new List<Vector2>();

            // 有可能是polygon
            if (pointsList[0].Equals(pointsList[pointsList.Count-1]))
            {
                var r1 = CompressHelper(pointsList.GetRange(0, pointsList.Count / 2));
                var r2 = CompressHelper(pointsList.GetRange(pointsList.Count / 2, pointsList.Count - pointsList.Count / 2));
                result.AddRange(r1);
                result.AddRange(r2);
                return result;
            }

            Line2 line = new Line2() { P1 = pointsList[0], P2 = pointsList[pointsList.Count-1] };

            double maxDistance = 0;
            int maxIndex = 0;

            for (int i = 1; i < pointsList.Count - 1; i++)
            {
                var distance = Distance(pointsList[i], line);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }

            if (maxDistance <= m_errorBound)
            {
                result.Add(pointsList[0]);
            }
            else
            {
                var r1 = CompressHelper(pointsList.GetRange(0, maxIndex));
                var r2 = CompressHelper(pointsList.GetRange(maxIndex + 1, pointsList.Count - maxIndex - 1));
                result.AddRange(r1);
                result.Add(pointsList[maxIndex]);
                result.AddRange(r2);
            }

            return result;
        }

        private double Distance(Vector2 p, Line2 line)
        {
            var p1 = line.P1;
            var p2 = line.P2;
            return Math.Abs(
                    ((p2.X - p1.X) * p.Y + (p1.Y - p2.Y) * p.X + (p1.X - p2.X) * p1.Y + (p2.Y - p1.Y) * p1.X) /
                    Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y))
                );
        }
    }

   
}

