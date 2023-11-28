using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.Geometry
{
    public class Plane3
    {
        public bool VerticalOrHorizontal { get; set; }=false;
        public Vector3 Normal { get; set; }
        public Vector3 P0 { get; set; }

        /// <summary>
        /// 平面到原点的最短距离
        /// </summary>
        public float Distance { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="normal">平面法线</param>
        /// <param name="distance">平面到原点的最短距离</param>
        public Plane3(Vector3 normal,float distance)
        {
            Normal = normal;
            Distance = distance;
        }
        
        public Plane3(Vector3 normal, Vector3 point)
        {
            Normal = normal;
            P0 = point;
        }


        /// <summary>
        /// 判断点和面的关系
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Sign OrientedSide(Vector3 point)
        {
            if(VerticalOrHorizontal)
            {
                return OrientedSideFast(point);
            }
            
            var l = point.MinusToOther(P0).Normalize<Vector3>();
            var dot = l.Dot(Normal);
            var sign = Sign.ON_ORIENTED_BOUNDARY;
            if (dot > 0)
                sign = Sign.ON_POSITIVE_SIDE;
            else if(dot<0)
                sign = Sign.ON_NEGATIVE_SIDE;
            return sign;

        }
        public Sign OrientedSideFast(Vector3 point)
        {
            var sign = Sign.ON_ORIENTED_BOUNDARY;
            if (Normal.X!=0)
            {
                var right = point.X > P0.X;
                if(right)
                {
                    sign = Normal.X>0?Sign.ON_POSITIVE_SIDE:Sign.ON_NEGATIVE_SIDE;
                }
                else
                {
                    sign = Normal.X < 0 ? Sign.ON_POSITIVE_SIDE : Sign.ON_NEGATIVE_SIDE;
                }
            }
            else if(Normal.Y!=0)
            {
                var back = point.Y > P0.Y;
                if (back)
                {
                    sign = Normal.Y > 0 ? Sign.ON_POSITIVE_SIDE : Sign.ON_NEGATIVE_SIDE;
                }
                else
                {
                    sign = Normal.Y < 0 ? Sign.ON_POSITIVE_SIDE : Sign.ON_NEGATIVE_SIDE;
                }
            }
            else
            {
                var top = point.Z > P0.Z;
                if (top)
                {
                    sign = Normal.Z > 0 ? Sign.ON_POSITIVE_SIDE : Sign.ON_NEGATIVE_SIDE;
                }
                else
                {
                    sign = Normal.Z < 0 ? Sign.ON_POSITIVE_SIDE : Sign.ON_NEGATIVE_SIDE;
                }
            }
            return sign;
        }

        /// <summary>
        /// 将点投影到平面
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 ProjectPointOntoPlane(Plane3 plane,Vector3 point)
        {
            var pointDistance = GetPointDistance(plane, point);
            var tempNormal = new Vector3(plane.Normal);
            var scaledNormal = tempNormal.Multiply<Vector3>(pointDistance);
            return point.MinusToOther(scaledNormal);
        }

        /// <summary>
        /// 点到平面的距离
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float GetPointDistance(Plane3 plane, Vector3 point)
        {
            return plane.Normal.Dot(point) + plane.Distance;
        }
    }
    public class PlanWithBox : Plane3
    {
        public Vector3 P1
        {
            get; set;
        }
        public BoundingBox Box { get; set; }
        public PlanWithBox(Vector3 normal, Vector3 point) : base(normal, point)
        {
        }

        /// <summary>
        /// 初步判断平面（xy有限）与点集相交情况
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool IsBoxIntersectXY(List<Vector3> points)
        {
            if (Box == null)
            {
                Box = new BoundingBox();
                Box.Update(P0);
                Box.Update(P1);
            }

            var polygonBox = new BoundingBox();
            polygonBox.Update(points);

            return Box.IntersectXY(polygonBox);
        }
    }

    public enum Sign
    {
        NEGATIVE = -1, ZERO = 0, POSITIVE = 1,

        // 方向:
        RIGHT_TURN = -1, LEFT_TURN = 1,

        CLOCKWISE = -1, COUNTERCLOCKWISE = 1,

        COLLINEAR = 0, COPLANAR = 0, DEGENERATE = 0,

        // Oriented_side constants:
        ON_NEGATIVE_SIDE = -1, ON_ORIENTED_BOUNDARY = 0, ON_POSITIVE_SIDE = 1,

        // 结果值:
        SMALLER = -1, EQUAL = 0, LARGER = 1
    };
}
