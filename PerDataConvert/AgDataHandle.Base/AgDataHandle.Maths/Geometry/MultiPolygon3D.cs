using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AgDataHandle.Maths.Geometry
{
    /// <summary>
    /// 三维空间的复合多边形
    /// </summary>
    public class MultiPolygon3D
    {
        public float RotationY { get; set; } = 0;
        public List<Polygon3D> Polygons { get; set; } = new List<Polygon3D>();

        /// <summary>
        /// 是否存在非闭合的多边形
        /// </summary>
        /// <returns></returns>
        public bool HaveOpenPolygon()
        {
            var flag = false;
            foreach (var polygon in Polygons)
            {
                if (polygon.IsClosed())
                    continue;
                flag = true;
                break;
            }
            return flag;
        }

        /// <summary>
        /// 获取所有的点
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetAllPoints()
        {
            var ps = new List<Vector3>();
            foreach (var item in Polygons)
            {
                ps.AddRange(item.Path);
            }
            return ps;
        }

        /// <summary>
        /// 从线段创建复合多边形
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static MultiPolygon3D CreateMultiPolygon3DFromLines(List<Line> lines)
        {
            MultiPolygon3D multiPolygon3D = new MultiPolygon3D();
            while (lines.Count > 0)
            {
                var polygon = new Polygon3D();
                polygon.Path = new List<Vector3>();
                var startLine = lines[0];
                polygon.Path.Add(startLine.startPoint);
                polygon.Path.Add(startLine.endPoint);
                lines.RemoveAt(0);
                var index = FindNextLine(startLine, lines);
                while (index != -1)
                {
                    startLine = lines[index];
                    polygon.Path.Add(lines[index].endPoint);
                    lines.RemoveAt(index);
                    index = FindNextLine(startLine, lines);
                }
                if (polygon.Path[0].Equals(polygon.Path[polygon.Path.Count - 1]))
                    polygon.Closed = true;
                multiPolygon3D.Polygons.Add(polygon);
            }

            return multiPolygon3D;
        }
        
        /// <summary>
        /// 寻找下一条连接的线段
        /// </summary>
        /// <param name="start"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static int FindNextLine(Line start, List<Line> lines)
        {
            int nextLineIndex = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (start.endPointIndex.Equals(line.startPointIndex))
                {
                    nextLineIndex = i;
                    break;
                }
                else if (start.endPointIndex.Equals(line.endPointIndex))
                {
                    line.Swap();
                    nextLineIndex = i;
                    break;
                }
            }

            return nextLineIndex;
        }

        /// <summary>
        /// 简化复合多边形
        /// </summary>
        /// <param name="multiPolygon3D"></param>
        public static void SimplifyMultiPolygon3D(MultiPolygon3D multiPolygon3D)
        {
            if (multiPolygon3D.Polygons == null)
                return;
            foreach (var item in multiPolygon3D.Polygons)
            {
                item.SimplifyPolygon();
            }
        }
    }
}
