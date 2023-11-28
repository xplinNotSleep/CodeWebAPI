using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;

namespace AgDataHandle.Maths.Geometry
{
    public class Line
    {
        public int startPointIndex;
        public int endPointIndex;
        public Vector3 startPoint;
        public Vector3 endPoint;
        public float Lenght { get; set; }

        public Line(Vector3 startPoint, Vector3 endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }
        public Line(Vector2 startPoint, Vector2 endPoint)
        {
            this.startPoint = new Vector3(startPoint.X,startPoint.Y,0);
            this.endPoint = new Vector3(endPoint.X, endPoint.Y, 0); 
        }

        public Line(int startPointIndex, int endPointIndex)
        {
            this.startPointIndex = startPointIndex;
            this.endPointIndex = endPointIndex;
        }
        /// <summary>
        /// 判断两条线是否相连，0表示连接的是起点，1表示连接的是终点，-1表示不相连
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int IsConnected(Line line,float offset=0)
        {
            if (endPoint.Equals(line.startPoint, offset))
                return 0;
            else if (endPoint.Equals(line.endPoint, offset))
                return 1;
            else
                return -1;
        }

        /// <summary>
        /// 是否共线
        /// </summary>
        /// <param name="line0"></param>
        /// <param name="line1"></param>
        /// <returns></returns>
        public static bool IsCollineation(Line line0,Line line1)
        {
            var flag = (line0.startPoint.Equals(line1.startPoint) && line0.endPoint.Equals(line1.endPoint))
                || (line0.endPoint.Equals(line1.startPoint) && line0.startPoint.Equals(line1.endPoint));
        
            return flag;


        }
        /// <summary>
        /// 点是否在线上
        /// </summary>
        /// <param name="line0"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsOnLine(Line line0, Vector3 point,float threshold = 0.001f)
        {
            var line1 = point.MinusToOther(line0.startPoint).Normalize<Vector3>();
            var line2 = point.MinusToOther(line0.endPoint).Normalize<Vector3>();
            var angle = line2.AngleWithDegree(line1);

            return (Math.Abs(angle-180) < threshold);

        }

        /// <summary>
        /// 交换起点和终点
        /// </summary>
        public void Swap()
        {
            var line = this;
            var s = new Vector3(line.startPoint);
            line.startPoint = line.endPoint;
            line.endPoint = s;
            var tIndex = line.startPointIndex;
            line.startPointIndex = line.endPointIndex;
            line.endPointIndex = tIndex;
        }

        public static Vector3 GetLinesBoxCenter(List<Line> lines)
        {
            var box = new BoundingBox();
            foreach (var item in lines)
            {
                box.Update(item.startPoint);
                box.Update(item.endPoint);
            }
            return box.Center();
        }

    }
}

